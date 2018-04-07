using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tangy.Data;
using Tangy.Models.OrderDetailsViewModel;

namespace Tangy.Controllers
{
    public class OrderController : Controller
    {
        private ApplicationDbContext _db;

        public OrderController(ApplicationDbContext db)
        {
            _db = db;
        }


        //CONFIRM GET
        [Authorize]
        public async Task<IActionResult> Confirm(int id)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            OrderDetailsViewModel OrderDetailsViewModel = new OrderDetailsViewModel()
            {
                OrderHeader = _db.OrderHeader.Where(o => o.Id == id && o.UserId == claim.Value).FirstOrDefault(),
                OrderDetail = _db.OrderDetails.Where(o => o.OrderId == id).ToList()
            };

            return View(OrderDetailsViewModel);
        }
    }
}