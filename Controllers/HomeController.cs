using SchoolManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SchoolManagement.ViewModels;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using SchoolManagement.Security;

namespace SchoolManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IDataProtector _dataProtector;
        private readonly DataProtectionPurposeStrings _dataProtectionPurposeStrings;

        public HomeController(IStudentRepository studentRepository,
            IWebHostEnvironment webHostEnvironment,
            IDataProtectionProvider dataProtectionProvider,
            DataProtectionPurposeStrings dataProtectionPurposeStrings
            )
        {
            this._studentRepository = studentRepository;
            this._webHostEnvironment = webHostEnvironment;
            this._dataProtectionPurposeStrings = dataProtectionPurposeStrings;

            _dataProtector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.EmployeeIdRouteValue);

        }
        
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            //IEnumerable<Student> model = _studentRepository.GetAllStudents();

            //string jsonString = JsonSerializer.Serialize(_studentRepository.GetAllStudents());

            //IEnumerable<Student> model = from student in _studentRepository.GetAllStudents()
            //                             where student.Gender == "Male"
            //                             select student;

            //return Json(_studentRepository.GetAllStudents());

            string jsonString = 
                "[" +
                "{\"id\":101,\"firstName\":\"Mark\",\"middleName\":\"MOO\",\"lastName\":\"MEE\",\"email\":\"Mark@m.com\",\"age\":20,\"gender\":\"Male\"}," +
                "{\"id\":102,\"firstName\":\"Mary\",\"middleName\":\"MAY\",\"lastName\":\"MEE\",\"email\":\"Mary@m.com\",\"age\":25,\"gender\":\"Female\"}," +
                "{\"id\":103,\"firstName\":\"John\",\"middleName\":\"JILL\",\"lastName\":\"MEE\",\"email\":\"John@m.com\",\"age\":29,\"gender\":\"Male\"}," +
                "{\"id\":104,\"firstName\":\"Mark\",\"middleName\":\"MOO\",\"lastName\":\"MEE\",\"email\":\"Mark@m.com\",\"age\":20,\"gender\":\"Male\"}," +
                "{\"id\":105,\"firstName\":\"Mary\",\"middleName\":\"MAY\",\"lastName\":\"MEE\",\"email\":\"Mary@m.com\",\"age\":25,\"gender\":\"Female\"}," +
                "{\"id\":106,\"firstName\":\"John\",\"middleName\":\"JILL\",\"lastName\":\"MEE\",\"email\":\"John@m.com\",\"age\":29,\"gender\":\"Male\"}," +
                "{\"id\":107,\"firstName\":\"Mark\",\"middleName\":\"MOO\",\"lastName\":\"MEE\",\"email\":\"Mark@m.com\",\"age\":20,\"gender\":\"Male\"}," +
                "{\"id\":108,\"firstName\":\"Mary\",\"middleName\":\"MAY\",\"lastName\":\"MEE\",\"email\":\"Mary@m.com\",\"age\":25,\"gender\":\"Female\"}," +
                "{\"id\":109,\"firstName\":\"John\",\"middleName\":\"JILL\",\"lastName\":\"MEE\",\"email\":\"John@m.com\",\"age\":29,\"gender\":\"Male\"}," +
                "{\"id\":110,\"firstName\":\"Mark\",\"middleName\":\"MOO\",\"lastName\":\"MEE\",\"email\":\"Mark@m.com\",\"age\":20,\"gender\":\"Male\"}," +
                "{\"id\":111,\"firstName\":\"Mary\",\"middleName\":\"MAY\",\"lastName\":\"MEE\",\"email\":\"Mary@m.com\",\"age\":25,\"gender\":\"Female\"}," +
                "{\"id\":112,\"firstName\":\"John\",\"middleName\":\"JILL\",\"lastName\":\"MEE\",\"email\":\"John@m.com\",\"age\":29,\"gender\":\"Male\"}," +
                "{\"id\":113,\"firstName\":\"Mark\",\"middleName\":\"MOO\",\"lastName\":\"MEE\",\"email\":\"Mark@m.com\",\"age\":20,\"gender\":\"Male\"}," +
                "{\"id\":114,\"firstName\":\"Mary\",\"middleName\":\"MAY\",\"lastName\":\"MEE\",\"email\":\"Mary@m.com\",\"age\":25,\"gender\":\"Female\"}," +
                "{\"id\":115,\"firstName\":\"John\",\"middleName\":\"JILL\",\"lastName\":\"MEE\",\"email\":\"John@m.com\",\"age\":29,\"gender\":\"Male\"}," +
                "{\"id\":116,\"firstName\":\"Mark\",\"middleName\":\"MOO\",\"lastName\":\"MEE\",\"email\":\"Mark@m.com\",\"age\":20,\"gender\":\"Male\"}," +
                "{\"id\":117,\"firstName\":\"Mary\",\"middleName\":\"MAY\",\"lastName\":\"MEE\",\"email\":\"Mary@m.com\",\"age\":25,\"gender\":\"Female\"}," +
                "{\"id\":118,\"firstName\":\"John\",\"middleName\":\"JILL\",\"lastName\":\"MEE\",\"email\":\"John@m.com\",\"age\":29,\"gender\":\"Male\"}," +
                "{\"id\":119,\"firstName\":\"Mark\",\"middleName\":\"MOO\",\"lastName\":\"MEE\",\"email\":\"Mark@m.com\",\"age\":20,\"gender\":\"Male\"}," +
                "{\"id\":120,\"firstName\":\"Mary\",\"middleName\":\"MAY\",\"lastName\":\"MEE\",\"email\":\"Mary@m.com\",\"age\":25,\"gender\":\"Female\"}," +
                "{\"id\":121,\"firstName\":\"John\",\"middleName\":\"JILL\",\"lastName\":\"MEE\",\"email\":\"John@m.com\",\"age\":29,\"gender\":\"Male\"}" +
                "]";

            IEnumerable<Student> student = JsonSerializer.Deserialize<List<Student>>(jsonString);

            return View(student);
        }

        public IActionResult Students()
        {
            IEnumerable<Student> students = _studentRepository.GetAllStudents().
                Select(s =>
                {
                    s.EncryptedId = _dataProtector.Protect(s.id.ToString());
                    return s;
                });

            return View(students);
        }


        [HttpGet]
        public IActionResult Details(string id)
        {
            string unProtectedStudentId = _dataProtector.Unprotect(id);
            int studentId = Convert.ToInt32(unProtectedStudentId);

            Student student = _studentRepository.GetStudentByID(studentId);
            if (student != null)
            {
                HomeDetailsViewModel model = new HomeDetailsViewModel();
                model.Student = student;
                model.StudentIdRouteValue = id;
                return View(model);
            }
            return View("Index");
        }

        [HttpGet]
        [HttpPost]
        public IActionResult Delete(string Id)
        {
            int studentId = Convert.ToInt32(_dataProtector.Unprotect(Id));
            Student student = _studentRepository.GetStudentByID(studentId);
            if(student != null)
            {
                _studentRepository.DeleteStudentByID(studentId);
                return RedirectToAction("Students","Home");
            }
            return View("Index");
        }

        [HttpGet]
        public IActionResult Update(string ID)
        {
            int studentId = Convert.ToInt32(_dataProtector.Unprotect(ID));
            Student student = _studentRepository.GetStudentByID(studentId);
            EditStudentViewModel model = new EditStudentViewModel()
            {
                ID = student.id,
                FirstName = student.firstName,
                MiddleName = student.middleName,
                LastName = student.lastName,
                Age = student.age,
                Email = student.email,
                BirthDate = student.birthDate,
                EntryYear = student.entryYear,
                Semester = student.season,
                PhoneNumber=student.phoneNumber,
                Grade=student.grade,
                Gender=student.gender,
                Address=student.address,
                City=student.city,
                State=student.state,
                existingPhotoPath=student.photoPath,
               
            };

            return View(model);
        }
        
        [HttpPost]
        public IActionResult Update(EditStudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessFileUpload(model);
                
                Student student = _studentRepository.GetStudentByID(model.ID);

                if(uniqueFileName != null)
                {
                    student.photoPath = uniqueFileName;
                }
                student.firstName = model.FirstName;
                student.middleName = model.MiddleName;
                student.lastName = model.LastName;
                student.email = model.Email;
                student.age = model.Age;
                student.birthDate = model.BirthDate;
                student.entryYear = model.EntryYear;
                student.season = model.Semester;
                student.phoneNumber = model.PhoneNumber;
                student.grade = model.Grade;
                student.gender = model.Gender;
                student.address = model.Address;
                student.city = model.City;
                student.state = model.State;

                Student updateStudent = _studentRepository.UpdateStudent(student);
                if (updateStudent != null)
                {
                    return RedirectToAction("Details", "Home", new { ID = model.ID });

                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CreateStudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessFileUpload(model);
                Student student = new Student()
                {
                    firstName = model.FirstName,
                    middleName = model.MiddleName,
                    lastName = model.LastName,
                    email = model.Email,
                    age = model.Age,
                    entryYear = model.EntryYear,
                    birthDate = model.BirthDate,
                    grade = model.Grade,
                    phoneNumber = model.PhoneNumber,
                    season = model.Semester,
                    gender=model.Gender,
                    photoPath=uniqueFileName,
                    address=model.Address,
                    city=model.City,
                    state=model.State
                };
                _studentRepository.AddStudent(student);
                
                return RedirectToAction("Details", "Home", new { id = student.id });
            }
            return View(model);
        }

        private string ProcessFileUpload(CreateStudentViewModel model)
        {
            string uniqueFileName = null;
            if (model.PhotoPath != null)
            {
                string rootPath = _webHostEnvironment.WebRootPath;
                string imagesPath = Path.Combine(rootPath, "images/Students");
                uniqueFileName = Guid.NewGuid() + "_" + model.PhotoPath.FileName;
                string filePath = Path.Combine(imagesPath, uniqueFileName);
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    model.PhotoPath.CopyTo(fs);
                }
            }
            return uniqueFileName;

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult About()
        {
            return View();
        }
    }
}

