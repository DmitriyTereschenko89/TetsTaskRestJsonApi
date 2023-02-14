using System.Text.Json.Serialization;

namespace TetsTaskRestJsonApi.Models
{
    public class EmployeeInfo
    {
        public EmployeeInfo()
        {
            Positions = new List<string>();
        }

        // <summary>
        // Employee id
        // </summary>
        [JsonIgnore]
        public int Id { get; set; }
        // <summary>
        // Employee last name
        // </summary>
        public string? LastName { get; set; }

        // <summary>
        // Employee first name
        // </summary>
        public string? FirstName { get; set; }

        // <summary>
        // Employee middle name
        // </summary>
        public string? MiddleName { get; set; }

        // <summary>
        // Employee's positions
        // </summary>
        public List<string> Positions { get; set; }

        // <summary>
        // Employee birthday
        // </summary>
        public DateTime Birthday { get; set; }
    }
}
