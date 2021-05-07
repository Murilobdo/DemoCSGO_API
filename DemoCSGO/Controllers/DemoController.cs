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
        private readonly string[] demos = Directory.GetFiles("C:\\Users\\vitor\\source\\repos\\DemoCSGO_API\\Jupyter_Notebook\\partidas");

        [HttpPost]
        [Route("LoadData")]
        public async Task<ActionResult> LoadData([FromServices]IDemoParserCore _core)
        {
            var cronometro = new Stopwatch();
            string jsonFilePath = @"C:\Users\vitor\source\repos\DemoCSGO_API\DemoCSGO\JsonResults\AllPlayersStats.json";
            
            if (System.IO.File.Exists(jsonFilePath))
                System.IO.File.Delete(jsonFilePath);

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