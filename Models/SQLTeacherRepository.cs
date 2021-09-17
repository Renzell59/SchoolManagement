using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.Models
{
    public class SQLTeacherRepository : ITeacherRepository
    {
        private readonly ApplicationDBContext _context;

        public SQLTeacherRepository(ApplicationDBContext context)
        {
            this._context = context;
        }
        public Teacher AddTeacher(Teacher teacher)
        {
            if(teacher != null)
            {
                _context.Teachers.Add(teacher);
                _context.SaveChanges();
            }
            return teacher;
        }

        public Teacher DeleteTeacherById(int id)
        {
            Teacher teacher = _context.Teachers.FirstOrDefault(t => t.id == id);
            if(teacher != null)
            {
                _context.Teachers.Remove(teacher);
                _context.SaveChanges();
            }
            return teacher;
        }

        public IEnumerable<Teacher> GetAllTeachers()
        {
            return _context.Teachers;
        }

        public Teacher GetTeacherById(int id)
        {
            return _context.Teachers.FirstOrDefault(t => t.id ==id);
        }

        public Teacher UpdateTeacherById(Teacher updateTeacher)
        {
            EntityEntry<Teacher> teacher =_context.Teachers.Attach(updateTeacher);
            teacher.State = EntityState.Modified;
            _context.SaveChanges();
            return updateTeacher;
        }
    }
}
