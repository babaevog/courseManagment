using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseManagment.Data;
using CourseManagment.Models;

namespace EventPlaner.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CategoriesController : ControllerBase
	{
		private readonly ApplicationDBContext _context;

		public CategoriesController(ApplicationDBContext context)
		{
			_context = context;
		}

		[HttpGet]
		[Authorize]
		public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
		{
			return await _context.Categories.ToListAsync();
		}

		[HttpGet("{id}")]
		[Authorize]
		public async Task<ActionResult<Category>> GetCategoryById(int id)
		{
			var category = await _context.Categories.FindAsync(id);
			if (category == null)
			{
				return NotFound();
			}
			return category;
		}


		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<Category>> CreateCategory(Category category)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			_context.Categories.Add(category);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
		}


		[HttpPut("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateCategory(int id, Category category)
		{
			if (id != category.Id)
			{
				return BadRequest("Category id mismatch");
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var existingCategory = await _context.Categories.FindAsync(id);
			if (existingCategory == null)
			{
				return NotFound();
			}

			existingCategory.Name = category.Name;
			existingCategory.Description = category.Description;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!CategoryExists(id))
				{
					return NotFound();
				}
				throw;
			}

			return NoContent();
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteCategory(int id)
		{
			var category = await _context.Categories.FindAsync(id);
			if (category == null)
			{
				return NotFound();
			}

			_context.Categories.Remove(category);
			await _context.SaveChangesAsync();

			return NoContent();
		}

		private bool CategoryExists(int id)
		{
			return _context.Categories.Any(c => c.Id == id);
		}
	}
}