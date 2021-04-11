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
                 return BadRequest(new {ModelState = ModelState, Message = ex.Message});
            }
        } 

        [HttpPost]
        [Route("LoadDemo")]
     
        public async Task<ActionResult> LoadDemo([FromServices]IDemoParserCore _core, [FromBody]FileStream demo)
        {
            try
            {
                _core.LoadDemo(demo);
                return Ok("Demo carregada.");
            }
            catch (Exception ex)
            {
                 return BadRequest(new {ModelState = ModelState, Message = ex.Message});
            }
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