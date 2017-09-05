using System;

namespace BenNetwork
{
    class Program
    {
        static void Main(string[] args)
        {
            RobotServer robotServer = new RobotServer();
            robotServer.Init(InitSuccess, InitFail);

            while (true)
            {
                string str = Console.ReadLine();
                if (str == "exit")
                {
                    if (robotServer != null)
                        robotServer.OnDestory();
                    break;
                }
            }
            Console.ReadKey();
        }

        static void InitSuccess()
        {
            Console.WriteLine("Init Success");
        }

        static void InitFail()
        {
            Console.WriteLine("Init Fail");
        }
    }
}
