using AutoMapper;
using Jumia.Application.Contract;
using Jumia.Application.IServices;
using Jumia.Dtos.Category;
using Jumia.Dtos.Order;
using Jumia.Dtos.OrderItems;
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


        

        public async Task<ResultView<CreatOrUpdateProductItemsDTO>> Create(CreatOrUpdateProductItemsDTO creatOrUpdateProductItemsDTO, IFormFile image)
        {
            var Data = await _productItemsRepository.GetAllAsync();
            var OldProItems = Data.Where(c => c.Id == creatOrUpdateProductItemsDTO.Id).FirstOrDefault();

            if (OldProItems != null)
            {
                return new ResultView<CreatOrUpdateProductItemsDTO> { Entity = null, IsSuccess = false, Message = "Category Already Exist" };

            }
            else
            {

                var CategoryWithSameImage = Data.FirstOrDefault(c => c.Images.SequenceEqual(creatOrUpdateProductItemsDTO.Images));
                if (CategoryWithSameImage != null)
                {
                    return new ResultView<CreatOrUpdateProductItemsDTO> { Entity = null, IsSuccess = false, Message = "Cannot add the same image for a category added before" };
                }

                if (image != null && image.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);
                        //creatOrUpdateProductItemsDTO.Images = memoryStream.ToArray();
                    }
                }

                var proItems = _mapper.Map<ProductItems>(creatOrUpdateProductItemsDTO);
                var proItemEdit = await _productItemsRepository.CreateAsync(proItems);
                await _productItemsRepository.SaveChangesAsync();
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




        public async Task<ResultView<CreatOrUpdateProductItemsDTO>> Update(CreatOrUpdateProductItemsDTO creatOrUpdateProductItemsDTO, IFormFile image)
        {
            try
            {
                var OldData = await _productItemsRepository.GetOneAsync(creatOrUpdateProductItemsDTO.Id);

                if (OldData == null)
                {
                    return new ResultView<CreatOrUpdateProductItemsDTO> { Entity = null, IsSuccess = false, Message = "ProductItems Not Found!" };

                }
                var Data = await _productItemsRepository.GetAllAsync();
                var ProductItemsWithSameImage = Data.FirstOrDefault(c => c.Id != creatOrUpdateProductItemsDTO.Id && c.Images.SequenceEqual(creatOrUpdateProductItemsDTO.Images));

                if (ProductItemsWithSameImage != null)
                {
                    return new ResultView<CreatOrUpdateProductItemsDTO> { Entity = null, IsSuccess = false, Message = "image already in use by another category." };
                }


                if (image == null || image.Length == 0)
                {
                    creatOrUpdateProductItemsDTO.Images = creatOrUpdateProductItemsDTO.Images;
                }
                else
                {

                    using (var memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);
                        //creatOrUpdateProductItemsDTO.Images = memoryStream.ToArray();
                    }
                }
                _mapper.Map(creatOrUpdateProductItemsDTO, OldData);

                var UPProductItems = await _productItemsRepository.UpdateAsync(OldData);
                await _productItemsRepository.SaveChangesAsync();
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
