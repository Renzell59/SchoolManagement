using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.Models
{
    public class SQLStudentCommentRepository : IStudentCommentRepository
    {
        private readonly ApplicationDBContext _context;

        public SQLStudentCommentRepository(ApplicationDBContext context)
        {
            this._context = context;
        }
        public StudentComment AddStudentComment(StudentComment studentComment)
        {
            if(studentComment != null)
            {
                _context.StudentComments.Add(studentComment);
                _context.SaveChanges();
            }
            return studentComment;
        }

        public StudentComment DeleteStudentCommentById(int id)
        {
            StudentComment studentComment = _context.StudentComments.FirstOrDefault(c => c.ID == id);
            if(studentComment != null)
            {
                _context.StudentComments.Remove(studentComment);
                _context.SaveChanges();
            }
            return studentComment;
        }

        public IEnumerable<StudentComment> GetAllStudentComments()
        {
            return _context.StudentComments;
        }

        public StudentComment GetStudentCommentById(int id)
        {
            return _context.StudentComments.FirstOrDefault(c => c.ID == id);
        }

        public StudentComment UpdateStudentComment(StudentComment updateStudentComment)
        {
            if(updateStudentComment != null)
            {
                EntityEntry<StudentComment> studentComment = _context.StudentComments.Attach(updateStudentComment);
                studentComment.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.SaveChanges();
            }
            return updateStudentComment;
        }
    }
}
