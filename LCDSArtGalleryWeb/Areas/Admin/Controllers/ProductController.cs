using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LCDSArtGallery.DataAccess.Repository.IRepository;
using LCDSArtGallery.Models;
using LCDSArtGallery.Models.ViewModels;
using LCDSArtGallery.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PagedList;

namespace LCDSArtGallery.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = SD.Admin_Role)]
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product=new Product(),
         /*       CustomerList = _unitOfWork.Customer.GetAll().Select(i=> new SelectListItem { 
                    Text = i.FirstName,
                    Value = i.Id.ToString()
                }),*/
                ProductTypeList = _unitOfWork.ProductType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                //DS: change here and retrieve users based on identity Role = Artist
            /*    ArtistList = _unitOfWork.Artist.GetAll().Select(i => new SelectListItem
                {
                    Text = i.FirstName,
                    Value = i.Id.ToString()
                })*/
            };
            if (id == null)
            {
                //this is for create
                return View(productVM);
            }
            //this is for edit
            productVM.Product = _unitOfWork.Product.Get(id.GetValueOrDefault());
            if (productVM.Product == null)
            {
                return NotFound();
            }
            return View(productVM);

        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SD.Admin_Role)]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"images\products");
                    var extenstion = Path.GetExtension(files[0].FileName);

                    if (productVM.Product.ImageUrl != null)
                    {
                        //this is an edit and we need to remove old image
                        var imagePath = Path.Combine(webRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using(var filesStreams = new FileStream(Path.Combine(uploads,fileName+extenstion),FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }
                    productVM.Product.ImageUrl = @"\images\products\" + fileName + extenstion;
                }
                else
                {
                    //update when they do not change the image
                    if(productVM.Product.Id != 0)
                    {
                        Product objFromDb = _unitOfWork.Product.Get(productVM.Product.Id);
                        productVM.Product.ImageUrl = objFromDb.ImageUrl;
                    }
                }


                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);

                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
/*                productVM.CustomerList = _unitOfWork.Customer.GetAll().Select(i => new SelectListItem
                {
                    Text = i.FirstName,
                    Value = i.Id.ToString()
                });*/
                productVM.ProductTypeList = _unitOfWork.ProductType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                //change here and retrieve user with role Artist
             /*   productVM.ArtistList = _unitOfWork.Artist.GetAll().Select(i => new SelectListItem
                {
                    Text = i.FirstName,
                    Value = i.Id.ToString()
                });*/
                if (productVM.Product.Id != 0)
                {
                    productVM.Product = _unitOfWork.Product.Get(productVM.Product.Id);
                }
            }
            return View(productVM);
        }
        //Grouping - DS
        /* TO DO:
         1. Display all products and include product type
         3. Check if product type (FK) contains "OIL"
         4. Display all products from oil category
         5. return all products and reference in the view
         */
        public IActionResult GroupingOil(int? id)
        {

            var products = from p in _unitOfWork.Product.GetAll(includeProperties:"ProductType")
                           select p;
            if (products != null)
            {
                products = products.Where(p => p.ProductType.Name.ToUpper() == "Oil".ToUpper()
                                      );
            }
         
            return View(products);
        }
         public ViewResult SearchProduct(int? id, string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var products = from p in _unitOfWork.Product.GetAll()
                           select p;
            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.ToUpper().Contains(searchString.ToUpper())
                                       || p.Description.ToUpper().Contains(searchString.ToUpper())
                                       );
            }
            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(p => p.Name);
                    break;
                case "Size":
                    products = products.OrderBy(p => p.Size);
                    break;
                case "Price_desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                default:  // Name ascending 
                    products = products.OrderBy(p => p.Name);
                    break;
            }

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(products.ToPagedList(pageNumber, pageSize));
            var productList = _unitOfWork.Product.GetFirstOrDefault(i => i.Id == id);
            return View(productList);
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Product.GetAll(includeProperties:"ProductType");
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _unitOfWork.Product.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            string webRootPath = _hostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRootPath, objFromDb.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            _unitOfWork.Product.Remove(objFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });

        }

        #endregion
    }
}