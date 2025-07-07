namespace CourseManagment.Models
{
	public class Category
	{
			public int Id { get; set; } 
			public string Name { get; set; } 
			public string Description { get; set; } 

			//связь с курсами
			public ICollection<Course> Courses { get; set; }
	}
}
