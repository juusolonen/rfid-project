class OverwriteException(Exception):
    """Cannot overwrite existing admin tag"""
    pass

class DeleteException(Exception):
    """Cannot delete admin tag"""
    pass


class AdminLimitReachedException(Exception):
    """Cannot create anymore admin tags"""
    pass


class NameTooLongException(Exception):
    """Maximum name length is 12 characters"""
    pass