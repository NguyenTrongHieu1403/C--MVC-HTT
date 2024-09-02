using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using H2TSHOP2024.Models;

namespace H2TSHOP2024.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrdersController : Controller
    {
        private readonly BookstoreDbContext _context;

        public OrdersController(BookstoreDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Orders
        public async Task<IActionResult> Index()
        {
            var bookstoreDbContext = _context.Orders.Include(o => o.Cus).Include(o => o.Location).Include(o => o.OrderStatus);
            return View(await bookstoreDbContext.ToListAsync());
        }

        // GET: Admin/Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Cus)
                .Include(o => o.Location)
                .Include(o => o.OrderStatus)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Admin/Orders/Create
        public IActionResult Create()
        {
            ViewData["CusId"] = new SelectList(_context.Customers, "CusId", "CusId");
            ViewData["LocationId"] = new SelectList(_context.Locations, "LocationId", "LocationId");
            ViewData["OrderStatusId"] = new SelectList(_context.OrderStatuses, "Osid", "Osid");
            return View();
        }

        // POST: Admin/Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,CusId,CreateDate,OrderStatusId,IsDeleted,LocationId,PaymentMethod,IsPaid")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CusId"] = new SelectList(_context.Customers, "CusId", "CusId", order.CusId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "LocationId", "LocationId", order.LocationId);
            ViewData["OrderStatusId"] = new SelectList(_context.OrderStatuses, "Osid", "Osid", order.OrderStatusId);
            return View(order);
        }

        // GET: Admin/Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CusId"] = new SelectList(_context.Customers, "CusId", "CusId", order.CusId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "LocationId", "LocationId", order.LocationId);
            ViewData["OrderStatusId"] = new SelectList(_context.OrderStatuses, "Osid", "Osid", order.OrderStatusId);
            return View(order);
        }

        // POST: Admin/Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,CusId,CreateDate,OrderStatusId,IsDeleted,LocationId,PaymentMethod,IsPaid")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["CusId"] = new SelectList(_context.Customers, "CusId", "CusId", order.CusId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "LocationId", "LocationId", order.LocationId);
            ViewData["OrderStatusId"] = new SelectList(_context.OrderStatuses, "Osid", "Osid", order.OrderStatusId);
            return View(order);
        }

        // GET: Admin/Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Cus)
                .Include(o => o.Location)
                .Include(o => o.OrderStatus)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
