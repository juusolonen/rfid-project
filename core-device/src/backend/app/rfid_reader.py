from MFRC522 import SimpleMFRC522
import logging
from socket_handlers import read_mode, write_queue, write_permission, handle_rfid_write, handle_rfid_read, tag_created_helper, handle_error

logger = logging.getLogger(__name__)

def reader_loop(socket_io, app):
    sleep_time = 1
    try:
        reader = SimpleMFRC522(1, 2)
        logger.info("Starting RFID reader loop.")
        with app.app_context():
            while True:
                if read_mode.is_set():
                    sleep_time = 1
                    id, text = reader.read_no_block()
                    if id:
                        socket_io.start_background_task(handle_rfid_read, socket_io, id, text, app)
                else:
                    sleep_time = 3
                    try:
                        write_data = write_queue.get_nowait()
                        logger.info(f"Writing tag with data: {write_data}")
                        # Example: Pora,TOOL,3
                        slot = 0 if write_data['slot'] == None else int(write_data['slot'])
                        id, text = reader.read()
                        socket_io.start_background_task(handle_rfid_write, socket_io, id, write_data, app)
                        permission = write_permission.wait(10)
                        if permission:
                            tag_id = write_queue.get_nowait()
                            id, text = reader.write(f"{tag_id},{write_data['type']},{slot}")
                            socket_io.start_background_task(tag_created_helper, socket_io, id, tag_id, write_data)
                        write_permission.clear()
                        read_mode.set()
                    except Exception as ex:
                        logger.info(str(ex))
                        socket_io.start_background_task(handle_error, socket_io, str(ex))
                socket_io.sleep(sleep_time)
    except Exception as e:
        logger.error(f"Reading error: {str(e)}")
        socket_io.start_background_task(handle_error, socket_io, str(ex))
