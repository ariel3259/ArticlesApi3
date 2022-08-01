using ArticlesApi.Model;

namespace ArticlesApi.Dto
{
    public class ArticlesResponse: Articles
    {
        public int Id;

        public override string ToString()
        {
            return $"ArticlesResponse[Id={Id}, Name={Name}, Description={Description}, Price={Price}, Stock ={Stock}]";
        }
    }
}
