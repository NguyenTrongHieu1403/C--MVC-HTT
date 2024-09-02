using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using H2TSHOP2024.Models;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.CodeAnalysis.Scripting;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace H2TSHOP2024.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminAccountsController : Controller
    {
        private readonly BookstoreDbContext _context;
        public INotyfService _notifyService { get; }

        public AdminAccountsController(BookstoreDbContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;

        }

        // GET: Admin/AdminAccounts
        public async Task<IActionResult> Index()
        {
            ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "Description");

            List<SelectListItem> lsTrangThai = new List<SelectListItem>();
            lsTrangThai.Add(new SelectListItem() { Text = "Active", Value = "1" });
            lsTrangThai.Add(new SelectListItem() { Text = "Block", Value = "0" });
            ViewData["lsTrangThai"] = lsTrangThai;

            var bookstoreDbContext = _context.Accounts.Include(a => a.Role);
            return View(await bookstoreDbContext.ToListAsync());
        }

        // GET: Admin/AdminAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AcId == id);
            if (account == null)
            {
                return NotFound();
            }
            return View(account);
        }

        // GET: Admin/AdminAccounts/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            return View();
        }

        // POST: Admin/AdminAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AcId,Username,Password,Email,Phone,RoleId,Active,CreateDate")] Account account)
        {
            // Kiểm tra trùng lặp Username và Email
            if (_context.Accounts.Any(a => a.Username == account.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                _notifyService.Error("Tên đăng nhập đã tồn tại.");
            }
            if (_context.Accounts.Any(a => a.Email == account.Email))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại.");
                _notifyService.Error("Email đã tồn tại.");
            }

            // Kiểm tra định dạng không dấu cho Username
            if (!System.Text.RegularExpressions.Regex.IsMatch(account.Username, @"^[a-zA-Z0-9]+$"))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập chỉ được chứa các ký tự không dấu và số.");
                _notifyService.Error("Tên đăng nhập chỉ được chứa các ký tự không dấu và số.");
            }

            // Kiểm tra độ dài mật khẩu và định dạng không có dấu
            if (string.IsNullOrWhiteSpace(account.Password) || account.Password.Length < 5 || account.Password.Length > 15 || !System.Text.RegularExpressions.Regex.IsMatch(account.Password, @"^[a-zA-Z0-9]*$"))
            {
                ModelState.AddModelError("Password", "Mật khẩu phải có độ dài từ 5 đến 15 ký tự, không dấu và không chứa ký tự đặc biệt.");
                _notifyService.Error("Mật khẩu phải có độ dài từ 5 đến 15 ký tự, không dấu và không chứa ký tự đặc biệt.");
            }
            else
            {
                // Hash mật khẩu nếu mật khẩu hợp lệ
                account.Password = BCrypt.Net.BCrypt.HashPassword(account.Password);
            }

            if (ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                _notifyService.Success("Tạo tài khoản thành công.");
                return RedirectToAction(nameof(Index));
            }

            // Nếu dữ liệu không hợp lệ, hiển thị lại form cùng với thông báo lỗi
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", account.RoleId);
            return View(account);
        }


        // GET: Admin/AdminAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", account.RoleId);
            return View(account);
        }

        // POST: Admin/AdminAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AcId,Username,Password,Email,Phone,RoleId,Active,CreateDate")] Account account)
        {
            if (id != account.AcId)
            {
                _notifyService.Error("Không tìm thấy tài khoản.");
                return NotFound();
            }

            // Kiểm tra Username (không được trùng với những tài khoản khác)
            if (_context.Accounts.Any(a => a.Username == account.Username && a.AcId != id))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                _notifyService.Error("Tên đăng nhập đã tồn tại.");
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(account.Username, @"^[a-zA-Z0-9]*$"))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập chỉ được chứa ký tự không dấu và số.");
                _notifyService.Error("Tên đăng nhập chỉ được chứa ký tự không dấu và số.");
            }

            // Kiểm tra Email (không được trùng với những tài khoản khác)
            if (_context.Accounts.Any(a => a.Email == account.Email && a.AcId != id))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại.");
                _notifyService.Error("Email đã tồn tại.");
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(account.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                ModelState.AddModelError("Email", "Email không hợp lệ.");
                _notifyService.Error("Email không hợp lệ.");
            }

            // Kiểm tra Password (nếu người dùng thay đổi mật khẩu)
            if (!string.IsNullOrEmpty(account.Password))
            {
                if (account.Password.Length < 5 || account.Password.Length > 15 || !System.Text.RegularExpressions.Regex.IsMatch(account.Password, @"^[a-zA-Z0-9]*$"))
                {
                    ModelState.AddModelError("Password", "Mật khẩu phải có độ dài từ 5 đến 15 ký tự, không dấu và không chứa ký tự đặc biệt.");
                    _notifyService.Error("Mật khẩu phải có độ dài từ 5 đến 15 ký tự, không dấu và không chứa ký tự đặc biệt.");
                }
                else
                {
                    // Hash password
                    account.Password = BCrypt.Net.BCrypt.HashPassword(account.Password);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Chỉnh sửa tài khoản thành công.");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AcId))
                    {
                        _notifyService.Error("Không tìm thấy tài khoản.");
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", account.RoleId);
            return View(account);
        }


        // GET: Admin/AdminAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AcId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Admin/AdminAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                _context.Accounts.Remove(account);
            }

            await _context.SaveChangesAsync();
            _notifyService.Success("Xóa thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AcId == id);
        }
    }
}
