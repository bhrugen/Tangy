using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tangy.Data;
using Tangy.Models;
using Tangy.Models.HomeViewModel;

namespace Tangy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync(),
                Category = _db.Category.OrderBy(c=>c.DisplayOrder),
                Coupons = _db.Coupons.Where(c=>c.isActive==true).ToList()
            };
            return View(IndexVM);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var MenuItemFromDb = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == id).FirstOrDefaultAsync();

            ShoppingCart CartObj = new ShoppingCart()
            {
                MenuItem = MenuItemFromDb,
                MenuItemId = MenuItemFromDb.Id
            };

            return View(CartObj);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart CartObject)
        {
            CartObject.Id = 0;
            if(ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                CartObject.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = await _db.ShoppingCart.Where(c => c.ApplicationUserId == CartObject.ApplicationUserId
                                                    && c.MenuItemId == CartObject.MenuItemId).FirstOrDefaultAsync();
                
                if(cartFromDb==null)
                {
                    //this menu item does not exists
                    _db.ShoppingCart.Add(CartObject);
                }
                else
                {
                    //menu item exists in shopping cart for that user, so just update the count
                    cartFromDb.Count = cartFromDb.Count + CartObject.Count;
                }

                await _db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            else
            {
                var MenuItemFromDb = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == CartObject.MenuItemId).FirstOrDefaultAsync();

                ShoppingCart CartObj = new ShoppingCart()
                {
                    MenuItem = MenuItemFromDb,
                    MenuItemId = MenuItemFromDb.Id
                };

                return View(CartObj);
            }
        }
        
    }
}
