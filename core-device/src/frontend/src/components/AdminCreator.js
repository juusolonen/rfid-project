import React, {useState, useEffect, useContext} from 'react'
import Action from '../socketActions';
import AdminContext from '../context/AdminContext';
import UserContext from '../context/UserContext';

export default function AdminCreator(props) {

    const [createClicked, setCreateClicked] = useState(false)
    const {adminExists} = useContext(AdminContext)
    const {error} = useContext(UserContext)

      useEffect(() => {
        if (!adminExists) {
          props.socket.emit(Action.INIT)
        }
      }, [])

      useEffect(() => {
        if (error) {
          setCreateClicked(false)
        }
      }, [error])
  
  
      const createAdmin = () => {
        if (props.socket.connected) {
          setCreateClicked(true)
          props.socket.emit(Action.CREATE_ADMIN);
        }
        else {
          console.log("socket not connected")
        }
   
      }
  
        return (
          <div className="contentDiv">
            <h4>Luo järjestelmänvalvoja:</h4>
            <p>Paina allaolevaa nappia ja näytä tunnistetta lukijalle</p>
      
            <button onClick={createAdmin} disabled={createClicked}>
              {createClicked ? "Odota..." : "Luo"} 
            </button>
          </div>
          );
      
}