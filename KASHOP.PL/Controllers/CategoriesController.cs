using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KASHOP.DAL.Data;
using Microsoft.AspNetCore.Mvc;

namespace KASHOP.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
            // _localizer = localizer;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            try
            {
               if (_context.Database.CanConnect())
               {
                   return Ok("Done");
               }
               else
               {
                   return StatusCode(500, "Cannot connect to the database");
               }
            }
            catch (Exception ex)
            {
               return StatusCode(500, $"An error occurred: {ex.Message}");
            }
            // return Ok();
        }

    }
}