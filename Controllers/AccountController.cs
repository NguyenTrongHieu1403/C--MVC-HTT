using H2TSHOP2024.Extension;
using H2TSHOP2024.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace H2TSHOP2024.Controllers
{
    public class AccountController : Controller
    {
        private readonly BookstoreDbContext _context;

        public AccountController(BookstoreDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> DangKyTaiKhoan([Bind("Fullname,Email,Phone,Password")] Customer taikhoan)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Customer tk = new Customer
                    {
                        Fullname = taikhoan.Fullname.Trim(),
                        Phone = taikhoan.Phone.Trim().ToLower(),
                        Email = taikhoan.Email.Trim().ToLower(),
                        Password = taikhoan.Password.ToMD5(),  // Sử dụng phương thức băm MD5 đã tạo
                        Active = true,  // Set mặc định là true
                        CreatedDate = DateTime.Now,  // Gán giá trị ngày tạo là thời gian hiện tại
                        BirthDate = null,  // Hoặc để trống nếu không cần nhập ngày sinh
                        Sex = null  // Nếu không yêu cầu giới tính, bạn có thể để null hoặc gán một giá trị mặc định
                    };

                    _context.Customers.Add(tk);
                    await _context.SaveChangesAsync();

                    // Lưu thông tin đăng nhập vào session
                    HttpContext.Session.Set("CustomerSession", tk);

                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                ModelState.AddModelError("", "Có lỗi xảy ra trong quá trình đăng ký.");
            }

            return View(taikhoan);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine("ModelState is valid"); // Kiểm tra model state
                var user = _context.Customers
                    .FirstOrDefault(c => c.Email.ToLower() == Email.ToLower() && c.Password == Password && c.Active);

                if (user != null)
                {
                    Console.WriteLine("User found"); // Kiểm tra xem user có được tìm thấy không
                    HttpContext.Session.Set("CustomerSession", user);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    Console.WriteLine("User not found or inactive");
                    ModelState.AddModelError("", "Thông tin đăng nhập không chính xác hoặc tài khoản của bạn chưa được kích hoạt.");
                }
            }
            else
            {
                Console.WriteLine("ModelState is invalid");
            }

            return View();
        }


        [HttpPost]
        [Authorize]
        public IActionResult Logout()
        {
            // Xóa session khi người dùng đăng xuất
            HttpContext.Session.Remove("CustomerSession");

            // Chuyển hướng về trang đăng nhập
            return RedirectToAction("Login", "Account");
        }
    }



}

