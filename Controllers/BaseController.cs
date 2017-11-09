using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NightQL.Data;
using NightQL.Data.DbEntities;

namespace NightQL.Controllers
{
    public abstract class BaseSchemaController: Controller
    {
        public ConfigContext Db { get; set; }
        
        public BaseSchemaController(ConfigContext db)
        {
            Db = db;
        }

        protected async Task<IActionResult> StandardValidationStuff(string schema)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var s = await Db.GetSchemaAsync(schema);
            if(s == null){
                return NotFound();
            }
            return null; // aka continue!
        }
    }
}