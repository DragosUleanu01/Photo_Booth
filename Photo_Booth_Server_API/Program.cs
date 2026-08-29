using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Photo_Booth_Server_API.Data;
using Photo_Booth_Server_API.Models;
using Photo_Booth_Server_API.Services;
using System.Text;


namespace Photo_Booth_Server_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var jwtKey = builder.Configuration["Jwt:Key"];

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            //Adaugare context pentru baza de date si configurare SQL Server
            builder.Services.AddDbContext<Context>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //Implementare Identity pentru autentificare si autorizare
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


            //Adaugare autentificare JWT
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters =
                     new TokenValidationParameters
                     {
                         ValidateIssuer = true,
                         ValidateAudience = true,
                         ValidateLifetime = true,
                         ValidateIssuerSigningKey = true,

                         ValidIssuer = builder.Configuration["Jwt:Issuer"],
                         ValidAudience = builder.Configuration["Jwt:Audience"],

                         IssuerSigningKey =
                             new SymmetricSecurityKey(
                                 Encoding.UTF8.GetBytes(jwtKey!)
                             )
                     };

                 options.Events = new JwtBearerEvents
                 {
                     OnAuthenticationFailed = context =>
                     {
                         System.Diagnostics.Debug.WriteLine("JWT AUTH FAILED:");
                         System.Diagnostics.Debug.WriteLine(context.Exception.ToString());

                         return Task.CompletedTask;
                     },

                     OnTokenValidated = context =>
                     {
                         System.Diagnostics.Debug.WriteLine("JWT VALIDATED");
                         return Task.CompletedTask;
                     }
                 };
             });

            builder.Services.AddScoped<EncryptionService>();
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

            //Autorizare si autentificare pentru a proteja endpoint-urile API
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
