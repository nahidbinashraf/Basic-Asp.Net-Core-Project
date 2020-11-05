using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASPBasicProjectWithAuth.Models;
using ASPBasicProjectWithAuth.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ASPBasicProjectWithAuth.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;

        public HomeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        public IActionResult Index()
        {
            return View(_employeeRepository.GetAllEmployees());
        }
        public IActionResult Details()
        {
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel
            {
                Employee = _employeeRepository.GetEmployee(1),
                PageTitle = "Details Employee"
            };
           
            return View(homeDetailsViewModel);
        }
    }
}