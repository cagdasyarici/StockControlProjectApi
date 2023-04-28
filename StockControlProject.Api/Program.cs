using Microsoft.EntityFrameworkCore;
using StockControlProject.Repository.Abstract;
using StockControlProject.Repository.Concrete;
using StockControlProject.Repository.Context;
using StockControlProject.Service.Abstract;
using StockControlProject.Service.Concrete;

namespace StockControlProject.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<StockControlContext>(
                opt=>opt.UseSqlServer
                ("Server=Chadowa;Database=KD-14StockDB;Trusted_Connection=True;")
                );
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped(typeof(IGenericService<>),typeof(GenericService<>));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}