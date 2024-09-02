using System.ComponentModel.DataAnnotations;

namespace H2TSHOP2024.ModelView
{
    public class LoginViewModel
    {
        [MaxLength(100)]
        [Required(ErrorMessage ="Vui lòng nhập EMAIL")]
        [Display(Name="EMAIL")]
        public string UserName { get; set; }


        [Display(Name = "Mật khẫu")]
        [Required(ErrorMessage ="Vui lòng nhập mật khẩu")]
        [MinLength(5,ErrorMessage ="Tối thiểu 6 ký tự")]
        public string Password { get; set; }
    }
}
