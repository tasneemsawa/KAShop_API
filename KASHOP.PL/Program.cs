using KASHOP.BLL.Common;
using KASHOP.BLL.Mapping;
using KASHOP.BLL.Services;
using KASHOP.DAL.Data;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using KASHOP.PL.Extensions;
using KASHOP.PL.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;

namespace KASHOP.PL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);



            builder.Services.AddApplicationServices(builder.Configuration);

            var app = builder.Build();
            app.UseExceptionHandler();
            app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
           MapsterConfig.MapsterConfigRegister();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            //we use this to serve static files like images, css, js etc. from wwwroot folder
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            //if we want to seed the data when the application starts, you can call the DataSeed method of each seed data class here
            await app.SeedDatabaseAsync();

            app.MapControllers();

            app.Run();
        }
    }
}