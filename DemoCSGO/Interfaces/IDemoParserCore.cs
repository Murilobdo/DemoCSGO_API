using System.Collections.Generic;
using System.IO;
using DemoCSGO.Models;
using DemoInfo;

namespace DemoCSGO.Core
{
    public interface IDemoParserCore
    {
        void GetWeapons(DemoParser demo);
        void GenerateWeapons();
        void LoadDemo(FileStream demo);
        void GeneratePlayers();
        void GenerateHeadMap();
    }
}