using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using DemoCSGO.Models;
using DemoCSGO.Shared.Core;
using DemoInfo;

namespace DemoCSGO.Core
{
    public class DemoParserCore : CoreBase, IDemoParserCore
    {
        private DemoParser _demo;
        public DemoParserCore()
        {
            _demo = new DemoParser(File.OpenRead("C:\\Downloads_hltv\\vitality-vs-natus-vincere-m3-dust2.dem"));
        }

        public List<Weapon> GetWeapons()
        {
            List<Weapon> result = new List<Weapon>();
            _demo.ParseHeader();
            bool hasMatchStarted = false;
            _demo.MatchStarted += (sender, e) => {
                hasMatchStarted = true;
            };

            int cont = 0;
            _demo.PlayerKilled  += (sender, e) => {
                if(hasMatchStarted){
                    cont++;
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
            return result;
        }

        public List<Weapon> GetWeapons(DemoParser demo)
        {
            throw new System.NotImplementedException();
        }

        public void LoadDemo(FileStream file)
        {
            _demo = new DemoParser(file);
            throw new NotImplementedException();
        }
    }
}