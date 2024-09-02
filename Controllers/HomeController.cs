using H2TSHOP2024.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace H2TSHOP2024.Controllers
{
    public class HomeController : Controller
    {
        private readonly BookstoreDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, BookstoreDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Phương thức để tải danh sách thể loại vào ViewBag
        private void LoadGenres()
        {
            var genres = _context.Genres.AsNoTracking().ToList();
            ViewBag.Genres = genres;
        }


        public IActionResult Index()
        {
            var genres = _context.Genres.AsNoTracking().ToList();
            if (genres != null && genres.Count > 0)
            {
                ViewBag.Genres = genres;
            }
            else
            {
                ViewBag.Genres = new List<Genre>(); // Tránh null reference
            }
            LoadGenres();
            // Lấy 8 sản phẩm từ cơ sở dữ liệu
            var products = _context.Books.AsNoTracking().Take(8).ToList();

            // Kiểm tra xem danh sách sản phẩm có bị null hay không
            if (products == null || !products.Any())
            {
                // Xử lý trường hợp không có sản phẩm
                ViewBag.ErrorMessage = "Không có sản phẩm nào được tìm thấy.";
                products = new List<Book>(); // Tạo danh sách rỗng để tránh lỗi NullReferenceException
            }
            return View(products);
        }


        public IActionResult About()
        {
            LoadGenres(); // Gọi phương thức LoadGenres để tải danh sách thể loại
            return View();
        }

        public IActionResult Contact()
        {
            LoadGenres(); // Gọi phương thức LoadGenres để tải danh sách thể loại
            return View();
        }

        public IActionResult Privacy()
        {
            LoadGenres(); // Gọi phương thức LoadGenres để tải danh sách thể loại
            return View();
        }

        public IActionResult SomeAction()
        {
            LoadGenres(); // Gọi phương thức LoadGenres để tải danh sách thể loại
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
