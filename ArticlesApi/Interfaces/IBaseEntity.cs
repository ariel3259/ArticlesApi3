namespace ArticlesApi.Interfaces
{
    public interface IBaseEntity
    {
        public int Id { get; set; }
        public DateTime? Created_at_utc { get; set; }
        public DateTime? Updated_at_utc { get; set; }
        public bool Active { get; set; }
    }
}
