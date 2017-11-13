using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NightQL.Data;
using NightQL.Data.DbEntities;
using NightQL.Models;
using System.Linq;
namespace NightQL.Controllers
{
    public abstract class BaseSchemaController: Controller
    {
        public ConfigContext Db { get; set; }
        
        public BaseSchemaController(ConfigContext db)
        {
            Db = db;
        }

        protected async Task<IActionResult> StandardValidationStuff(string schema, string entityName = null)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var s = await Db.GetSchemaAsync(schema);
            if(s == null){
                return NotFound();
            }
            if(!string.IsNullOrEmpty(entityName))
            {
                var existingEntities = Db.GetEntityDefinitions(schema, new EntitySearch{Name = entityName});
                if(!existingEntities.Any())
                {
                    return NotFound();
                }
            }

            return null; // aka continue!
        }
    }
}