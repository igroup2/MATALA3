using Microsoft.AspNetCore.Mvc;
using SteamStore.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SteamStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        // GET: api/<GamesController>
        [HttpGet]
        public IEnumerable<Game> Get()
        {

            return SteamStore.Models.Game.Read();
        }


        // GET: api/<GamesController>
        [HttpGet("{UserID}")]
        public IEnumerable<Game> Get(int UserID)
        {

            return SteamStore.Models.Game.ReadUserGames(UserID);
        }

        [HttpGet("search")] // this uses the QueryString
        public IEnumerable<Game> GetByPrice(double MaxPrice, int UserId)
        {
            return SteamStore.Models.Game.GetByPrice(MaxPrice, UserId);
        }

     

        [HttpGet("searchByRankScore/maxRank/{maxRank}/{UserId}")] // this uses resource routing
        public IEnumerable<Game> GetByRankScore(int maxRank, int UserId)
        {
            return SteamStore.Models.Game.GetByRankScore(maxRank, UserId);

        }

        // POST api/<GamesController>
        [HttpPost("BuyAGame")]
        public List<Game> Post(int appID, int UserId)
        {
            return SteamStore.Models.Game.BuyGame(appID,UserId);
        }

        // PUT api/<GamesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<GamesController>/5
        [HttpDelete("{GameId}&{UserId}")]
        public int DeleteById(int GameId , int UserId)
        {
           return  SteamStore.Models.Game.DeleteById(GameId, UserId);
        }


        // GET api/<UsersController>/GetId
        [HttpGet("SpecificGameInfo")]
        public Object GetGameInfo()
        {
            Game game = new Game();
            return game.GetGameInfo();
        }

    }
}
