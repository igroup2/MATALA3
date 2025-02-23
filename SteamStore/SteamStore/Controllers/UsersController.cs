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
        [HttpPost("checkDetails")]
        public IActionResult Login([FromBody] User user)
        {
            try
            {
                // בדיקת המשתמש
                var result = SteamStore.Models.User.CheckLogin(user.Email, user.Password);

                // אם המשתמש לא קיים, מחזירים שגיאה
                if (result == null)
                {
                    return BadRequest(new { message = "User not found or inactive." });
                }

                // החזרת הצלחה
                return Ok(result);
            }
            catch (Exception ex)
            {
                // טיפול בחריגות
                return StatusCode(500, new { message = ex.Message });
            }
        }



        // GET api/<UsersController>/GetId
        [HttpGet("{email}")]
        public User GetUserId(string email)
        {
            return SteamStore.Models.User.GetUserId(email);
        }

        // POST api/<UsersController>/addUser
        [HttpPost]
        public User Post([FromBody] User user)
        {

            return SteamStore.Models.User.Insert(user);
        }
        

        // POST api/<UsersController>/login
        /*
        [HttpPost("login")]
        public User Login([FromBody] User user)
          {
              return SteamStore.Models.User.CheckLogin(user.Email,user.Password); 
         }
        */

        // PUT api/<UsersController>/5
        [HttpPut("UpdateDetails")]
        public int Put([FromBody] User user)
        {
            return user.Update();
        }
        [HttpPut("UpdateUserStatus")]
        public int Put([FromBody] int userID, bool isActive)
        
        {
            User u = new User(); 
            return u.UpdateUserStatus( userID,  isActive);
        }


        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        // GET api/<UsersController>/GetId
        [HttpGet("SpecificUserInfo")]
        public Object GetUserInfo()
        {
            User user = new User();
            return user.GetUserInfo();
        }
    }
}
