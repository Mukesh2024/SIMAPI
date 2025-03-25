using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SIMAPI.Model;
using System.Net.Http.Json;

namespace SIMAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        private readonly string _jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Database");


        [HttpPost(Name = "Login")]
        public async Task<bool> Login(User model)
        {
            if (ModelState.IsValid)
            {
                var userString = await System.IO.File.ReadAllTextAsync(Path.Combine(_jsonFilePath, "Users.json"));
                var userObject = JsonConvert.DeserializeObject<User>(userString);

                if (model.Username == userObject.Username && model.Password == userObject.Password)
                {
                    return true;
                }

                return false;
            }
            return false;
        }
    }
}
