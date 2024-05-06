using Microsoft.EntityFrameworkCore;
using BlogAPI.Models;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace BlogAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PizzaStore API", Description = "Making the Pizzas you love", Version = "v1" });
            });
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<BlogContext>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaStore API V1");
                });
            }

            app.UseHttpsRedirection();

            app.MapControllers();

            app.Run();
        }
    }
}