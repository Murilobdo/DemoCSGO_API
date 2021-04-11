using System.IO;
using System;
using System.Collections.Generic;
using DemoCSGO.Shared.Interfaces;
using DemoInfo;

namespace DemoCSGO.Shared.Core
{
    public class CoreBase : ICoreBase
    {
        private readonly string path;

        public CoreBase()
        {
            path = Path.Combine(Environment.CurrentDirectory, "JsonResults");
        }
        public string GetNameWeapon(EquipmentElement equipment)
        {
            switch ((int)equipment)
            {
                case 0 : return "Desconhecido";
                case 1 : return "P2000";
                case 2 : return "Glock";
                case 3 : return "P250";
                case 4 : return "Deagle";
                case 5 : return "FiveSeven";
                case 6 : return "DualBarettas";
                case 7 : return "Tec9";
                case 8 : return "CZ";
                case 9 : return "USP";
                case 10 : return "Revolver";
                case 101 : return "MP7";
                case 102 : return "MP9";
                case 103 : return "Bizon";
                case 104 : return "Mac10";
                case 105 : return "UMP";
                case 106 : return "P90";
                case 107 : return "MP5SD";
                case 201 : return "SawedOff";
                case 202 : return "Nova";
                case 203 : return "Swag7";
                case 204 : return "XM1014";
                case 205 : return "M249";
                case 206 : return "Negev";
                case 301 : return "Gallil";
                case 302 : return "Famas";
                case 303 : return "AK47";
                case 304 : return "M4A4";
                case 305 : return "M4A1";
                case 306 : return "Scout";
                case 307 : return "SG556";
                case 308 : return "AUG";
                case 309 : return "AWP";
                case 310 : return "Scar20";
                case 311 : return "G3SG1";
                case 401 : return "Zeus";
                case 402 : return "Kevlar";
                case 403 : return "Helmet";
                case 404 : return "Bomb";
                case 405 : return "Knife";
                case 406 : return "DefuseKit";
                case 407 : return "World";
                case 501 : return "Decoy";
                case 502 : return "Molotov";
                case 503 : return "Incendiary";
                case 504 : return "Flash";
                case 505 : return "Smoke";
                case 506 : return "HE";
                default: return "Desconhecido";
            }
        }

        public void WriteJsonFile(string nameFile, string json)
        {
            if(File.Exists(Path.Combine(path, $"{nameFile}.json")))
                File.Delete(Path.Combine(path, $"{nameFile}.json"));
                
            File.WriteAllText(Path.Combine(path, $"{nameFile}.json"), json);
        }
    }
}