using System;
using System.Collections.Generic;
using System.Text;

namespace LCDSArtGallery.Models.ViewModels
{
    public class OrderVM
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetails> OrderDetails { get; set; }
    }
}
