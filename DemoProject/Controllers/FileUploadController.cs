using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Authentication;
using static System.Net.WebRequestMethods;

namespace DemoProject.Controllers
{
    public class FileUploadController : Controller
    {
        protected readonly IHttpContextAccessor _contextAccessor;
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<string> UploadDocument()
        {
            try
            {
                var supportTypes = new[] { "pdf", "png" };
                var file = _contextAccessor.HttpContext.Request.Form.Files.Count > 0 ? _contextAccessor.HttpContext.Request.Form.Files[0] : null;
                if (file != null)
                {
                    var fileExtension = Path.GetExtension(file.FileName).Substring(1);
                    if (!supportTypes.Contains(fileExtension))
                    {
                        return $"Unsupported file types";
                    }
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine("~/Docs", fileName);
                    using (Stream fs = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(fs);
                    }
                }
                return file != null ? "/uploadDocs/" + file.FileName : null;
            }
            catch (AuthenticationException authEx)
            {
                return $"Internal server error: {authEx}";
            }
            catch (HttpRequestException httpEx)
            {
                return $"Internal server error: {httpEx}";
            }
            catch (JsonException jsonEx)
            {
                return $"Internal server error: {jsonEx}";
            }
            catch (InvalidOperationException invalidOpEx)
            {
                return $"Internal server error: {invalidOpEx}";
            }
            catch (Exception ex)
            {
                return $"Internal server error: {ex}";

            }

        }

        [HttpGet]
        public async Task<string> DeleteFile(string fname)
        {
            var supportTypes = new[] { "pdf", "png" };
            string returnRes = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(fname))
                {
                    bool isValidPath = fname.IndexOfAny(Path.GetInvalidPathChars()) == -1;

                    if (isValidPath)
                    {
                        returnRes = $"Wrong file name";
                    }

                    var fileExt = Path.GetExtension(fname).Substring(1);

                    if (!supportTypes.Contains(fileExt))
                    {
                        returnRes = $"Invalid file type";
                    }

                    var path = Path.Combine("~/Docs", fname);

                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                        returnRes = fname + $" has been deleted";
                    }
                    else
                    {
                        returnRes = fname + $" Not in the Application";
                    }
                }
            }
            catch (AuthenticationException authEx)
            {
                return $"Internal server error: {authEx}";
            }
            catch (HttpRequestException httpEx)
            {
                return $"Internal server error: {httpEx}";
            }
            catch (JsonException jsonEx)
            {
                return $"Internal server error: {jsonEx}";
            }
            catch (InvalidOperationException invalidOpEx)
            {
                return $"Internal server error: {invalidOpEx}";
            }
            catch (Exception ex)
            {
                returnRes= $"Internal server error: {ex}";
            }

            return returnRes ;
        }
    }
}
