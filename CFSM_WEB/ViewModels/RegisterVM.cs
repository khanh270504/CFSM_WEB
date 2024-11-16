using System.ComponentModel.DataAnnotations;

namespace CFSM_WEB.ViewModels
{
	public class RegisterVM
	{
		[Key]
		[Display(Name = "Tên đăng nhập")]
		[Required(ErrorMessage = "*")]
		[MaxLength(20, ErrorMessage = "Tối đa 20 kí tự")]
		public string UserName { get; set; }


		[Display(Name = "Mật khẩu")]
		[Required(ErrorMessage = "* Vui lòng điền mật khẩu")]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Display(Name = "Họ tên")]
		[Required(ErrorMessage = "*")]
		[MaxLength(50, ErrorMessage = "Tối đa 50 kí tự")]
		public string FullName { get; set; }

		[Display(Name = "Địa chỉ")]
		[MaxLength(60, ErrorMessage = "Tối đa 60 kí tự")]
		public string Address { get; set; }

		[Display(Name = "Điện thoại")]
		[MaxLength(24, ErrorMessage = "Tối đa 24 kí tự")]
		[RegularExpression(@"0[9875]\d{8}", ErrorMessage = "Chưa đúng định dạng di động Việt Nam")]
		public string PhoneNumber { get; set; }

		public string Email { get; set; }
	}
}
