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
using Microsoft.EntityFrameworkCore;
using AutoMapper;

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
        [Route("api/{schema}/entities/{name}/relationships")]
        [HttpGet]
        [SwaggerOperation(Tags = new [] {"Entities"})]
        [SwaggerResponse(200, typeof(Relationship))]
        [SwaggerResponseExample(200, typeof(RelationshipListExamples))]
        public async Task<IActionResult> Get([FromRoute]string schema, [FromRoute] string name)
        {
            var result = await StandardValidationStuff(schema, entityName: name);
            if(result != null){
                return result;
            }
            var relationships = Db.Relationships.Include(r=>r.Schema)
                            .Where(r=>r.Schema.Name == schema)
                            .Where(r=>r.ParentEntity == name || r.ChildEntity == name);
            var content = Mapper.Map<IEnumerable<Relationship>>(relationships);
            return Ok(content);
        }

        /// <summary>
        /// used to validate entities are actaully new
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Route("api/{schema}/relationships")]
        [HttpGet]
        [SwaggerResponse(200, typeof(Relationship))]
        [SwaggerResponseExample(200, typeof(RelationshipListExamples))]
        public async Task<IActionResult> GetAll([FromRoute]string schema)
        {
            var result = await StandardValidationStuff(schema);
            var relationships = Db.Relationships.Include(r=>r.Schema)
                            .Where(r=>r.Schema.Name == schema);
            var content = Mapper.Map<IEnumerable<Relationship>>(relationships);
            return Ok(content);
        }

        

        [Route("api/{schema}/relationships")]
        [HttpPost]
        [SwaggerResponse(200, typeof(RelationshipList))]
        [SwaggerResponseExample(200, typeof(RelationshipExamples))]
        public async Task<IActionResult> Post([FromRoute]string schema, [FromBody] RelationshipList model)
        {
            var result = await StandardValidationStuff(schema)
                      ??       ValidateNewRelationships(schema, model);
            if(result != null){
                return result;
            }
            Db.ChangeDatabase(userId:"user_"+schema, password:"Nate5462"+schema, integratedSecuity:false);
            Db.CreateRelationships(schema, model);

            return Accepted();
        }

        [Route("api/{schema}/relationships")]
        [HttpDelete]
        [SwaggerResponse(203, typeof(RelationshipList))]
        public async Task<IActionResult> Delete([FromRoute]string schema, [FromBody] RelationshipList model)
        {
            var result = await StandardValidationStuff(schema)
                        ?? ValidateDeleteRelationships(schema, model);
            if (result != null){
                return result;
            }
            throw new NotImplementedException();
        }

        private IActionResult ValidateDeleteRelationships(string schema, RelationshipList model)
        {
            throw new NotImplementedException();
            for(int i = 0; i < model.Count; i++){

            }
        }

        private IActionResult ValidateNewRelationships(string schema,  RelationshipList relationships)
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
                    ModelState.AddModelError($"[{i}].parentAlias",  msg);
                    ModelState.AddModelError($"[{i}].childAlias",  msg);
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