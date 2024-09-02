using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using H2TSHOP2024.Models;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace H2TSHOP2024.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminNxbsController : Controller
    {
        private readonly BookstoreDbContext _context;
        public INotyfService _notifyService { get; }

        public AdminNxbsController(BookstoreDbContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/AdminNxbs
        public async Task<IActionResult> Index()
        {
            return View(await _context.Nxbs.ToListAsync());
        }

        // GET: Admin/AdminNxbs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nxb = await _context.Nxbs
                .FirstOrDefaultAsync(m => m.Nxbid == id);
            if (nxb == null)
            {
                return NotFound();
            }

            return View(nxb);
        }

        // GET: Admin/AdminNxbs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminNxbs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nxbid,Nxbname")] Nxb nxb)
        {
            // Kiểm tra trùng lặp tên nhà xuất bản
            if (_context.Nxbs.Any(n => n.Nxbname == nxb.Nxbname))
            {
                ModelState.AddModelError("Nxbname", "Tên nhà xuất bản đã tồn tại.");
                _notifyService.Warning("Tên nhà xuất bản đã tồn tại.");
                return View(nxb);
            }

            if (ModelState.IsValid)
            {
                _context.Add(nxb);
                await _context.SaveChangesAsync();
                _notifyService.Success("Tạo mới nhà xuất bản thành công.");
                return RedirectToAction(nameof(Index));
            }

            return View(nxb);
        }

        // GET: Admin/AdminNxbs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nxb = await _context.Nxbs.FindAsync(id);
            if (nxb == null)
            {
                return NotFound();
            }
            return View(nxb);
        }

        // POST: Admin/AdminNxbs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Nxbid,Nxbname")] Nxb nxb)
        {
            if (id != nxb.Nxbid)
            {
                return NotFound();
            }

            // Kiểm tra trùng lặp tên nhà xuất bản nhưng bỏ qua chính nó
            if (_context.Nxbs.Any(n => n.Nxbname == nxb.Nxbname && n.Nxbid != id))
            {
                ModelState.AddModelError("Nxbname", "Tên nhà xuất bản đã tồn tại.");
                return View(nxb);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nxb);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Cập nhật nhà xuất bản thành công.");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NxbExists(nxb.Nxbid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(nxb);
        }

        // GET: Admin/AdminNxbs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nxb = await _context.Nxbs
                .FirstOrDefaultAsync(m => m.Nxbid == id);
            if (nxb == null)
            {
                return NotFound();
            }

            return View(nxb);
        }

        // POST: Admin/AdminNxbs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nxb = await _context.Nxbs.FindAsync(id);
            if (nxb != null)
            {
                _context.Nxbs.Remove(nxb);
                await _context.SaveChangesAsync();
                _notifyService.Success("Xóa nhà xuất bản thành công.");
            }
            else
            {
                _notifyService.Error("Nhà xuất bản không tồn tại.");
            }

            return RedirectToAction(nameof(Index));
        }

        private bool NxbExists(int id)
        {
            return _context.Nxbs.Any(e => e.Nxbid == id);
        }
    }
}
