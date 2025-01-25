namespace SteamStore.Models

{
    public class User
    {
        int id;
        string name;
        string email;
        string password;
        bool isActive;
        static List<User> UserList = new List<User>();
        
        public User() { }   
        public User(int id, string name, string email, string password,bool isActive)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            IsActive = isActive;
        }

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Email { get => email; set => email = value; }
        public string Password { get => password; set => password = value; }
        public bool IsActive { get => isActive;  set=> isActive = value; }

        static public User CheckLogin(string email,string password)
        {
            DBservices dbs = new DBservices();
            return dbs.Check(email,password);
            //  return 0;
        }

      // public int logIn()
    //    {
      //      DBservices dbs = new DBservices();
      //      return dbs.logIn(this);


            //  return 1;
      //  }
        static public User Insert(User user)
        {
            DBservices dbs = new DBservices();
            return dbs.Insert(user);

         
          //  return 1;
        }

        public int Update()
        {
            DBservices dbs = new DBservices();
            return dbs.Update(this);
        }

       static public List<User> Read()
        {
            DBservices dbs = new DBservices();
            return dbs.Read();
        }

       static public User GetUserId(String email)
        {
            DBservices dbs = new DBservices();
            return dbs.GetUserId(email);

        }
         public object GetUserInfo()
        {
            DBservices dbs = new DBservices();
            return dbs.GetUserInfo();
        }
    }
}
