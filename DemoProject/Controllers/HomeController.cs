using Microsoft.AspNetCore.Mvc;
using DemoProject.Models;
using DemoProject.Services.Implimentation;
using System.Reflection.Metadata;
using DemoProject.Logics;
using Newtonsoft.Json;
using DemoProject.Services;
using Newtonsoft.Json.Serialization;

namespace DemoProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRegistrationServices _registrationServices;
        private const int PAGE_SIZE = 5;

        public HomeController(ILogger<HomeController> logger, IRegistrationServices registrationServices)
        {
            _logger = logger;
            _registrationServices = registrationServices;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                if (page < 1) page = 1;

                var regData = await _registrationServices.GetAllRegistration(page - 1, PAGE_SIZE);
                var totalCount = await _registrationServices.GetTotalRegistrationCount();
                var totalPages = (int)Math.Ceiling((double)totalCount / PAGE_SIZE);

                ViewBag.AllData = regData;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.TotalRecords = totalCount;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Index action");
                return RedirectToAction("Error", "Home", new { statusCode = 500 });
            }
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
                        return Json(new { response = res.Item1, result = res.Item2 });
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

        [HttpGet("Home/GetRegistration/{id}")]
        public async Task<IActionResult> GetRegistration(int id)
        {
            try
            {
                var registration = await _registrationServices.GetRegistrationById(id);
                if (registration == null)
                {
                    return NotFound();
                }

                return Json(registration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetRegistration action");
                return Json(new { response = false, result = "An error occurred while fetching registration." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var registration = await _registrationServices.GetRegistrationById(id);
                if (registration == null)
                {
                    return NotFound();
                }

                ViewBag.IsEditMode = true;
                ViewBag.RegistrationId = id;
                ViewBag.ModelJson = JsonConvert.SerializeObject(registration, new JsonSerializerSettings { 
                    ContractResolver = new CamelCasePropertyNamesContractResolver() 
                });
                return View("Index", registration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Edit action");
                return RedirectToAction("Error", "Home", new { statusCode = 500 });
            }
        }

        [HttpPost("Home/Update")]
        public async Task<IActionResult> Update([FromBody] RegistrationModel registrationModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Tuple<bool, string> res = await _registrationServices.UpdateRegistration(registrationModel);
                    return Json(new { response = res.Item1, result = res.Item2 });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in the Update action.");
                    return Json(new { response = false, result = "An error occurred while updating registration." });
                }
            }
            else
            {
                return Json(new { response = false, result = "Invalid model state." });
            }
        }

        [HttpDelete("Home/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                Tuple<bool, string> res = await _registrationServices.DeleteRegistration(id);
                return Json(new { response = res.Item1, result = res.Item2 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in the Delete action.");
                return Json(new { response = false, result = "An error occurred while deleting registration." });
            }
        }
    }
}
