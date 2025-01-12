namespace SteamStore.Models
{
    public class Game
    {
        int appID;
        string name;
        DateTime releaseDate;
        double price;
        string description;
        string headerImage;
        string website;
        bool windows;
        bool mac;
        bool linux;
        int scoreRank;
        string recommendations;
        string publisher;
        int numberOfPurchases;


        static List<Game> GamesList = new List<Game>();

        public Game() { }

        public Game(int appID, string name, DateTime releaseDate, double price, string description, string headerImage, string website, bool windows, bool mac, bool linux, int scoreRank, string recommendations, string publisher, int numberOfPurchases)
        {
            AppID = appID;
            Name = name;
            ReleaseDate = releaseDate;
            Price = price;
            Description = description;
            HeaderImage = headerImage;
            Website = website;
            Windows = windows;
            Mac = mac;
            Linux = linux;
            ScoreRank = scoreRank;
            Recommendations = recommendations;
            Publisher = publisher;
            NumberOfPurchases = numberOfPurchases;
        }

        public int AppID { get => appID; set => appID = value; }
        public string Name { get => name; set => name = value; }
        public DateTime ReleaseDate { get => releaseDate; set => releaseDate = value; }
        public double Price { get => price; set => price = value; }
        public string Description { get => description; set => description = value; }
        public string HeaderImage { get => headerImage; set => headerImage = value; }
        public string Website { get => website; set => website = value; }
        public bool Windows { get => windows; set => windows = value; }
        public bool Mac { get => mac; set => mac = value; }
        public bool Linux { get => linux; set => linux = value; }
        public int ScoreRank { get => scoreRank; set => scoreRank = value; }
        public string Recommendations { get => recommendations; set => recommendations = value; }
        public string Publisher { get => publisher; set => publisher = value; }
        public int NumberOfPurchases { get => numberOfPurchases; set => numberOfPurchases = value; }

        static public List<Game> BuyGame(int appID, int UserId)
        {
            DBservices dbs = new DBservices();
            return dbs.BuyGame(appID, UserId);
        }


        static public List<Game> Read()
            {
            DBservices dbs = new DBservices();
            return dbs.ReadGames();
        }


        static public List<Game> ReadUserGames(int UserID)
        {
            DBservices dbs = new DBservices();
            return dbs.ReadUserGames(UserID);
        }


        public static List<Game> GetByPrice(double MaxPrice, int UserId)
        {
            DBservices dbs = new DBservices();
            return dbs.GetByPrice(MaxPrice, UserId);
        }

        public static List<Game> GetByRankScore(int MaxRank, int UserId)
        {
            DBservices dbs = new DBservices();
            return dbs.GetByRankScore(MaxRank, UserId);
        }


        static public int DeleteById (int GameId,int UserId)
        {
            DBservices dbs = new DBservices();
             return dbs.DeleteById(GameId, UserId);
        }



    }

    
} 
