using DemoCSGO.Shared.Models;
using DemoInfo;

namespace DemoCSGO.Models
{
    public class Weapon : EntityBase
    {
        public Weapon(string nameWeapon, int deathQuantity, string weaponType)
        {
            NameWeapon = nameWeapon;
            DeathQuantity = deathQuantity;
            WeaponType = weaponType;
        }

        public string NameWeapon { get; set; }
        public int DeathQuantity { get; set; }
        public string WeaponType { get; set; }
    }
}