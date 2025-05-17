from enums import TagType

def extractValues(tag_data):
    tag_id = None
    type = None
    slot = None
    parts = tag_data.split(',')
    if len(parts) >= 3:
        tag_id = parts[0]
        type = TagType[parts[1].upper()]
        slot = parts[2]

    return tag_id, type, slot

admin_range = ["1", "2", "3", "4"]