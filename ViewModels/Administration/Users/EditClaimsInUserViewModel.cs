using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.ViewModels.Administration.Users
{
    public class EditClaimsInUserViewModel
    {
        public EditClaimsInUserViewModel()
        {
            UserClaims = new List<UserClaim>();
        }
        public string UserID { get; set; }
        public List<UserClaim> UserClaims { get; set; }


    }
}
