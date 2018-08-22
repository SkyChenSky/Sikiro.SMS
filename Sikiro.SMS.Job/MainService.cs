using System;
using PeterKottas.DotNetCore.WindowsService.Interfaces;

namespace Sikiro.SMS.Job
{
    public class MainService : IMicroService
    {
        public void Start()
        {
            Console.WriteLine("I started");
        }

        public void Stop()
        {
            Console.WriteLine("I stopped");
        }
    }
}
