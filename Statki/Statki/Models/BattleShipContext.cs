using System.Data.Entity;

namespace Statki.Models
{
    class BattleShipContext:DbContext
    {
        public BattleShipContext():base("BattleShipBase")
        {
            
        }
        public DbSet<Player> Players { get; set; }
        public DbSet<Field> Fields { get; set; }
    }
}
