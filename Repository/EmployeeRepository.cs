using MySql.Data.MySqlClient;
using TetsTaskRestJsonApi.Connectors;
using TetsTaskRestJsonApi.Models;

namespace TetsTaskRestJsonApi.Repository
{
    public sealed class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDBEmployeeConnector _connector;

        public EmployeeRepository(IDBEmployeeConnector connector)
        {
            _connector = connector;
        }

        public bool Add(Employee employee)
        {
            return _connector.ExecuteAdd(employee);
        }

        public bool Delete(EmployeeRequest employee)
        {
            const string query = "select employeeId from employees where lastName=@lastName and firstName=@firstName and middleName=@middleName";
            return _connector.ExecuteDelete(query, employee);
        }

        public EmployeeInfo? Get(EmployeeRequest employee)
        {
            return _connector.ExecuteGet(employee);
        }

        public IEnumerable<EmployeeInfo> GetAll()
        {
            return _connector.ExecuteGetAll();
        }

        public bool Update(EmployeeInfo employee)
        {
            return _connector.ExecuteUpdate(employee);
        }
    }
}
