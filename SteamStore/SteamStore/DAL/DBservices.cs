using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using SteamStore.Models;

/// <summary>
/// DBServices is a class created by me to provides some DataBase Services
/// </summary>
public class DBservices
{

    public DBservices()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    //--------------------------------------------------------------------------------------------------
    // This method creates a connection to the database according to the connectionString name in the web.config 
    //--------------------------------------------------------------------------------------------------
    public SqlConnection connect(String conString)
    {

        // read the connection string from the configuration file
        IConfigurationRoot configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json").Build();
        string cStr = configuration.GetConnectionString("myProjDB");
        SqlConnection con = new SqlConnection(cStr);
        con.Open();
        return con;
    }



    //--------------------------------------------------------------------------------------------------
    // This method update a User to the User table 
    //--------------------------------------------------------------------------------------------------
    public int Update(User user)
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        cmd = CreateCommandWithStoredProcedure("SP_InfoUserUpdate", con, user);             // create the command

        try
        {
            int numEffected = cmd.ExecuteNonQuery(); // execute the command
            return numEffected;
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }

    }



    //---------------------------------------------------------------------------------
    // Create the SqlCommand using a stored procedure
    //---------------------------------------------------------------------------------
    private SqlCommand CreateCommandWithStoredProcedure(String spName, SqlConnection con, User user)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        cmd.Parameters.AddWithValue("@Id", user.Id);

        cmd.Parameters.AddWithValue("@Name", user.Name);

        cmd.Parameters.AddWithValue("@Email", user.Email);

        cmd.Parameters.AddWithValue("@Password", user.Password);


        return cmd;
    }



    //--------------------------------------------------------------------------------------------------
    // This method insert a User to the User table 
    //--------------------------------------------------------------------------------------------------
    //
    public User Insert(User user)
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@Name", user.Name);
        paramDic.Add("@Email", user.Email);
        paramDic.Add("@Password", user.Password);
    

        cmd = CreateCommandWithStoredProcedureGeneral("SP_InsertUser", con, paramDic);          // create the command
        SqlParameter returnValue = new SqlParameter
        {
            Direction = ParameterDirection.ReturnValue
        };
        cmd.Parameters.Add(returnValue);
        SqlDataReader TheUser = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        int status = (int)returnValue.Value;



        if (returnValue.Value != null && (int)returnValue.Value == 1)
        {
            // סגור את החיבור במקרה של כשל
            if (con.State == ConnectionState.Open)
                con.Close();

            throw new Exception("A user with these details already exists. ");
        }

       

            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        

        return user;
    }
    //

    //---------------------------------------------------------------------------------
    // Create the SqlCommand
    //---------------------------------------------------------------------------------
    private SqlCommand CreateCommandWithStoredProcedureGeneral(String spName, SqlConnection con, Dictionary<string, object> paramDic)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        if (paramDic != null)
            foreach (KeyValuePair<string, object> param in paramDic)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);

            }


        return cmd;
    }


    //--------------------------------------------------------------------------------------------------
    // This method Check a User to the User table 
    //--------------------------------------------------------------------------------------------------
    //
    public User Check(string email, string password)
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw new Exception("Database connection error: " + ex.Message);
        }

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@Email", email);
        paramDic.Add("@Password", password);

        cmd = CreateCommandWithStoredProcedureCheck("SP_CheckLogIn", con, paramDic);

        SqlParameter returnValue = new SqlParameter
        {
            Direction = ParameterDirection.ReturnValue
        };
        cmd.Parameters.Add(returnValue);
        SqlDataReader TheUser = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        int status = (int)returnValue.Value;


        if (returnValue.Value != null && (int)returnValue.Value == 0)
        {
            // סגור את החיבור במקרה של כשל
            if (con.State == ConnectionState.Open)
                con.Close();

            throw new Exception("The User is inactive.");
        }

        User user = new User();

        while (TheUser.Read())
        {
            user.Id = Convert.ToInt32(TheUser["Id"]);
            user.Name = TheUser["Name"].ToString();
            user.Email = TheUser["Email"].ToString();
            user.Password = TheUser["Password"].ToString();
            user.IsActive = Convert.ToBoolean(TheUser["isActive"]);
        }
      if (user == null)
        {
            throw new Exception("Unexpected error: User data not retrieved.");
        }


        // create the command
        return user;



    }
    //

    //---------------------------------------------------------------------------------
    // Create the SqlCommand
    //---------------------------------------------------------------------------------
    private SqlCommand CreateCommandWithStoredProcedureCheck(String spName, SqlConnection con, Dictionary<string, object> paramDic)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        if (paramDic != null)
            foreach (KeyValuePair<string, object> param in paramDic)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);

            }


        return cmd;
    }
    //--------------------------------------------------------------------------------------------------
    // This method Read The Users Table
    //--------------------------------------------------------------------------------------------------
    //
    public List<User> Read()
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        List<User> Users = new List<User>();

        Dictionary<string, object> paramDic = new Dictionary<string, object>();

        cmd = CreateCommandWithStoredProcedureRead("SP_GetAllUsers", con, paramDic);

        SqlDataReader UsersTable = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        while (UsersTable.Read())
        {
            User user = new User();
            user.Id = Convert.ToInt32(UsersTable["Id"]);
            user.Name = UsersTable["Name"].ToString();
            user.Email = UsersTable["Email"].ToString();
            user.Password = UsersTable["Password"].ToString();

            Users.Add(user);
        }

        // create the command
        return Users;

    
    }
    //

    //---------------------------------------------------------------------------------
    // Create the SqlCommand
    //---------------------------------------------------------------------------------
    private SqlCommand CreateCommandWithStoredProcedureRead(String spName, SqlConnection con, Dictionary<string, object> paramDic)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        if (paramDic != null)
            foreach (KeyValuePair<string, object> param in paramDic)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);

            }


        return cmd;
    }


    //--------------------------------------------------------------------------------------------------
    // This method Get User Id 
    //--------------------------------------------------------------------------------------------------
    //
    public User GetUserId(string email)
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

      

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@Email",email);

        cmd = CreateCommandWithStoredProcedureGetId("SP_GetUserId", con, paramDic);

        SqlDataReader TheUser = cmd.ExecuteReader(CommandBehavior.CloseConnection);


        User user = new User();
        while (TheUser.Read())
        {
            user.Id = Convert.ToInt32(TheUser["Id"]);
            user.Name = TheUser["Name"].ToString();
            user.Email = TheUser["Email"].ToString();
            user.Password = TheUser["Password"].ToString();
          }
       

        // create the command
        return user;


    }
    //

    //---------------------------------------------------------------------------------
    // Create the SqlCommand
    //---------------------------------------------------------------------------------
    private SqlCommand CreateCommandWithStoredProcedureGetId(String spName, SqlConnection con, Dictionary<string, object> paramDic)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        if (paramDic != null)
            foreach (KeyValuePair<string, object> param in paramDic)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);

            }


        return cmd;
    }



    //--------------------------------------------------------------------------------------------------
    // This method Read The Games Table
    //--------------------------------------------------------------------------------------------------
    //
    public List<Game> ReadGames()
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        List<Game> Games = new List<Game>();

        Dictionary<string, object> paramDic = new Dictionary<string, object>();

        cmd = CreateCommandWithStoredProcedureReadGame("SP_GetAllGames", con, paramDic);

        SqlDataReader GamesTable = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        while (GamesTable.Read())
        {
            Game game = new Game();
            game.AppID = Convert.ToInt32(GamesTable["AppID"]);
            game.Name = GamesTable["Name"].ToString();
            game.ReleaseDate = DateTime.Parse(GamesTable["Release_Date"].ToString());
            game.Price = Convert.ToDouble(GamesTable["Price"]);
            game.Description = GamesTable["Description"].ToString();
            game.HeaderImage = GamesTable["Header_Image"].ToString();
            game.Website = GamesTable["Website"].ToString();
            game.Windows = Convert.ToBoolean(GamesTable["Windows"]);
            game.Mac = Convert.ToBoolean(GamesTable["Mac"]);
            game.Linux = Convert.ToBoolean(GamesTable["Linux"]);
            game.ScoreRank = Convert.ToInt32(GamesTable["Score_Rank"]);
            game.Recommendations = GamesTable["Recommendations"].ToString();
            game.NumberOfPurchases = Convert.ToInt32(GamesTable["NumberOfPurchases"]);


            Games.Add(game);
        }

        // create the command
        return Games;


    }
    //

    //---------------------------------------------------------------------------------
    // Create the SqlCommand
    //---------------------------------------------------------------------------------
    private SqlCommand CreateCommandWithStoredProcedureReadGame(String spName, SqlConnection con, Dictionary<string, object> paramDic)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        if (paramDic != null)
            foreach (KeyValuePair<string, object> param in paramDic)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);

            }


        return cmd;
    }




    //--------------------------------------------------------------------------------------------------
    // This method Read The User Games Table
    //--------------------------------------------------------------------------------------------------
    //
    public List<Game> ReadUserGames(int UserID)
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        List<Game> Games = new List<Game>();
    

        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@Id", UserID);

        cmd = CreateCommandWithStoredProcedureReadUserGame("SP_GetUserGames", con, paramDic);

        SqlDataReader UserGamesTable = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        while (UserGamesTable.Read())
        {
            Game game = new Game();
            game.AppID = Convert.ToInt32(UserGamesTable["AppID"]);
            game.Name = UserGamesTable["Name"].ToString();
            game.ReleaseDate = DateTime.Parse(UserGamesTable["Release_Date"].ToString());
            game.Price = Convert.ToDouble(UserGamesTable["Price"]);
            game.Description = UserGamesTable["Description"].ToString();
            game.HeaderImage = UserGamesTable["Header_Image"].ToString();
            game.Website = UserGamesTable["Website"].ToString();
            game.Windows = Convert.ToBoolean(UserGamesTable["Windows"]);
            game.Mac = Convert.ToBoolean(UserGamesTable["Mac"]);
            game.Linux = Convert.ToBoolean(UserGamesTable["Linux"]);
            game.ScoreRank = Convert.ToInt32(UserGamesTable["Score_Rank"]);
            game.Recommendations = UserGamesTable["Recommendations"].ToString();
            game.NumberOfPurchases = Convert.ToInt32(UserGamesTable["NumberOfPurchases"]);

            Games.Add(game);
        }

        // create the command
        return Games;


    }
    //

    //---------------------------------------------------------------------------------
    // Create the SqlCommand
    //---------------------------------------------------------------------------------
    private SqlCommand CreateCommandWithStoredProcedureReadUserGame(String spName, SqlConnection con, Dictionary<string, object> paramDic)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        if (paramDic != null)
            foreach (KeyValuePair<string, object> param in paramDic)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);

            }


        return cmd;
    }



    //--------------------------------------------------------------------------------------------------
    // This method filter the games by price
    //--------------------------------------------------------------------------------------------------
    //
    public List<Game> GetByPrice(double MaxPrice ,int UserId)
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        List<Game> Games = new List<Game>();


        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@MaxPrice", MaxPrice);
        paramDic.Add("@UserId", UserId);

        cmd = CreateCommandWithStoredProcedureGetByPrice("SP_GetByPrice", con, paramDic);

        SqlDataReader PriceGamesTable = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        while (PriceGamesTable.Read())
        {
            Game game = new Game();
            game.AppID = Convert.ToInt32(PriceGamesTable["AppID"]);
            game.Name = PriceGamesTable["Name"].ToString();
            game.ReleaseDate = DateTime.Parse(PriceGamesTable["Release_Date"].ToString());
            game.Price = Convert.ToDouble(PriceGamesTable["Price"]);
            game.Description = PriceGamesTable["Description"].ToString();
            game.HeaderImage = PriceGamesTable["Header_Image"].ToString();
            game.Website = PriceGamesTable["Website"].ToString();
            game.Windows = Convert.ToBoolean(PriceGamesTable["Windows"]);
            game.Mac = Convert.ToBoolean(PriceGamesTable["Mac"]);
            game.Linux = Convert.ToBoolean(PriceGamesTable["Linux"]);
            game.ScoreRank = Convert.ToInt32(PriceGamesTable["Score_Rank"]);
            game.Recommendations = PriceGamesTable["Recommendations"].ToString();
            game.NumberOfPurchases = Convert.ToInt32(PriceGamesTable["NumberOfPurchases"]);

            Games.Add(game);
        }

        // create the command
        return Games;


    }
    //

    //---------------------------------------------------------------------------------
    // Create the SqlCommand
    //---------------------------------------------------------------------------------
    private SqlCommand CreateCommandWithStoredProcedureGetByPrice(String spName, SqlConnection con, Dictionary<string, object> paramDic)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        if (paramDic != null)
            foreach (KeyValuePair<string, object> param in paramDic)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);

            }


        return cmd;
    }



    //--------------------------------------------------------------------------------------------------
    // This method filter the games by Rank
    //--------------------------------------------------------------------------------------------------
    //
    public List<Game> GetByRankScore(int MaxRank, int UserId)
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        List<Game> Games = new List<Game>();


        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@MaxRank", MaxRank);
        paramDic.Add("@UserId", UserId);

        cmd = CreateCommandWithStoredProcedureGetByMaxRank("SP_GetByRankScore", con, paramDic);

        SqlDataReader PriceGamesTable = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        while (PriceGamesTable.Read())
        {
            Game game = new Game();
            game.AppID = Convert.ToInt32(PriceGamesTable["AppID"]);
            game.Name = PriceGamesTable["Name"].ToString();
            game.ReleaseDate = DateTime.Parse(PriceGamesTable["Release_Date"].ToString());
            game.Price = Convert.ToDouble(PriceGamesTable["Price"]);
            game.Description = PriceGamesTable["Description"].ToString();
            game.HeaderImage = PriceGamesTable["Header_Image"].ToString();
            game.Website = PriceGamesTable["Website"].ToString();
            game.Windows = Convert.ToBoolean(PriceGamesTable["Windows"]);
            game.Mac = Convert.ToBoolean(PriceGamesTable["Mac"]);
            game.Linux = Convert.ToBoolean(PriceGamesTable["Linux"]);
            game.ScoreRank = Convert.ToInt32(PriceGamesTable["Score_Rank"]);
            game.Recommendations = PriceGamesTable["Recommendations"].ToString();
            game.NumberOfPurchases = Convert.ToInt32(PriceGamesTable["NumberOfPurchases"]);

            Games.Add(game);
        }

        // create the command
        return Games;


    }
    //

    //---------------------------------------------------------------------------------
    // Create the SqlCommand
    //---------------------------------------------------------------------------------
    private SqlCommand CreateCommandWithStoredProcedureGetByMaxRank(String spName, SqlConnection con, Dictionary<string, object> paramDic)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        if (paramDic != null)
            foreach (KeyValuePair<string, object> param in paramDic)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);

            }


        return cmd;
    }



    //--------------------------------------------------------------------------------------------------
    // This method buy a Game
    //--------------------------------------------------------------------------------------------------
    //
    public List<Game> BuyGame(int appID, int UserId)
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        List<Game> Games = new List<Game>();


        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@gameID", appID);
        paramDic.Add("@userID", UserId);
        
        cmd = CreateCommandWithStoredProcedureBuyAGame("SP_BuyGame", con, paramDic);

        SqlDataReader GamesTable = cmd.ExecuteReader(CommandBehavior.CloseConnection);
        
        object PriceGamesTable = null;
        while (GamesTable.Read())
        {
            Game game = new Game();
            game.AppID = Convert.ToInt32(GamesTable["AppID"]);
            game.Name =GamesTable["Name"].ToString();
            game.ReleaseDate = DateTime.Parse(GamesTable["Release_Date"].ToString());
            game.Price = Convert.ToDouble(GamesTable["Price"]);
            game.Description = GamesTable["Description"].ToString();
            game.HeaderImage = GamesTable["Header_Image"].ToString();
            game.Website = GamesTable["Website"].ToString();
            game.Windows = Convert.ToBoolean(GamesTable["Windows"]);
            game.Mac = Convert.ToBoolean(GamesTable["Mac"]);
            game.Linux = Convert.ToBoolean(GamesTable["Linux"]);
            game.ScoreRank = Convert.ToInt32(GamesTable["Score_Rank"]);
            game.Recommendations = GamesTable["Recommendations"].ToString();
            game.NumberOfPurchases = Convert.ToInt32(GamesTable["NumberOfPurchases"]);

            Games.Add(game);
        }

        // create the command
        return Games;


    }
    //

    //---------------------------------------------------------------------------------
    // Create the SqlCommand
    //---------------------------------------------------------------------------------
    private SqlCommand CreateCommandWithStoredProcedureBuyAGame(String spName, SqlConnection con, Dictionary<string, object> paramDic)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        if (paramDic != null)
            foreach (KeyValuePair<string, object> param in paramDic)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);

            }


        return cmd;
    }

    //--------------------------------------------------------------------------------------------------
    // This method delete game by id
    //--------------------------------------------------------------------------------------------------
    //
    public int DeleteById(int GameId, int UserId)
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }


        Dictionary<string, object> paramDic = new Dictionary<string, object>();
        paramDic.Add("@gameID", GameId);
        paramDic.Add("@userID", UserId);

        cmd = CreateCommandWithStoredProcedureDeleteAGame("SP_DeleteGame", con, paramDic);

        try
        {
            int numEffected = cmd.ExecuteNonQuery(); // execute the command
            return numEffected;
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        finally
        {
            if (con != null)
            {
                // close the db connection
                con.Close();
            }
        }
    }
    //

    //---------------------------------------------------------------------------------
    // Create the SqlCommand
    //---------------------------------------------------------------------------------
    private SqlCommand CreateCommandWithStoredProcedureDeleteAGame(String spName, SqlConnection con, Dictionary<string, object> paramDic)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        if (paramDic != null)
            foreach (KeyValuePair<string, object> param in paramDic)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);

            }


        return cmd;
    }

    //--------------------------------------------------------------------------------------------------
    // This method bring specific info about the user
    //--------------------------------------------------------------------------------------------------
    //
    public Object GetUserInfo()
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        List<object> listOfUsers = new List<object>();
       

        Dictionary<string, object> paramDic = new Dictionary<string, object>();

        cmd = CreateCommandWithStoredProcedureUserSpecificInfo("SP_UsersSpecificInfo", con, paramDic);

        SqlDataReader UserSpecificInfo = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        while (UserSpecificInfo.Read())
        {
            listOfUsers.Add(new
            {
                Id = Convert.ToInt32(UserSpecificInfo["Id"]),
                name = UserSpecificInfo["name"].ToString(),
                NumOfGamesBought = Convert.ToInt32(UserSpecificInfo["NumOfGamesBought"]),
                TotalSpent = Convert.ToInt32(UserSpecificInfo["TotalSpent"]),
                isActive = Convert.ToBoolean(UserSpecificInfo["isActive"])
            });
        }

        return listOfUsers;
    
    }
    //

    //---------------------------------------------------------------------------------
    // Create the SqlCommand
    //---------------------------------------------------------------------------------
    private SqlCommand CreateCommandWithStoredProcedureUserSpecificInfo(String spName, SqlConnection con, Dictionary<string, object> paramDic)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        if (paramDic != null)
            foreach (KeyValuePair<string, object> param in paramDic)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);

            }


        return cmd;
    }

    //--------------------------------------------------------------------------------------------------
    // This method bring specific info about the Game
    //--------------------------------------------------------------------------------------------------
    //
    public Object GetGameInfo()
    {

        SqlConnection con;
        SqlCommand cmd;

        try
        {
            con = connect("myProjDB"); // create the connection
        }
        catch (Exception ex)
        {
            // write to log
            throw (ex);
        }

        List<object> listOfGames = new List<object>();


        Dictionary<string, object> paramDic = new Dictionary<string, object>();

        cmd = CreateCommandWithStoredProcedureGameSpecificInfo("SP_GameSpecificInfo", con, paramDic);

        SqlDataReader GameSpecificInfo = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        while (GameSpecificInfo.Read())
        {
            listOfGames.Add(new
            {
                appID = Convert.ToInt32(GameSpecificInfo["appID"]),
                Name = GameSpecificInfo["Name"].ToString(),
                numberOfPurchases = Convert.ToInt32(GameSpecificInfo["numberOfPurchases"]),
                TotalSpent = Convert.ToInt32(GameSpecificInfo["TotalSpent"])            });
        }

        return listOfGames;

    }
    //

    //---------------------------------------------------------------------------------
    // Create the SqlCommand
    //---------------------------------------------------------------------------------
    private SqlCommand CreateCommandWithStoredProcedureGameSpecificInfo(String spName, SqlConnection con, Dictionary<string, object> paramDic)
    {

        SqlCommand cmd = new SqlCommand(); // create the command object

        cmd.Connection = con;              // assign the connection to the command object

        cmd.CommandText = spName;      // can be Select, Insert, Update, Delete 

        cmd.CommandTimeout = 10;           // Time to wait for the execution' The default is 30 seconds

        cmd.CommandType = System.Data.CommandType.StoredProcedure; // the type of the command, can also be text

        if (paramDic != null)
            foreach (KeyValuePair<string, object> param in paramDic)
            {
                cmd.Parameters.AddWithValue(param.Key, param.Value);

            }


        return cmd;
    }

}
