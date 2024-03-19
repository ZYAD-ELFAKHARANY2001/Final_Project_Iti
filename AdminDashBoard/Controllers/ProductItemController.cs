using AutoMapper;
using Jumia.Application.IServices;
using Jumia.Dtos.ProductItems;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminDashBoard.Controllers
{
    public class ProductItemController : Controller
    {
        private readonly IProductItemsService _productItemsService;
        private readonly IMapper _mapper;
        public ProductItemController(IProductItemsService productItemsService, IMapper mapper)
        {
            _productItemsService = productItemsService;
            _mapper = mapper;

        }

        // GET: ProductItem
        public ActionResult GetAll()
        {
            return View();
        }

        // GET: ProductItem/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProductItem/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductItem/Create
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

        // GET: ProductItem/Edit/5
        public async Task<IActionResult> Edit([FromRoute]int id)
        {
            var res = await _productItemsService.GetProductItemsByID(id);
            if(res.Equals(null))
            {
                return NotFound();
            }
            //var createOrUpdateProductItemDto = _mapper.Map<CreatOrUpdateProductItemsDTO>(res.Entity);
            return View(res);
        }

        // POST: ProductItem/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CreatOrUpdateProductItemsDTO creatOrUpdateProductItem, List<IFormFile> images)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    var res = await _productItemsService.Update(creatOrUpdateProductItem, images);
                    return RedirectToAction(nameof(GetAll));

                }
                return View(creatOrUpdateProductItem);

            }
            catch
            {
                return View(creatOrUpdateProductItem);
            }
        }

        // GET: ProductItem/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductItem/Delete/5
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
