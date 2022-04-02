using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace LCDSArtGallery.Models.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }
        public IEnumerable<SelectListItem> ArtistList { get; set; }
        public IEnumerable<SelectListItem> ProductTypeList { get; set; }
    }
}
