using System.Collections.Generic;
using System.IO;
using DemoCSGO.Models;
using DemoInfo;

namespace DemoCSGO.Core
{
    public interface IDemoParserCore
    {
        List<Weapon> GetWeapons(DemoParser demo);
        List<Weapon> GetWeapons();
        void LoadDemo(FileStream demo);
    }
}