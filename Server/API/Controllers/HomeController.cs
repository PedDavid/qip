using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    [Route("api")]
    public class HomeController : Controller {
        [HttpGet]
        public string Get() {
            return "Home";
        }
    }
}
