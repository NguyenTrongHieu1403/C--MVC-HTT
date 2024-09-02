using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using H2TSHOP2024.Models;
using PagedList.Core;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace H2TSHOP2024.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminBooksController : Controller
    {
        private readonly BookstoreDbContext _context;
        public INotyfService _notifyService { get; }

        public AdminBooksController(BookstoreDbContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        public async Task<IActionResult> Filter(int idgenre, int idnxb, int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 10;

            var books = _context.Books.AsQueryable();

            if (idgenre > 0)
            {
                books = books.Where(b => b.GenreId == idgenre);
            }

            if (idnxb > 0)
            {
                books = books.Where(b => b.Nxbid == idnxb);
            }

            var pagedBooks = books.Include(b => b.Genre)
                          .Include(b => b.Nxb)
                          .OrderBy(b => b.BookName)
                          .ToPagedList(pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return PartialView("_BooksTablePartialView", pagedBooks);
        }

        // GET: Admin/AdminBooks
        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 20;
            var IsBooks = _context.Books.AsNoTracking().Include(x => x.Genre).Include(x => x.Nxb).OrderByDescending(x => x.BookId);

            PagedList<Book> models = new PagedList<Book>(IsBooks, pageNumber, pageSize);

            ViewData["DanhMuc"] = new SelectList(_context.Genres, "GenreId", "GenreName");

            ViewData["NXB"] = new SelectList(_context.Nxbs, "Nxbid", "Nxbname");

            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        // GET: Admin/AdminBooks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Genre)
                .Include(b => b.Nxb)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Admin/AdminBooks/Create
        public IActionResult Create()
        {
            ViewData["DanhMuc"] = new SelectList(_context.Genres, "GenreId", "GenreName");
            ViewData["NXB"] = new SelectList(_context.Nxbs, "Nxbid", "Nxbname");
            return View();
        }

        // POST: Admin/AdminBooks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,BookName,AuthorName,Description,Price,Nxbid,GenreId,Stock")] Book book, IFormFile ImageFile)
        {
            // Kiểm tra trùng lặp tên sách
            if (_context.Books.Any(b => b.BookName == book.BookName))
            {
                ModelState.AddModelError("BookName", "Tên sách đã tồn tại.");
            }

            // Xử lý upload ảnh
            if (ImageFile != null)
            {
                // Lưu ảnh vào thư mục wwwroot/Images/Books/
                string fileName = MyTool.UploadImageToFolder(ImageFile, "Books");

                if (!string.IsNullOrEmpty(fileName))
                {
                    // Lưu đường dẫn ảnh vào cơ sở dữ liệu
                    book.Image = "/Images/Books/" + fileName;
                }
                else
                {
                    ModelState.AddModelError("Image", "Có lỗi khi tải lên ảnh.");
                }
            }


            // Kiểm tra ModelState trước khi lưu dữ liệu
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                _notifyService.Success("Tạo mới sách thành công.");
                return RedirectToAction(nameof(Index));
            }

            _notifyService.Error("Tạo mới sách thất bại. Vui lòng kiểm tra lại thông tin.");
            ViewData["DanhMuc"] = new SelectList(_context.Genres, "GenreId", "GenreName", book.GenreId);
            ViewData["NXB"] = new SelectList(_context.Nxbs, "Nxbid", "Nxbname", book.Nxbid);
            return View(book);
        }


        // POST: Admin/AdminBooks/Create
        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,BookName,AuthorName,Description,Price,Nxbid,GenreId,Stock")] Book book, IFormFile ImageFile)
        {
            // Kiểm tra trùng lặp tên sách
            if (_context.Books.Any(b => b.BookName == book.BookName))
            {
                ModelState.AddModelError("BookName", "Tên sách đã tồn tại.");
            }

            // Xử lý upload ảnh
            if (ImageFile != null)
            {
                // Kiểm tra xem ảnh đã tồn tại trong thư mục chưa
                string fileName = MyTool.GetUniqueFileName(ImageFile.FileName, "Books");
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/Books", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    // Nếu ảnh đã tồn tại, sử dụng lại đường dẫn
                    book.Image = "/Images/Books/" + fileName;
                }
                else
                {
                    // Nếu ảnh chưa tồn tại, lưu ảnh mới vào thư mục
                    fileName = MyTool.UploadImageToFolder(ImageFile, "Books");

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        // Lưu đường dẫn ảnh vào cơ sở dữ liệu
                        book.Image = "/Images/Books/" + fileName;
                    }
                    else
                    {
                        ModelState.AddModelError("Image", "Có lỗi khi tải lên ảnh.");
                    }
                }
            }

            // Kiểm tra ModelState trước khi lưu dữ liệu
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                _notifyService.Success("Tạo mới sách thành công.");
                return RedirectToAction(nameof(Index));
            }

            _notifyService.Error("Tạo mới sách thất bại. Vui lòng kiểm tra lại thông tin.");
            ViewData["DanhMuc"] = new SelectList(_context.Genres, "GenreId", "GenreName", book.GenreId);
            ViewData["NXB"] = new SelectList(_context.Nxbs, "Nxbid", "Nxbname", book.Nxbid);
            return View(book);
        }