//////////////////////////////////////////////////////////////////////////////////////////// Different Ways of Writing LINQ Queries /////////////////////////////////////////////////////////////////////////
///

/*
 * Different ways of Writing LINQ Queries:
 * To write LINQ queries we use the LINQ Standard Query Operators:
 * select
 * from
 * where
 * orderby
 * join
 * groupby
 * 
 * There are 2 ways to write LINQ queries using thes Standard Query Operators
 * 1. Uinsg Lambda Expressions
 * Using SQL like query expressions
 * 
 * From a performance perspective there is no difference between the two. Which one to use dpeends on your personal preference.
 * 
 * Note: Behind the scene, LINQ queries written using SQL like query expressions are translated into their lambda expressions before they are compiled.
 * 
 * The Standard Query Operators are implemented as extension methods on IEnumrable<T> interface
 */

//////////////////////////////////////////////////////////////////////////////////////////// Extension Methods /////////////////////////////////////////////////////////////////////////

/*
 * Extension Methods:
 * What are Extension Methods:
 * According to MSDN, Extension methods enable you to "add" methoids to existing types without creating a new derived typem recompiling, or otherwise modifying the original type.
 * Extension methods are a special kind of static method, but they are called as if they were instance methods on the extended type.
 * 
 * For client code writtten in C# and Visual BAsic, there is no apparent difference between calling an extension method and the methods that are actually defined in
 * a type.
 * 
 * LINQ's standard query operators(select,where, etc) are implemented in Enumerable class as extension methods on the IEnumerable<T> interface.
 * 
 * Now look at the following query
 * List<int> Numbers = new List<int> {1,2,3,4,5,6,7,8,9,10};
 * IEnumerable<int> EvenNumbers = Numbers.Where( n => n % 2 ==0);
 * 
 * In spite of Where() method not belonging to List<T> class, we are still able to use it as though it belong to List<T> class. This is possible because Where() method is implemented
 * as extension method in IEnumerable<T> interface and List<T> implements IEnumerable<T> interface.
 */
