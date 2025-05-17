import React, {useState, useContext, useEffect} from 'react'
import ToolTable from './ToolTable';
import AdminContext from '../context/AdminContext';
import Action from '../socketActions';
import SaveTag from './SaveTag';
import UserContext from '../context/UserContext';

export default function Tools(props) {

    const {tools} = useContext(AdminContext)
    const {error} = useContext(UserContext)
    const [waiting, setWaiting] = useState(false)

    useEffect(() => {
      console.log("requesting fresh data")
      props.socket.emit(Action.REFRESH_DATA)
    }, [])

    useEffect(() => {
      setWaiting(false)
    }, [tools, error])


    const handleSaveTool = (toolName, slot) => {
      setWaiting(true)
      console.log('Saving new tools:', toolName);

      props.socket.emit(Action.NEW_TAG, {name: toolName, type: "TOOL", slot: slot})
    };


    const handleDelete = (id) => {
      setWaiting(true)
      console.log('Removing tool:', id);

      props.socket.emit(Action.DELETE, id)
    };

        return (
          <div className="contentDiv">

            <h4>Työkalut</h4>

            <ToolTable tools={tools}
                       onDelete={(id) => handleDelete(id)}
            />

            <SaveTag tools
                     onSave={(name, slot) => handleSaveTool(name, slot)}
                     disabled={waiting}
                  />

          </div>
          );
      
}