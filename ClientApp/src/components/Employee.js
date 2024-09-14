import React, { Component, useState, useEffect } from 'react';
function Employee({ employee, remove, update }){
    const [isEditing, setIsEditing] = useState(false);
    const [newName, setNewName] = useState(employee.name); 
    const [newValue, setNewValue] = useState(employee.value);

  
    const handleUpdate = () => {
        update(employee.value, newName, newValue)
        setIsEditing(false)
    }

    return(
        <>
            {isEditing ?( <tr>
                    <td>
                    <input 
                        type="text" 
                        value={newName}
                        onChange={(e) => setNewName(e.target.value)}
                    />
                    </td>
                    <td>
                    <input 
                        type="number" 
                        value={newValue}
                        onChange={(e) => setNewValue(e.target.value)}
                    />
                    </td>
                    <td>
                        <button onClick={handleUpdate}>Save</button>
                    </td>
                    <td>
                        <button onClick={() => setIsEditing(false)}>Cancel</button>
                    </td>
            </tr>): 
                ( 
                        <tr>
                            <td>{employee.name}</td>
                            <td>{employee.value}</td>
                            <td><button onClick={() => setIsEditing(true)}>Edit</button></td>
                            <td><button onClick={() => remove(employee.value)}>Remove</button></td>
                        </tr>      

                )}
           
        </>
    )
}

export default Employee