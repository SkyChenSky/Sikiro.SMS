namespace Sikiro.SMS.Job
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
        public Job[] Jobs { get; set; }
    }

    public class Job
    {
        public string Name { get; set; }
        public string Cron { get; set; }
    }
}
