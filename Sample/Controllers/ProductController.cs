using Harry.Caching;
using Microsoft.AspNetCore.Mvc;
using Sample.Models;
using System.Text.Json;

namespace Sample.Controllers
{
    public class ProductController : Controller
    {
        public async Task<IActionResult> Item(int id,
            [FromServices] ICache<ProductModel> cache)
        {
            //当返回true时,依然不能排除model非null值.
            (var result, var model) = await cache.GetAsync(id.ToString());
            if (result && model != null)
            {
                return Content(JsonSerializer.Serialize(model));
            }
            else
            {
                return NotFound();
            }
        }
    }
}
