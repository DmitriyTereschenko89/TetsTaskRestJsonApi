using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using TetsTaskRestJsonApi.Models;
using TetsTaskRestJsonApi.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TetsTaskRestJsonApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeController(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Employee
        [HttpGet]
        public IEnumerable<EmployeeInfo> Get()
        {
            return _repository.GetAll();
        }

        // GET api/Employee/5
        [HttpGet("{lastName}/{firstName}/{middleName}")]
        public EmployeeInfo? Get(string lastName, string firstName, string middleName)
        {
            return _repository.Get(new EmployeeRequest { FirstName = firstName, LastName = lastName, MiddleName = middleName });
        }

        // POST api/Employee
        [HttpPost]
        public bool Post([FromBody] Employee employee)
        {
            return _repository.Add(employee);
        }

        // PUT api/Employee/5
        [HttpPut]
        public bool Put([FromBody] EmployeeInfo employee)
        {
            return _repository.Update(employee);
        }

        // DELETE api/Employee/5
        [HttpDelete]
        public bool Delete([FromBody] EmployeeRequest employeeDelete)
        {
            return _repository.Delete(employeeDelete);
        }
    }
}
