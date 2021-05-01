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
            
        private void OpenDemo() => _demo = new DemoParser(File.OpenRead("C:\\Users\\vitor\\Downloads\\BLAST-Pro-Series-Madrid-2019-astralis-vs-natus-vincere-dust2\\astralis-vs-natus-vincere-dust2.dem"));

        public void GenerateData()
        {
            List<Models.Player> players = new List<Models.Player>();
            List<Weapon> weapons = new List<Weapon>();

            OpenDemo();
            _demo.ParseHeader();
            bool hasMatchStarted = false;

            _demo.MatchStarted += (sender, e) => {
                hasMatchStarted = true;
            };

            GeneratePlayers();

            #region GetPlayers
            _demo.PlayerKilled += (sender, e) => {
                if (hasMatchStarted)
                {
                    string nameWeaponFired = GetNameWeapon(e.Weapon.Weapon);

                    //Vitima
                    if (players.Any(p => p.Name == e.Victim.Name))
                    {
                        var victim = players.Where(p => p.Name == e.Victim.Name).First();
                        victim.Death++;
                    }
                    else
                    {
                        players.Add(new Models.Player(e.Victim.Name, 0, 1, new List<Weapon>()));
                    }

                    //Assasino
                    if (players.Any(p => p.Name == e.Killer.Name))
                    {
                        bool foundWeapon = false;
                        var killer = players.Where(p => p.Name == e.Killer.Name).First();
                        killer.Killed++;
                        
                        foreach (Weapon weapon in killer.Weapons)
                        {
                            if (weapon.NameWeapon == nameWeaponFired)
                            {
                                weapon.DeathQuantity++;
                                foundWeapon = true;
                            }
                        }
                        
                        if (!foundWeapon)
                        {
                            killer.Weapons.Add(new Weapon(nameWeaponFired, 1, Enum.GetName(typeof(EquipmentClass), e.Weapon.Class)));
                        }
                    }
                    else
                    {
                        players.Add(new Models.Player(e.Killer.Name, 1, 0, new List<Weapon>()));
                        var killer = players.Where(p => p.Name == e.Killer.Name).First();
                        killer.Weapons.Add(new Weapon(nameWeaponFired, 1, Enum.GetName(typeof(EquipmentClass), e.Weapon.Class)));
                    }


                }
            };
            #endregion

            #region GetWeapons
            _demo.PlayerKilled += (sender, e) => {
                if (hasMatchStarted)
                {
                    string nameWeaponFired = GetNameWeapon(e.Weapon.Weapon);
                    if (weapons.Any(p => p.NameWeapon == nameWeaponFired))
                    {
                        var weapon = weapons.Where(p => p.NameWeapon == nameWeaponFired).FirstOrDefault();
                        weapon.DeathQuantity++;
                    }
                    else
                    {
                        weapons.Add(new Weapon(nameWeaponFired, 1, Enum.GetName(typeof(EquipmentClass), e.Weapon.Class)));
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
                if (e.Victim.Name.Contains(nomeJogador) && hasMatchStarted)
                {
                    Vector2 vet = TrasnlateScale(e.Victim.LastAlivePosition.X, e.Victim.LastAlivePosition.Y);
                    deathPositions.Add(vet);
                }
            };
            _demo.WeaponFired += (sender, e) => {
                if (e.Shooter.Name.Contains(nomeJogador) && hasMatchStarted
                   && e.Weapon.Weapon != EquipmentElement.Knife && e.Weapon.Weapon != EquipmentElement.Molotov
                   && e.Weapon.Weapon != EquipmentElement.Smoke && e.Weapon.Weapon != EquipmentElement.Flash
                   && e.Weapon.Weapon != EquipmentElement.Decoy && e.Weapon.Weapon != EquipmentElement.HE)
                {
                    Vector2 vet = TrasnlateScale(e.Shooter.Position.X, e.Shooter.Position.Y);
                    shootingPositions.Add(vet);
                }
            };
            #endregion

            _demo.ParseToEnd();

            WriteJsonFile("players", JsonConvert.SerializeObject(players, Formatting.Indented));
            WriteJsonFile("weapons", JsonConvert.SerializeObject(weapons, Formatting.Indented));
            DrawingPoints(shootingPositions, deathPositions);
        }

        public void GeneratePlayers()
        {
            List<Models.Player> result = new List<Models.Player>();
            OpenDemo();
            _demo.ParseHeader();
            bool hasMatchStarted = false;
            _demo.MatchStarted += (sender, e) => {
                hasMatchStarted = true;
            };


            _demo.PlayerKilled += (sender, e) => {
                if(hasMatchStarted){
                    //Vitima
                    if(result.Any(p => p.Name == e.Victim.Name)){
                        var victim = result.Where(p => p.Name == e.Victim.Name).First();
                        victim.Death++;
                    }else{
                        result.Add(new Models.Player(e.Victim.Name, 0, 1));
                    }

                    //Assasino
                    if(result.Any(p => p.Name == e.Killer.Name)){
                        var Killer = result.Where(p => p.Name == e.Killer.Name).First();
                        Killer.Killed++;
                    }else{
                        result.Add(new Models.Player(e.Killer.Name, 1, 0));
                    }
                }
            };
            _demo.ParseToEnd();

            WriteJsonFile("players", JsonConvert.SerializeObject(result));

        }

        public void GenerateWeapons()
        {
            List<Weapon> result = new List<Weapon>();

            OpenDemo();
            _demo.ParseHeader();
            bool hasMatchStarted = false;

            _demo.MatchStarted += (sender, e) => {
                hasMatchStarted = true;
            };

            _demo.PlayerKilled  += (sender, e) => {
                if(hasMatchStarted){
                    string nameWeaponFired = GetNameWeapon(e.Weapon.Weapon);
                    if(result.Any(p => p.NameWeapon == nameWeaponFired)){
                        var weapon = result.Where(p => p.NameWeapon == nameWeaponFired).FirstOrDefault();
                        weapon.DeathQuantity++;
                    }
                    else{
                        result.Add(new Weapon(nameWeaponFired, 1, Enum.GetName(typeof(EquipmentClass), e.Weapon.Class)));
                    }
                }
            };
            _demo.ParseToEnd();

            WriteJsonFile("weapons", JsonConvert.SerializeObject(result));
        }

        public void GetWeapons(DemoParser demo)
        {
            throw new System.NotImplementedException();
        }

        public void LoadDemo(FileStream file)
        {
            _demo = new DemoParser(file);
        }

        public void GenerateHeatMap()
        {
            string name = "dupreeh";
            mapDust2 = MakeMap("de_dust2", -2476, 3239, 4.4f);
            OpenDemo();
            _demo.ParseHeader();

            List<Vector2> shootingPositions = new List<Vector2>();
            List<Vector2> deathPositions = new List<Vector2>();
            bool hasMatchStarted = false;

            _demo.MatchStarted += (sender, e) => {
                hasMatchStarted = true;
            };

            _demo.PlayerKilled += (sender, e) => {
                if (e.Victim.Name.Contains(name) && hasMatchStarted){
                    Vector2 vet = TrasnlateScale(e.Victim.LastAlivePosition.X, e.Victim.LastAlivePosition.Y);
                    deathPositions.Add(vet);
                }
            };
            _demo.WeaponFired += (sender, e) => {
                if (e.Shooter.Name.Contains(name) && hasMatchStarted 
                   && e.Weapon.Weapon != EquipmentElement.Knife && e.Weapon.Weapon != EquipmentElement.Molotov
                   && e.Weapon.Weapon != EquipmentElement.Smoke && e.Weapon.Weapon != EquipmentElement.Flash
                   && e.Weapon.Weapon != EquipmentElement.Decoy && e.Weapon.Weapon != EquipmentElement.HE){
                    Vector2 vet = TrasnlateScale(e.Shooter.Position.X, e.Shooter.Position.Y);
                    shootingPositions.Add(vet);
                }
            };

            _demo.ParseToEnd();

            DrawingPoints(shootingPositions, deathPositions);
        }
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

    }
}