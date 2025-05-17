import json
import awsiot.greengrasscoreipc
import awsiot.greengrasscoreipc.model as model
import os
import logging
from threading import Event
from queue import Queue
from database import get_all_tags, save_tag, get_tag_by_tag_id, delete_tag, get_tags_by_type, get_admin_tags
from enums import WriteResult, Triggers, TagType, Actions
from tagHelper import extractValues, admin_range
from collections import defaultdict
from toolshelf import ToolShelf
import time
import datetime


read_mode = Event()
read_mode.set()

write_permission = Event()
write_permission.clear()

action_mode = Event()
action_mode.clear()

logger = logging.getLogger(__name__)

write_queue = Queue()
borrow_queue = Queue()

shelf = ToolShelf()

def connect_handler(socket_io):
    logger.info("connected")

def handle_admin_create(socket_io):
    send_to_aws(socket_io, Actions.CREATE_ADMIN)
    purge_write_queue()
    write_queue.put({"type": "USER", "name": "admin", "slot": 0})
    read_mode.clear()

def handle_new_tag(new_tag):
    logger.info(f"writing to queue {new_tag}")
    purge_write_queue()
    write_queue.put(new_tag)
    read_mode.clear()

def handle_message(msg):
    formatted_msg = format_aws_msg(msg)
    logger.info(f"message to aws: {formatted_msg}")
    #return
    #TODO: Uncomment when AWS messgae is wanted
    ipc_client = awsiot.greengrasscoreipc.connect()
    topic = "{}/publish".format(os.environ["AWS_IOT_THING_NAME"])
    data = formatted_msg

    publish_operation = ipc_client.new_publish_to_iot_core()
    publish_operation.activate(
        request=model.PublishToIoTCoreRequest(
            topic_name=topic,
            qos=model.QOS.AT_LEAST_ONCE,
            payload=json.dumps(data).encode(),
        )
    )

def format_aws_msg(msg):
    formatted = {}
    utc_now = datetime.datetime.now(datetime.timezone.utc).isoformat()
    formatted['timestamp'] = utc_now
    formatted['user'] = msg.get('user')
    formatted['user_tagid'] = msg.get('user_tagid')
    formatted['user_id'] = msg.get('user_id')
    formatted['action'] = msg.get('msg_type')

    tag = msg.get('tag')
    if tag:
        formatted['tag'] = tag
    
    if formatted['action'] == Actions.BORROW.topic:
        formatted['tools'] = []

        for tool in msg.get('tools', []):
            formatted_tool = {
                'name': tool.get('name'),
                'slot': tool.get('slot'),
                'out': tool.get('outgoing'),
                'in': tool.get('incoming'),
                'id': tool.get('id'),
                'tag_id': tool.get('tag_id')  
            }
            formatted['tools'].append(formatted_tool) 

    return formatted

def emit_with_message(socket_io, topic, msg):
    socket_io.emit(topic, msg)

def handle_rfid_write(socket_io, id, msg, app):
    with app.app_context():
        logger.info("writing")
        logger.info(msg)
        msg_to_client = None
        result = None
        try:
            tag_id = save_tag(id, msg['type'], msg['name'], msg['slot'])
            result = WriteResult.ADMIN_WRITE_SUCCESS if msg['name'] == "admin" else WriteResult.SUCCESS
            logger.info(f"{result}")
            purge_write_queue()
            write_queue.put(tag_id)
            write_permission.set()
        except Exception as e:
            msg_to_client = str(e)
            result = WriteResult.FAIL
            write_permission.clear()
        if result != WriteResult.FAIL:
            send_admin_data(socket_io, result)
        else:
            socket_io.start_background_task(emit_with_message, socket_io, result.topic, msg_to_client)

def purge_write_queue():
    queue_empty = False
    while not queue_empty:
        try:
            write_queue.get(block=False)
        except Exception as e:
            queue_empty = True
            logger.info("Queue emptied")
    return

