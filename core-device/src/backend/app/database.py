from flask_sqlalchemy import SQLAlchemy
from sqlalchemy.sql import func
from sqlalchemy import cast, Integer
import logging
from enums import TagType
from dataclasses import dataclass
from errors import OverwriteException, DeleteException, AdminLimitReachedException, NameTooLongException
import os
from tagHelper import admin_range


logger = logging.getLogger(__name__)
db = SQLAlchemy()

@dataclass
class Tags(db.Model):
    id:int = db.Column(db.BigInteger, primary_key=True)
    tag_id:str = db.Column(db.String(255), unique=True, nullable=False)
    type:str = db.Column(db.Enum(TagType), nullable=False)
    slot:str = db.Column(db.Integer, nullable=True)
    name:str = db.Column(db.String(12), nullable=False)


def init_db(app):
    basedir = os.path.abspath(os.path.dirname(__file__))
    app.config['SQLALCHEMY_DATABASE_URI'] = f'sqlite:///{os.path.join(basedir, "data", "database.db")}'
    app.config['SQLALCHEMY_TRACK_MODIFICATIONS'] = False
    db.init_app(app)

def init_app(app):
    with app.app_context():
        db.create_all()


#DB Operations

## Query
def get_all_tags():
    return Tags.query.all()

def get_tag_by_tag_id(tag_id):
    if tag_id is None:
        return None
    return Tags.query.filter_by(tag_id=tag_id).first()

def get_tags_by_tag_ids(tag_ids):
    if not tag_ids: 
        return []
    return Tags.query.filter(Tags.tag_id.in_(tag_ids)).all()

def get_admin_tags():
    return get_tags_by_tag_ids(['1','2','3','4'])

def get_tags_by_type(tag_type):
    return Tags.query.filter_by(type=tag_type).all()


## Write
def save_tag(id, tag_type, name, slot=None):
    try:
        if len(name) > 12:
            raise NameTooLongException("Nimi on liian pitkä")
        existing_tag = Tags.query.filter_by(id=id).first()
        new_tag_id = None
        if name == "admin":
            max_admin_tag_id = db.session.query(func.max(cast(Tags.tag_id, Integer))).filter(Tags.name == "admin").scalar()
            if max_admin_tag_id is None:
                new_tag_id = '1'
            elif max_admin_tag_id < 4:
                new_tag_id = str(max_admin_tag_id+1)
            else:
                raise AdminLimitReachedException("Ylläpitäjiä ei voi luoda enempää.")
        else:
            new_tag_id = db.session.query(func.max(cast(Tags.tag_id, Integer))).scalar() + 1
            if new_tag_id < 5:
                new_tag_id = new_tag_id + (5 - new_tag_id)
            new_tag_id = str(new_tag_id)
        with_tag_id = Tags.query.filter_by(tag_id=new_tag_id).first()
        
        if existing_tag:
            logger.info("existing: ")
            logger.info(existing_tag)
            if existing_tag.tag_id in admin_range:
                raise OverwriteException("Ylläpitäjän ylikirjoitus estetty")
            if existing_tag.type != tag_type:
                raise OverwriteException(f"Tägi on jo asetettu tyypiksi {existing_tag.type}, id:llä {existing_tag.tag_id}. Jos haluat muuttaa sitä, poista olemassaoleva rivi ensin.")
            existing_tag.type = tag_type
            existing_tag.slot = slot
            existing_tag.name = name
            db.session.commit()
            return existing_tag.tag_id
        elif with_tag_id:
            raise OverwriteException(f"Tyypillä {with_tag_id.tag_type} on jo rivi tag id:llä {with_tag_id.tag_id}.")
        else:
            new_tag = Tags(id=id, tag_id=new_tag_id, type=tag_type, slot=slot, name=name)
            db.session.add(new_tag)
            db.session.commit()
            logger.info("new tag")
            logger.info(new_tag)
            return new_tag_id
    except Exception as e:
        db.session.rollback()
        logger.error(str(e))
        raise e


## Delete

def delete_tag(id):
    tag = Tags.query.filter_by(id=id).first()
    if tag:
        if tag.tag_id in admin_range:
            raise DeleteException("Ylläpitäjän poistaminen ei sallittu")
        db.session.delete(tag)
        db.session.commit()
    return tag