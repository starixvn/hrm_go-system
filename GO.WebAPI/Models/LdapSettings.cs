namespace GO.WebAPI.Models
{
    public class LdapSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Domain { get; set; }
        public string LoginSchema { get; set; }
    }
}