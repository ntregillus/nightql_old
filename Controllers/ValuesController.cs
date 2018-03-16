using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NightQL.Data;
using NightQL.Models;

namespace NightQL.Controllers
{
    
    public class ValuesController : BaseSchemaController
    {
        public ValuesController(ConfigContext db) : base(db){ }

        // GET api/values
        [HttpGet]
        [Route("api/{schema}/{*url}")]
        public async Task<IActionResult> Get([FromRoute]string schema, [FromRoute]string url)
        {
            var result = await StandardValidationStuff(schema);
            if(result != null){
                return result;
            }

            throw new NotImplementedException();
        }

        // POST api/values
        [HttpPost]
        [Route("api/{schema}/{*url}")]
        public async Task<IActionResult> Post([FromRoute] string schema, [FromRoute]string url)
        {
            var result = await StandardValidationStuff(schema)
                ?? ValidateRequest(schema, Request.Method, url, Request.Body);
            if(result != null){
                return result;
            }
            throw new NotImplementedException();
        }

        private IActionResult ValidateRequest(string schema, string httpVerb, string url, Stream payload)
        {
            var urlParts = url.Split("/").ToList();
            if(urlParts.Count % 2 != 0 && Request.Method == "Post")
            {
                ModelState.AddModelError("route", "Cannot post to " + url + ".");               
            }
            DataModel model;
            using (StreamReader sr = new StreamReader(payload))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();
            
                // read the json from a stream
                // json size doesn't matter because only a small piece is read at a time from the HTTP request
                model = serializer.Deserialize<DataModel>(reader);
            }
            
            string entityAlias;
            string identifier;
            for(int i = 0; i < urlParts.Count; i++)
            {
                if(i % 2 == 1) // entityAlias
                {
                    entityAlias = urlParts[i];
                    var result = ValidateEntity(schema, entityAlias, i > 0, model); 
                    if(result != null)
                    {
                        return result;
                    }
                }
                else // entityAlias
                {
                    identifier = urlParts[i];
                    
                }
            }


            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return null;
        }

        protected IActionResult ValidateEntity(string schema, string name, bool isAlias, DataModel model){
            var query = new EntitySearch();
            if(isAlias)
            {
                query.AssociatedEntity = name;
            }
            else
            {
                query.Name = name;                
            }
            var entities = Db.GetEntityDefinitions(schema, query);
            if(!entities.Any()){
                return NotFound();
            }
            throw new NotImplementedException();
        }

        // PUT api/values/5
        [HttpPut]
        [Route("api/{schema}/{*url}")]
        public async Task<IActionResult> Put([FromRoute] string schema, [FromBody]string model)
        {
            var result = await StandardValidationStuff(schema);
            if(result != null){
                return result;
            }
            throw new NotImplementedException();
        }

        // DELETE api/values/5
        [HttpDelete]
        [Route("api/{schema}/{*url}")]
        public async Task<IActionResult> Delete([FromRoute] string schema)
        {
            var result = await StandardValidationStuff(schema);
            if(result != null){
                return result;
            }
            throw new NotImplementedException();
        }
    }
}
