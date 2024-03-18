using AutoMapper;
using Jumia.Application.IServices;
using Jumia.Application.Services;
using Jumia.Dtos.Category;
using Jumia.Dtos.Product;
using Microsoft.AspNetCore.Mvc;

namespace AdminDashBoard.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductServices _productService;
        private readonly IMapper _mapper;


        public ProductController(IProductServices productService,IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;

        }
        // GET: ProductController
        public async Task<ActionResult> GetPagination()
        {
            var Prds = await _productService.GetAllPagination(5, 1);
            return View(Prds);
        }
        public ActionResult Create()
        {

            return View();
        }

        // POST: ProductController/Create
        [HttpPost]
        public async Task<ActionResult> Create(CreateOrUpdateProductDto ProductDto,List<IFormFile> Images)
        {
            if (ModelState.IsValid)
            {

                if (Images != null)
                {
                    foreach (var img in Images)
                    {
                        var imageBytes = new byte[img.Length];
                        using (var stream = img.OpenReadStream())
                        {
                            await stream.ReadAsync(imageBytes, 0, imageBytes.Length);
                        }
                        ProductDto.Images.Add(imageBytes);
                    }
                }


                var res = await _productService.Create(ProductDto, Images);

                if (res.IsSuccess)
                {

                    return RedirectToAction("GetPagination");
                }


            }
            return View(ProductDto);



        }



        public async Task<ActionResult> Update([FromRoute]int id)
        {
            var res = await _productService.GetOne(id);

            if (res == null)
            {
                return NotFound();

            }

            var productDto = _mapper.Map<CreateOrUpdateProductDto>(res.Entity);
            return View(productDto);
        }



        [HttpPost]
        public async Task<ActionResult> Update(CreateOrUpdateProductDto productDto, List<IFormFile> Image)
        {
            if (ModelState.IsValid)
            {

                var res  = await  _productService.Update(productDto, Image);
                return RedirectToAction(nameof(GetPagination));

            }

            return View(productDto);

        }





        public async Task<ActionResult> Delete(int id)
        {
            var res = await _productService.GetOne(id);
            if (res == null)
            {
                return NotFound();
            }

            var ProductToDelete = _mapper.Map<CreateOrUpdateProductDto>(res.Entity);
            await _productService.Delete(ProductToDelete);


            return RedirectToAction(nameof(GetPagination));
        }

    }
}
