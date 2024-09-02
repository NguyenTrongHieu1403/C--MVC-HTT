using H2TSHOP2024.Models;
using H2TSHOP2024.ViewModels;
using Microsoft.AspNetCore.Mvc;
using H2TSHOP2024.Extension;

namespace H2TSHOP2024.Controllers
{
	public class CartController : Controller
	{
		private readonly BookstoreDbContext db;

		public CartController(BookstoreDbContext context)
		{
			db = context;
		}

		const string CART_KEY = "MYCART";
		public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(CART_KEY) ?? new List<CartItem>();

		public IActionResult Index()
		{
			return View(Cart);
		}

		public IActionResult AddToCart(int id, int quantity = 1)
		{
			var gioHang = Cart;
			var item = gioHang.SingleOrDefault(p => p.BookId == id);
			if (item == null)
			{
				var book = db.Books.SingleOrDefault(p => p.BookId == id);
				if (book == null)
				{
					TempData["Message"] = $"Không tìm thấy sách có mã {id}";
					return Redirect("/404");
				}
				item = new CartItem
				{
					BookId = book.BookId,
					BookName = book.BookName,
					Price = (double)book?.Price,
					Image = book.Image ?? string.Empty,
					Quantity = quantity
				};
				gioHang.Add(item);
			}
			else
			{
				item.Quantity += quantity;
			}

			HttpContext.Session.Set(CART_KEY, gioHang);

			return RedirectToAction("Index");
		}

		public IActionResult RemoveCart(int id)
		{
			var gioHang = Cart;
			var item = gioHang.SingleOrDefault(p => p.BookId == id);
			if (item != null)
			{
				gioHang.Remove(item);
				HttpContext.Session.Set(CART_KEY, gioHang);
			}
			return RedirectToAction("Index");
		}
	}
}
