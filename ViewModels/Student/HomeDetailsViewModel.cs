using SchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.ViewModels
{
    public class HomeDetailsViewModel
    {
        public Student Student { get; set; }
        public string StudentIdRouteValue { get; set; }
        public int MyProperty { get; set; }
    }
}
