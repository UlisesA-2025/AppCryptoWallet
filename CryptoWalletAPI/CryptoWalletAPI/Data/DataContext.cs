using Microsoft.EntityFrameworkCore;
using CryptoWalletAPI.Models;

namespace CryptoWalletAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options) { }

        public DbSet<Criptomoneda> Criptomonedas { get; set; }
        public DbSet<Exchange> Exchanges { get; set; }
        public DbSet<Transaccion> Transacciones { get; set; }
    }
}
