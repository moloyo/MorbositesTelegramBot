using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MorbositesBotApi.Models;

namespace MorbositesBotApi
{
    public class MorbisitesContext : DbContext
    {
        private readonly string _connectionString;

        public MorbisitesContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DataConnection");
        }

        public DbSet<Morbosite> Morbosites { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
