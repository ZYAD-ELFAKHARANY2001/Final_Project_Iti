using Jumia.Application.Contract;
using Jumia.Dtos.Product;
using Jumia.DTOS.ViewResultDtos;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Application.IServices
{
    public interface IProductServices
    {
        Task<ResultView<CreateOrUpdateProductDto>> Create(CreateOrUpdateProductDto product);

    //    Task<ResultView<CreateOrUpdateProductDTO>> HardDelete(CreateOrUpdateProductDTO product);
    //    Task<ResultView<CreateOrUpdateProductDTO>> SoftDelete(CreateOrUpdateProductDTO product);
        Task<ResultDataForPagination<GetAllProducts>> GetAllPagination(int items, int pagenumber);
        //Task<CreateOrUpdateProductDTO> GetOne(int ID);
    }
}
