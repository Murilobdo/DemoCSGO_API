using CsvHelper.Configuration;
using Newtonsoft.Json;
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
        public string TeamName { get; set; }
        public int Killed { get; set; }
        public int Death { get; set; }
        public int LastAliveQuantity { get; set; } // Quantidade de vezes que foi o último a ficar vivo
        public int Clutches { get; set; } // Quantidade de vezes que o jogador foi o último a ficar vivo e ganhou a rodada
        public int FirstKills { get; set; } // Primeiro abate em uma rodada
        public int FirstDeaths { get; set; } // Sofreu a primeira morte durante a rodada
        public int FlashedEnemies { get; set; } // Inimigos que o jogador atingiu com a granada Flash
        public int WalkQuantityAsTR { get; set; } // Quantidade de vezes que o jogador andou sem fazer barulho (Walk) de Terrorista
        public int WalkQuantityAsCT { get; set; } // Quantidade de vezes que o jogador andou sem fazer barulho (Walk) de Contra-Terrorista
        public double DistanceTraveledAsTR { get; set; } // Distancia total percorrida do jogador no jogo de Terrorista
        public double DistanceTraveledAsCT { get; set; } // Distancia total percorrida do jogador no jogo de Contra-Terrorista
        public List<Weapon> Weapons { get; set; }
        [JsonIgnore]
        public DemoInfo.Team TeamSide { get; set; } // Atributo auxiliar para saber em qual lado o jogador está jogando no momento (Terrorista ou Contra-Terrorista)
        [JsonIgnore]
        public bool IsAlive { get; set; } // Atributo auxiliar para saber se o jogador está vivo na rodada
        [JsonIgnore]
        public bool IsLastAliveThisRound { get; set; } // Atributo auxiliar para saber se o jogador é o último a ficar vivo da atual rodada
        public enum Role
        {
            Awper,
            Support,
            Lurker,
            Fragger,
            Leader
        }
    }

    public class PlayerMap : ClassMap<Player>
    {
        public PlayerMap()
        {
            Map(p => p.Name);
            Map(p => p.TeamName);
            Map(p => p.Killed);
            Map(p => p.Death);
            Map(p => p.Weapons).Index(5);
        }
    }
}