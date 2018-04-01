using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tangy.Data;

namespace Tangy.Controllers
{
    public class CouponsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CouponsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _db.Coupons.ToListAsync());
        }
    }
}