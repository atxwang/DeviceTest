// See https://aka.ms/new-console-template for more information

using zhinst;
using HF2LI_namespace;
using MFLI_namespace;
using System;

/*
ziDotNET daq = new ziDotNET();

HF2LI hf2li = new HF2LI();
MFLI mfli = new MFLI();

DataGather dataGather = new DataGather(hf2li, mfli);
*/

//daq.connectDevice();


class Program
{
    private static void Main(string[] args)
    {

        ziDotNET daq = new ziDotNET();

        HF2LI hf2li = new HF2LI();
        MFLI mfli = new MFLI();

        DataGather dataGather = new DataGather(hf2li, mfli);

        while (true)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);

                dataGather.set_controller_type(daq, "dev5488"); //represent from settings?

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        Console.WriteLine(dataGather.get_data(1));
                        break;
                    case ConsoleKey.D2:
                        Console.WriteLine(dataGather.get_data(1));
                        break;
                    case ConsoleKey.D3:
                        Console.WriteLine(dataGather.get_data(1));
                        break;
                    case ConsoleKey.D4:
                        Console.WriteLine(dataGather.get_data(1));
                        break;
                    case ConsoleKey.D5:
                        Console.WriteLine(dataGather.get_data(1));
                        break;
                    case ConsoleKey.D6:
                        Console.WriteLine(dataGather.get_data(1));
                        break;
                    case ConsoleKey.Q:
                        Environment.Exit(0);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

/*
while (true)
{
   if (Console.KeyAvailable)
   {
       var key = Console.ReadKey(true);

       dataGather.set_controller_type(daq, "dev968"); //represent from settings?

       switch (key.Key)
       {
           case ConsoleKey.D1:
               Console.WriteLine(dataGather.get_data(1));
               break;
           case ConsoleKey.D2:
               Console.WriteLine(dataGather.get_data(1));
               break;
           case ConsoleKey.D3:
               Console.WriteLine(dataGather.get_data(1));
               break;
           case ConsoleKey.D4:
               Console.WriteLine(dataGather.get_data(1));
               break;
           case ConsoleKey.D5:
               Console.WriteLine(dataGather.get_data(1));
               break;
           case ConsoleKey.D6:
               Console.WriteLine(dataGather.get_data(1));
               break;
           case ConsoleKey.Q:
               System.Environment.Exit(0);
               break;
           default:
               break;
       }
   }
}
*/

//daq.disconnect();

