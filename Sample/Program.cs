using Harry.Caching;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Sample.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddCache(builder =>
{
    builder.AddMemoryCache(options =>
    {
        options.SetSlidingExpiration(TimeSpan.FromSeconds(15));
    })
    .AddDistributedCache(options =>
    {
        options.SetSlidingExpiration(TimeSpan.FromMinutes(30));
    })
    .AddCustomCache<ProductModel>((sp, key) =>
    {
        if (int.TryParse(key, out int id) && id > 0 && id <= 10)
        {
            return Task.FromResult((true, new ProductModel() { Id = id, Name = "Product " + key }));
        }
        else
        {
            //这样设计是为了避免请求不存在数据时,穿透缓存.
            return Task.FromResult<(bool, ProductModel)>((true, null));
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
