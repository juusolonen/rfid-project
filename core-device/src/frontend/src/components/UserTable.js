import React from 'react'
import 'bootstrap/dist/css/bootstrap.css';
import '../App.css'

export default function UserTable(props) {

    return(

        <table className="table table-striped table-hover">
          <thead className="table-dark">
            <tr>
              <th scope="col">ID</th>
              <th scope="col">Tag ID</th>
              <th scope="col">Nimi</th>
              <th scope="col"></th>
            </tr>
          </thead>
          <tbody>
            { props.users &&
              props.users.map(user => (
                <tr>
                  <th scope="row">{user.id}</th>
                  <td>{user.tag_id}</td>
                  <td>{user.name}</td>
                  <td onClick={() => props.onDelete(user.id)}>
                    <span className="red-x">&times;</span>
                  </td>
                </tr>
              ))
            }
          </tbody>
      </table>
    )
}