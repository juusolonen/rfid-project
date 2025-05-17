import React, {useState} from 'react'

export default function SaveTag(props){

  const [newName, setNewName] = useState(''); 
  const [slots, setSlots] = useState([1,2,3,4,5])
  const [selectedSlot, setSelectedSlot] = useState(1)

  if (props.tools) {
    return (
      <div className="mt-3">
        <div className="row">
          <div className="col-md-6">
            <input
              type="text"
              className="form-control"
              placeholder="Nimi"
              value={newName}
              maxLength={12}
              onChange={(e) => setNewName(e.target.value)}
            />
          </div>
          <div className="col-md-6">
            <select
              className="form-select"
              value={selectedSlot}
              onChange={(e) => setSelectedSlot(e.target.value)}
            >
              {slots.map((slot) => (
                <option key={slot} value={slot}>
                  Paikka {slot}
                </option>
              ))}
            </select>
          </div>
        </div>
        <div className="row">
          <div className="col-md-12">
            <button disabled={props.disabled}
              type="button"
              className="btn btn-primary"
              onClick={() => props.onSave(newName, selectedSlot)}
            >
              {props.disabled ? "Lue tunniste.." : "Lisää"}
            </button>
          </div>
        </div>
      </div>
    
    )
  } else {
    return(
      <div className="mt-3">
        <div className="row">
          <div className="col-md-6">
            <input
              type="text"
              className="form-control"
              placeholder="Nimi"
              value={newName}
              maxLength={12}
              onChange={(e) => setNewName(e.target.value)}
            />
          </div>
          <div className="col-md-6">
            <button disabled={props.disabled}
              type="button"
              className="btn btn-primary"
              onClick={() => props.onSave(newName, selectedSlot)}
            >
              {props.disabled ? "Lue tunniste.." : "Lisää"}
            </button>
          </div>
        </div>
      </div>
    )
  }
    
}