def handle_rfid_read(socket_io, id, msg, app):
    with app.app_context():
        tag_id, type, slot = extractValues(msg)
        tag = get_tag_by_tag_id(tag_id)
    
        if tag is None:
            logger.info(f"No tag found with tag_id {tag_id}")
            admintags = get_admin_tags()
            logger.info(admintags)
            if not admintags:
                socket_io.start_background_task(emit_with_message,socket_io, Triggers.START_CONFIGURE.topic, {"adminExists": False})
            return
        
        if id == tag.id and tag_id == tag.tag_id:
            if tag.tag_id in admin_range: #admin tag
                handle_admin_read(socket_io, tag)
            else:
                if tag.type == TagType.USER:
                    logger.info(f"Tag read: ID={id}, Text={msg}")
                    handle_user_read(socket_io, tag)
                elif tag.type == TagType.TOOL:
                    handle_tool_read(socket_io, tag)
                    

# Depending on the slot reading snapshot on door first open
# Mark tool as outgoing or incoming
def handle_tool_read(socket_io, tag):
    #Snapshot missing = no logging in happened yet
    if shelf.admin_mode:
        msg = f"Työkalu: {tag.tag_id}/{tag.name}, paikka {tag.slot}"
        socket_io.start_background_task(emit_with_message, socket_io, Triggers.MESSAGE.topic, msg)
        return
    elif shelf.snapshot is None:
         socket_io.start_background_task(emit_with_message, socket_io, Triggers.ERROR_MESSAFE.topic, "Tunniste ei kelpaa")
         return
    elif shelf.snapshot[tag.slot-1]:
        shelf.outgoing[tag.slot-1] = True
    else:
        shelf.incoming[tag.slot-1] = True
    send_app_data(socket_io, Triggers.REFRESH)
                    
def handle_admin_read(socket_io, tag):
    if not shelf.admin_mode:
        socket_io.start_background_task(shelf.open_door)
        logger.info("BEEEEEEEEEEEEEEEEEEEEEEEEEEEP")
        shelf.start_admin_mode(tag)
    msg = {}
    msg['user_id'] = tag.id
    msg['user_tagid'] = tag.tag_id
    msg['user'] = tag.name
    send_to_aws(socket_io, Actions.ADMIN_READ, msg=msg)
    send_admin_data(socket_io, Triggers.START_CONFIGURE)

def handle_admin_logout(socket_io):
    socket_io.start_background_task(shelf.open_door)
    logger.info("BEEEEEEEEEEEEEEEEEEEEEEEEEEEP")
    send_app_data(socket_io, Triggers.REFRESH, door_only=True)
    shelf.end_admin_mode()
    send_to_aws(socket_io, Actions.ADMIN_LOGOUT)

def send_admin_data(socket_io, topic, tag_type=None):
    tags = get_all_tags() if tag_type is None else get_tags_by_type(tag_type)

    tags_by_type = defaultdict(list)
    for tag in tags:
        tags_by_type[tag.type].append(tag)
    users_list = [{'id': tag.id, 'tag_id': tag.tag_id, 'name': tag.name} for tag in sorted(tags_by_type[TagType.USER], key=lambda tag: tag.tag_id)]
    tools_list = [{'id': tag.id, 'tag_id': tag.tag_id, 'name': tag.name, 'slot': tag.slot} for tag in sorted(tags_by_type[TagType.TOOL], key=lambda tag: tag.tag_id)]
    toolshelf_data, door = get_toolshelf_data()
    socket_io.start_background_task(emit_with_message, socket_io, topic.topic, {"adminExists": True, "data": {"toolshelf": toolshelf_data, "door": door, "users": users_list, "tools": tools_list}})

def send_app_data(socket_io, topic, username=None, message=None, door_only=False):
    tools_list, door = get_toolshelf_data(door_only)
    if door_only:
        socket_io.start_background_task(emit_with_message, socket_io, topic.topic, {"data": {"door": door}})
        return
    socket_io.start_background_task(emit_with_message, socket_io, topic.topic, {"data": {"toolshelf": tools_list, "door": door, "username": username, "message": message}})

def get_toolshelf_data(door_only=False):
    door = shelf.read_door()

    if door_only:
        return None, door
    outgoing_tools = shelf.outgoing
    incoming_tools = shelf.incoming
    tools = get_tags_by_type(TagType.TOOL)

    slots = None
    while not slots:
        slots = shelf.read_shelf()
        time.sleep(0.25)

    logger.info("slots: ")
    logger.info(slots)
    
    tools_list = [{'id': tool.id, 'tag_id': tool.tag_id, 'name': tool.name, 'slot': tool.slot, 'present': slots[tool.slot-1], 'outgoing': outgoing_tools[tool.slot-1], 'incoming': incoming_tools[tool.slot-1]} for tool in tools]
    tools_list = sorted(tools_list, key=lambda x: x['slot'])
    logger.info(tools_list)
    return tools_list, door

