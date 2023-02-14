using System.Text.Json.Serialization;

namespace TetsTaskRestJsonApi.Models
{
    public sealed class Position
    {
        public Position(int id = int.MinValue, int grade = int.MinValue)
        {
            Id = id;
            Name = string.Empty;
            Grade = grade;
        }
        // <summary>
        // Position id
        // </summary>
        [JsonIgnore]
        public int Id { get; set; }
        // <summary>
        // Position name
        // </summary>
        public string Name { get; set; }
        
        // <summary>
        // Position grade
        // </summary>
        public int Grade { get; set; }
    }
}
