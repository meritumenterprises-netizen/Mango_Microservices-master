using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xango.Models.Dto
{
	public class AddProductToCartDto
	{
		public string? UserId { get; set; } = null!;
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public int StockQuantity { get; set; }
	}
}
