import logging
import sys

def configure_logging():
    logging.basicConfig(stream=sys.stdout, level=logging.DEBUG)
    #logging.getLogger('sqlalchemy.engine').setLevel(logging.INFO)
    logger = logging.getLogger(__name__)
    logger.info("Logging is set up")
