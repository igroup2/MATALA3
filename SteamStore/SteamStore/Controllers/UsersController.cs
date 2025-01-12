using Microsoft.AspNetCore.Mvc;
using SteamStore.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SteamStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        // GET: api/<UsersController>
        [HttpGet("GetAllUsers")]
        public IEnumerable<User> Get()
        {
            return SteamStore.Models.User.Read();
        }

 
        // GET api/<UsersController>/GetId
        [HttpGet("{email}")]
        public User GetUserId(string email)
        {
            return SteamStore.Models.User.GetUserId(email);
        }

        // POST api/<UsersController>/addUser
        [HttpPost]
        public int Post([FromBody] User user)
        {
            return user.Insert();
        }

        // POST api/<UsersController>/login
           [HttpPost("login")]
         public User Login([FromBody] User user)
          {
              return SteamStore.Models.User.CheckLogin(user.Email,user.Password); 
         }


        // PUT api/<UsersController>/5
        [HttpPut("UpdateDetails")]
        public int Put([FromBody] User user)
        {
            return user.Update();
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
