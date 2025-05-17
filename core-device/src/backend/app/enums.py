from enum import Enum

class Topic(Enum):
    @property
    def topic(self):
        return f"{self.value}"
    
class TagType(Enum):
    TOOL = 'Tool'
    USER = 'User'

class WriteResult(Topic):
    SUCCESS = 'write_success'
    FAIL = 'write_fail'
    ADMIN_WRITE_SUCCESS = 'write_admin_success'
    
class Triggers(Topic):
    START_CONFIGURE = 'start_configure'
    MESSAGE = 'message'
    ERROR_MESSAFE= 'error_message'
    REFRESH = 'refresh'

class Actions(Topic):
    CREATE_ADMIN = 'create_admin'
    NEW_TAG = 'newtag'
    PUBLISH = 'publish'
    CONNECT = 'connect'
    REFRESH_DATA = 'refreshdata'
    DELETE = 'delete'
    BORROW = 'borrow'
    USER_IN = 'user_in'
    USER_OUT = 'user_out'
    INIT = 'init'
    ADMIN_READ = 'admin_read'
    ADMIN_LOGOUT = 'admin_logout'

