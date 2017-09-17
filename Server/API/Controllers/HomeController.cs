using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QIP.API.Controllers {
    [Route("api")]
    public class HomeController : Controller {

        [HttpGet]
        [AllowAnonymous]
        public string Get() {
            return "Home";
        }
    }
}
