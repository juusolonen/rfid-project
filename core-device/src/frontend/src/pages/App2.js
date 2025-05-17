import React, { useState, useEffect, useContext } from 'react';
import '../App.css';
import UserContext from '../context/UserContext';
import ToolShelf from '../components/ToolShelf';
import Action from '../socketActions';
import AdminContext from '../context/AdminContext';

export default function App2(props) {

  const {username, toolShelf} = useContext(UserContext)
  const {adminConfiguring} = useContext(AdminContext)
  const [waiting, setWaiting] = useState(false)


  useEffect(() => {
    if (username) {
      props.socket.emit(Action.USER_IN, {user: username});
    }
  }, [])

  useEffect(() => {
    setWaiting(false)
  }, [toolShelf])

  const somethingSelected = () => {
    return toolShelf.some(tool => tool.outgoing || tool.incoming);
  }

  const send = () => {
    setWaiting(true)
    const selectedTools = toolShelf.filter(tool => tool.outgoing || tool.incoming);
    props.socket.emit(Action.BORROW, {user: username, tools: selectedTools});
  }

  const getGreeting = () => {
    if (adminConfiguring) {
      return (
        <>
          <h4>Hei ylläpitäjä</h4>
          <p>Alla olevassa taulukossa näet paikalla olevat työkalut valkealla taustalla ja poissaolevat harmaalla.</p>
          <p>Tässä näkymässä ei ole mahdollista lainata/palauttaa työkaluja.</p>
        </>
      )
    }
    if (username) {
      return (
        <>
          <h4>Hei {username}</h4>
          <p>Lue <span style={{color: 'yellow'}}>palautuvien</span> työkalujen tunnisteet ja vie ne paikoilleen.</p>
          <p>Seuraavaksi lue <span style={{color: 'green'}}>lainaan lähtevien</span> työkalujen tunnisteet.</p>
          <p>Kun olet valmis, paina "OK" ja lue vielä oma tunnisteesi.</p>
        </>
      )
    }
  }
  

  return (
    
    <>
      {getGreeting()}

      <br/><br/>

      <ToolShelf/>

      <div className="row">
          <div className="col-md-12">
            <button disabled={adminConfiguring || waiting || !somethingSelected()}
              type="button"
              className="btn btn-success"
              onClick={() => send()}
            >
              {waiting ? "Lue tunniste.." : "OK"}
            </button>
          </div>
      </div>

    </>
)}


