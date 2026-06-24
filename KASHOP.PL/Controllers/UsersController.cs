using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KASHOP.DAL;
using KASHOP.DAL.Data;
using Microsoft.AspNetCore.Mvc;

namespace KASHOP.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        // حقن الـ DbContext عبر الـ Constructor
        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }


    }
}