using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using DemoCSGO.Models;
using DemoCSGO.Shared.Core;
using DemoInfo;
using Newtonsoft.Json;
using System.Numerics;
using System.Drawing;
using System.Drawing.Imaging;

namespace DemoCSGO.Core
{
    public class DemoParserCore : CoreBase, IDemoParserCore
    {
        private DemoParser _demo;
        Map mapDust2;

        public DemoParserCore()
        {
        }

        //private void OpenDemo() => _demo = new DemoParser(File.OpenRead("C:\\Users\\vitor\\Downloads\\BLAST-Pro-Series-Madrid-2019-astralis-vs-natus-vincere-dust2\\astralis-vs-natus-vincere-dust2.dem"));
        private void OpenDemo(string file) => _demo = new DemoParser(File.OpenRead(file));        

        public void GenerateData(string demo)
        {
            List<Models.Player> players = new List<Models.Player>();
            List<Models.Player> alivePlayers = new List<Models.Player>();
            List<Weapon> weapons = new List<Weapon>();
            bool firstKillFlag = true;
            bool lastAliveTR = false;
            bool lastAliveCT = false;
            bool roundStarted = false;
            int roundCount = 0;

            OpenDemo(demo);
            _demo.ParseHeader();
            bool hasMatchStarted = false;

            _demo.MatchStarted += (sender, e) => {
                hasMatchStarted = true;
            };

            #region SetDistanceTraveled and WalkQuantityAsTR
            _demo.TickDone += (sender, e) => { 
                if (hasMatchStarted && IsAllPlayersRegistered(players) && roundStarted && _demo.Participants != null)
                {
                    foreach (var player in players)
                    {
                        var jogador = _demo.Participants.Where(p => p.Name == player.Name).FirstOrDefault();
                        if (jogador != null)
                        {
                            if (player.TeamSide == Team.Terrorist)
                            {
                                player.DistanceTraveledAsTR += (jogador.Velocity.Absolute * _demo.TickTime);
                                player.DistanceTraveledAsTR = Math.Round(player.DistanceTraveledAsTR, 2);
                            }
                            else
                            {
                                player.DistanceTraveledAsCT += (jogador.Velocity.Absolute * _demo.TickTime);
                                player.DistanceTraveledAsCT = Math.Round(player.DistanceTraveledAsCT, 2);
                            }

                            if (IsPlayerWalking(jogador))
                            {
                                if (player.TeamSide == Team.Terrorist)
                                    player.WalkQuantityAsTR++;
                                else
                                    player.WalkQuantityAsCT++;
                            }
                        }
                    }
                }
            };
            #endregion

            #region RoundStart Event
            _demo.RoundStart += (sender, e) => {
                firstKillFlag = true;
                roundStarted = true;
                roundCount++;

                if (IsAllPlayersRegistered(players))
                {
                    lastAliveTR = false;
                    lastAliveCT = false;
                    UpdateTeamSide(players, _demo.Participants);

                    foreach (var player in players)
                    {
                        player.IsAlive = true;
                        player.IsLastAliveThisRound = false;
                    }
                }
            };
            #endregion

            #region RoundEnd Event
            _demo.RoundEnd += (sender, e) =>
            {
                roundStarted = false;

                SetClutches(players, e);
            };
            #endregion

            #region GetBlindedEnemies
            _demo.FlashNadeExploded += (sender, e) =>
            {
                if (hasMatchStarted)
                {
                    if (players.Any(p => p.Name == e.ThrownBy.Name))
                    {
                        int blindedEnemies = BlindedEnemies(e.FlashedPlayers, e.ThrownBy);

                        var player = players.Where(p => p.Name == e.ThrownBy.Name).FirstOrDefault();
                        player.FlashedEnemies += blindedEnemies;

                    }
                    else
                    {
                        players.Add(new Models.Player(e.ThrownBy.Name, 0, 0, BlindedEnemies(e.FlashedPlayers, e.ThrownBy), new List<Weapon>()));
                    }
                }
            };
            #endregion

            #region GetPlayersStats
            _demo.PlayerKilled += (sender, e) => {
                if (hasMatchStarted)
                {
                    string nameWeaponFired = GetNameWeapon(e.Weapon.Weapon);

                    //Vitima
                    if (e.Victim != null)
                    {
                        if (players.Any(p => p.Name == e.Victim.Name))
                        {
                            bool foundWeapon = false;
                            var victim = players.Where(p => p.Name == e.Victim.Name).FirstOrDefault();
                            if (victim != null)
                            {
                                victim.IsAlive = false;
                                victim.Death++;
                            }

                            foreach (Weapon weapon in victim.Weapons)
                            {
                                if (weapon.NameWeapon.Equals(nameWeaponFired))
                                {
                                    weapon.DeathQuantity++;
                                    foundWeapon = true;
                                }
                            }

                            if (!foundWeapon)
                            {
                                victim.Weapons.Add(new Weapon(nameWeaponFired, 0, 1, Enum.GetName(typeof(EquipmentClass), e.Weapon.Class)));
                            }
                        }
                        else
                        {
                            players.Add(new Models.Player(e.Victim.Name, 0, 1, 0, new List<Weapon>()));
                            var victim = players.Where(p => p.Name == e.Victim.Name).FirstOrDefault();

                            if (victim != null)
                            {
                                victim.Weapons.Add(new Weapon(nameWeaponFired, 0, 1, Enum.GetName(typeof(EquipmentClass), e.Weapon.Class)));
                                victim.TeamSide = e.Victim.Team;
                                SetPlayerTeamName(e.Victim, victim);
                            }
                        }
                    }

                    //Assasino
                    if (e.Killer != null)
                    {
                        if (players.Any(p => p.Name == e.Killer.Name))
                        {
                            bool foundWeapon = false;
                            var killer = players.Where(p => p.Name == e.Killer.Name).FirstOrDefault();
                            if (killer != null)
                                killer.Killed++;

                            if (IsAllPlayersRegistered(players))
                                (lastAliveCT, lastAliveTR) = SetLastAliveQuantity(players, lastAliveCT, lastAliveTR);

                            if (firstKillFlag)
                            {
                                killer.FirstKills++;
                                var victim = players.Where(p => p.Name == e.Victim.Name).FirstOrDefault();

                                if (victim != null)
                                    victim.FirstDeaths++;

                                firstKillFlag = false;
                            }

                            foreach (Weapon weapon in killer.Weapons)
                            {
                                if (weapon.NameWeapon.Equals(nameWeaponFired))
                                {
                                    weapon.KillQuantity++;
                                    foundWeapon = true;
                                }
                            }

                            if (!foundWeapon)
                            {
                                killer.Weapons.Add(new Weapon(nameWeaponFired, 1, 0, Enum.GetName(typeof(EquipmentClass), e.Weapon.Class)));
                            }
                        }
                        else
                        {
                            players.Add(new Models.Player(e.Killer.Name, 1, 0, 0, new List<Weapon>()));
                            var killer = players.Where(p => p.Name == e.Killer.Name).FirstOrDefault();

                            if (killer != null)
                            {
                                killer.Weapons.Add(new Weapon(nameWeaponFired, 1, 0, Enum.GetName(typeof(EquipmentClass), e.Weapon.Class)));
                                killer.TeamSide = e.Killer.Team;
                                SetPlayerTeamName(e.Killer, killer);
                            }
                        }
                    }
                }
            };
            #endregion

            #region GetHeatMap
            string nomeJogador = "dupreeh";
            mapDust2 = MakeMap("de_dust2", -2476, 3239, 4.4f);
            List<Vector2> shootingPositions = new List<Vector2>();
            List<Vector2> deathPositions = new List<Vector2>();

            _demo.PlayerKilled += (sender, e) => {
                if (e.Victim != null)
                {
                    if (e.Victim.Name.Contains(nomeJogador) && hasMatchStarted)
                    {
                        Vector2 vet = TrasnlateScale(e.Victim.LastAlivePosition.X, e.Victim.LastAlivePosition.Y);
                        deathPositions.Add(vet);
                    }
                }
            };
            _demo.WeaponFired += (sender, e) => {
                if (e.Shooter != null)
                {
                    if (e.Shooter.Name.Contains(nomeJogador) && hasMatchStarted
                       && e.Weapon.Weapon != EquipmentElement.Knife && e.Weapon.Weapon != EquipmentElement.Molotov
                       && e.Weapon.Weapon != EquipmentElement.Smoke && e.Weapon.Weapon != EquipmentElement.Flash
                       && e.Weapon.Weapon != EquipmentElement.Decoy && e.Weapon.Weapon != EquipmentElement.HE)
                    {
                        Vector2 vet = TrasnlateScale(e.Shooter.Position.X, e.Shooter.Position.Y);
                        shootingPositions.Add(vet);
                    }
                }
            };
            #endregion

            _demo.ParseToEnd();

            //SetRoles(players);
            WriteJsonPlayers(players);
            DrawingPoints(shootingPositions, deathPositions);
            players.Clear();
        }

