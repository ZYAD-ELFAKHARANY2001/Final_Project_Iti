﻿using Jumia.Application.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminDashBoard.Controllers
{
    public class ShippmentController : Controller
    {
        private readonly IShippmentService _shippmentService;
        private readonly IOrderService _orderService;
        public ShippmentController(IShippmentService shippmentService, IOrderService orderService)
        {
            _shippmentService = shippmentService;
            _orderService = orderService;
        }
        // GET: ShippmentController
        public  ActionResult Index()
        {
            
            return View();
        }

        // GET: ShippmentController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var AllData = await _shippmentService.GetAll();
            var details = AllData.Where(o => o.OrderId == id).FirstOrDefault();
            ViewBag.orderId = details.OrderId;
            return View(details);
        }

        // GET: ShippmentController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ShippmentController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ShippmentController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ShippmentController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ShippmentController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ShippmentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
