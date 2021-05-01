using System.Collections.Generic;

namespace DemoCSGO.Models
{
    public class Player
    {
        public Player(string name, int killed, int death)
        {
            Name = name;
            Killed = killed;
            Death = death;
        }

        public Player(string name, int killed, int death, List<Weapon> weapons)
        {
            Name = name;
            Killed = killed;
            Death = death;
            Weapons = weapons;
        }

        public string Name { get; set; }
        public int Killed { get; set; }
        public int Death { get; set; }
        public List<Weapon> Weapons { get; set; }
        public enum Role
        {
            Awper,
            Support,
            Lurker,
            Fragger,
            Leader
        }
    }
}