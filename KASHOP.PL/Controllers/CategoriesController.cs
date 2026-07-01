using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KASHOP.DAL.Data;
using KASHOP.DAL.Dto;
using KASHOP.PL.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using KASHOP.BLL.Services;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Mapster;
using Microsoft.EntityFrameworkCore;



namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        // private ApplicationDbContext _context;
        private IStringLocalizer<SharedResource> _localizer;
        private readonly ICategoryService _categoryService;

        public CategoriesController(ApplicationDbContext context, IStringLocalizer<SharedResource> localizer, ICategoryService categoryService)
        {
            // _context = context;
            _localizer = localizer;
             _categoryService = categoryService;
        }

        [HttpGet("")]

        public async Task<IActionResult> Index()
        {
            //    var categories = _context.Categories.Include(c=>c.Translations).ToList();
            //    var res= categories.Adapt<List<CategoryResponse>>();
            var categories = await _categoryService.GetAllCategories();
            return Ok(new { _localizer["success"].Value, categories });
        }
        [HttpPost("")]
        public async Task<IActionResult> Create(CategoryRequest request)
        {

            //  var category = request.Adapt<Category>();
            // _context.Add(category);
            // _context.SaveChanges();
            // return Ok();
            var response = await _categoryService.CreateCategory(request);
            return Ok();
        }
    }
}