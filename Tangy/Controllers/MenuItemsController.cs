using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tangy.Data;
using Tangy.Models;
using Tangy.Models.MenuItemViewModels;
using Tangy.Utility;

namespace Tangy.Controllers
{
    public class MenuItemsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHostingEnvironment _hostingEnvironment;

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }


        public MenuItemsController(ApplicationDbContext db, IHostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            MenuItemVM = new MenuItemViewModel()
            {
                Category = _db.Category.ToList(),
                MenuItem = new Models.MenuItem()
            };
        }


        //GET : MenuItems
        public async Task<IActionResult> Index()
        {
            var menuItems = _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory);
            return View(await menuItems.ToListAsync());
        }

        //GET : MenuItems Create
        public IActionResult Create()
        {
            return View(MenuItemVM);
        }

        //POST : MenuItems Create
        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());
            if(!ModelState.IsValid)
            {
                return View(MenuItemVM);
            }

            _db.MenuItem.Add(MenuItemVM.MenuItem);
            await _db.SaveChangesAsync();

            //Image Being Saved
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = _db.MenuItem.Find(MenuItemVM.MenuItem.Id);

            if(files[0]!=null && files[0].Length>0)
            {
                //when user uploads an image
                var uploads = Path.Combine(webRootPath, "images");
                var extension = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."), files[0].FileName.Length - files[0].FileName.LastIndexOf("."));

                using (var filestream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension;
            }
            else
            {
                //when user does not upload image
                var uploads = Path.Combine(webRootPath, @"images\"+ SD.DefaultFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + MenuItemVM.MenuItem.Id + ".png");
                menuItemFromDb.Image= @"\images\" + MenuItemVM.MenuItem.Id + ".png";
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }


        public JsonResult GetSubCategory(int CategoryId)
        {
            List<SubCategory> subCategoryList = new List<SubCategory>();

            subCategoryList = (from subCategory in _db.SubCategory
                               where subCategory.CategoryId == CategoryId
                               select subCategory).ToList();

            return Json(new SelectList(subCategoryList, "Id", "Name"));
        }

        //GET : Edit MenuItem
        public async Task<IActionResult> Edit(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
            MenuItemVM.SubCategory = _db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToList();

            if(MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }
        
    }
}