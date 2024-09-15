import React, { Component, useState, useEffect } from 'react';
import Employee from './components/Employee';
import './App.css'
// export default class App extends Component {

//   render () {
//     return (
//       <div>Complete your app here</div>
//     );
//   }
// }

function App(){
  const [employees, setEmployees] = useState([])
  const [name, setName] = useState('')
  const [sumValues, setSumValues] = useState([])
  const [show, setShow] = useState(false)

  //remove employee
  const remove = async (value) => {
    await fetch(`http://localhost:5000/Employees/${value}`,{
      method:'DELETE'
    })
    setEmployees(employees.filter((employee) => employee.value != value))
  }

  //add employee
  const handleSubmit = async (e) => {
    if(name === ''){
      alert('enter a name')
    }else{
      e.preventDefault()

      const val = Math.floor(Math.random() * (10000 + 1)) + 10000;
  
      const newEmployee = {name:name, value: val}
  
      try{
        const res = await fetch('http://localhost:5000/Employees',{
          method: 'POST',
          headers:{
            'Content-type': 'application/json',
          },
          body: JSON.stringify(newEmployee),
        })
      
        const data = await res.json()
        
        setEmployees([...employees, newEmployee])
        setName('')
      }catch(error){
        console.error("error:" ,error)
      }
    }


  }

  //load all employees
  useEffect(() => {
    const fetchEmployees = async () => {
      try{
        const res = await fetch('http://localhost:5000/Employees')
        const data = await res.json()
        console.log(data)
        setEmployees(data)
      }catch(error){
        console.error('Error fetching employees:', error)
      }
  
    }
    fetchEmployees()
  },[])

//update employee name and value
const update = async (value, newName, newValue) => {
  try{
    const updatedEmployee = {name:newName, value:newValue}
    const res = await fetch(`http://localhost:5000/Employees/${value}`, {
          method: 'PUT',
          headers: {
              'Content-Type': 'application/json',
          },
          body: JSON.stringify(updatedEmployee),
      });

      if (!res.ok) {
          throw new Error(`Failed to update employee with value: ${value}. Status: ${res.status}`);
      }

      const data = await res.json();

      setEmployees( employees.map((employee) => (employee.value === value ? {...employee, name: newName, value:newValue} : employee)))
  }catch(error){
    console.log("Error: " + error)
  }
 
}
//increment employee values
const incrementValues = async () => {
 
  try{
    await fetch('http://localhost:5000/Employees/increment-values', { method: 'POST' });
  }catch(error){
    console.log('Error: ' + error)
  }

  setEmployees(employees.map((employee) => {
    if(employee.name.startsWith('E')){
      return { ...employee, value: employee.value + 1 };
    }else if(employee.name.startsWith('G')){
      return { ...employee, value: employee.value + 10 };
    }else{
      return { ...employee, value: employee.value + 100 };
    }
  }))
  
};

//sum values
const fetchSumValues = async () => {
  setShow(true)
  try {
      const response = await fetch('http://localhost:5000/Employees/sum-values');
      
      if (response.ok) {
          const data = await response.json();
          setSumValues(data)
          console.log('Alphabet Contributions:', data);

      } else {
          const error = await response.text();
          console.log('Server Error:', error);
      }
  } catch (error) {
      console.log('Error:', error);
  }
};

const fetchSumOfA = async () => {
  try {
    const response = await fetch('http://localhost:5000/Employees/sum-values-a');
    
    if (response.ok) {
        const data = await response.json();
        console.log('Sum A:', data);

    } else {
        // Handle server errors
        const error = await response.text();
        console.log('Server Error:', error);
    }
} catch (error) {
    console.log('Error:', error);
}
}

  return(
    <div className="container">
      <h3 className='center'>Metricell Interview Answers</h3>
      <h4 className='center'>List of Employees</h4> 
      <form onSubmit={handleSubmit} className="form-name">
        <input className='name-input'  placeholder='Name' value={name} onChange={(e) => setName(e.target.value)}/>
        <button className='name-button'>Add Name</button>
      </form>

      <div className="ctas">
        <button onClick={incrementValues}>Increment Values</button>
        <button onClick={fetchSumValues}>Fetch Sum</button>
        {/* <button onClick={fetchSumOfA}>SUM of A</button> */}
      </div>
    
      <div className="sum-values">
        {show && (<p className='center'>Sum of values greater than or equal to 11171 considering A,B,C</p>)}
        <div className='center'>
        {sumValues.map((sum) => (<div key={sum.startingLetter}>{sum.startingLetter}-{sum.totalValue}</div>))}
        </div>
        
      </div>
      <table>
        <tbody>
        {employees.map((employee) => <Employee key={employee.value} employee={employee} remove={remove} update={update}/>)}
        </tbody>
          
      </table>
     
    </div>
  )
}

export default App;