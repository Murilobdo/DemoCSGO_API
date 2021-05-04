using DemoCSGO.Shared.Models;
using DemoInfo;

namespace DemoCSGO.Models
{
    public class Weapon : EntityBase
    {
        public Weapon(string nameWeapon, int killQuantity, int deathQuantity, string weaponType)
        {
            NameWeapon = nameWeapon;
            KillQuantity = killQuantity;
            DeathQuantity = deathQuantity;
            WeaponType = weaponType;
        }

        public string NameWeapon { get; set; }
        public int KillQuantity { get; set; } // Quantidade de vezes que MATOU com a arma
        public int DeathQuantity { get; set; } // Quantidade de vezes que MORREU com a arma
        public string WeaponType { get; set; }
    }
}