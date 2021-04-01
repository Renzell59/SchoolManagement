using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.Models
{
    public class SQLStudentRepository:IStudentRepository
    {
        private readonly ApplicationDBContext _context;

        public SQLStudentRepository(ApplicationDBContext context)
        {
            this._context = context;
        }

        public Student AddStudent(Student student)
        {
            if(student != null)
            {
                _context.Students.Add(student);
                _context.SaveChanges();
            }
            return student;
        }

        public Student DeleteStudentByID(int Id)
        {
           Student student = _context.Students.Find(Id);
            if(student != null)
            {
                _context.Students.Remove(student);
                _context.SaveChanges();
            }
            return student;
        }

        public IEnumerable<Student> GetAllStudents()
        {

            return _context.Students;
        }

        public Student GetStudentByID(int Id)
        {
            Student student = _context.Students.Find(Id);
            return student;
        }

        public Student UpdateStudent(Student studentChange)
        {
            EntityEntry<Student> student = _context.Students.Attach(studentChange);
            student.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();

            return studentChange;
        }
    }
}
