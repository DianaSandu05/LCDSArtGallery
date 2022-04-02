using LCDSArtGallery.DataAccess.Repository.IRepository;
using LCDSArtGallery.Models;
using LCDSArtGalleryWeb.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace LCDSArtGallery.DataAccess.Repository
{
    public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailsRepository
    {
        private ApplicationDbContext _db;

        public OrderDetailsRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        public void Update(OrderDetails obj)
        {
            _db.OrderDetail.Update(obj);
        }
    }
}
