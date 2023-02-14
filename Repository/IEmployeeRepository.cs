using TetsTaskRestJsonApi.Models;

namespace TetsTaskRestJsonApi.Repository
{
    public interface IEmployeeRepository
    {
        IEnumerable<EmployeeInfo> GetAll();
        EmployeeInfo? Get(EmployeeRequest employee);
        bool Add(Employee employee);
        bool Update(EmployeeInfo employee);
        bool Delete(EmployeeRequest employee);
    }
}
