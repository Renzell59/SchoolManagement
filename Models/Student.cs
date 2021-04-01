using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchoolManagement.Models
{
    public class Student
    {
        [JsonInclude]
        public int id { get; set; }

        [JsonInclude]
        public string firstName { get; set; }

        [JsonInclude]
        public string middleName { get; set; }

        [JsonInclude]
        public string lastName { get; set; }

        [JsonInclude]
        public string email { get; set; }

        [JsonInclude]
        public int? age { get; set; }
        [JsonInclude]
        public string entryYear { get; set; }

        [JsonInclude]
        public int? grade { get; set; }

        public string phoneNumber { get; set; }

        [JsonInclude]
        public string gender { get; set; }

        [JsonInclude]
        public Semester? season { get; set; }

        [JsonInclude]
        public string photoPath { get; set; }
        [JsonInclude]
        public DateTime birthDate { get; set; }

        [JsonInclude]
        public string address { get; set; }
        [JsonInclude]
        public string city { get; set; }

        [JsonInclude]
        public string state { get; set; }
    }
}
