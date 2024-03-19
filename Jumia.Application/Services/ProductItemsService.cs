using AutoMapper;
using Jumia.Application.Contract;
using Jumia.Application.IServices;
using Jumia.Dtos.Category;
using Jumia.Dtos.Order;
using Jumia.Dtos.OrderItems;
using Jumia.Dtos.Product;
using Jumia.Dtos.ProductItems;
using Jumia.DTOS.ViewResultDtos;
using Jumia.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Jumia.Application.Services
{
    public class ProductItemsService :IProductItemsService
    {
        private readonly IProductItemsRepository _productItemsRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductItemsService(IUnitOfWork unitOfWork ,IProductItemsRepository productItemsRepository, IMapper mapper) {

           
            _mapper= mapper;
            _unitOfWork = unitOfWork;
            _productItemsRepository = productItemsRepository;
        }


        

        public async Task<ResultView<CreatOrUpdateProductItemsDTO>> Create(CreatOrUpdateProductItemsDTO creatOrUpdateProductItemsDTO, List<IFormFile> images)
        {
            var Data = await _unitOfWork.ProductItemsRepository.GetAllAsync();
            var OldProItems = Data.Where(c => c.Id == creatOrUpdateProductItemsDTO.Id).FirstOrDefault();

            if (OldProItems != null)
            {
                return new ResultView<CreatOrUpdateProductItemsDTO> { Entity = null, IsSuccess = false, Message = "Product Item Already Exist" };

            }
            else
            {

                var ProductItemWithSameImage = Data.FirstOrDefault(c => c.Images.SequenceEqual(creatOrUpdateProductItemsDTO.Images));
                if (ProductItemWithSameImage != null)
                {
                    return new ResultView<CreatOrUpdateProductItemsDTO> { Entity = null, IsSuccess = false, Message = "Cannot add the same image for a category added before" };
                }

                if (images != null)
                {
                    foreach (var image in images)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await image.CopyToAsync(memoryStream);

                            ProductItemWithSameImage.Images.Add(memoryStream.ToArray());
                        }
                    }
                }

                var proItems = _mapper.Map<ProductItems>(creatOrUpdateProductItemsDTO);
                var proItemEdit = await _unitOfWork.ProductItemsRepository.CreateAsync(proItems);
                await  _unitOfWork.SaveChangesAsync();
                var ordDto = _mapper.Map<CreatOrUpdateProductItemsDTO>(proItemEdit);

               


                return new ResultView<CreatOrUpdateProductItemsDTO> { Entity = creatOrUpdateProductItemsDTO, IsSuccess = true, Message = "Category Created Successfully" };
            }


        }

        public async Task<CreatOrUpdateProductItemsDTO> GetProductItemsByID(int id)
        {
            try
            {
                var b = await _productItemsRepository.GetOneAsync(id);
                var REturnb = _mapper.Map<CreatOrUpdateProductItemsDTO>(b);
                return REturnb;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        public async Task<ResultView<CreatOrUpdateProductItemsDTO>> HardDelete(int id)
        {
            try
            {
                // var book = _mapper.Map<Book>(bookDTO);
                var existingOrder = await _productItemsRepository.GetOneAsync(id);
                if (existingOrder == null)
                {
                    return new ResultView<CreatOrUpdateProductItemsDTO> { Entity = null, IsSuccess = false, Message = "Order not found" };
                }
                var OldOrder = _productItemsRepository.DeleteAsync(existingOrder);
                await _unitOfWork.SaveChangesAsync();

                var OrderDto = _mapper.Map<CreatOrUpdateProductItemsDTO>(OldOrder);
                return new ResultView<CreatOrUpdateProductItemsDTO> { Entity = OrderDto, IsSuccess = true, Message = "Deleted Successfully" };
            }
            catch (Exception ex)
            {
                return new ResultView<CreatOrUpdateProductItemsDTO> { Entity = null, IsSuccess = false, Message = ex.Message };

            }
        }
        public async Task<ResultView<CreatOrUpdateProductItemsDTO>> Update(CreatOrUpdateProductItemsDTO creatOrUpdateProductItemsDTO, List<IFormFile> images)
        {
            try
            {
                var OldProductItem = await _unitOfWork.ProductItemsRepository.GetOneAsync(creatOrUpdateProductItemsDTO.Id);

                if (OldProductItem == null)
                {
                    return new ResultView<CreatOrUpdateProductItemsDTO> { Entity = null, IsSuccess = false, Message = "ProductItems Not Found!" };

                }
                var Data = await _unitOfWork.ProductItemsRepository.GetAllAsync();

                if (images == null)
                {
                    creatOrUpdateProductItemsDTO.Images = creatOrUpdateProductItemsDTO.Images;
                }
                else
                {
                    _mapper.Map(creatOrUpdateProductItemsDTO, OldProductItem);
                    if (images != null)
                    {
                        foreach (var image in images)
                        {
                            using (var memorystream = new MemoryStream())
                            {
                                await image.CopyToAsync(memorystream);

                               creatOrUpdateProductItemsDTO.Images.Add(memorystream.ToArray());
                            }
                        }
                    }
                   
                }
                _mapper.Map(creatOrUpdateProductItemsDTO, OldProductItem);

                var UPProductItems = await _unitOfWork.ProductItemsRepository.UpdateAsync(OldProductItem);
                await _unitOfWork.SaveChangesAsync();
                var coruProductitems = _mapper.Map<CreateOrUpdateCategoryDto>(UPProductItems);

                return new ResultView<CreatOrUpdateProductItemsDTO> { Entity = creatOrUpdateProductItemsDTO, IsSuccess = true, Message = "Category Updated Successfully" };

            }

            catch (Exception ex)
            {
                return new ResultView<CreatOrUpdateProductItemsDTO>
                {
                    Entity = null,
                    IsSuccess = false,
                    Message = $"Something went wrong: {ex.Message}"
                };
                // Console.WriteLine($"An error occurred: {ex.Message}");
                //throw;
            }
        }

    }
}
