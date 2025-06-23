using System.ComponentModel.DataAnnotations;
namespace CourseManagment.Models
{
	public class Course
	{
		[Key]
		public int Id { get; set; }
		[Required(ErrorMessage = "Course name is required")]
		[StringLength(100, ErrorMessage = "Course name should be less than 100 characteres")]
		public string Name { get; set; }
		[StringLength(100, ErrorMessage = "Course descriptions should be less than 100 characteres")]
		public string Description { get; set; }
		[Required(ErrorMessage = "Course start date is required")]
		public DateTime StartDate { get; set; }
		[Required(ErrorMessage = "Course end date is required")]
		public DateTime EndDate { get; set; }
		public bool isActive { get; set; }
	}
}
