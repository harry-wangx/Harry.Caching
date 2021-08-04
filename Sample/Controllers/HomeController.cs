using Harry.Caching;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Sample.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Content("Hello World!");
        }

        public async Task<IActionResult> Get([FromServices] ICacheFactory cacheFactory)
        {
            var cache = cacheFactory.CreateCache<string>("test");
            (var result, var now) = await cache.GetAsync("now");
            if (result)
            {
                return Content(now);
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Set([FromServices] ICacheFactory cacheFactory)
        {
            var cache = cacheFactory.CreateCache<string>("test");
            var now = DateTime.Now.ToString();
            await cache.SetAsync("now", now);
            return Content(now);
        }
    }
}
