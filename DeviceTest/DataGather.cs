/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zhinst;
using HF2LI_namespace;
using MFLI_namespace;

public class DataGather
{
    private HF2LI hf2li;
    private MFLI mfli;
    private string controller_type;

    public DataGather(HF2LI hf2li, MFLI mfli)
    {
        this.hf2li = hf2li;
        this.mfli = mfli;
    }

    /*
    private static bool isDeviceFamily(ziDotNET daq, string dev, String family)
    {
        String path = String.Format("/{0}/features/devtype", dev);
        String devType = daq.getByte(path);
        return devType.StartsWith(family);
    }
    public void set_controller_type(ziDotNET daq, string dev)
    {
        if (isDeviceFamily(daq, dev, "HF2")) {
            controller_type = "HF2LI";
        } else if (isDeviceFamily(daq, dev, "MF")) {
            controller_type = "MFLI";
        } else {
            return;
        }
    }
    */
/*
    public double get_data(int CH)
    {
        //let controller type be controlled in settings
        //string data_controller_type = Properties.Settings.Default.data_controller; (original code from Greg)

        string data_controller_type = "MFLI";
        switch (data_controller_type)
        {
            case "HF2LI":
                return get_HF2LI_data(CH);
            case "MFLI":
                return get_MFLI_data(CH);
            default:
                return 0;
        }
    }

    private double get_HF2LI_data(int CH)
    {
        //string devstr = Properties.Settings.Default.HF2LI_dev; (original code from Greg)
        string devstr = "dev968";
        //if (hf2li.testconnection(devstr))
        if (hf2li.connected)
            return hf2li.get_data(CH);
        else
        {
            hf2li.devstr = devstr;
            bool connected = hf2li.testconnection();
            if (connected) return hf2li.get_data(CH);
        }
        Console.WriteLine("Problem with HF2LI connection");
        return 0;
    }

    private double get_MFLI_data(int CH)
    {
        //need to find actual devstr for MFLI
        string devstr = "dev5488";
        if (mfli.connected)
            return mfli.get_data(CH);
        else
        {
            mfli.devstr = devstr;
            bool connected = mfli.testconnection();
            if (connected) return mfli.get_data(CH);
        }
        Console.WriteLine("Problem with MFLI connection");
        return 0;
    }
}
*/