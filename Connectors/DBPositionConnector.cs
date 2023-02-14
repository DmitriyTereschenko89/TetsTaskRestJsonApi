using MySql.Data.MySqlClient;
using System.Text;
using TetsTaskRestJsonApi.Models;

namespace TetsTaskRestJsonApi.Connectors
{
    public sealed class DBPositionConnector : IDBPositionConnector
    {
        private readonly MySqlConnection _connection;
        private const string _mySqlConnectionString = "server=127.0.0.1;uid=root;pwd=0123456789;database=employeesdb";
        private const string _defaultNameValue = "string";

        public DBPositionConnector()
        {
            _connection = new MySqlConnection(_mySqlConnectionString);
        }

        public IEnumerable<Position> ExecuteGetAll(string query)
        {
            try
            {
                _connection.Open();
                var positions = new List<Position>();
                using (var connectionCmd = _connection.CreateCommand())
                {
                    connectionCmd.CommandText = query;
                    using (var connectionReader = connectionCmd.ExecuteReader())
                    {
                        while (connectionReader.Read())
                        {
                            var position = new Position
                            {
                                Id = connectionReader.GetInt32(0),
                                Name = connectionReader.GetString(1),
                                Grade = connectionReader.GetInt32(2)
                            };
                            positions.Add(position);
                        }
                    }
                }
                return positions;
            }
            catch (MySqlException)
            {
                return Enumerable.Empty<Position>();
            }
            finally
            {
                _connection.Close();
            }
        }

        public Position? ExecuteGet(string query, string positionName)
        {
            try
            {
                var position = new Position();
                _connection.Open();
                using (var connectionCmd = _connection.CreateCommand())
                {
                    connectionCmd.CommandText = query;
                    connectionCmd.Parameters.AddWithValue("@name", positionName);
                    using (var connectionReader = connectionCmd.ExecuteReader())
                    {
                        while (connectionReader.Read())
                        {
                            position.Id = connectionReader.GetInt32(0);
                            position.Name = connectionReader.GetString(1);
                            position.Grade = connectionReader.GetInt32(2);
                        }
                    }
                }
                return position;
            }
            catch (MySqlException)
            {
                return null;
            }
            finally
            {
                _connection.Close();
            }
        }

        public bool ExecuteAdd(string query, Position position)
        {
            try
            {
                if (position.Name.Equals(_defaultNameValue) || position.Grade < 1 || position.Grade > 15)
                {
                    return false;
                }
                _connection.Open();
                using (var connectionCmd = _connection.CreateCommand())
                {
                    connectionCmd.CommandText = query;
                    connectionCmd.Parameters.AddWithValue("@name", position.Name);
                    connectionCmd.Parameters.AddWithValue("@grade", position.Grade);
                    connectionCmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (MySqlException)
            {
                return false;
            }
            finally
            {
                _connection.Close();
            }
        }

        public bool ExecuteUpdate(string query, string positionName, Position position)
        {
            try
            {
                if (position.Grade == default && position.Name.Equals(_defaultNameValue))
                {
                    return false;
                }
                _connection.Open();
                var queryBuilder = new StringBuilder(query);
                var queryParameters = new List<string>();
                using (var connectionCmd = _connection.CreateCommand())
                {

                    if (!position.Name.Equals(_defaultNameValue))
                    {
                        queryParameters.Add("name=@name");
                        connectionCmd.Parameters.AddWithValue("@name", position.Name);
                    }
                    if (position.Grade != default)
                    {
                        queryParameters.Add("grade=@grade");
                        connectionCmd.Parameters.AddWithValue("@grade", position.Grade);
                    }
                    queryBuilder.Append(string.Join(",", queryParameters));
                    queryBuilder.Append(" where name=@name");
                    connectionCmd.Parameters.AddWithValue("@name", positionName);
                    connectionCmd.CommandText = queryBuilder.ToString();
                    connectionCmd.ExecuteNonQuery();
                }
                return true;
            }
            catch (MySqlException)
            {
                return false;
            }
            finally
            {
                _connection.Close();
            }
        }

        public bool ExecuteDelete(string query, string positionName)
        {
            try
            {
                _connection.Open();
                using (var connectionCmd = _connection.CreateCommand())
                {
                    connectionCmd.CommandText = "select positionId from positions where name=@name";
                    connectionCmd.Parameters.AddWithValue("@name", positionName);
                    var positionId = connectionCmd.ExecuteScalar();
                    if (positionId != null)
                    {
                        connectionCmd.CommandText = $"select positionId from employeespositioninfo where positionId={positionId}";
                        var linkPositionCount = connectionCmd.ExecuteScalar();
                        if (linkPositionCount is null)
                        {
                            connectionCmd.CommandText = query;
                            connectionCmd.ExecuteNonQuery();
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (MySqlException)
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
