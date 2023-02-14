using MySql.Data.MySqlClient;
using System.Diagnostics.CodeAnalysis;
using TetsTaskRestJsonApi.Models;

namespace TetsTaskRestJsonApi.Connectors
{
    public class DBEmployeeConnector : IDBEmployeeConnector
    {
        private readonly MySqlConnection _connection;
        private const string _mySqlConnectionString = "server=127.0.0.1;uid=root;pwd=0123456789;database=employeesdb";
        private const string queryEmployeeInfo = @"select emp.employeeId, emp.lastName, emp.firstName, emp.middleName, emp.birthday, pos.name
                                   from `employees` as emp 
                                   left join `employeespositioninfo` as empPosInfo on empPosInfo.employeeId = emp.employeeId
                                   left join `positions` as pos on pos.positionId = empPosInfo.positionId
                                   where emp.lastName=@lastName and emp.firstName=@firstName and emp.middleName=@middleName";
        private const int defaultGrade = 15;

        private class EmployeeMapComparer : IEqualityComparer<Employee>
        {
            public bool Equals(Employee? x, Employee? y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;
                return x.LastName.Equals(y.LastName, StringComparison.Ordinal) &&
                        x.FirstName.Equals(y.FirstName, StringComparison.Ordinal) &&
                        x.MiddleName.Equals(y.MiddleName, StringComparison.Ordinal) &&
                        x.Birthday.Equals(y.Birthday);
            }

            public int GetHashCode([DisallowNull] Employee obj)
            {
                return (obj.LastName.GetHashCode() + obj.FirstName.GetHashCode() + obj.MiddleName + obj.Birthday).GetHashCode();
            }
        }

        public DBEmployeeConnector()
        {
            _connection = new MySqlConnection(_mySqlConnectionString);
        }

        public IEnumerable<EmployeeInfo> ExecuteGetAll()
        {
            try
            {
                var employeesMap = new Dictionary<Employee, List<string>>(new EmployeeMapComparer());
                var employees = new List<EmployeeInfo>();
                _connection.Open();
                using (var connectionCmd = _connection.CreateCommand())
                {
                    connectionCmd.CommandText = @"select emp.employeeId, emp.lastName, emp.firstName, emp.middleName, emp.birthday, pos.name
                                   from `employees` as emp 
                                   left join `employeespositioninfo` as empPosInfo on empPosInfo.employeeId = emp.employeeId
                                   left join `positions` as pos on pos.positionId = empPosInfo.positionId"; 
                    using(var connectionReader = connectionCmd.ExecuteReader())
                    {

                        while(connectionReader.Read())
                        {
                            var employee = new Employee
                            {
                                Id = connectionReader.GetInt32(0),
                                LastName = connectionReader.GetString(1),
                                FirstName = connectionReader.GetString(2),
                                MiddleName = connectionReader.GetString(3),
                                Birthday = connectionReader.GetDateTime(4)
                            };
                            if (!employeesMap.ContainsKey(employee))
                            {
                                employeesMap.Add(employee, new List<string>());
                            }
                            employeesMap[employee].Add(connectionReader.GetString(5));
                        }
                        foreach(var employee in employeesMap.Keys)
                        {
                            var employeeInfo = new EmployeeInfo()
                            {
                                Id = employee.Id,
                                LastName = employee.LastName,
                                FirstName = employee.FirstName,
                                MiddleName = employee.MiddleName,
                                Birthday = employee.Birthday,
                                Positions = employeesMap[employee]
                            };
                            employees.Add(employeeInfo);
                        }

                    }
                }
                return employees;
            }
            catch(MySqlException)
            {
                return Enumerable.Empty<EmployeeInfo>();
            }
            finally
            {
                _connection.Close();
            }
        }

