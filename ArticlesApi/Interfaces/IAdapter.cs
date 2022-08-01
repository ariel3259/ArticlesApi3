namespace ArticlesApi.Interfaces
{
    public interface IAdapter<T, S>
    {
        public S AdaptTo(T arg);
    }
}
