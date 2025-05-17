import React, {useState, useContext, useEffect} from 'react'
import AdminContext from '../context/AdminContext';
import UserTable from './UserTable';
import Action from '../socketActions';
import SaveTag from './SaveTag';
import AdminCreator from './AdminCreator';
import UserContext from '../context/UserContext';

export default function Users(props) {

  const {users} = useContext(AdminContext)
  const {error} = useContext(UserContext)
  const [waiting, setWaiting] = useState(false)
  const [showAdminCreator, setShowAdminCreator] = useState(false)

  useEffect(() => {
    console.log("requesting fresh data")
    props.socket.emit(Action.REFRESH_DATA)
  }, [])

  useEffect(() => {
    setWaiting(false)
  }, [users, error])


  const handleSaveUser = (newUserName) => {
    setWaiting(true)
    console.log('Saving new user:', newUserName);

    props.socket.emit(Action.NEW_TAG, {name: newUserName, type: "USER", slot: 0})
  };


  const handleDelete = (id) => {
    setWaiting(true)
    console.log('Removing user:', id);

    props.socket.emit(Action.DELETE, id)
  };

    return (
      <div className="contentDiv">

      <>
            <button
              type="button"
              className="btn btn-dark"
              onClick={() => setShowAdminCreator(!showAdminCreator)}
            >
              {showAdminCreator ? "Käyttäjänluontiin" : "Ylläpitäjän luontiin"}
            </button>
        </>

        <h4>Käyttäjät</h4>

        {
          showAdminCreator &&
          (
             <AdminCreator socket={props.socket}/>
          )
        }

        {
          !showAdminCreator &&
          (
            <>
              <UserTable users={users}
              onDelete={(id) => handleDelete(id)}
              />

              <SaveTag 
                onSave={(val) => handleSaveUser(val)}
                disabled={waiting}
              />
            </>
          )
        }
        </div>
    );
  
}