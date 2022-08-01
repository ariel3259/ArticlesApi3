using ArticlesApi.Interfaces;
using ArticlesApi.Dto;
using ArticlesApi.Entities;

namespace ArticlesApi.Adapters
{
    public class ArticlesEntityAdapter : IAdapter<ArticlesRequest, ArticlesEntity>
    {
        public ArticlesEntity AdaptTo(ArticlesRequest arg)
        {
            return new ArticlesEntity
            {
                Name = arg.Name,
                Description = arg.Description,
                Price = arg.Price,
                Stock = arg.Stock,
                Active = true
            };
        }
    }
}
