﻿using AutoMapper;
using Jumia.Application.Contract;
using Jumia.Application.IServices;
using Jumia.Application.Services;
using Jumia.Application.Services.IServices;
using Jumia.Context.Migrations;
using Jumia.Dtos.Category;
using Jumia.Dtos.Product;
using Jumia.Dtos.ProductSpecificationSubCategory;
using Jumia.Dtos.Reports;
using Jumia.Dtos.SubCategorySpecifications;
using Jumia.Infrastructure;
using Jumia.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace AdminDashBoard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : BaseController
    {
        private readonly IProductServices _productService;
        private readonly ISubCategoryService _subCategoryService;
        private readonly IBrandService _brandService;
        private readonly IMapper _mapper;
        private readonly ISpecificationServices _specificationServices;
        private readonly ISubCategorySpecificationsService _subCategorySpecificationsService;
        private readonly IProductSpecificationSubCategoryServices _productSpecificationSubCategoryServices;
        private readonly IOrderItemService _OrderServe;
        private readonly IUnitOfWork _unitOfWork;


        public ProductController(IProductServices productService,
            IBrandService brandService,
            IMapper mapper, ISubCategoryService subCategoryService,
            ISpecificationServices specificationServices,
            ISubCategorySpecificationsService subCategorySpecificationsService,
            IProductSpecificationSubCategoryServices productSpecificationSubCategoryServices,
            IOrderItemService orderServe, IUnitOfWork unitOfWork)
        {
            _productService = productService;
            _mapper = mapper;
            _brandService = brandService;
            _subCategoryService = subCategoryService;
            _specificationServices = specificationServices;
            _subCategorySpecificationsService = subCategorySpecificationsService;
            _productSpecificationSubCategoryServices = productSpecificationSubCategoryServices;
            _OrderServe = orderServe;
            _unitOfWork = unitOfWork;

        }
        // GET: ProductController
        public async Task<ActionResult> GetPagination(int pageNumber = 1)
        {
            var pageSize = 10;
            var Prds = await _productService.GetAllPagination(pageSize, pageNumber);
            return View(Prds);
        }

       


        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create()
        {

            var subCategory = await _subCategoryService.GetAll(250, 1);
            var subCatName = subCategory.Entities.Select(a => new { a.Id, a.Name }).ToList();
            ViewBag.SubCategory = subCatName;

            var subCategorySpec = (await _subCategorySpecificationsService.GetAll()).ToList();
            ViewBag.subCategorySpecs = subCategorySpec;

            var brand = (await _brandService.GetAll()).Entities.Select(a => new { a.BrandID, a.Name }).ToList();
            ViewBag.brand = brand;
            return View();
        }

        // POST: ProductController/Create
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(CreateOrUpdateProductDto ProductDto,List<IFormFile> Images, CreateOrUpdateProductSpecificationSubCategory prdSubCategorySpecDto, int selectedSubCategoryId)
        {
            var subCategorySpec = (await _subCategorySpecificationsService.GetAll()).Where(i => i.SubCategoryId == selectedSubCategoryId).ToList();

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
                    if(prdSubCategorySpecDto != null) {
                               foreach (var specItems in subCategorySpec)
                    {
                       // var specName = (await _specificationServices.GetAll()).Where(s => s.Name == specItems).FirstOrDefault();
                        var subCategorySpecification = new CreateOrUpdateProductSpecificationSubCategory
                        {
                            ProductId = res.Entity.Id,
                            SubSpecId= specItems.Id,
                            Value=prdSubCategorySpecDto.Value
                        };
                        await _productSpecificationSubCategoryServices.Create(subCategorySpecification);
                    }
                        TempData["SuccessMessage1"] = "Product Created successfully.";
                        return RedirectToAction("GetPagination", TempData["SuccessMessage1"]);

                    }

                }


            }
            var subcategory = await _subCategoryService.GetAll(200, 1);
            var subcatname = subcategory.Entities.Select(a => new { a.Id, a.Name }).ToList();
            ViewBag.subcategory = subcatname;
            var brand = (await _brandService.GetAll()).Entities.Select(a => new { a.BrandID, a.Name }).ToList();
            ViewBag.brand = brand;
            TempData["SuccessMessage"] = "Failed";
            ViewBag.subCategorySpecs = subCategorySpec;
            return View(ProductDto);



        }
        [HttpGet]
        public async Task<IActionResult> Action(int selectedSubCategoryId)
        {
            var subCategorySpec = (await _subCategorySpecificationsService.GetAll()).Where(i => i.SubCategoryId == selectedSubCategoryId).ToList(); 
            ViewBag.subCategorySpecs = subCategorySpec;
            return PartialView("_SubCategorySpecsPartial");
        }

        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(int id)
        {
            var res = await _productService.GetOne(id);

            if (res == null)
            {
                return NotFound();

            }
            var subCategory = await _subCategoryService.GetAll(200, 1);
            var subCatName = subCategory.Entities.Select(a => new { a.Id, a.Name }).ToList();
            ViewBag.SubCategory = subCatName;
            var brand = (await _brandService.GetAll()).Entities.Select(a => new { a.BrandID, a.Name }).ToList();
            ViewBag.brand = brand;
            var productDto = _mapper.Map<CreateOrUpdateProductDto>(res.Entity);
            var prdSpecs = (await _productSpecificationSubCategoryServices.GetAll())
                 .Entities.Where(p => p.ProductId == id).Select(i => new { i.SpecificationName, i.Value });
            ViewBag.prdSpecs = prdSpecs;
            return View(productDto);
        }



        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(CreateOrUpdateProductDto productDto, List<IFormFile> Images)
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
                        productDto.Images.Add(imageBytes);
                    }
                }

                var res  = await  _productService.Update(productDto, Images);
                TempData["SuccessMessage2"] = "Product Updated successfully.";
                return RedirectToAction("GetPagination", TempData["SuccessMessage2"]);


            }
            var subCategory = await _subCategoryService.GetAll(200, 1);
            var subCatName = subCategory.Entities.Select(a => new { a.Id, a.Name }).ToList();
            ViewBag.SubCategory = subCatName;
            var brand = (await _brandService.GetAll()).Entities.Select(a => new { a.BrandID, a.Name }).ToList();
            ViewBag.brand = brand;
            var prdSpecs = (await _productSpecificationSubCategoryServices.GetAll())
               .Entities.Where(p => p.ProductId == productDto.Id).Select(i => new { i.SpecificationName, i.Value });
            ViewBag.prdSpecs = prdSpecs;
            TempData["SuccessMessage"] = "Failed.";
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
            var orderItems = (await _OrderServe.GetAllOrderItems()).Where(i => i.ProductName==res.Entity.Name).ToList();
            if (orderItems.Count== 0)
            {
                var del = await _productService.Delete(ProductToDelete);
                if (del.IsSuccess)
                {
                    TempData["SuccessMessage3"] = "Product Deleted Successfully";
                    return RedirectToAction(nameof(GetPagination));
                }
                else
                {
                    TempData["SuccessMessage"] = "Sorry, Failed to Delete this product";
                    return RedirectToAction(nameof(GetPagination));
                }
            }
            else
            {
                TempData["SuccessMessage"] = "Sorry, Failed to Delete this product";
                return RedirectToAction(nameof(GetPagination));

            }



        }


        //
        public async Task<IActionResult> OutOfStock()
        {
            var outOfStockProducts = await _productService.GetOutOfStockProducts();
            return View(outOfStockProducts);
        }

        public async Task<IActionResult> AlmostFinished()
        {
            int threshold = 5; // Set your desired threshold here
            var almostFinishedProducts = await _productService.GetProductsAlmostFinished(threshold);
            return View(almostFinishedProducts);
        }

        public async Task<IActionResult> TopProductsSold()
        {
            var topProducts = await _productService.GetTopProductsSold();
            return View(topProducts);
        }

        public async Task<IActionResult> OrdersPerMonth()
        {
            var ordersPerMonth = await _productService.GetOrdersPerMonth();
            return View(ordersPerMonth);
        }

        public async Task<IActionResult> TotalAmount()
        {
            var orderItems = await _unitOfWork.OrderItemsRepository.FindAll(
        o => o.Order.Status == "Delivered", // Filter by order status
        includeProperties: "Order"
    );

            // Calculate total amount
            var totalAmount = orderItems
                .Sum(detail => detail.ProductQuantity * detail.TotalPrice);

            var totalAmountDto = new TotalAmountDTO { TotalAmount = totalAmount };
            return View(totalAmountDto);

        }

       
        //
        public async Task<IActionResult> ExportToExcel()
        {
            var pageSize = 200;
            var Prds = await _productService.GetAllPagination(pageSize, 1);

            ExcelPackage excelPackage = new ExcelPackage();
            ExcelWorksheet Worksheet = excelPackage.Workbook.Worksheets.Add("Prds");

            // Set column headers
            Worksheet.Cells[1, 1].Value = "English Name";
            Worksheet.Cells[1, 2].Value = "Arabic Name";
            Worksheet.Cells[1, 3].Value = "Description";
            Worksheet.Cells[1, 4].Value = "Quantity";
            Worksheet.Cells[1, 5].Value = "Price";
            Worksheet.Cells[1, 6].Value = "Brand";


            // Populate the Excel worksheet with data from Categoryes
            int row = 2;
            foreach (var product in Prds.Entities)
            {
                Worksheet.Cells[row, 1].Value = product.Name;
                Worksheet.Cells[row, 2].Value = product.NameAr;
                Worksheet.Cells[row, 3].Value = product.ShortDescription;
                Worksheet.Cells[row, 4].Value = product.StockQuantity;
                Worksheet.Cells[row, 5].Value = product.RealPrice;
                Worksheet.Cells[row, 6].Value = product.BrandName;



                row++;
            }

            using (var memoryStream = new MemoryStream())
            {
                excelPackage.SaveAs(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Products.xlsx");
            }
        }

    }
}
