using Microsoft.EntityFrameworkCore;
using ArticlesApi.Dto;
using ArticlesApi.Interfaces;

namespace ArticlesApi.Entities
{
    public class AppDbContext: DbContext
    {
        public DbSet<ArticlesEntity> Articles { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
            Articles = Set<ArticlesEntity>();
        }

        public override int SaveChanges()
        {
            GenerateTimestamp();
            return base.SaveChanges();
        }

        public Pages<T> GetAll<T>(int offset, int limit) where T: class, IBaseEntity
        {
            List<T> items = Set<T>().Where(x => x.Active).OrderBy(x => x.Id).Skip(offset).Take(limit).ToList();
            int total = (int)Math.Ceiling(Convert.ToDecimal(Set<T>().Count() / limit));
            return new Pages<T>
            {
                Total = total,
                Items = items
            };
        }
         
        public void GenerateTimestamp()
        {
            DateTime now = DateTime.UtcNow;
            var entities = ChangeTracker.Entries().Where(x => x.Entity is IBaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));
            foreach(var entity in entities)
            {
                if (entity.State == EntityState.Added) ((IBaseEntity)entity.Entity).Created_at_utc = now;
                ((IBaseEntity)entity.Entity).Updated_at_utc = now;
            }
        }
    }
}
