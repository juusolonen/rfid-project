import React from 'react'

export default function ToolTable(props) {


    return(

        <table className="table table-striped table-hover">
          <thead className="table-dark">
            <tr>
              <th scope="col">ID</th>
              <th scope="col">Tag ID</th>
              <th scope="col">Työkalu</th>
              <th scope="col">Paikka</th>
              <th scope="col"></th>
            </tr>
          </thead>
          <tbody>
            { props.tools &&
              props.tools.map(tool => (
                <tr>
                  <th scope="row">{tool.id}</th>
                  <td>{tool.tag_id}</td>
                  <td>{tool.name}</td>
                  <td>{tool.slot}</td>
                  <td onClick={() => props.onDelete(tool.id)}>
                    <span className="red-x">&times;</span>
                  </td>
                </tr>
              ))
            }
          </tbody>
      </table>
    )
}