        private void WriteJsonPlayers(List<Models.Player> players)
        {
            string jsonResultPath = @"C:\Users\vitor\source\repos\DemoCSGO_API\DemoCSGO\JsonResults\";
            if (!IsJsonAlreadyCreated(jsonResultPath))
            {
                WriteJsonFile("AllPlayersStats", JsonConvert.SerializeObject(players, Formatting.Indented));
            }
            else
            {
                string playersJson = JsonConvert.SerializeObject(players, Formatting.Indented);
                string allPlayersStatsJson = string.Empty;
                using (StreamReader r = new StreamReader(jsonResultPath + "AllPlayersStats.json"))
                {
                    allPlayersStatsJson = r.ReadToEnd();
                    allPlayersStatsJson += playersJson;

                    r.Close();
                }
                File.Delete(jsonResultPath + "AllPlayersStats.json");
                WriteJsonFile("AllPlayersStats", allPlayersStatsJson);
            }
        }

        private bool IsJsonAlreadyCreated(string jsonResultPath) => File.Exists(jsonResultPath + "AllPlayersStats.json");

        private void SetRoles(List<Models.Player> players)
        {
            throw new NotImplementedException();
        }

        private bool IsPlayerWalking(DemoInfo.Player jogador) => (jogador.Velocity.Absolute >= 75 && jogador.Velocity.Absolute <= 140);

