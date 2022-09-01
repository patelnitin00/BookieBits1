using BookieBits.DataAccess;
using BookieBits.DataAccess.Repository.IRepository;
using BookieBits.Models;
using Microsoft.AspNetCore.Mvc;


namespace BookieBitsWeb.Controllers;
[Area("Admin")]
public class CategoryController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
         IEnumerable<Category> categoryList = _unitOfWork.Category.GetAll();
        return View(categoryList);
    }


   
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Category obj)
    {
        if(obj.Name == obj.DisplayOrder.ToString())
        {
           // ModelState.AddModelError("Equal", "Name and Display Order cannot be Same");
            ModelState.AddModelError("Name", "Name and Display Order cannot be Same");
            //above name is not key - it is a property - so error will be displayed under name textbox
        }

        if (ModelState.IsValid)
        {
            _unitOfWork.Category.Add(obj);
            _unitOfWork.Save();
            TempData["Success"] = "Category Created Successfully";
            return RedirectToAction("Index");
        }

        return View(obj);
    }


    public IActionResult Edit(int? ID)
    {
        if(ID == null || ID ==0)
        {
            return NotFound();
        }
        Category category = _unitOfWork.Category.GetFirstOrDefault(x=>x.ID == ID);

        if(category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Category obj)
    {
        if (obj.Name == obj.DisplayOrder.ToString())
        {
           // ModelState.AddModelError("Equal", "Name and Display Order cannot be Same");
            ModelState.AddModelError("Name", "Name and Display Order cannot be Same");
            //above name is not key - it is a property - so error will be displayed under name textbox
        }
        if (ModelState.IsValid)
        {
             _unitOfWork.Category.Update(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category Edited Successfully";
            return RedirectToAction("Index");
        }
        return View(obj);

        
    }


    public IActionResult Delete(int? ID)
    {
        if(ID == null || ID == 0)
        {
            return NotFound();
        }

        Category category = _unitOfWork.Category.GetFirstOrDefault(x=>x.ID == ID);
        if(category == null)
        {
            return NotFound();
        }

        //I want to put model here for delete
        _unitOfWork.Category.Remove(category);
        _unitOfWork.Save();
        TempData["deleteCategory"] = "Category Deleted Successfully";

        return RedirectToAction("Index");
    }


}
