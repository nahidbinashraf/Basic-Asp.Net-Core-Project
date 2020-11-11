using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASPBasicProjectWithAuth.Models;
using Microsoft.AspNetCore.Http;

namespace ASPBasicProjectWithAuth.ViewModels
{
    public class EmployeeCreateViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public Dept? Department { get; set; }
        public IFormFile Photo { get; set; }
    }
}
