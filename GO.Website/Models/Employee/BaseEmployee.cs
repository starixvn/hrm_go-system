namespace GO.Website.Models.Employee
{
    using System;

    [Serializable]
    public class BaseEmployee
    {
        public short CompanyId { get; set; }
        public string CompanyName { get; set; }

        public int EmpId { get; set; }
        public int UserId { get; set; }
        public string Code { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public string DepartmentName { get; set; }
        public string SectionName { get; set; }
        public string TeamName { get; set; }
    }
}