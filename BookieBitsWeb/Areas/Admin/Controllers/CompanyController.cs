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
    public class CompanyController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;
      

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            
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
            Company company = new();
            if (ID == null || ID == 0)
            {

                //create
                return View(company);
            }
            else
            {
                //update
                company = _unitOfWork.Company.GetFirstOrDefault(u => u.ID == ID);
                return View(company);
            }
           

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company obj)
        {
            if (ModelState.IsValid)
            {
               
                if(obj.ID == 0)
                {
                    _unitOfWork.Company.Add(obj);
                    TempData["success"] = "Company Created Successfully ";
                }
                else
                {
                    _unitOfWork.Company.Update(obj);
                    TempData["success"] = "Company Updated Successfully ";
                }
                
                _unitOfWork.Save();
                
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
            var companyList = _unitOfWork.Company.GetAll();
            return Json(new { data = companyList });
        }

        [HttpDelete]
        public IActionResult Delete(int? ID)
        {
            var obj = _unitOfWork.Company.GetFirstOrDefault(x => x.ID == ID);

            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.Company.Remove(obj);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }


        #endregion


    }
}
