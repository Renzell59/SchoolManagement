using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.ViewModels.Administration.Users
{
    public class EditRolesInUserViewModel
    {
        public string RoleID { get; set; }
        public bool IsSelected { get; set; }
        public string RoleName { get; set; }
    }
}
