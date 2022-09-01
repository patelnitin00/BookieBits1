using BookieBits.DataAccess.Repository.IRepository;
using BookieBits.Models;
using BookieBits.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections;
using System.Collections.Generic;

namespace BookieBitsWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;
        //to access wwwroot foldr we need webHost environment
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            /*IEnumerable<Product> productsList = _unitOfWork.Product.GetAll();
            return View(productsList);*/
            return View();
        }


        public IActionResult Upsert(int? ID)
        {
            //we need catagegory and Covertype for product
            //so only product model is not sufficient
            // we need ViewModel to bind all
            ProductVM productVM = new()
            {
                Product = new(),
                CategoryListItems = _unitOfWork.Category.GetAll().Select(
                    u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.ID.ToString()
                    }),
                CoverTypeListItems = _unitOfWork.CoverType.GetAll().Select(
                    x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.ID.ToString()
                    })
            };




            if (ID == null || ID == 0)
            {

                //create
                return View(productVM);
            }
            else
            {
                //update
                productVM.Product = _unitOfWork.Product.GetFirstOrDefault(u => u.ID == ID);
                return View(productVM);
            }
           

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                //1. get root path
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                   /* string fileName = "";
                    string uniqueNumber = Guid.NewGuid().ToString();
                    fileName = uniqueNumber + file.FileName;*/
                    string fileName = Guid.NewGuid().ToString();

                    var upload = Path.Combine(wwwRootPath, @"images\products");

                    var ext = Path.GetExtension(file.FileName);

                    if(obj.Product.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var stream = new FileStream(Path.Combine(upload, fileName + ext), FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    obj.Product.ImageUrl = @"\images\products\" + fileName + ext;
                }


                if(obj.Product.ID == 0)
                {
                    _unitOfWork.Product.Add(obj.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(obj.Product);
                }
                
                _unitOfWork.Save();
                TempData["success"] = "Product Created Successfully ";
                return RedirectToAction("Index");
            }
            return View(obj);
  
        }




        //////////////////////////////////////////////////////
        #region API CALLS
        //here we will be call API calls

        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = _unitOfWork.Product.GetAll(includeProperties:"category,coverType");
            return Json(new { data = productList });
        }

        [HttpDelete]
        public IActionResult Delete(int? ID)
        {
            var obj = _unitOfWork.Product.GetFirstOrDefault(x => x.ID == ID);

            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }


            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }


        #endregion


    }
}
