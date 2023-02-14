using TetsTaskRestJsonApi.Models;

namespace TetsTaskRestJsonApi.Repository
{
    public interface IPositionRepository
    {
        IEnumerable<Position> GetAll();
        Position? Get(string positionName);
        bool Add(Position value);
        bool Update(string positionName, Position value);
        bool Delete(string positionName);
    }
}