def handle_delete(socket_io, id):
    try:
        deleted_tag = delete_tag(id)
        socket_io.start_background_task(tag_deleted_helper, socket_io, deleted_tag)
        send_admin_data(socket_io, Triggers.REFRESH, deleted_tag.type)
    except Exception as e:
        msg_to_client = str(e)
        result = WriteResult.FAIL
        socket_io.start_background_task(emit_with_message, socket_io, result.topic, msg_to_client)


def handle_user_in(socket_io, msg):
    action_mode.set()
    if shelf.user_in is None:
        shelf.user_in = msg.get('user')
    socket_io.start_background_task(shelf.open_door)
    logger.info("BEEEEEEEEEEEEEEEEEEEEEEEEEEEP")
    send_app_data(socket_io, Triggers.REFRESH, door_only=True)
    shelf.snapshot_slots()
    send_to_aws(socket_io, Actions.USER_IN, msg=msg)

def handle_init(socket_io):
    send_to_aws(socket_io, Actions.INIT)

def handle_user_out(socket_io, msg):
    action_mode.clear()
    shelf.user_in = None
    socket_io.start_background_task(shelf.open_door)
    logger.info("BEEEEEEEEEEEEEEEEEEEEEEEEEEEP")
    send_app_data(socket_io, Triggers.REFRESH, door_only=True)
    shelf.clear_snapshot()
    send_to_aws(socket_io, Actions.USER_OUT, msg=msg)

#No need to send_to_aws here, done in handle_user_read
def handle_borrow(msg):
    borrow_queue.put(msg)

def handle_user_read(socket_io, tag):
    if shelf.admin_mode:
        msg = f"Käyttäjä: {tag.tag_id}/{tag.name}"
        socket_io.start_background_task(emit_with_message, socket_io, Triggers.MESSAGE.topic, msg)
        return
    if shelf.user_in is not None and shelf.user_in == tag.name:
        if action_mode.is_set():
            msg = borrow_queue.get()
            msg['user_id'] = tag.id
            msg['user_tagid'] = tag.tag_id
            send_to_aws(socket_io, Actions.BORROW, msg)
            shelf.clear_selections()
            send_app_data(socket_io, Triggers.REFRESH)
            socket_io.start_background_task(emit_with_message, socket_io, Triggers.MESSAGE.topic, 'Lainaus/palautus rekisteröity!')
            return
    elif shelf.user_in is None and tag.name is not None:
        shelf.user_in = tag.name
        send_app_data(socket_io, Triggers.REFRESH, tag.name)
        return
    socket_io.start_background_task(emit_with_message, socket_io, Triggers.ERROR_MESSAFE.topic, 'Toinen käyttäjä on jo kirjautuneena!')

def send_to_aws(socket_io, msg_type, msg={}, user='unknown'):
    msg['msg_type'] = msg_type.topic
    logger.info(f"{msg.get('user')}")
    if msg.get('user') is None:
        msg['user'] = user
    #Uncomment inside handle_message to send message to the cloud
    socket_io.start_background_task(handle_message, msg)

def tag_created_helper(socket_io, id, tag_id, write_data):
    msg = {}
    msg['tag'] = {'id': id, 'tag_id': tag_id, 'type': write_data['type'], 'name': write_data['name'], 'slot': int(write_data['slot'])}
    msg['user_id'] = shelf.admin_id
    msg['user_tagid'] = shelf.admin_tagid
    send_to_aws(socket_io, Actions.NEW_TAG, msg, user='admin')

def tag_deleted_helper(socket_io, tag):
    msg = {}
    msg['tag'] = {'id': tag.id, 'tag_id': tag.tag_id, 'type': tag.type.value.upper(), 'name': tag.name, 'slot': tag.slot}
    msg['user_id'] = shelf.admin_id
    msg['user_tagid'] = shelf.admin_tagid
    send_to_aws(socket_io, Actions.DELETE, msg, user='admin')

def handle_error(socket_io, error):
    socket_io.start_background_task(emit_with_message, socket_io, Triggers.ERROR_MESSAFE.topic, error)