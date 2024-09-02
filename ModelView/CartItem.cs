namespace H2TSHOP2024.ViewModels
{
	public class CartItem
	{
		public int BookId { get; set; } // Mã sách
		public string BookName { get; set; } // Tên sách
		public double Price { get; set; } // Giá
		public string Image { get; set; } // Hình ảnh
		public int Quantity { get; set; } // Số lượng

		public double ThanhTien => Quantity * Price;
	}
}