        public EmployeeInfo ExecuteGet(EmployeeRequest employee)
        {
            var employeeInfo = new EmployeeInfo();
            try
            {
                _connection.Open();
                using(var connectionCmd = _connection.CreateCommand())
                {
                    connectionCmd.CommandText = queryEmployeeInfo;
                    connectionCmd.Parameters.AddWithValue("@lastName", employee.LastName);
                    connectionCmd.Parameters.AddWithValue("@firstName", employee.FirstName);
                    connectionCmd.Parameters.AddWithValue("@middleName", employee.MiddleName);
                    using (var connectionReader = connectionCmd.ExecuteReader())
                    {
                        int row = 1;
                        while(connectionReader.Read())
                        {
                            if (row <= 1)
                            {
                                employeeInfo.Id = connectionReader.GetInt32(0);
                                employeeInfo.LastName = connectionReader.GetString(1);
                                employeeInfo.FirstName = connectionReader.GetString(2);
                                employeeInfo.MiddleName = connectionReader.GetString(3);
                                employeeInfo.Birthday = connectionReader.GetDateTime(4);
                            }
                            employeeInfo.Positions.Add(connectionReader.GetString(5));
                            ++row;
                        }
                    }                    
                }
                return employeeInfo;
            }
            catch(MySqlException)
            {
                return employeeInfo;
            }
            finally
            {
                _connection.Close();
            }
        }

        public bool ExecuteAdd(Employee employee)
        {
            try
            {
                _connection.Open();
                using(var connectionTransaction = _connection.BeginTransaction())
                {
                    using (var connectionCmd = _connection.CreateCommand())
                    {
                        connectionCmd.Transaction= connectionTransaction;
                        try
                        {
                            const string insertQuery = "insert into employees (lastName, firstName, middleName, birthday) values(@lastName, @firstName, @middleName, @birthday)";
                            const string checkEmployeeQuery = "select employeeId from employees where lastName=@lastName and firstName=@firstName and middleName=@middleName and birthday=@birthday";
                            connectionCmd.CommandText = checkEmployeeQuery;
                            connectionCmd.Parameters.AddWithValue("@lastName", employee.LastName);
                            connectionCmd.Parameters.AddWithValue("@firstName", employee.FirstName);
                            connectionCmd.Parameters.AddWithValue("@middleName", employee.MiddleName);
                            connectionCmd.Parameters.AddWithValue("@birthday", employee.Birthday.Date);
                            var employeeId = connectionCmd.ExecuteScalar();
                            if (employeeId is null)
                            {
                                connectionCmd.CommandText = insertQuery;
                                connectionCmd.ExecuteNonQuery();
                                employeeId = connectionCmd.LastInsertedId;
                            }
                            connectionCmd.Parameters.Clear();
                            connectionCmd.CommandText = "select positionId from positions where name=@name";
                            connectionCmd.Parameters.AddWithValue("@name", employee.Position);
                            var positionId = connectionCmd.ExecuteScalar();
                            if (positionId is null)
                            {
                                connectionCmd.Parameters.Clear();
                                connectionCmd.CommandText = "insert into positions (name, grade) values (@name, @grade)";
                                connectionCmd.Parameters.AddWithValue("@name", employee.Position);
                                connectionCmd.Parameters.AddWithValue("@grade", defaultGrade);
                                connectionCmd.ExecuteScalar();
                                positionId = connectionCmd.LastInsertedId;
                            }
                            connectionCmd.Parameters.Clear();
                            connectionCmd.CommandText = "insert into employeespositioninfo (positionId, employeeId) values (@positionId, @employeeId)";
                            connectionCmd.Parameters.AddWithValue("@positionId", positionId);
                            connectionCmd.Parameters.AddWithValue("@employeeId", employeeId);
                            connectionCmd.ExecuteNonQuery();
                            connectionTransaction.Commit();
                        }
                        catch(Exception)
                        {
                            connectionTransaction.Rollback();
                            throw;
                        }
                    }                    
                }
                return true;
            }
            catch(MySqlException)
            {
                return false;
            }
            finally
            {
                _connection.Close();
            }
        }

        public bool ExecuteDelete(string query, EmployeeRequest employee)
        {
            try
            {
                _connection.Open();
                using (var connectionTransaction = _connection.BeginTransaction())
                {
                    using (var connectionCmd = _connection.CreateCommand())
                    {
                        connectionCmd.Transaction = connectionTransaction;
                        try
                        {
                            connectionCmd.CommandText = query;
                            connectionCmd.Parameters.AddWithValue("@lastName", employee.LastName);
                            connectionCmd.Parameters.AddWithValue("@firstName", employee.FirstName);
                            connectionCmd.Parameters.AddWithValue("@middleName", employee.MiddleName);
                            var employeeId = connectionCmd.ExecuteScalar();
                            if (employeeId != null)
                            {
                                connectionCmd.Parameters.Clear();
                                connectionCmd.CommandText = "delete from employees where employeeId=@employeeId";
                                connectionCmd.Parameters.AddWithValue("@employeeId", (int)employeeId);
                                connectionCmd.ExecuteNonQuery();
                                connectionCmd.Parameters.Clear();
                                connectionCmd.CommandText = "delete from employeespositioninfo where employeeId=@employeeId";
                                connectionCmd.Parameters.AddWithValue("@employeeId", (int)employeeId);
                                connectionCmd.ExecuteNonQuery();
                                connectionTransaction.Commit();
                                return true;
                            }
                        }
                        catch(Exception)
                        {
                            connectionTransaction.Rollback();
                            throw;
                        }
                    }
                }
                return false;
            }
            catch(MySqlException)
            {
                return false;
            }
            finally
            {
                _connection.Close();
            }
        }

