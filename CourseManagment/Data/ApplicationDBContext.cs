using Microsoft.EntityFrameworkCore;
using CourseManagment.Models;

namespace CourseManagment.Data
{
	public class ApplicationDBContext : DbContext
	{
		public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }
		public DbSet<Course> Courses { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<RefreshToken> Tokens { get; set; }
		public DbSet<Category> Categories { get; set; }
	}
}
