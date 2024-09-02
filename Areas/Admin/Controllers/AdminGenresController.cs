using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using H2TSHOP2024.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using System.Data;

namespace H2TSHOP2024.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminGenresController : Controller
    {
        private readonly BookstoreDbContext _context;
        public INotyfService _notifyService { get; }

        public AdminGenresController(BookstoreDbContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/Genres
        public async Task<IActionResult> Index()
        {
            return View(await _context.Genres.ToListAsync());
        }

        // GET: Admin/Genres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres
                .FirstOrDefaultAsync(m => m.GenreId == id);
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        // GET: Admin/Genres/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Genres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GenreId,GenreName")] Genre genre)
        {
            // Kiểm tra trùng lặp tên
            if (_context.Roles.Any(r => r.RoleName == genre.GenreName))
            {
                // Thêm thông báo lỗi vào ModelState
                ModelState.AddModelError("GenreName", "Tên thể loại đã tồn tại.");

                // Hiển thị thông báo cảnh báo sử dụng Toast Notification (nếu cần)
                _notifyService.Warning("Tên thể loại đã tồn tại.");

                // Trả về view với lỗi đã được thêm vào ModelState
                return View(genre);
            }

            if (ModelState.IsValid)
            {
                _context.Add(genre);
                await _context.SaveChangesAsync();
                _notifyService.Success("Tạo mới thể loại thành công");
                return RedirectToAction(nameof(Index));
            }

            // Trả về view trong trường hợp có lỗi khác
            return View(genre);
        }

        // GET: Admin/Genres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
            {
                return NotFound();
            }
            return View(genre);
        }

        // POST: Admin/Genres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GenreId,GenreName")] Genre genre)
        {
            if (id != genre.GenreId)
            {
                return NotFound();
            }

            // Kiểm tra trùng lặp tên thể loại nhưng bỏ qua chính nó
            if (_context.Genres.Any(g => g.GenreName == genre.GenreName && g.GenreId != id))
            {
                ModelState.AddModelError("GenreName", "Tên thể loại đã tồn tại.");
                return View(genre);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(genre);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Cập nhật thể loại thành công.");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GenreExists(genre.GenreId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(genre);
        }


        // GET: Admin/Genres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres
                .FirstOrDefaultAsync(m => m.GenreId == id);
            if (genre == null)
            {
                return NotFound();
            }

            return View(genre);
        }

        // POST: Admin/Genres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre != null)
            {
                _context.Genres.Remove(genre);
                await _context.SaveChangesAsync();
                _notifyService.Success("Xóa thể loại thành công.");
            }
            else
            {
                _notifyService.Error("Thể loại không tồn tại.");
            }

            return RedirectToAction(nameof(Index));
        }
        private bool GenreExists(int id)
        {
            return _context.Genres.Any(e => e.GenreId == id);
        }
    }
}
