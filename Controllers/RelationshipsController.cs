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
    public class RelationshipsController: BaseSchemaController
    {
        public RelationshipsController(ConfigContext db) : base(db)
        {
        }

        /// <summary>
        /// used to validate entities are actaully new
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("api/{schema}/Entities/{name}/relationships")]
        [HttpGet]
        [SwaggerResponse(200, typeof(Relationship))]
        [SwaggerResponseExample(200, typeof(RelationshipListExamples))]
        public IActionResult Get([FromRoute]string schema, [FromRoute] string name)
        {
            throw new NotImplementedException();
        }

        [Route("api/{schema}/Entities/{name}/relationships")]
        [HttpPost]
        [SwaggerResponse(200, typeof(RelationshipList))]
        [SwaggerResponseExample(200, typeof(RelationshipExamples))]
        public async Task<IActionResult> Post([FromRoute]string schema, [FromRoute] string name, [FromBody] RelationshipList model)
        {
            var result = await StandardValidationStuff(schema, entityName: name)
                      ??       ValidateNewRelationships(schema, model);
            if(result != null){
                return result;
            }
            Db.ChangeDatabase(userId:"user_"+schema, password:"Nate5462"+schema, integratedSecuity:false);
            await Db.CreateRelationships(schema, model);

            return Accepted();
        }

        private IActionResult ValidateNewRelationships(string schema, RelationshipList relationships)
        {
            var existing = Db.Relationships.ToList();
            var dupes = from e in existing
                        join n in relationships on 
                            new { Child = e.ChildAlias.ToLower(), Parent = e.ParentAlias.ToLower() }
                            equals
                            new { Child = n.ChildAlias.ToLower(), Parent = n.ParentAlias.ToLower() }
                            select n;
            for(var i = 0; i < relationships.Count; i++)
            {
                var current = relationships[i];
                if(existing.Any(e=> e.ChildAlias.ToLower() == current.ChildAlias.ToLower()
                                &&  e.ParentAlias.ToLower() == current.ParentAlias.ToLower()))
                {
                    var msg = $"A relationship already exists with a parentAlias of {current.ParentAlias} and childAlias of {current.ChildAlias}.";
                    ModelState.AddModelError($"model[{i}].parentAlias",  msg);
                    ModelState.AddModelError($"model[{i}].childAlias",  msg);
                }
            }
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return null;
        }
    }

}