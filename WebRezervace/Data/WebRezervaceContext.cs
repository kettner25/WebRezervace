using Microsoft.EntityFrameworkCore;
using WebRezervace.Models;

namespace WebRezervace.Data
{
    public class WebRezervaceContext : DbContext
    {
        public WebRezervaceContext(DbContextOptions<WebRezervaceContext> options) : base(options) { }

        public DbSet<Rezervace> Rezervace { get; set; }
        public DbSet<Uzivatel> Uzivatele { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Rezervace>().HasOne(a => a.Autor).WithMany(e => e.Rezervace);
        }
    }
}
