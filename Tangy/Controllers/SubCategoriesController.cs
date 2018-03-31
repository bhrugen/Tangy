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
                SubCategoryList = _db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToList()
            };

            return View(model);
        }

    }
}