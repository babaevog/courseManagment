using CourseManagment.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseManagment.Data
{
	public class CourseContext : DbContext
	{
		public CourseContext(DbContextOptions<CourseContext> options) : base(options)
		{
		}

		public DbSet<Course> Courses { get; set; }
	}
}
