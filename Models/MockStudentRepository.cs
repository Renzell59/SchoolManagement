using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.Models
{
    public class MockStudentRepository : IStudentRepository
    {
        private readonly List<Student> _studentTempList;
        private readonly List<Student> _studentList;

        public MockStudentRepository()
        {
            //_studentList = new List<Student>()
            //{
            //    new Student() { id = 101, firstName = "Mark", middleName = "MOO", lastName = "MEE", email = "Mark@m.com", birthDate="1993-06-03", age = 20, entryYear ="2021", grade=10, phoneNumber="(609)380-8198", season=Semester.Fall, gender = "Male" },
            //    new Student() { id = 102, firstName = "Mary", middleName = "MAY", lastName = "MEE", email = "Mary@m.com", birthDate="1993-03-30", age = 25, entryYear ="2021", grade=11, phoneNumber="(387)678-321", season=Semester.Spring, gender = "Female" },
            //    new Student() { id = 103, firstName = "John", middleName = "JILL", lastName = "MEE", email = "John@m.com", birthDate="1994-02-16", age = 29, entryYear ="2020", grade=12, phoneNumber="(321)213-8198", season=Semester.Summer, gender = "Male" },
            //    new Student() { id = 104, firstName = "Mark", middleName = "MOO", lastName = "MEE", email = "Mark@m.com", birthDate="1994-02-23", age = 20, entryYear ="2020", grade=9, phoneNumber="(321)674-8198", season=Semester.Spring, gender = "Male" },
            //    new Student() { id = 105, firstName = "Mary", middleName = "MAY", lastName = "MEE", email = "Mary@m.com", birthDate="1994-09-01", age = 25, entryYear ="2019", grade=11, phoneNumber="(609)380-765", season=Semester.Fall, gender = "Female" },
            //    new Student() { id = 106, firstName = "John", middleName = "JILL", lastName = "MEE", email = "John@m.com", birthDate="1994-12-01", age = 29, entryYear ="2021", grade=12, phoneNumber="(201)213-8198", season=Semester.Spring, gender = "Male" },
            //    new Student() { id = 107, firstName = "Mark", middleName = "MOO", lastName = "MEE", email = "Mark@m.com", birthDate="1996-02-20", age = 20, entryYear ="2019", grade=9, phoneNumber="(973)654-5765", season=Semester.Fall, gender = "Male" },
            //    new Student() { id = 108, firstName = "Mary", middleName = "MAY", lastName = "MEE", email = "Mary@m.com", birthDate="1996-09-03", age = 25, entryYear ="2018", grade=11, phoneNumber="(609)123-6755", season=Semester.Spring, gender = "Female" },
            //    new Student() { id = 109, firstName = "John", middleName = "JILL", lastName = "MEE", email = "John@m.com", birthDate="1996-12-17", age = 29, entryYear ="2021", grade=11, phoneNumber="(201)876-8765", season=Semester.Fall, gender = "Male" },
            //    new Student() { id = 110, firstName = "Mark", middleName = "MOO", lastName = "MEE", email = "Mark@m.com", birthDate="1996-12-24", age = 20, entryYear ="2020", grade=12, phoneNumber="(609)356-3214", season=Semester.Summer, gender = "Male" },
            //    new Student() { id = 111, firstName = "Mary", middleName = "MAY", lastName = "MEE", email = "Mary@m.com", birthDate="1996-12-25", age = 25, entryYear ="2020", grade=10, phoneNumber="(543)456-5325", season=Semester.Fall, gender = "Female" },
            //    new Student() { id = 112, firstName = "John", middleName = "JILL", lastName = "MEE", email = "John@m.com", birthDate="1997-01-02", age = 29, entryYear ="2021", grade=10, phoneNumber="(609)765-8198", season=Semester.Spring, gender = "Male" },
            //    new Student() { id = 113, firstName = "Mark", middleName = "MOO", lastName = "MEE", email = "Mark@m.com", birthDate="1997-03-19", age = 20, entryYear ="2019", grade=9, phoneNumber="(321)380-8293", season=Semester.Spring, gender = "Male" },
            //    new Student() { id = 114, firstName = "Mary", middleName = "MAY", lastName = "MEE", email = "Mary@m.com", birthDate="1997-05-29", age = 25, entryYear ="2019", grade=9, phoneNumber="(421)568-5677", season=Semester.Spring, gender = "Female" },
            //    new Student() { id = 115, firstName = "John", middleName = "JILL", lastName = "MEE", email = "John@m.com", birthDate="1997-07-17", age = 29, entryYear ="2018", grade=9, phoneNumber="(973)380-2134", season=Semester.Fall, gender = "Male" },
            //    new Student() { id = 116, firstName = "Mark", middleName = "MOO", lastName = "MEE", email = "Mark@m.com", birthDate="1997-10-21", age = 20, entryYear ="2021", grade=12, phoneNumber="(201)568-8198", season=Semester.Summer, gender = "Male" },
            //    new Student() { id = 117, firstName = "Mary", middleName = "MAY", lastName = "MEE", email = "Mary@m.com", birthDate="1998-01-14", age = 25, entryYear ="2018", grade=12, phoneNumber="(998)744-7665", season=Semester.Fall, gender = "Female" },
            //    new Student() { id = 118, firstName = "John", middleName = "JILL", lastName = "MEE", email = "John@m.com", birthDate="1998-03-18", age = 29, entryYear ="2018", grade=12, phoneNumber="(907)380-6745", season=Semester.Summer, gender = "Male" },
            //    new Student() { id = 119, firstName = "Mark", middleName = "MOO", lastName = "MEE", email = "Mark@m.com", birthDate="1998-06-22", age = 20, entryYear ="2020", grade=11, phoneNumber="(887)423-1342", season=Semester.Summer, gender = "Male" },
            //    new Student() { id = 120, firstName = "Mary", middleName = "MAY", lastName = "MEE", email = "Mary@m.com", birthDate="1999-02-25", age = 25, entryYear ="2020", grade=11, phoneNumber="(123)380-1245", season=Semester.Fall, gender = "Female" },
            //    new Student() { id = 121, firstName = "John", middleName = "JILL", lastName = "MEE", email = "John@m.com", birthDate="1999-03-23", age = 29, entryYear ="2021", grade=10, phoneNumber="(321)432-8879", season=Semester.Summer,gender = "Male" }
            //};
        }

        public Student AddStudent(Student student)
        {
            student.id = _studentList.Max(s => s.id) + 1;
            if(student != null)
            {
                _studentList.Add(student);
            }
            return student;
        }

        public Student DeleteStudentByID(int Id)
        {
            Student student = _studentList.FirstOrDefault(s => s.id == Id);
            if(student != null)
            {
                _studentList.Remove(student);
            }
            return student;
        }

        public IEnumerable<Student> GetAllStudents()
        {
            //foreach(Student student in _studentList)
            //{
            //    student.birthDate = DateTime.Parse(student.birthDate).ToShortDateString();
            //}

            return _studentList;
        }

        public Student GetStudentByID(int Id)
        {
            return _studentList.Find(s => s.id == Id);
        }

        public Student UpdateStudent(Student studentChange)
        {
            Student student =_studentList.SingleOrDefault(s => s.id == studentChange.id);
            if (studentChange != null)
            {
                student.photoPath = studentChange.photoPath;
            }
            student.firstName = studentChange.firstName;
            student.middleName = studentChange.middleName;
            student.lastName = studentChange.lastName;
            student.age = studentChange.age;
            student.email = studentChange.email;
            student.grade = studentChange.grade;
            student.phoneNumber = studentChange.phoneNumber;
            student.season = studentChange.season;
            student.gender = studentChange.gender;
            student.entryYear = studentChange.entryYear;
            student.address = studentChange.address;
            student.city = studentChange.city;
            student.state = studentChange.state;

            return student;
        }
    }
}
