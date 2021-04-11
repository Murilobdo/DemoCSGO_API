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

namespace DemoCSGO.Controllers
{
    [ApiController]
    [Route("v1/Demo")]
    public class DemoController : ControllerBase
    {
        private readonly string path = Path.Combine(Environment.CurrentDirectory, "JsonResults");

        [HttpPost]
        [Route("LoadData")]
        public async Task<ActionResult> LoadData([FromServices]IDemoParserCore _core)
        {
            try
            {
                //_core.GenerateWeapons();
                //_core.GeneratePlayers();
                _core.GenerateHeadMap();
                return Ok("Dados carregados");
            }
            catch (System.Exception ex)
            {
                return BadRequest(new {ModelState = ModelState, Message = ex.Message});
            }
        }

        [HttpGet]
        [Route("GetWeapons")]
        public async Task<ActionResult> GetWeapons()
        {
            return Ok(System.IO.File.OpenRead(Path.Combine(path, "weapons.json")));
        }

        [HttpGet]
        [Route("GetPlayers")]
        public async Task<ActionResult> GetPlayers()
        {
            return Ok(System.IO.File.OpenRead(Path.Combine(path, "players.json")));
        }

        [HttpGet]
        [Route("GetHeadMap")]
        public async Task<ActionResult> GetHeadMap()
        {
            return Ok(System.IO.File.OpenRead(Path.Combine(path, "players.json")));
        }

        [HttpPost]
        [Route("LoadDemo2")]
        [RequestSizeLimit(10L * 1024L * 1024L)]
        [DisableFormValueModelBindingAttribute]
        [RequestFormLimits(MultipartBodyLengthLimit = 10L * 1024L * 1024L)]
        public async Task<ActionResult> LoadDemo2(IFormFile demo)
        {
            try
            {
                return Ok("Demo carregada.");
            }
            catch (Exception ex)
            {
                 return BadRequest(new {ModelState = ModelState, Message = ex.Message});
            }
        }
    }
}