using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.Models
{
    interface IStudentCommentRepository
    {
        IEnumerable<StudentComment> GetAllStudentComments();
        StudentComment GetStudentCommentById(int id);
        StudentComment AddStudentComment(StudentComment studentComment);
        StudentComment DeleteStudentCommentById(int id);
        StudentComment UpdateStudentComment(StudentComment updateStudentComment);
    }
}
