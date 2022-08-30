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
        
        //HF2LI hf2li = new HF2LI();
        MFLI mfli = new MFLI("dev5488");

        //DataGather dataGather = new DataGather(hf2li, mfli);

        Console.WriteLine(mfli.testconnection());
        int[] input = {0, 1, 2, 3};
        double[] output = mfli.get_data(input);
        for (int i = 0; i < 4; i++)
        {
            Console.WriteLine(output[i].ToString());
        }

        while (true)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.D1)
                {
                    Console.WriteLine(mfli.close_conn());
                }
            }
        }
    }
}

