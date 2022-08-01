using ArticlesApi.Dto;
using ArticlesApi.Entities;
using ArticlesApi.Interfaces;

namespace ArticlesApi.Adapters
{
    public class ArticlesResponseAdapter : IAdapter<ArticlesEntity, ArticlesResponse>
    {
        public ArticlesResponse AdaptTo(ArticlesEntity arg)
        {
            return new ArticlesResponse
            {
                Id = arg.Id,
                Name = arg.Name,
                Description = arg.Description,
                Price = arg.Price,
                Stock = arg.Stock,
            };
        }
    }
}
