import React, {useState, useEffect} from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import './App.css';
import App2 from './pages/App2';
import reportWebVitals from './reportWebVitals';
import 'bootstrap/dist/css/bootstrap.css';
import Confiruration from './pages/Confiruration';
import { BrowserRouter, Routes, Route } from "react-router-dom";
import AdminContext from './context/AdminContext';
import UserContext from './context/UserContext';
import Layout from './Layout';
import { io } from "socket.io-client";
import topics from './socketTopics'
import Users from './components/Users';
import Tools from './components/Tools';


const host = process.env.REACT_APP_BE_HOST;
console.log(host)
let endPoint = `http://${host}:3000`;
let socket = io(endPoint, {
  autoConnect: false,
  rejectUnauthorized: false,
});


export default function App() {

      const [adminExists, setAdminExists] = useState(false);
      const [started, setStarted] = useState(false);
      const [socketMessage, setSocketMessage] = useState('');
      const [adminConfiguring, setAdminConfiguring] = useState(false)
      const [users, setUsers] = useState()
      const [tools, setTools] = useState()
      const [error, setError] = useState(false)
      const [toolShelf, setToolShelf] = useState([])
      const [username, setUsername] = useState("")
      const [doorOpen, setDoorOpen] = useState(false)
 
      useEffect(() => {
         startSocket();
       
          return () => {
            socket.disconnect();
          }
        }, []);

        useEffect(() => {
          let timer;
          if (socketMessage) {
            timer = setTimeout(() => {
              setSocketMessage('');
              setError(false)
            }, 5000);
          }
          return () => clearTimeout(timer);
        }, [socketMessage]);
        

        const dismissMessage = () => {
          setSocketMessage('');
        };

        const startSocket = () => {
          socket.on(topics.CONNECT, () => {
            console.log("connected");
          });
       
          socket.on(topics.MESSAGE, (msg) => {
            console.log(msg);
            setError(false)
            setSocketMessage(JSON.stringify(msg))
          });

          socket.on(topics.WriteAdminResult_SUCCESS, (msg) => {
            console.log("admin tehty")
            setAdminExists(true)
            setStarted(true)
            setAdminConfiguring(true)
            setSocketMessage(JSON.stringify(msg))
            setUsers(msg.data?.users ?? users)
            setTools(msg.data?.tools ?? tools)
          })

          socket.on(topics.START_CONFIGURE, (msg) => {
            console.log("admin luettu", msg)
            setAdminExists(msg.adminExists)
            setStarted(true)
            setUsers(msg.data?.users)
            setTools(msg.data?.tools)
            setToolShelf(msg.data?.toolshelf ?? toolShelf)
            if (msg.data?.door) {
              setDoorOpen(msg.data.door)
              setTimeout(() => {
                setDoorOpen(false)
              }, 5000);
            }
            setAdminConfiguring(true)
          })

          socket.on(topics.WriteResult_SUCCESS, (msg) => {
            setUsers(msg.data?.users ?? users)
            setTools(msg.data?.tools ?? tools)
            if (msg.data?.door) {
              setDoorOpen(msg.data.door)
              setTimeout(() => {
                setDoorOpen(false)
              }, 5000);
            }
          })


          socket.on(topics.REFRESH, (msg) => {
            console.log("refresg", msg)
            setUsers(msg.data?.users ?? users)
            setTools(msg.data?.tools ?? tools)
            setToolShelf(msg.data?.toolshelf ?? toolShelf)
            
            if (msg.data?.door) {
              setDoorOpen(msg.data.door)
              setTimeout(() => {
                setDoorOpen(false)
              }, 5000);
            }
        
            if (msg.data?.username !== null && msg.data?.username !== undefined) {
              setUsername(msg.data.username)
              setStarted(true)
            }
          })

          socket.on(topics.WriteResult_FAIL, (msg) => {
            setError(true)
            setSocketMessage(msg)
          })

          socket.on(topics.ERROR_MESSAFE, (msg) => {
            setError(true)
            setSocketMessage(msg)
          })

          socket.on('disconnect', () => {
            console.log('Disconnected from the server');
          });
          
          socket.on('connect_error', (err) => {
            console.log('Connection error:', err);
          });

          socket.on("error", (err) => {
            console.log("Socket.IO Error");
            console.log(err); 
          });
       
          socket.connect();
          socket.emit("hello")
        };
   
    let classname = `socket-message ${error ? "error" : ""}`
  return (
    <div className="App">  
    <AdminContext.Provider value={{ 
        adminExists, setAdminExists,
        started,setStarted,
        adminConfiguring, setAdminConfiguring,
        users, setUsers,
        tools, setTools,
        doorOpen
        }}>
        <UserContext.Provider value={{
          toolShelf, setToolShelf,
          username, setUsername,
          error
        }}>
          {socketMessage && (
        
            <div className={classname}>
              <button 
                onClick={dismissMessage}
                style={{
                  position: 'absolute',
                  top: '5px',
                  right: '5px',
                  background: error ? 'red' : 'none',
                  border: 'none',
                  fontSize: '1.2em',
                  cursor: 'pointer'
                }}
              >
                &times;
              </button>
              <p>{socketMessage}</p>
            </div>
          )}
            <BrowserRouter>
              <Routes>
                <Route path="/" element={<Layout socket={socket}/>}>
                  <Route path="borrow" element={<App2 socket={socket}/>}>
                  </Route>
                  <Route path="conf" element={<Confiruration socket={socket}/>}>
                    <Route path="users" element={<Users socket={socket}/>} />
                    <Route path="tools" element={<Tools socket={socket}/>} />
                    <Route path="borrow" element={<App2 socket={socket}/>} />
                  </Route>
                </Route>
              </Routes>
            </BrowserRouter>
        </UserContext.Provider>
    </AdminContext.Provider>
    </div>
  );
}

ReactDOM.render(
  <React.StrictMode>
    <App />
  </React.StrictMode>,
  document.getElementById('root')
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
