using LCDSArtGallery.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LCDSArtGallery.DataAccess.Repository.IRepository
{
    public interface IArtistRepository : IRepository<Artist>
    {
        void Update(Artist artist);
    }
}
