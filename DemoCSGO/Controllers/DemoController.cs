using System.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DemoCSGO.Attributes;
using DemoCSGO.Core;
using DemoCSGO.Data;
using DemoCSGO.Models;
using DemoInfo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DemoCSGO.Controllers
{
    [ApiController]
    [Route("v1/Demo")]
    public class DemoController : ControllerBase
    {
        private readonly string path = Path.Combine(Environment.CurrentDirectory, "JsonResults");
<<<<<<< Updated upstream
        private readonly string[] demos = Directory.GetFiles("C:\\Users\\vitor\\source\\repos\\DemoCSGO_API\\Jupyter_Notebook\\partidas");
=======
        private readonly string[] demos = Directory.GetFiles(@"V:\Users\vitor\DemoCSGO_API\Jupyter_Notebook\partidas");
        //private readonly string[] demos = Directory.GetFiles("C:\\Users\\muril\\Desktop\\TCC\\Jupyter_Notebook\\partidas");
>>>>>>> Stashed changes

        [HttpPost]
        [Route("LoadData")]
        public async Task<ActionResult> LoadData([FromServices]IDemoParserCore _core)
        {
            var cronometro = new Stopwatch();
            string path = @"C:\Users\vitor\source\repos\DemoCSGO_API\DemoCSGO\JsonResults\";
<<<<<<< Updated upstream
=======
            //string path = @"C:\Users\muril\Desktop\TCC\DemoCSGO\JsonResults\";
>>>>>>> Stashed changes

            CheckAndRemoveExistingFiles(path);

            try
            {
                cronometro.Start();

                foreach (string demo in demos)
                {
                    _core.GenerateData(demo);
                }

                cronometro.Stop();
                return Ok("Dados carregados em " + cronometro.ElapsedMilliseconds / 1000 + "s");
            }
            catch (System.Exception ex)
            {
                return BadRequest(new {ModelState = ModelState, Message = ex.Message});
            }
        }

        private void CheckAndRemoveExistingFiles(string path)
        {
            //if (System.IO.File.Exists(path + "AllPlayersStats.json"))
            //    System.IO.File.Delete(path + "AllPlayersStats.json");

            if (System.IO.File.Exists(path + "AllPlayersStats.csv"))
                System.IO.File.Delete(path + "AllPlayersStats.csv");

            if (System.IO.File.Exists(path + "AllWeaponsStats.csv"))
                System.IO.File.Delete(path + "AllWeaponsStats.csv");
        }

        [HttpGet]
        [Route("GetPlayers")]
        public async Task<ActionResult> GetPlayers()
        {
            return Ok(System.IO.File.OpenRead(Path.Combine(path, "AllPlayersStats.json")));
        }

        [HttpGet]
        [Route("GetHeatMap")]
        public async Task<ActionResult> GetHeatMap()
        {
            return Ok(System.IO.File.OpenRead(Path.Combine(Environment.CurrentDirectory, "images", "heat_map.png")));
        }
    }
}