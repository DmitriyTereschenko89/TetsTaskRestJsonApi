using MySql.Data.MySqlClient;
using TetsTaskRestJsonApi.Connectors;
using TetsTaskRestJsonApi.Models;

namespace TetsTaskRestJsonApi.Repository
{
    public class PositionRepository : IPositionRepository
    {
        private readonly IDBPositionConnector _connector;

        public PositionRepository(IDBPositionConnector connector)
        {
            _connector = connector;
        }

        public bool Add(Position newPosition)
        {
            const string query = "insert into positions (`name`, `grade`) values (@name, @grade)";
            return _connector.ExecuteAdd(query, newPosition);
        }

        public bool Delete(string positionName)
        {
            const string query = "delete from positions where name = @name";
            return _connector.ExecuteDelete(query, positionName);
        }

        public Position? Get(string positionName)
        {
            const string query = "select positionId, name, grade from positions where name=@name";
            return _connector.ExecuteGet(query, positionName);
        }

        public IEnumerable<Position> GetAll()
        {
            const string query = "select positionId, name, grade from positions";
            return _connector.ExecuteGetAll(query);
        }

        public bool Update(string positionName, Position value)
        {
            const string query = "update positions set ";
            return _connector.ExecuteUpdate(query, positionName, value);            
        }
    }
}
