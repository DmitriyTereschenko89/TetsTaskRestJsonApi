using System.Text.Json.Serialization;

namespace TetsTaskRestJsonApi.Models
{
    public sealed class Employee
    {

        // <summary>
        // Employee id
        // </summary>
        [JsonIgnore]
        public int Id { get; set; }
        // <summary>
        // Employee last name
        // </summary>
        public string LastName { get; set; }

        // <summary>
        // Employee first name
        // </summary>
        public string FirstName { get; set; }

        // <summary>
        // Employee middle name
        // </summary>
        public string MiddleName { get; set; }

        public string Position { get; set; }

        // <summary>
        // Employee birthday
        // </summary>
        public DateTime Birthday { get; set; }
    }
}