*/


        // GET: Admin/AdminBooks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();

            }
            ViewData["DanhMuc"]= new SelectList(_context.Genres, "GenreId", "GenreName", book.GenreId);
            ViewData["NXB"] = new SelectList(_context.Nxbs, "Nxbid", "Nxbname", book.Nxbid);
            return View(book);
        }
        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
        // POST: Admin/AdminBooks/Edit
        // POST: Admin/AdminBooks/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,BookName,AuthorName,Description,Price,Nxbid,GenreId,Stock")] Book book, IFormFile ImageFile)
        {
            if (id != book.BookId)
            {
                return NotFound();
            }

            // Kiểm tra trùng lặp tên sách (ngoại trừ sách hiện tại)
            if (_context.Books.Any(b => b.BookName == book.BookName && b.BookId != id))
            {
                ModelState.AddModelError("BookName", "Tên sách đã tồn tại.");
            }

            // Xử lý upload ảnh (nếu có ảnh mới)
            if (ImageFile != null)
            {
                // Kiểm tra xem ảnh đã tồn tại trong thư mục chưa
                string fileName = MyTool.GetUniqueFileName(ImageFile.FileName, "Books");
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/Books", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    // Nếu ảnh đã tồn tại, sử dụng lại đường dẫn
                    book.Image = "/Images/Books/" + fileName;
                }
                else
                {
                    // Nếu ảnh chưa tồn tại, lưu ảnh mới vào thư mục
                    fileName = MyTool.UploadImageToFolder(ImageFile, "Books");

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        // Lưu đường dẫn ảnh vào cơ sở dữ liệu
                        book.Image = "/Images/Books/" + fileName;
                    }
                    else
                    {
                        ModelState.AddModelError("Image", "Có lỗi khi tải lên ảnh.");
                    }
                }
            }
            else
            {
                // Giữ nguyên đường dẫn ảnh cũ nếu không có ảnh mới
                book.Image = _context.Books.AsNoTracking().FirstOrDefault(b => b.BookId == id)?.Image;
            }

            // Kiểm tra ModelState trước khi lưu dữ liệu
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Cập nhật sách thành công.");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            _notifyService.Error("Cập nhật sách thất bại. Vui lòng kiểm tra lại thông tin.");
            ViewData["DanhMuc"] = new SelectList(_context.Genres, "GenreId", "GenreName", book.GenreId);
            ViewData["NXB"] = new SelectList(_context.Nxbs, "Nxbid", "Nxbname", book.Nxbid);
            return View(book);
        }


        // GET: Admin/AdminBooks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Genre)
                .Include(b => b.Nxb)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Admin/AdminBooks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                try
                {
                    // Kiểm tra xem sách có ảnh không
                    if (!string.IsNullOrEmpty(book.Image))
                    {
                        // Lấy đường dẫn đầy đủ của ảnh
                        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", book.Image.TrimStart('/'));

                        // Kiểm tra xem tệp ảnh có tồn tại không và xóa nó nếu có
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }

                    _context.Books.Remove(book);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Xóa sách thành công.");
                }
                catch (Exception ex)
                {
                    _notifyService.Error($"Xóa sách thất bại: {ex.Message}");
                    return RedirectToAction(nameof(Delete), new { id = id });
                }
            }
            else
            {
                _notifyService.Error("Không tìm thấy sách.");
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }




    }
}
