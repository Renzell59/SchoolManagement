using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.Models
{
    interface ITeacherRepository
    {
        public IEnumerable<Teacher> GetAllTeachers();
        public Teacher GetTeacherById(int id);
        public Teacher AddTeacher(Teacher teacher);
        public Teacher DeleteTeacherById(int id);
        public Teacher UpdateTeacherById(Teacher updateTeacher);
    }
}
