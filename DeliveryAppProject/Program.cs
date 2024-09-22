using DeliveryApp.Core.Interfaces;
using DeliveryApp.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMemoryCache();
builder.Services.AddRazorPages();

builder.Services.AddTransient<INavigationService>(_ =>
{
    return new ServiceBusQueue(builder.Configuration.GetConnectionString("ServiceBus")!);
});
builder.Services.AddTransient<IProductService, ProductService>(serviceProvider =>
{
    var connectionString = builder.Configuration.GetConnectionString("Storage");
    var loggerProductService = serviceProvider.GetRequiredService<ILogger<ProductService>>();
    var loggerBlobService = serviceProvider.GetRequiredService<ILogger<BlobService>>();

    return new ProductService(connectionString, loggerProductService, loggerBlobService);

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
