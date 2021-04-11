using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using DemoCSGO.Models;
using DemoCSGO.Shared.Core;
using DemoInfo;
using Newtonsoft.Json;

namespace DemoCSGO.Core
{
    public class DemoParserCore : CoreBase, IDemoParserCore
    {
        private DemoParser _demo;
        public DemoParserCore()
        {
        }
            
        private void OpenDemo() => _demo = new DemoParser(File.OpenRead("C:\\Downloads_hltv\\vitality-vs-natus-vincere-m3-dust2.dem"));

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
    }
}