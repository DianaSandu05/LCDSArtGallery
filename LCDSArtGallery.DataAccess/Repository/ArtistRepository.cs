using LCDSArtGallery.DataAccess.Repository.IRepository;
using LCDSArtGallery.Models;
using LCDSArtGalleryWeb.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LCDSArtGallery.DataAccess.Repository
{
    public class ArtistRepository : Repository<Artist>, IArtistRepository
    {
        private readonly ApplicationDbContext _db;

        public ArtistRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Artist artist)
        {
            var objFromDb = _db.Artists.FirstOrDefault(s => s.Id == artist.Id);
            if (objFromDb != null)
            {
                objFromDb.FirstName = artist.FirstName;
                objFromDb.LastName = artist.LastName;
                objFromDb.Email = artist.Email;
                objFromDb.ContactNumber = artist.ContactNumber;
                objFromDb.Address = artist.Address;
                objFromDb.Overview = artist.Overview;
                objFromDb.Biography = artist.Biography;
               
            }
        }
    }
}
