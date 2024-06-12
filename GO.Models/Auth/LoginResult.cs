namespace GO.Models.Auth
{
    using System;

    [Serializable]
    public class LoginResult
    {
        public int UserId { get; set; }
        public int EmpId { get; set; }
        public int? LinkToUserId { get; set; }
        public JwtToken JwtToken { get; set; }
        public long OnUserId { get; set; }

        public bool IsSuccessfull { get; set; }
        public string ErrorMessage { get; set; }
    }
}