        private void SetClutches(List<Models.Player> players, RoundEndedEventArgs e)
        {
            foreach (var player in players)
            {
                if (player.IsLastAliveThisRound && player.TeamSide == Team.Terrorist && e.Reason == RoundEndReason.TerroristWin)
                    player.Clutches++;
                else if (player.IsLastAliveThisRound && player.TeamSide == Team.CounterTerrorist && e.Reason == RoundEndReason.CTWin)
                    player.Clutches++;
            }
        }

        private void UpdateTeamSide(List<Models.Player> players, IEnumerable<DemoInfo.Player> participants)
        {
            foreach (var player in players)
            {
                var jogador = participants.Where(p => p.Name == player.Name).FirstOrDefault();

                if (jogador != null)
                    player.TeamSide = jogador.Team;
            }
        }

        private bool IsAllPlayersRegistered(List<Models.Player> players) => (players.Count == 10);

        private (bool, bool) SetLastAliveQuantity(List<Models.Player> players, bool lastAliveCT, bool lastAliveTR)
        {
            int aliveCT = 0;
            int aliveTR = 0;

            foreach (Models.Player player in players)
            {
                if (player.IsAlive && (player.TeamSide == Team.CounterTerrorist))
                    aliveCT++;
                else if (player.IsAlive && (player.TeamSide == Team.Terrorist))
                    aliveTR++;
            }

            if (aliveCT == 1 && !lastAliveCT)
            {
                foreach (var player in players)
                {
                    if (player.IsAlive && (player.TeamSide == Team.CounterTerrorist))
                    {
                        player.LastAliveQuantity++;
                        player.IsLastAliveThisRound = true;
                    }
                }
                lastAliveCT = true;
            }
            if (aliveTR == 1 && !lastAliveTR)
            {
                foreach (var player in players)
                {
                    if (player.IsAlive && (player.TeamSide == Team.Terrorist))
                    {
                        player.LastAliveQuantity++;
                        player.IsLastAliveThisRound = true;
                    }
                }
                lastAliveTR = true;
            }

            return (lastAliveCT, lastAliveTR);
        }

