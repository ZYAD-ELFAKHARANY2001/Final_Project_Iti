using AutoMapper;
using Jumia.Application.Contract;
using Jumia.Application.IServices;
using Jumia.Dtos.Product;
using Jumia.DTOS.ViewResultDtos;
using Jumia.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Application.Services
{
    public class ProductService:IProductServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        //private readonly IProductRepository _productRepository;
        //private readonly IMapper _mapper;

        //public ProductService(IProductRepository productRepository, IMapper mapper)
        //{
        //    _productRepository = productRepository;
        //    _mapper = mapper;
        //}
        public async Task<ResultDataForPagination<GetAllProducts>> GetAllPagination(int items, int pagenumber) //10 , 3 -- 20 30
        {
            var AlldAta = (await _unitOfWork.ProductRepository.GetAllAsync());
            var Prds = AlldAta
                .Skip(items * (pagenumber - 1))
                .Take(items)
                .Select(p => new GetAllProducts(p))
                .ToList();
            ResultDataForPagination<GetAllProducts> resultDataList = new ResultDataForPagination<GetAllProducts>();
            resultDataList.Entities = Prds;
            //resultDataList.Count = AlldAta.Count();
            return resultDataList;
        }


        public async Task<ResultView<CreateOrUpdateProductDto>> Create(CreateOrUpdateProductDto product)
        {
            var Query = (await _unitOfWork.ProductRepository.GetAllAsync()); // se;ect * from product
            var OldProduct = Query
                             .Where(p => p.Name == product.Name)
                             .FirstOrDefault();
            if (OldProduct != null)
            {
                return new ResultView<CreateOrUpdateProductDto> { Entity = null, IsSuccess = false, Message = "Already Exist" };
            }
            else
            {
                var Prd = _mapper.Map<Product>(product);
                var NewPrd = await _unitOfWork.ProductRepository.CreateAsync(Prd);
                await _unitOfWork.SaveChangesAsync();
                var PrdDto = _mapper.Map<CreateOrUpdateProductDto>(NewPrd);
                return new ResultView<CreateOrUpdateProductDto> { Entity = PrdDto, IsSuccess = true, Message = "Created Successfully" };
            }

        }

        //public async Task<ResultView<CreateOrUpdateProductDto>> HardDelete(CreateOrUpdateProductDto product)
        //{
        //    try
        //    {
        //        var PRd = _mapper.Map<Product>(product);
        //        var Oldprd = _productRepository.DeleteAsync(PRd);
        //        await _productRepository.SaveChangesAsync();
        //        var PrdDto = _mapper.Map<CreateOrUpdateProductDto>(Oldprd);
        //        return new ResultView<CreateOrUpdateProductDto> { Entity = PrdDto, IsSuccess = true, Message = "Deleted Successfully" };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResultView<CreateOrUpdateProductDto> { Entity = null, IsSuccess = false, Message = ex.Message };

        //    }
        //}
        //public async Task<ResultView<CreateOrUpdateProductDto>> SoftDelete(CreateOrUpdateProductDto product)
        //{
        //    try
        //    {
        //        var PRd = _mapper.Map<Product>(product);
        //        var Oldprd = (await _productRepository.GetAllAsync()).FirstOrDefault(p => p.Id == product.Id);
        //        Oldprd.IsDeleted = true;
        //        await _productRepository.SaveChangesAsync();
        //        var PrdDto = _mapper.Map<CreateOrUpdateProductDto>(Oldprd);
        //        return new ResultView<CreateOrUpdateProductDto> { Entity = PrdDto, IsSuccess = true, Message = "Deleted Successfully" };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResultView<CreateOrUpdateProductDto> { Entity = null, IsSuccess = false, Message = ex.Message };

        //    }
        //}


        //public async Task<CreateOrUpdateProductDto> GetOne(int ID)
        //{
        //    var prd = await _productRepository.GetByIdAsync(ID);
        //    var REturnPrd = _mapper.Map<CreateOrUpdateProductDto>(prd);
        //    return REturnPrd;
        //}
    }
}
