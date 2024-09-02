using H2TSHOP2024.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

namespace H2TSHOP2024.Controllers
{
    public class ProductController : Controller
    {
        private readonly BookstoreDbContext _context;

        public ProductController(BookstoreDbContext context)
        {
            _context = context;
        }
        private void LoadGenres()
        {
            var genres = _context.Genres.AsNoTracking().ToList();
            ViewBag.Genres = genres;
        }

        [Route("/shop.html", Name = "ShopProduct")]
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 9;  // Giả sử bạn muốn hiển thị 9 sản phẩm mỗi trang
            var IsBook = _context.Books.AsNoTracking().OrderByDescending(x => x.BookId);
            PagedList<Book> models = new PagedList<Book>(IsBook, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            LoadGenres();
            return View(models); // Truyền models sang view
        }



        [Route("Product/List/{id:int}", Name = "ListBookByGenre")]
        public IActionResult List(int id, int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10;

            var genre = _context.Genres.Find(id);
            if (genre == null)
            {
                return NotFound(); // Hoặc xử lý trường hợp genre không tồn tại
            }

            var books = _context.Books.AsNoTracking()
                            .Where(b => b.GenreId == id)
                            .OrderByDescending(b => b.BookId);

            var model = new PagedList<Book>(books, pageNumber, pageSize);

            ViewBag.CurrentCat = genre;
            ViewBag.CurrentPage = pageNumber;

            LoadGenres();
            return View(model);
        }





        [Route("/{Alias}-{id}.html", Name = "ProductDetails")]
        public IActionResult Details(int id)
        {
            try
            {
                var product = _context.Books.Include(x => x.Genre).Include(x => x.Nxb).FirstOrDefault(x => x.BookId == id);
                if (product == null)
                {
                    LoadGenres();
                    return RedirectToAction("Index");
                }

                var lsProduct = _context.Books.AsNoTracking()
            .Where(x => (x.GenreId == product.GenreId || x.Nxbid == product.Nxbid) && x.BookId != product.BookId)
            .OrderByDescending(x => x.BookName)
            .Take(4)
            .ToList();

                ViewBag.SanPham = lsProduct;
                LoadGenres();
                return View(product);
            }
            catch
            {
                LoadGenres();
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
