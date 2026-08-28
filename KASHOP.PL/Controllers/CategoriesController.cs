using KASHOP.BLL.Services;
using KASHOP.DAL.Data;
using KASHOP.DAL.Dto;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using KASHOP.PL.Resources;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Authorization;
namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        // private ApplicationDbContext _context;
        private readonly IStringLocalizer<SharedResource> _localizer;
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
           // var lang = Request.Headers["Accept-Language"].ToString();
            var categories = await _categoryService.GetAllCategories();
            return Ok(new { _localizer["success"].Value, categories });
        }
        [HttpPost("")]
        public async Task<IActionResult> Create(CategoryRequest request)
        {
            //to get the id 
            //var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //  var category = request.Adapt<Category>();
            // _context.Add(category);
            // _context.SaveChanges();
            // return Ok();
            var response = await _categoryService.CreateCategory(request);
            return Ok();
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetCategory(c => c.Id == id);
            return Ok(category);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _categoryService.DeleteCategory(id);
            if (!deleted) return BadRequest();
            return Ok();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CategoryRequest request)
        {
            var updated = await _categoryService.UpdateCategory(id, request);
            if (updated == null) return BadRequest();
            return Ok(updated);
        }
    }
}
