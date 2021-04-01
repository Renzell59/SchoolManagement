using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.ViewModels.Administration
{
    public class EditRoleViewModel:CreateRoleViewModel
    {
        [Display(Name = "Id")]
        public string RoleID { get; set; }

        public List<string> UserNames { get; set; }
    }
}
