using InterviewTest.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace InterviewTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmployeesController : ControllerBase
    {
        [HttpGet]
        public List<Employee> Get()
        {
            var employees = new List<Employee>();

            var connectionStringBuilder = new SqliteConnectionStringBuilder() { DataSource = "./SqliteDB.db" };
            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                var queryCmd = connection.CreateCommand();
                queryCmd.CommandText = @"SELECT Name, Value FROM Employees";
                using (var reader = queryCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employees.Add(new Employee
                        {
                            Name = reader.GetString(0),
                            Value = reader.GetInt32(1)
                        });
                    }
                }
            }

            return employees;
        }
        [HttpDelete("{value}")]
        public IActionResult Delete(int value)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder() { DataSource = "./SqliteDB.db" };
            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                var deleteCmd = connection.CreateCommand();
                deleteCmd.CommandText = @"DELETE FROM Employees WHERE Value = @value";
                deleteCmd.Parameters.AddWithValue("@value", value);

                int rowsAffected = deleteCmd.ExecuteNonQuery();
                
                if (rowsAffected > 0)
                {
                    return Ok(new { message = "Employee deleted successfully" });
                }
                else
                {
                    return NotFound(new { message = "Employee not found" });
                }
            }
        }

        [HttpPost]
        public IActionResult Add([FromBody] Employee newEmployee)
        {
            if (newEmployee == null || string.IsNullOrEmpty(newEmployee.Name) || newEmployee.Value <= 0)
            {
                return BadRequest(new { message = "Invalid employee data." });
            }

            var connectionStringBuilder = new SqliteConnectionStringBuilder() { DataSource = "./SqliteDB.db" };
            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                var insertCmd = connection.CreateCommand();
                insertCmd.CommandText = @"INSERT INTO Employees (Name, Value) VALUES (@name, @value)";
                insertCmd.Parameters.AddWithValue("@name", newEmployee.Name);
                insertCmd.Parameters.AddWithValue("@value", newEmployee.Value);

                int rowsAffected = insertCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "Employee added successfully" });
                }
                else
                {
                    return StatusCode(500, new { message = "An error occurred while adding the employee." });
                }
            }
        }
        [HttpPut("{value}")]
        public IActionResult Update(int value, [FromBody] Employee updatedEmployee)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder() { DataSource = "./SqliteDB.db" };
            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                var updateCmd = connection.CreateCommand();
                updateCmd.CommandText = @"UPDATE Employees SET Name = @name, Value = @newValue WHERE Value = @value";
                updateCmd.Parameters.AddWithValue("@name", updatedEmployee.Name);
                updateCmd.Parameters.AddWithValue("@newValue", updatedEmployee.Value);  // Update value as well
                updateCmd.Parameters.AddWithValue("@value", value);  // Original value to match

                int rowsAffected = updateCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    return Ok(new { message = "Employee updated successfully" });
                }
                else
                {
                    return NotFound(new { message = "Employee not found" });
                }
            }
        }


        [HttpPost("increment-values")]
        public IActionResult IncrementValues()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder() { DataSource = "./SqliteDB.db" };
            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                var updateCmd = connection.CreateCommand();
                updateCmd.CommandText = @"UPDATE Employees
                                        SET Value = CASE
                                            WHEN Name LIKE 'E%' THEN Value + 1
                                            WHEN Name LIKE 'G%' THEN Value + 10
                                            ELSE Value + 100
                                        END";
                updateCmd.ExecuteNonQuery();
            }

            return Ok(new { message = "Values updated successfully" });
        }

        [HttpGet("sum-values")]
        public IActionResult SumValues()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder() { DataSource = "./SqliteDB.db" };
            using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                connection.Open();

                var queryCmd = connection.CreateCommand();
                queryCmd.CommandText = @"
                    SELECT SUBSTR(Name, 1, 1) AS StartingLetter, SUM(Value) AS TotalValue
                    FROM Employees
                    WHERE Name LIKE 'A%' OR Name LIKE 'B%' OR Name LIKE 'C%'
                    GROUP BY SUBSTR(Name, 1, 1)
                    HAVING SUM(Value) >= 11171";
                
                using (var reader = queryCmd.ExecuteReader())
                {
                    var results = new List<object>();

                    while (reader.Read())
                    {
                        results.Add(new
                        {
                            StartingLetter = reader.GetString(0),
                            TotalValue = reader.GetInt32(1)
                        });
                    }

                    if (results.Count > 0)
                    {
                        return Ok(results);
                    }
                    else
                    {
                        return NotFound(new { message = "No data found" });
                    }
                }
            }
        }

    [HttpGet("sum-values-a")]
public IActionResult SumValuesStartingWithA()
{
    var connectionStringBuilder = new SqliteConnectionStringBuilder() { DataSource = "./SqliteDB.db" };
    using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
    {
        connection.Open();

        var queryCmd = connection.CreateCommand();
        queryCmd.CommandText = @"
            SELECT SUM(Value) AS TotalValue
            FROM Employees
            WHERE Name LIKE 'B%'";
        
        using (var reader = queryCmd.ExecuteReader())
        {
            if (reader.Read())
            {
                var totalValue = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                return Ok(new { totalValue });
            }
            else
            {
                return NotFound(new { message = "No data found" });
            }
        }
    }
}



    
    }
}
