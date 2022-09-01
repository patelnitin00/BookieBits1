using BookieBits.DataAccess.Repository;
using BookieBits.DataAccess.Repository.IRepository;
using BookieBits.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace BookieBitsWeb.Controllers;
[Area("Admin")]

public class CoverTypeController : Controller
{
    public readonly IUnitOfWork _unitOfWork;
    

    public CoverTypeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        IEnumerable<CoverType> coverTypes = _unitOfWork.CoverType.GetAll();
        return View(coverTypes);
    }


    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CoverType obj)
    {
        if (!ModelState.IsValid)
        {
            return View(obj);
        }

        _unitOfWork.CoverType.Add(obj);
        _unitOfWork.Save();
        TempData["Success"] = "Cover Type Created Successfully";

        return RedirectToAction("Index");
    }

    public IActionResult Edit(int? ID)
    {
        if(ID == null || ID == 0)
        {
            return NotFound();
        }

        CoverType coverType = _unitOfWork.CoverType.GetFirstOrDefault(x=>x.ID == ID);

        if(coverType == null)
        {
            return NotFound();
        }

        return View(coverType);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(CoverType obj)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.CoverType.Update(obj);
            _unitOfWork.Save();
            TempData["Success"] = "Cover Type Updated Successfully";
            return RedirectToAction("Index");
        }

        return View(obj);
    }


    public IActionResult Delete(int? ID)
    {
        if(ID==null || ID == 0)
        {
            return NotFound();
        }

        CoverType coverType = _unitOfWork.CoverType.GetFirstOrDefault(x=>x.ID==ID);

        if (coverType == null)
        {
            return NotFound();
        }

        _unitOfWork.CoverType.Remove(coverType);
        _unitOfWork.Save();
        TempData["deleteCategory"] = "Covertype Deleted Successfully";

        return RedirectToAction("Index");
    }


}
