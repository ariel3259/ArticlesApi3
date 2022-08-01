namespace ArticlesApi.Dto
{
    public class Pages<T>
    {
        public List<T> Items { get; set; }
        public int Total { get; set; }
    }
}
