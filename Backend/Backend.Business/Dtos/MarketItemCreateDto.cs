using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Business.Dtos
{
    public class MarketItemCreateDto
    {
        public string Name { get; set; } = null!;
        public int Price { get; set; }
        public int Stock { get; set; }
    }

    public class BuyItemDto
    {
        public string ItemName { get; set; } = null!;
        public int Quantity { get; set; }
    }

    public class SellItemDto
    {
        public string ItemName { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