        public bool ExecuteUpdate(EmployeeInfo employeeInfo)
        {
            try
            {
                var dbEmployeeInfo = ExecuteGet(new EmployeeRequest { LastName = employeeInfo.LastName, FirstName = employeeInfo.FirstName, MiddleName = employeeInfo.MiddleName });
                if (dbEmployeeInfo != null)
                {
                    _connection.Open();
                    using (var connectionTransaction = _connection.BeginTransaction())
                    {
                        using (var connectionCmd = _connection.CreateCommand())
                        {
                            connectionCmd.Transaction = connectionTransaction;
                            try
                            {
                                connectionCmd.CommandText = "update employees set lastName=@lastName, firstName=@firstName, middleName=@middleName, birthday=@birthday where employeeId=@id";
                                connectionCmd.Parameters.AddWithValue("@lastName", employeeInfo.LastName);
                                connectionCmd.Parameters.AddWithValue("@firstName", employeeInfo.FirstName);
                                connectionCmd.Parameters.AddWithValue("@middleName", employeeInfo.MiddleName);
                                connectionCmd.Parameters.AddWithValue("@birthday", employeeInfo.Birthday);
                                connectionCmd.Parameters.AddWithValue("@id", dbEmployeeInfo.Id);
                                connectionCmd.ExecuteNonQuery();
                                bool isDeleted = dbEmployeeInfo.Positions.Count > employeeInfo.Positions.Count;
                                var positions = isDeleted ?
                                                dbEmployeeInfo.Positions.Except(employeeInfo.Positions) :
                                                employeeInfo.Positions.Except(dbEmployeeInfo.Positions);
                                var positionsId = new List<int>();
                                connectionCmd.CommandText = "select positionId from positions where name=@name";
                                connectionCmd.Parameters.Add("@name", MySqlDbType.VarString);
                                connectionCmd.Parameters.Add("@employeeId", MySqlDbType.Int32);
                                connectionCmd.Parameters.Add("@positionId", MySqlDbType.Int32);
                                foreach (var position in positions )
                                {
                                    connectionCmd.Parameters["@name"].Value = position;
                                    positionsId.Add((int)connectionCmd.ExecuteScalar());
                                }                             
                                if (isDeleted)
                                {
                                    connectionCmd.CommandText = "delete from employeespositioninfo where employeeId=@employeeId and positionId=@positionId";
                                    foreach (var positionId in positionsId)
                                    {
                                        connectionCmd.Parameters["@employeeId"].Value = dbEmployeeInfo.Id;
                                        connectionCmd.Parameters["@positionId"].Value = positionId;
                                        connectionCmd.ExecuteNonQuery();
                                    }                                    
                                }
                                else
                                {
                                    var newPositions = employeeInfo.Positions.Except(dbEmployeeInfo.Positions);
                                    connectionCmd.CommandText = "insert into employeespositioninfo (positionId, employeeId) values (@positionId, @employeeId)";
                                    foreach (var positionId in positionsId)
                                    {
                                        connectionCmd.Parameters["@positionId"].Value = positionId;
                                        connectionCmd.Parameters["@employeeId"].Value = dbEmployeeInfo.Id;
                                        connectionCmd.ExecuteNonQuery();
                                    }
                                }
                                connectionTransaction.Commit();
                                return true;
                            }
                            catch(MySqlException) 
                            {
                                connectionTransaction.Rollback();
                                throw;
                            }
                        }
                    }
                }
                return false;
            }
            catch(MySqlException)
            {
                return false;
            }
            finally
            {
                _connection.Close();
            }
        }
    }        
}
