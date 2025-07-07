using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseManagment.Models
{
	public class RefreshToken
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Token { get; set; }
		[Required]
		public int JwtId { get; set; }
		public bool IsUsed { get; set; }
		public bool IsRevoked { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime ExpiresAt
		{
			get; set;
		}

		[Required]
		public int UserId { get; set; }
		[ForeignKey(nameof(UserId))]
		public User User { get; set; }
	}
}
