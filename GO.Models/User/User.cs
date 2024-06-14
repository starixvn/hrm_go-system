namespace GO.Models.User
{
    using System;

    [Serializable]
    public class User
    {
        public int UserId { get; set; }
        public string DomainUsername { get; set; }
        public string Username { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string OldPassword { get; set; }
        public string BCryptPassword { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsResetPassword { get; set; }
        public int? LinkToUserId { get; set; }

        public int EmpId { get; set; }
        public short CompanyId { get; set; }
        public string CompanyName { get; set; }
    }
}