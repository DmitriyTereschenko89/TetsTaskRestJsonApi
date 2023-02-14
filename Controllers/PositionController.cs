using Microsoft.AspNetCore.Mvc;
using TetsTaskRestJsonApi.Models;
using TetsTaskRestJsonApi.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TetsTaskRestJsonApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionController : ControllerBase
    {
        private readonly IPositionRepository _repository;

        public PositionController(IPositionRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Position
        [HttpGet]
        public IEnumerable<Position> Get()
        {
            return _repository.GetAll();
        }

        // GET api/Position/5
        [HttpGet("{positionName}")]
        public Position? Get(string positionName)
        {
            return _repository.Get(positionName);
        }

        // POST api/Position
        [HttpPost]
        public bool Post([FromBody] Position value)
        {
            return _repository.Add(value);
        }

        // PUT api/Position/5
        [HttpPut("{positionName}")]
        public bool Put(string positionName, [FromBody] Position value)
        {
            return _repository.Update(positionName, value);
        }

        // DELETE api/Position/5
        [HttpDelete("{positionName}")]
        public bool Delete(string positionName)
        {
            return _repository.Delete(positionName);
        }
    }
}
