import React, {useState, useEffect, useContext} from 'react'
import AdminContext from '../context/AdminContext';
import AdminCreator from '../components/AdminCreator';
import { Link, useNavigate, Outlet, useLocation } from 'react-router-dom';

export default function Confiruration(props) {

  const {adminExists, started, setStarted} = useContext(AdminContext)
  const location = useLocation()
  const isSubPage = location.pathname !== '/conf'
  const navigate = useNavigate()

    useEffect(() => {
      if (!started) {
        navigate("/")
      }
    }, [])


    if (!adminExists && started) {
      return (
        <AdminCreator socket={props.socket}/>
      )
    }

    return (
      <>
        {
          isSubPage ?
          <>
          <div className='col-8 text-center'>
              <Link to="/conf">
                Takaisin
              </Link>
            </div>
            <div className='col-8 text-center'>
            </div>
          </>
          :
          <>
            <div className='col-8 text-center'>
              <Link to="users">
                Käyttäjät
              </Link>
            </div>
            <div className='col-8 text-center'>
              <Link to="tools">
                Työkalut
              </Link>
            </div>
            <div className='col-8 text-center'>
              <Link to="/conf/borrow">
                Lainausnäkymä
              </Link>
            </div>
          </>
        }

  
        <Outlet />
      </>
    )

}