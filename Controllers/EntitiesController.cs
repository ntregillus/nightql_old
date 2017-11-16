using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NightQL.Data;
using NightQL.Data.DbEntities;
using NightQL.Examples;
using NightQL.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Examples;
namespace NightQL.Controllers
{
    public class EntitiesController: BaseSchemaController
    {
        public EntitiesController(ConfigContext db):base(db){}

        [Route("api/{schema}/entities")]
        [HttpGet]
        [SwaggerResponse(200, typeof(EntityList))]
        [SwaggerResponseExample(200, typeof(EntityListExample))]
        public async Task<IActionResult> GetEntities([FromRoute]string schema, [FromQuery(Name="")]EntitySearch query){
            var result = await StandardValidationStuff(schema);
            if(result != null){
                return result;
            }
            
           var matchingEntities = Db.GetEntityDefinitions(schema, query);
           
           return Ok(matchingEntities);
        }

        [Route("api/{schema}/entities")]
        [HttpPost]
        [SwaggerResponse(203, typeof(Entity))]
        [SwaggerRequestExample(typeof(Entity), typeof(EntityExample))]
        public async Task<IActionResult> CreateEntity([FromRoute]string schema, [FromBody]Entity model)
        {
            var result = await StandardValidationStuff(schema)
                       ??  ValidateNewEntity(schema, model);
            if(result != null)
            {
                return result;
            }

            Db.ChangeDatabase(userId:"user_"+schema, password:"Nate5462"+schema, integratedSecuity:false);
            Db.CreateEntity(schema, model);
            return Accepted();
        }
        private IActionResult ValidateNewEntity(string schema, Entity model)
        {
            var existingEntities = Db.GetEntityDefinitions(schema, new EntitySearch{ Name = model.Name });
            if(existingEntities.Any()){
                ModelState.AddModelError("model", $"An entity with the name {existingEntities.First().Name} already exists.");
            }
            IActionResult result = null;
            if(!ModelState.IsValid)
            {
                result = BadRequest(ModelState);
            }
            return result;
        }

    }
}