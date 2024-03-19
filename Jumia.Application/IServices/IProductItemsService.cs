using Jumia.Dtos.ProductItems;
using Jumia.DTOS.ViewResultDtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Application.IServices
{
    public interface IProductItemsService
    {
        Task<CreatOrUpdateProductItemsDTO> GetProductItemsByID(int id);
        Task<ResultView<CreatOrUpdateProductItemsDTO>> Create(CreatOrUpdateProductItemsDTO creatOrUpdateProductItemsDTO, List<IFormFile> images);
        Task<ResultView<CreatOrUpdateProductItemsDTO>> HardDelete(int id);
        Task<ResultView<CreatOrUpdateProductItemsDTO>> Update(CreatOrUpdateProductItemsDTO creatOrUpdateProductItemsDTO, List<IFormFile> images);
    }
}
