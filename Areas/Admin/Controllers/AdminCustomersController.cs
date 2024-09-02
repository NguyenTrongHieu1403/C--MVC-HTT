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
    public class AdminCustomersController : Controller
    {
        private readonly BookstoreDbContext _context;
        public INotyfService _notifyService { get; }

        public AdminCustomersController(BookstoreDbContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

        // GET: Admin/AdminCustomers
        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 20;
            var IsCustomers = _context.Customers.AsNoTracking().Include(x=>x.Locations).OrderByDescending(x => x.CreatedDate);

/*
            ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "Description");

            List<SelectListItem> lsTrangThai = new List<SelectListItem>();

            lsTrangThai.Add(new SelectListItem() { Text = "Active", Value = "1" });
            lsTrangThai.Add(new SelectListItem() { Text = "Block", Value = "0" });*/


/*            ViewData["lsTrangThai"] = lsTrangThai;
*/

            PagedList<Customer> models = new PagedList<Customer>(IsCustomers, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;

            return View(models);
        }

        public async Task<IActionResult> Filter(int? idstatus, int? page)
        {
            var pageNumber = page ?? 1;
            var pageSize = 10;

            // Truy vấn danh sách khách hàng từ cơ sở dữ liệu
            var customers = _context.Customers.AsQueryable();

            // Lọc theo trạng thái Active (True/False)
            if (idstatus.HasValue && idstatus > 0)
            {
                bool isActive = idstatus == 1; // 1 = Active, 2 = Inactive
                customers = customers.Where(c => c.Active == isActive);
            }

            // Sắp xếp theo tên khách hàng
            var pagedCustomers = customers
                                 .OrderBy(c => c.Fullname)
                                 .ToPagedList(pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;

            // Trả về kết quả lọc
            return PartialView("_CustomerTablePartialView", pagedCustomers);
        }



















        // GET: Admin/AdminCustomers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CusId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Admin/AdminCustomers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminCustomers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CusId,Fullname,Email,Phone,CreatedDate,Active,BirthDate,Password,Sex")] Customer customer)
        {
            // Kiểm tra trùng lặp Email
            if (_context.Customers.Any(c => c.Email == customer.Email))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại.");
            }

            // Kiểm tra độ dài mật khẩu và định dạng không có dấu
            if (string.IsNullOrWhiteSpace(customer.Password) || customer.Password.Length < 5 || customer.Password.Length > 15 || !System.Text.RegularExpressions.Regex.IsMatch(customer.Password, @"^[a-zA-Z0-9]*$"))
            {
                ModelState.AddModelError("Password", "Mật khẩu phải có độ dài từ 5 đến 15 ký tự, không dấu và không chứa ký tự đặc biệt.");
            }
            else
            {
                // Hash mật khẩu nếu mật khẩu hợp lệ
                customer.Password = BCrypt.Net.BCrypt.HashPassword(customer.Password);
            }

            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                _notifyService.Success("Tạo mới khách hàng thành công");
                return RedirectToAction(nameof(Index));
            }
            else
            {
                _notifyService.Error("Tạo mới khách hàng thất bại. Vui lòng kiểm tra lại thông tin.");
            }

            return View(customer);
        }


        // GET: Admin/AdminCustomers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Admin/AdminCustomers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CusId,Fullname,Email,Phone,CreatedDate,Active,BirthDate,Password,Sex")] Customer customer)
        {
            if (id != customer.CusId)
            {
                return NotFound();
            }

            // Kiểm tra trùng lặp Email (không được trùng với những khách hàng khác)
            if (_context.Customers.Any(c => c.Email == customer.Email && c.CusId != id))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại.");
            }

            // Kiểm tra Password (nếu người dùng thay đổi mật khẩu)
            if (!string.IsNullOrEmpty(customer.Password))
            {
                if (customer.Password.Length < 5 || customer.Password.Length > 15 || !System.Text.RegularExpressions.Regex.IsMatch(customer.Password, @"^[a-zA-Z0-9]*$"))
                {
                    ModelState.AddModelError("Password", "Mật khẩu phải có độ dài từ 5 đến 15 ký tự, không dấu và không chứa ký tự đặc biệt.");
                }
                else
                {
                    // Hash password
                    customer.Password = BCrypt.Net.BCrypt.HashPassword(customer.Password);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Cập nhật thông tin khách hàng thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CusId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                _notifyService.Error("Cập nhật thông tin khách hàng thất bại. Vui lòng kiểm tra lại thông tin.");
            }

            return View(customer);
        }

        // GET: Admin/AdminCustomers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CusId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Admin/AdminCustomers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            _notifyService.Success("Xóa thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CusId == id);
        }
    }
}
