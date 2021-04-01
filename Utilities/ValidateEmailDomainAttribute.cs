using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.Utilities
{
    public class ValidateEmailDomainAttribute : ValidationAttribute
    {
        private readonly string _requiredEmailDomain;

        public ValidateEmailDomainAttribute(string requiredEmailDomain)
        {
            this._requiredEmailDomain = requiredEmailDomain;
        }

        public override bool IsValid(object value)
        {
            string userDomain = value.ToString();

            string[] userDomainArr= userDomain.Split('@').ToArray();

           return _requiredEmailDomain.ToLower() == userDomainArr[1].ToLower();
        }
    }
}
