using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DemoCSGO.Core;
using DemoCSGO.Data;
using DemoCSGO.Models;
using DemoInfo;
using Microsoft.AspNetCore.Mvc;

namespace DemoCSGO.Controllers
{
    [ApiController]
    [Route("v1/Demo")]
    public class DemoController : ControllerBase
    {
        [HttpGet]
        [Route("GetWeapons")]
        public async Task<ActionResult<List<Weapon>>> GetWeapons([FromServices]IDemoParserCore _core)
        {
            try
            {
                return _core.GetWeapons();
            }
            catch (Exception ex)
            {
                 return BadRequest(ModelState);
            }
        } 

        [HttpPost]
        [Route("LoadDemo")]
        public async Task<ActionResult> LoadDemo([FromServices]IDemoParserCore _core, [FromBody]dynamic file)
        {
            try
            {
                _core.LoadDemo(file);
                return Ok("Demo carregada.");
            }
            catch (Exception ex)
            {
                return BadRequest(ModelState);
            }
        }
    }
}