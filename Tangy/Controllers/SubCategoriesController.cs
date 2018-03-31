using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tangy.Data;
using Tangy.Models;
using Tangy.Models.SubCategoryViewModels;

namespace Tangy.Controllers
{
    public class SubCategoriesController : Controller
    {
        private readonly ApplicationDbContext _db;


        [TempData]
        public string StatusMessage { get; set; }


        public SubCategoriesController(ApplicationDbContext db)
        {
            _db = db;
        }

        //GET Action 
        public async Task<IActionResult> Index()
        {
            var subCategories = _db.SubCategory.Include(s => s.Category);

            return View(await subCategories.ToListAsync());
        }

        //GET Action for Create
        public IActionResult Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = _db.Category.ToList(),
                SubCategory = new SubCategory(),
                SubCategoryList = _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToList()
            };

            return View(model);
        }

        //POST Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if(ModelState.IsValid)
            {
                var doesSubCategoryExists = _db.SubCategory.Where(s => s.Name == model.SubCategory.Name).Count();
                var doesSubCatAndCatExists = _db.SubCategory.Where(s => s.Name == model.SubCategory.Name && s.CategoryId==model.SubCategory.CategoryId).Count();


                if(doesSubCategoryExists > 0 && model.isNew)
                {
                    //error
                    StatusMessage = "Error : Sub Category Name already Exists";
                }
                else
                {
                    if(doesSubCategoryExists==0 && !model.isNew)
                    {
                        //error 
                        StatusMessage = "Error : Sub Category does not exists";
                    }
                    else
                    {
                        if(doesSubCatAndCatExists > 0)
                        {
                            //error
                            StatusMessage = "Error : Category and Sub Cateogry combination exists";
                        }
                        else
                        {
                            _db.Add(model.SubCategory);
                            await _db.SaveChangesAsync();
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }

            }
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = _db.Category.ToList(),
                SubCategory = model.SubCategory,
                SubCategoryList = _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToList(),
                StatusMessage = StatusMessage
            };
            return View(modelVM);

        }



        //GET Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = _db.Category.ToList(),
                SubCategory = subCategory,
                SubCategoryList = _db.SubCategory.Select(p => p.Name).Distinct().ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,SubCategoryAndCategoryViewModel model)
        {
            if(ModelState.IsValid)
            {
                var doesSubCategoryExists = _db.SubCategory.Where(s => s.Name == model.SubCategory.Name).Count();
                var doesSubCatAndCatExists = _db.SubCategory.Where(s => s.Name == model.SubCategory.Name && s.CategoryId == model.SubCategory.CategoryId).Count();

                if(doesSubCategoryExists == 0)
                {
                    StatusMessage = "Error : Sub Category does not exists. You cannot add a new subcategory here.";
                }
                else
                {
                    if(doesSubCatAndCatExists > 0)
                    {
                        StatusMessage = "Error : Category and Sub Category combination already exists.";
                    }
                    else
                    {
                        var subCatFromDb = _db.SubCategory.Find(id);
                        subCatFromDb.Name = model.SubCategory.Name;
                        subCatFromDb.CategoryId = model.SubCategory.CategoryId;
                        await _db.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
                
            }
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = _db.Category.ToList(),
                SubCategory = model.SubCategory,
                SubCategoryList = _db.SubCategory.Select(p => p.Name).Distinct().ToList(),
                StatusMessage = StatusMessage
            };
            return View(modelVM);
        }

    }
}