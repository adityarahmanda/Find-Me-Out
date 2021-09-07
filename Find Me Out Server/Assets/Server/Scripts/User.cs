using System;

[System.Serializable]
public class User
{
   private string userId;
   public string UserId
   {
       get { return userId; }
       private set { userId = value; }
   }

   private string displayName;
   public string DisplayName
   {
       get { return displayName; }
       private set { displayName = value; }
   }

   private string email;
   public string Email
   {
       get { return email; }
       private set { email = value; }
   }

   public User(string _userId, string _displayName, string _email)
   {
       userId = _userId;
       displayName = _displayName;
       email = _email;
   }
}