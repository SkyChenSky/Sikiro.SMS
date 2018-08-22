namespace Sikiro.SMS.Bus
{
    public class Config
    {
        public Server Server { get; set; }
    }

    public class Server
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }
}
