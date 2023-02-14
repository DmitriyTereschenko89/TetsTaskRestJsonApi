using TetsTaskRestJsonApi.Models;

namespace TetsTaskRestJsonApi.Connectors
{
    public interface IDBEmployeeConnector
    {
        IEnumerable<EmployeeInfo> ExecuteGetAll();
        EmployeeInfo? ExecuteGet(EmployeeRequest employee);
        bool ExecuteAdd(Employee value);
        bool ExecuteDelete(string query, EmployeeRequest employee);
        bool ExecuteUpdate(EmployeeInfo newEmployeeInfo);
    }
}
