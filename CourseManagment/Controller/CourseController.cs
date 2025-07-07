using CourseManagment.Data;
using CourseManagment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace EventPlaner.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class CoursesController : ControllerBase
	{
		private readonly ApplicationDBContext _context;

		public CoursesController(ApplicationDBContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Course>>> GetCourses(bool? isActive = null)
		{
			var query = _context.Courses.AsQueryable();
			if (isActive.HasValue)
			{
				query = query.Where(c => c.isActive == isActive.Value);
			}
			return await query.ToListAsync();
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Course>> GetCourseById(int id)
		{
			var course = await _context.Courses.FindAsync(id);
			if (course == null)
			{
				return NotFound();
			}
			return course;
		}


		[HttpGet("Filtered")]
		public async Task<ActionResult<IEnumerable<Course>>> GetFilteredCourses(
			bool? isActive = null,
			DateTime? startDate = null,
			DateTime? endDate = null,
			string? nameContains = null)
		{
			var query = _context.Courses.AsQueryable();

			if (isActive.HasValue)
			{
				query = query.Where(c => c.isActive == isActive.Value);
			}
			if (startDate.HasValue)
			{
				query = query.Where(c => c.StartDate >= startDate.Value);
			}
			if (endDate.HasValue)
			{
				query = query.Where(c => c.EndDate <= endDate.Value);
			}
			if (!string.IsNullOrEmpty(nameContains))
			{
				query = query.Where(c => c.Name.Contains(nameContains));
			}

			return await query.ToListAsync();
		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult<Course>> CreateCourse(Course courseItem)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			if (courseItem.StartDate < DateTime.Now)
			{
				return BadRequest("Course start date cannot be in the past");
			}
			if (courseItem.EndDate < courseItem.StartDate)
			{
				return BadRequest("Course end date cannot be earlier than start date");
			}

			_context.Courses.Add(courseItem);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(GetCourseById), new { id = courseItem.Id }, courseItem);
		}

		[HttpPut("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateCourse(int id, Course courseItem)
		{
			if (id != courseItem.Id)
			{
				return BadRequest("Course id mismatch");
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			if (courseItem.StartDate < DateTime.Now)
			{
				return BadRequest("Course start date cannot be in the past");
			}
			if (courseItem.EndDate < courseItem.StartDate)
			{
				return BadRequest("Course end date cannot be earlier than start date");
			}

			var existingCourse = await _context.Courses.FindAsync(id);
			if (existingCourse == null)
			{
				return NotFound();
			}

			existingCourse.Name = courseItem.Name;
			existingCourse.Description = courseItem.Description;
			existingCourse.StartDate = courseItem.StartDate;
			existingCourse.EndDate = courseItem.EndDate;
			existingCourse.isActive = courseItem.isActive;

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!CourseExists(id))
				{
					return NotFound();
				}
				throw;
			}

			return NoContent();
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteCourse(int id)
		{
			var courseItem = await _context.Courses.FindAsync(id);
			if (courseItem == null)
			{
				return NotFound();
			}

			_context.Courses.Remove(courseItem);
			await _context.SaveChangesAsync();
			return NoContent();
		}

		private bool CourseExists(int id)
		{
			return _context.Courses.Any(c => c.Id == id);
		}
	}
}
