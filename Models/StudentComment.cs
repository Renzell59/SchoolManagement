using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.Models
{
    public class StudentComment
    {
        public int ID { get; set; }
        public Student Student { get; set; }
        public Teacher Teacher { get; set; }
        public DateTime CommentDate { get; set; }
    }
}
