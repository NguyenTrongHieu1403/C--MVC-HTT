using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace H2TSHOP2024.ModelView
{
    public class RegisterVM
    {
        public class Customer
        {
            public int CusID { get; set; } // Maps to the CusID column in the database

            [Display(Name = "Họ Và Tên")]
            [Required(ErrorMessage = "Vui lòng nhập Họ Tên")]
            [StringLength(100)]
            public string Fullname { get; set; } // Maps to the Fullname column in the database
            [Required(ErrorMessage = "Vui lòng nhập Email")]
            [DataType(DataType.EmailAddress)]
            [StringLength(100)]
            [Remote(action:"ValidateEmail",controller:"Account")]
            public string Email { get; set; } // Maps to the Email column in the database

            [Required(ErrorMessage = "Vui lòng nhập Số điện thoại")]
            [Display(Name = "Điện thoại")]
            [DataType(DataType.PhoneNumber)]
            [StringLength(20)]
            [Remote(action: "ValidatePhone", controller: "Account")]
            public string Phone { get; set; } // Maps to the Phone column in the database

            [Display(Name = "Ngày tạo")]
            [DataType(DataType.DateTime)]
            public DateTime CreatedDate { get; set; } // Maps to the CreatedDate column in the database

            [Display(Name = "Hoạt động")]
            public bool Active { get; set; } // Maps to the Active column in the database

            [Display(Name = "Ngày sinh")]
            [DataType(DataType.Date)]
            public DateTime? BirthDate { get; set; } // Maps to the BirthDate column in the database

            [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
            [DataType(DataType.Password)]
            [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.", MinimumLength = 6)]
            public string Password { get; set; } // Maps to the Password column in the database
            [DataType(DataType.Password)]
            [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.", MinimumLength = 6)]
            [Compare("Password", ErrorMessage ="Vui lòng nhập mật khẩu giống")]
            public string ConfirmPassword { get; set; } // Maps to the Password column in the database
            [Display(Name = "Giới tính")]
            [StringLength(10)]
            public string Sex { get; set; } // Maps to the Sex column in the database
        }


    }
}
