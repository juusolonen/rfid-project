import React, {useContext} from 'react'
import UserContext from '../context/UserContext';


export default function ToolShelf() {

    const {toolShelf, setToolShelf} = useContext(UserContext);

    const cancelSelection = (idx) => {
      const tools = JSON.parse(JSON.stringify(toolShelf));
      tools[idx].outgoing = false;
      tools[idx].incoming = false;
      setToolShelf([...tools])
    }

    const getRowStyle = (tool) => {
      return tool.outgoing 
              ? "outgoing-row"
              : tool.incoming ? "incoming-row"
                : !tool.present ? "disabled-row" : "";
    }

    return(

        <table className="table table-striped table-hover">
          <thead className="table-dark">
            <tr>
              <th scope="col">Paikka</th>
              <th scope="col">Työkalu</th>
              <th scope="col"></th>
            </tr>
          </thead>
          <tbody>
            { toolShelf &&
              toolShelf.map((tool, idx) => (
                <tr key={tool.slot} 
                    className={getRowStyle(tool)}>
                  <th scope="row">{tool.slot}</th>
                  <td>{tool.name}</td>
                  {
                    tool.outgoing || tool.incoming
                    ? <td onClick={() => cancelSelection(idx)}>
                        <span className="red-x">&times;</span>
                      </td> 
                  : <td></td>
                  }
                </tr>
              ))
            }
          </tbody>
      </table>
    )
}