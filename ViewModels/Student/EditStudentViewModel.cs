using Microsoft.AspNetCore.Http;
using SchoolManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.ViewModels
{
    public class EditStudentViewModel:CreateStudentViewModel
    {

        public string existingPhotoPath { get; set; }

    }
}
