import React, {useContext, useEffect} from 'react'
import AdminContext from './context/AdminContext';
import UserContext from './context/UserContext';
import { Outlet, Link, useLocation, useNavigate } from "react-router-dom";
import Action from "./socketActions"

const Layout = (props) => {

    const {doorOpen, adminExists, started, setStarted, adminConfiguring, setAdminConfiguring} = useContext(AdminContext);
    const {username, setUsername} = useContext(UserContext)
    const location = useLocation();
    const navigate = useNavigate();


    useEffect(() => {
        if (!started) {
            navigate("/")
        }
    
    }, [])

    useEffect(() => {
        if (adminConfiguring) {
          navigate("/conf")
        } else if (username) {
            navigate("/borrow")
        } 
      }, [adminConfiguring, username]);


    const logout = () => {
        // Username should exist when not admin
        if (adminConfiguring) {
            props.socket.emit(Action.ADMIN_LOGOUT);
        }
        else if (username) {
            props.socket.emit(Action.USER_OUT, {user: username});
        }
        setAdminConfiguring(false)
        setUsername("")
        setStarted(false)
        navigate(0)
    }

    const title = () => {
        return adminExists ? "Konfigurointi" : "Lue tunniste"
    }

    const renderLink = () => {
        if (!started) {
            return <h3>{title()}</h3>
        }
        else if (!adminExists) {
            if (location.pathname === "/") {
                return <Link to="/conf">{ adminConfiguring ? "Konfiguroi" : "Aloita tästä"}</Link>;
            } else if (location.pathname === "/conf") {
                return <Link to="/">Alkuun</Link>;
            }
        } 
        return null;
    }
  
  return (
    <div className="layoutDiv">  
        <header className="App-header">
            <nav>
                <i style={{fontSize: "36px", color: doorOpen ? "green" : "red"}}>&#xf52b;</i>
                <ul className='list-unstyled'>
                    <li>
                        {
                            renderLink()
                        }
                    </li>
                </ul>
            </nav>
        </header>
        <div className='contentDiv'>
        {
            (username || adminConfiguring) &&
            (
            <div className='col-8 text-center'>
                <span style={{color: 'red'}}
                    onClick={() => {logout()}}
                >
                    Ulos
                </span>
            </div>
            )
        }
        <Outlet />
        </div>
    </div>
  )
};

export default Layout;
