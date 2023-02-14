using TetsTaskRestJsonApi.Models;

namespace TetsTaskRestJsonApi.Connectors
{
    public interface IDBPositionConnector
    {
        IEnumerable<Position> ExecuteGetAll(string query);
        Position? ExecuteGet(string query, string positionName);
        bool ExecuteAdd(string query, Position value);
        bool ExecuteDelete(string query, string positionName);
        bool ExecuteUpdate(string query, string positionName, Position value);
    }
}
