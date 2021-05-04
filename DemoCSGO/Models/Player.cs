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

        public Player(string name, int killed, int death, int flashedEnemies, List<Weapon> weapons)
        {
            Name = name;
            Killed = killed;
            Death = death;
            FlashedEnemies = flashedEnemies;
            Weapons = weapons;
        }

        public string Name { get; set; }
        public int Killed { get; set; }
        public int Death { get; set; }
        public int FirstKills { get; set; }
        public int FlashedEnemies { get; set; } // Inimigos que o jogador atingiu com a granada Flash
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