        private void SetPlayerTeamName(DemoInfo.Player player, Models.Player victim)
        {
            if (player.Team == Team.Terrorist)
                victim.TeamName = _demo.TClanName;
            else
                victim.TeamName = _demo.CTClanName;
        }

        private int BlindedEnemies(DemoInfo.Player[] players, DemoInfo.Player playerThrownBy)
        {
            int blindedEnemies = 0;

            if (players.Length > 0)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i].Team != playerThrownBy.Team)
                        blindedEnemies++;
                }
            }

            return blindedEnemies;
        }

        //public void GeneratePlayers()
        //{
        //    List<Models.Player> result = new List<Models.Player>();
        //    OpenDemo();
        //    _demo.ParseHeader();
        //    bool hasMatchStarted = false;
        //    _demo.MatchStarted += (sender, e) => {
        //        hasMatchStarted = true;
        //    };


        //    _demo.PlayerKilled += (sender, e) => {
        //        if(hasMatchStarted){
        //            //Vitima
        //            if(result.Any(p => p.Name == e.Victim.Name)){
        //                var victim = result.Where(p => p.Name == e.Victim.Name).First();
        //                victim.Death++;
        //            }else{
        //                result.Add(new Models.Player(e.Victim.Name, 0, 1));
        //            }

        //            //Assasino
        //            if(result.Any(p => p.Name == e.Killer.Name)){
        //                var Killer = result.Where(p => p.Name == e.Killer.Name).First();
        //                Killer.Killed++;
        //            }else{
        //                result.Add(new Models.Player(e.Killer.Name, 1, 0));
        //            }
        //        }
        //    };
        //    _demo.ParseToEnd();

        //    WriteJsonFile("players", JsonConvert.SerializeObject(result));

        //}

        //public void GenerateWeapons()
        //{
        //    List<Weapon> result = new List<Weapon>();

        //    OpenDemo();
        //    _demo.ParseHeader();
        //    bool hasMatchStarted = false;

        //    _demo.MatchStarted += (sender, e) => {
        //        hasMatchStarted = true;
        //    };

        //    _demo.PlayerKilled  += (sender, e) => {
        //        if(hasMatchStarted){
        //            string nameWeaponFired = GetNameWeapon(e.Weapon.Weapon);
        //            if(result.Any(p => p.NameWeapon == nameWeaponFired)){
        //                var weapon = result.Where(p => p.NameWeapon == nameWeaponFired).FirstOrDefault();
        //                weapon.KillQuantity++;
        //            }
        //            else{
        //                result.Add(new Weapon(nameWeaponFired, 1, 0, Enum.GetName(typeof(EquipmentClass), e.Weapon.Class)));
        //            }
        //        }
        //    };
        //    _demo.ParseToEnd();

        //    WriteJsonFile("weapons", JsonConvert.SerializeObject(result));
        //}

        public void GetWeapons(DemoParser demo)
        {
            throw new System.NotImplementedException();
        }

        public void LoadDemo(FileStream file)
        {
            _demo = new DemoParser(file);
        }

        //public void GenerateHeatMap()
        //{
        //    string name = "dupreeh";
        //    mapDust2 = MakeMap("de_dust2", -2476, 3239, 4.4f);
        //    OpenDemo();
        //    _demo.ParseHeader();

        //    List<Vector2> shootingPositions = new List<Vector2>();
        //    List<Vector2> deathPositions = new List<Vector2>();
        //    bool hasMatchStarted = false;

        //    _demo.MatchStarted += (sender, e) => {
        //        hasMatchStarted = true;
        //    };

        //    _demo.PlayerKilled += (sender, e) => {
        //        if (e.Victim.Name.Contains(name) && hasMatchStarted){
        //            Vector2 vet = TrasnlateScale(e.Victim.LastAlivePosition.X, e.Victim.LastAlivePosition.Y);
        //            deathPositions.Add(vet);
        //        }
        //    };
        //    _demo.WeaponFired += (sender, e) => {
        //        if (e.Shooter.Name.Contains(name) && hasMatchStarted 
        //           && e.Weapon.Weapon != EquipmentElement.Knife && e.Weapon.Weapon != EquipmentElement.Molotov
        //           && e.Weapon.Weapon != EquipmentElement.Smoke && e.Weapon.Weapon != EquipmentElement.Flash
        //           && e.Weapon.Weapon != EquipmentElement.Decoy && e.Weapon.Weapon != EquipmentElement.HE){
        //            Vector2 vet = TrasnlateScale(e.Shooter.Position.X, e.Shooter.Position.Y);
        //            shootingPositions.Add(vet);
        //        }
        //    };

        //    _demo.ParseToEnd();

        //    DrawingPoints(shootingPositions, deathPositions);
        //}
        private void DrawingPoints(List<Vector2> shootingPositions, List<Vector2> deathPositions)
        {
            
            string path = Path.Combine(Environment.CurrentDirectory, "images");
            using (var image = System.IO.File.Open(Path.Combine(path, "de_dust2.jpg"), FileMode.Open))
            {
                var bitmap = new Bitmap(image);
                Graphics graph = Graphics.FromImage(bitmap);

                Brush brush= new SolidBrush(Color.Red);
                foreach (Vector2 Position in shootingPositions)
                {
                    graph.FillEllipse(brush, Position.X, Position.Y, 10, 10);
                }

                
                Image deathIcon = Image.FromFile(Path.Combine(path, "rip.png"));
                foreach (Vector2 Position in deathPositions)
                {
                    graph.DrawImage(deathIcon, Position.X - 15, Position.Y - 15);
                }

                bitmap.Save(Path.Combine(path, "heat_map.png"), ImageFormat.Png);
                
                graph.Dispose();
                bitmap.Dispose();
                image.Dispose();
            }
        }

        private Map MakeMap(string name, float x, float y, float scale){
            return  new Map(name, new Vector2(x,y) ,scale);
        }  
         private Vector2 TrasnlateScale(float x, float y)
        {
            Vector2 v = Translate(x, y);
            return new Vector2(v.X / mapDust2.Scale, v.Y / mapDust2.Scale);
        }

        private Vector2 Translate(float x, float y) => new Vector2(x - mapDust2.PZero.X, mapDust2.PZero.Y - y);
        private Color HeatMapColor(decimal value, decimal min, decimal max)
        {
            decimal val = (value - min) / (max - min);
            int r = Convert.ToByte(255 * val);
            int g = Convert.ToByte(255 * (1 - val));
            int b = 0;

            return Color.FromArgb(255,r,g,b);                                    
        }

        public void GenerateWeapons()
        {
            throw new NotImplementedException();
        }

        public void GenerateHeatMap()
        {
            throw new NotImplementedException();
        }

        public void GeneratePlayers()
        {
            throw new NotImplementedException();
        }
    }
}