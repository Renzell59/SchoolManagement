using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.Models
{
    public interface IStudentRepository
    {
        IEnumerable<Student> GetAllStudents();
        Student GetStudentByID(int Id);
        Student DeleteStudentByID(int Id);
        Student UpdateStudent(Student studentChange);
        Student AddStudent(Student student);
    }
}
