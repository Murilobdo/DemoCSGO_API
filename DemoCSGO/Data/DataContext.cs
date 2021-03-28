using System.Data.Common;
using DemoCSGO.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoCSGO.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options)
        {

        }

        public DbSet<Weapon> Weapons {get;set;}
    }
}