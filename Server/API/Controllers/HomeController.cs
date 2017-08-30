using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    [Route("api")]
    public class HomeController : Controller {

        [HttpGet]
        [AllowAnonymous]
        public string Get() {
            return "Home";
        }
    }
}
