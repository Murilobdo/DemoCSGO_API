using CsvHelper.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DemoCSGO.Models
{
    public class Player
    {
        public Player(string name, double killed, double death)
        {
            Name = name;
            Killed = killed;
            Death = death;
        }

        public Player(string name, double killed, double death, double flashedEnemies, List<Weapon> weapons)
        {
            Name = name;
            Killed = killed;
            Death = death;
            FlashedEnemies = flashedEnemies;
            Weapons = weapons;
        }

        public string Name { get; set; }
        public string TeamName { get; set; }
        public double Killed { get; set; }
        public double Death { get; set; }
        public double ADR { get; set; } // Average Damage per Round, dano médio por round
        public double TotalDamageDealt { get; set; } // Total de dano causado no jogo
        public double RoundMVPs { get; set; } // Quantidade de vezes que foi o jogador que mais matou na rodada
        public double LastAliveQuantity { get; set; } // Quantidade de vezes que foi o último a ficar vivo
        public double Clutches { get; set; } // Quantidade de vezes que o jogador foi o último a ficar vivo e ganhou a rodada
        public double FirstKills { get; set; } // Primeiro abate em uma rodada
        public double FirstDeaths { get; set; } // Sofreu a primeira morte durante a rodada
        public double FlashedEnemies { get; set; } // Inimigos que o jogador atingiu com a granada Flash
        public double FlashAssists { get; set; } // Assisências de flash
        public double BombsPlanted { get; set; } // Quantidade de vezes que o jogador plantou a bomba
        public double WalkQuantityAsTR { get; set; } // Quantidade de vezes que o jogador andou sem fazer barulho (Walk) de Terrorista
        public double WalkQuantityAsCT { get; set; } // Quantidade de vezes que o jogador andou sem fazer barulho (Walk) de Contra-Terrorista
        public double DistanceTraveledAsTR { get; set; } // Distancia total percorrida do jogador no jogo de Terrorista
        public double DistanceTraveledAsCT { get; set; } // Distancia total percorrida do jogador no jogo de Contra-Terrorista
        public double AK47Kills { get; set; }
        public double MAC10Kills { get; set; }
        public double M4A4Kills { get; set; }
        public double MP9Kills { get; set; }
        public double AWPKills { get; set; }
        public double ScoutKills { get; set; }
        public List<Weapon> Weapons { get; set; } // Lista de armas utilizada pelo jogador durante a partida
        [JsonIgnore]
        public DemoInfo.Team TeamSide { get; set; } // Atributo auxiliar para saber em qual lado o jogador está jogando no momento (Terrorista ou Contra-Terrorista)
        [JsonIgnore]
        public bool IsAlive { get; set; } // Atributo auxiliar para saber se o jogador está vivo na rodada
        [JsonIgnore]
        public bool IsLastAliveThisRound { get; set; } // Atributo auxiliar para saber se o jogador é o último a ficar vivo da atual rodada
    }
}