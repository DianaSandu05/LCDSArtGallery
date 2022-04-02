using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LCDSArtGallery.DataAccess.Repository.IRepository;
using LCDSArtGallery.Models;
using LCDSArtGallery.Utility;
using Microsoft.AspNetCore.Mvc;

namespace LCDSArtGallery.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ArtistController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ArtistController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Artist artist = new Artist();
            if (id == null)
            {
                //this is for create
                return View(artist);
            }
            //this is for edit
            artist = _unitOfWork.Artist.Get(id.GetValueOrDefault());
            if (artist == null)
            {
                return NotFound();
            }
            return View(artist);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Artist artist)
        {
            if (ModelState.IsValid)
            {
                if (artist.Id == 0)
                {
                    _unitOfWork.Artist.Add(artist);

                }
                else
                {
                    _unitOfWork.Artist.Update(artist);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(artist);
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Artist.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Artist.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.Artist.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });

        }

        #endregion
    }
}