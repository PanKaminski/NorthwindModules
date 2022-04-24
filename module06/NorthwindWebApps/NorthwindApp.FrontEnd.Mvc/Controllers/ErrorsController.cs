using Microsoft.AspNetCore.Mvc;
using NorthwindApp.FrontEnd.Mvc.ViewModels;

namespace NorthwindApp.FrontEnd.Mvc.Controllers
{
    public class ErrorsController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HandleHttpStatusCode(ErrorViewModel model)
        {
            return this.View("Error", model);
        }
    }
}
