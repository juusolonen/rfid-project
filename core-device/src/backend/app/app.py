# Set this variable to "threading", "eventlet" or "gevent" to test the
# different async modes, or leave it set to None for the application to choose
# the best option based on available packages.
async_mode = None

if async_mode is None:
    try:
        import eventlet
        async_mode = 'eventlet'
    except ImportError:
        pass

    if async_mode is None:
        try:
            from gevent import monkey
            async_mode = 'gevent'
        except ImportError:
            pass

    print('async_mode is ' + async_mode)

# monkey patching is necessary because this application uses a background
# thread
if async_mode == 'eventlet':
    import eventlet
    eventlet.monkey_patch()
elif async_mode == 'gevent':
    from gevent import monkey
    monkey.patch_all()

from flask import Flask, current_app, jsonify
from flask_socketio import SocketIO
import logging
from config import configure_logging
from rfid_reader import reader_loop
from subscribe_handler import subscribe_to_core
from socket_handlers import connect_handler, handle_message, handle_admin_create, handle_new_tag, send_admin_data, handle_delete, handle_borrow, handle_user_in, handle_user_out, handle_init, handle_admin_logout
from database import init_db, init_app
from flask_cors import CORS
from enums import Actions, Triggers
from toolshelf import ToolShelf

# Setup logging
configure_logging()

# Flask setup
app = Flask(__name__)
shelf = ToolShelf()
init_db(app)
init_app(app)
CORS(app)
app.config.update(DEBUG=True)
background_task_started = False


logger = logging.getLogger(__name__)

# SocketIO setup
socket_io = SocketIO(app, cors_allowed_origins="*", async_mode=async_mode)

# Register SocketIO events
socket_io.on_event(Actions.PUBLISH.topic, handle_message)
socket_io.on_event(Actions.NEW_TAG.topic, handle_new_tag)
socket_io.on_event(Actions.BORROW.topic, handle_borrow)


# Start RFID reader in a background task
@socket_io.on(Actions.CONNECT.topic)
def on_connect(auth):
    global background_task_started
    if not background_task_started:
        socket_io.start_background_task(reader_loop, socket_io, current_app._get_current_object())
        background_task_started = True
    connect_handler(socket_io)


@socket_io.on(Actions.INIT.topic)
def on_init():
    handle_init(socket_io)

@socket_io.on(Actions.CREATE_ADMIN.topic)
def on_create_admin():
    handle_admin_create(socket_io)

@socket_io.on(Actions.REFRESH_DATA.topic)
def on_refresh():
    send_admin_data(socket_io, Triggers.REFRESH)

@socket_io.on(Actions.DELETE.topic)
def on_delete(id):
    handle_delete(socket_io, id)


@socket_io.on(Actions.USER_IN.topic)
def on_user_in(msg):
    if msg.get('user') is None:
        return
    handle_user_in(socket_io, msg=msg)


@socket_io.on(Actions.USER_OUT.topic)
def on_user_out(msg):
    handle_user_out(socket_io, msg=msg)


@socket_io.on(Actions.ADMIN_LOGOUT.topic)
def on_admin_logout():
    handle_admin_logout(socket_io)

if __name__ == "__main__":
    ## Uncomment when running with GG/Nucleus
    subscribe_to_core(socket_io)
    socket_io.run(app, host="0.0.0.0", port=5000)
