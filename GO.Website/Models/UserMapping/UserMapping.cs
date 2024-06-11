namespace GO.Website.Models.UserMapping
{
    using System;
    using System.Collections.Generic;
    using GO.Website.Models.Employee;

    [Serializable]
    public class UserMapping : BaseEmployee
    {
        public int? LinkToUserId { get; set; }
        public IList<UserMapping> SubUsers { get; set; }
    }
}