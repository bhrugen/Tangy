using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tangy.Data;
using Tangy.Models;
using Tangy.Models.OrderDetailsViewModel;
using Tangy.Utility;

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


        [Authorize]
        public IActionResult OrderHistory()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            List<OrderDetailsViewModel> OrderDetailsVM = new List<OrderDetailsViewModel>();

            List<OrderHeader> OrderHeaderList = _db.OrderHeader.Where(u => u.UserId == claim.Value).OrderByDescending(u => u.OrderDate).ToList();

            foreach(OrderHeader item in OrderHeaderList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetail = _db.OrderDetails.Where(o => o.OrderId == item.Id).ToList()
                };
                OrderDetailsVM.Add(individual);

            }
            return View(OrderDetailsVM);
        }

        [Authorize(Roles =SD.AdminEndUser)]
        public IActionResult ManageOrder()
        {
            List<OrderDetailsViewModel> OrderDetailsVM = new List<OrderDetailsViewModel>();
            List<OrderHeader> OrderHeaderList = _db.OrderHeader.Where(o=>o.Status==SD.StatusSubmitted || o.Status==SD.StatusInProcess)
                .OrderByDescending(u => u.PickUpTime).ToList();

            foreach (OrderHeader item in OrderHeaderList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetail = _db.OrderDetails.Where(o => o.OrderId == item.Id).ToList()
                };
                OrderDetailsVM.Add(individual);

            }
            return View(OrderDetailsVM);
        }



        [Authorize(Roles =SD.AdminEndUser)]
        public async Task<IActionResult> OrderPrepare(int orderId)
        {
            OrderHeader orderHeader = _db.OrderHeader.Find(orderId);
            orderHeader.Status = SD.StatusInProcess;
            await _db.SaveChangesAsync();
            return RedirectToAction("ManageOrder", "Order");

        }

        [Authorize(Roles = SD.AdminEndUser)]
        public async Task<IActionResult> OrderReady(int orderId)
        {
            OrderHeader orderHeader = _db.OrderHeader.Find(orderId);
            orderHeader.Status = SD.StatusReady;
            await _db.SaveChangesAsync();
            return RedirectToAction("ManageOrder", "Order");

        }

        [Authorize(Roles = SD.AdminEndUser)]
        public async Task<IActionResult> OrderCancel(int orderId)
        {
            OrderHeader orderHeader = _db.OrderHeader.Find(orderId);
            orderHeader.Status = SD.StatusCancelled;
            await _db.SaveChangesAsync();
            return RedirectToAction("ManageOrder", "Order");

        }

        //GET Order Pickup
        public IActionResult OrderPickup(string searchEmail=null,string searchPhone=null,string searchOrder=null)
        {
            List<OrderDetailsViewModel> OrderDetailsVM = new List<OrderDetailsViewModel>();

            if (searchEmail != null || searchPhone != null || searchOrder != null)
            {
                //filtering the criteria
                var user = new ApplicationUser();
                List<OrderHeader> OrderHeaderList = new List<OrderHeader>();

                if(searchOrder!=null)
                {
                    OrderHeaderList = _db.OrderHeader.Where(o => o.Id == Convert.ToInt32(searchOrder)).ToList();
                }
                else
                {
                    if(searchEmail!=null)
                    {
                        user = _db.Users.Where(u => u.Email.ToLower().Contains(searchEmail.ToLower())).FirstOrDefault();
                    }
                    else
                    {
                        if (searchPhone != null)
                        {
                            user = _db.Users.Where(u => u.PhoneNumber.ToLower().Contains(searchPhone.ToLower())).FirstOrDefault();
                        }
                    }
                }
                if(user!=null || OrderHeaderList.Count>0)
                {
                    if(OrderHeaderList.Count==0)
                    {
                        OrderHeaderList = _db.OrderHeader.Where(o => o.UserId == user.Id).OrderByDescending(o => o.OrderDate).ToList();
                    }
                    foreach(OrderHeader item in OrderHeaderList)
                    {
                        OrderDetailsViewModel individual = new OrderDetailsViewModel
                        {
                            OrderHeader = item,
                            OrderDetail = _db.OrderDetails.Where(o => o.OrderId == item.Id).ToList()
                        };
                        OrderDetailsVM.Add(individual);
                    }
                }


            }
            else
            {
                List<OrderHeader> OrderHeaderList = _db.OrderHeader.Where(o => o.Status == SD.StatusReady)
                    .OrderByDescending(u => u.PickUpTime).ToList();

                foreach (OrderHeader item in OrderHeaderList)
                {
                    OrderDetailsViewModel individual = new OrderDetailsViewModel
                    {
                        OrderHeader = item,
                        OrderDetail = _db.OrderDetails.Where(o => o.OrderId == item.Id).ToList()
                    };
                    OrderDetailsVM.Add(individual);

                }
            }
            return View(OrderDetailsVM);
        }
    }
}