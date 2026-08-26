using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Photo_Booth_Server_API.Data;
using Microsoft.AspNetCore.Identity;
using Photo_Booth_Server_API.Models;
namespace Photo_Booth_Server_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<Context>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services
            .AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<Context>()
            .AddDefaultTokenProviders();

            //modificare multipartheader si multipartbody length limit pentru a permite upload-ul de fisiere mari
            // testata in Postman.
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartHeadersLengthLimit = 1_048_576; 
                options.MultipartBodyLengthLimit = 104_857_600;  
                options.ValueLengthLimit = 1_048_576;
            });

            var app = builder.Build();

            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if(!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(uploadsPath),
                RequestPath = "/Uploads"
            });


        



            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
