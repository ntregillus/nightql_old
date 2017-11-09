using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NightQL.Data;
using NightQL.Data.DbEntities;
using NightQL.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NightQL.Controllers
{
    [Route("api/Schemas")]
    public class SchemasController:BaseSchemaController {
        public SchemasController(ConfigContext db):base(db){}

        [HttpGet]
        public IActionResult Get()
        {
            var dbSchemas = Db.Schemas.ToList();
            var result = Mapper.Map<List<Schema>>(dbSchemas);
            return Ok(result);
        }

        [HttpPost]
        [SwaggerResponse(200, typeof(Schema), "Creates a new schema")]
        public async Task<IActionResult> CreateSchema([FromBody]Schema model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var allSchemas = Db.Schemas.ToList();
            var existingSchema = await Db.GetSchemaAsync(model.Name);
            if(existingSchema != null){
                ModelState.AddModelError("Name", $"A schema already exists with the name {model.Name}");
            }
            if (!string.IsNullOrWhiteSpace(model.CopiedSchema) && await Db.GetSchemaAsync(model.CopiedSchema) == null){
                ModelState.AddModelError("CopiedSchema", $"Could not find {model.CopiedSchema} to duplicate.");
            }
            if(model.ReferenceSchemas != null && model.ReferenceSchemas.Any()){
                foreach(var reference in model.ReferenceSchemas){
                    var refSchema = await Db.GetSchemaAsync(reference);
                    if(refSchema == null){
                        ModelState.AddModelError("ReferenceSchemas", $"Could not find {reference} schema to reference.");
                    }
                }
            }
            if (!ModelState.IsValid){
                return BadRequest(ModelState);
            }

            var result = Db.CreateSchema(model);

            return Ok(result);
        }

        

    }
}