using LCDSArtGallery.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LCDSArtGallery.DataAccess.Repository.IRepository
{
    public interface IProductTypeRepository : IRepository<ProductType>
    {
        void Update(ProductType productType);
    }
}
