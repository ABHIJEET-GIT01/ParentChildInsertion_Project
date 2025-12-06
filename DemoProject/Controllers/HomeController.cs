using Microsoft.AspNetCore.Mvc;
using DemoProject.Models;
using DemoProject.Services.Implimentation;
using System.Reflection.Metadata;
using DemoProject.Logics;
using Newtonsoft.Json;
using DemoProject.Services;

namespace DemoProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRegistrationServices _registrationServices;

        public HomeController(ILogger<HomeController> logger, IRegistrationServices registrationServices)
        {
            _logger = logger;
            _registrationServices = registrationServices;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var regData = await _registrationServices.GetAllRegistration(0, 5);
            ViewBag.AllData = regData;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] RegistrationModel registrationModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Tuple<bool, string> res = await _registrationServices.InsertRegistration(registrationModel);
                    
                    if (res.Item1)
                    {
                        var allData = await _registrationServices.GetAllRegistration(0, 5);
                        ViewBag.AllData = allData;
                        return Json(new { response = res.Item1, result = res.Item2, redirectUrl = Url.Action("Index", "Home") });
                    }
                    else
                    {
                        return Json(new { response = res.Item1, result = res.Item2 });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in the Index action.");
                    return Json(new { response = false, result = "An error occurred while processing registration." });
                }
            }
            else
            {
                return Json(new { response = false, result = "Invalid model state." });
            }
        }

    }
}
