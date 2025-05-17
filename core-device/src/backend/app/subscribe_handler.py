import json
import os
import awsiot.greengrasscoreipc
import awsiot.greengrasscoreipc.client as client
import awsiot.greengrasscoreipc.model as model
import logging
from enums import Triggers

logger = logging.getLogger(__name__)

class SubscribeTopicHandler(client.SubscribeToIoTCoreStreamHandler):
    def __init__(self, socket_io_app):
        super().__init__()
        self.socket_io = socket_io_app

    def on_stream_event(self, event: model.IoTCoreMessage) -> None:
        payload = {"payload": json.loads(event.message.payload.decode())}
        self.socket_io.emit(Triggers.MESSAGE.topic, payload)
        logger.info("Message sent from SubscribeTopicHandler!")

def subscribe_to_core(socket_io_app):
    ipc_client = awsiot.greengrasscoreipc.connect()

    subscribe_operation = ipc_client.new_subscribe_to_iot_core(
        stream_handler=SubscribeTopicHandler(socket_io_app)
    )
    subscribe_operation.activate(
        request=model.SubscribeToIoTCoreRequest(
            topic_name="{}/subscribe".format(os.environ["AWS_IOT_THING_NAME"]),
            qos=model.QOS.AT_LEAST_ONCE,
        )
    )
