using LCDSArtGallery.DataAccess.Repository.IRepository;
using LCDSArtGallery.Models;
using LCDSArtGalleryWeb.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LCDSArtGallery.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;

        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

    }
}
