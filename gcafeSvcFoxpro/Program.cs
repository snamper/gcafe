using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.ServiceModel;

namespace gcafeSvcFoxpro
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost serviceHost = new ServiceHost(typeof(gcafeSvc)))
            {
                serviceHost.Open();

                // The service can now be accessed.
                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();

                serviceHost.Close();
            }
        }
    }
}
