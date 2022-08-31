using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using zhinst;
using System.Runtime.InteropServices;
using System.Runtime.ExceptionServices;
using System.Diagnostics;

namespace MFLI_namespace
{
    public class MFLI
    {
        public string devstr;
        public ziDotNET daq;
        public bool connected;
        public string current_command;
        string nl = System.Environment.NewLine;
        public bool auxmode = false;
        
        public MFLI(string devstr)
        {
            this.devstr = devstr;
        }

        public bool testconnection()
        {
            if (!connected)
            {
                try
                {
                    connected = false;

                    daq = new ziDotNET();
                    String id = daq.discoveryFind(devstr);
                    String iface = daq.discoveryGetValueS(devstr, "connected");
                    if (string.IsNullOrWhiteSpace(iface))
                    {
                        String ifacesList = daq.discoveryGetValueS(devstr, "interfaces");
                        string[] ifaces = ifacesList.Split('\n');
                        if (ifaces.Length > 0)
                        {
                            iface = ifaces[0];
                        }
                    }
                    String host = daq.discoveryGetValueS(devstr, "serveraddress");
                    long port = daq.discoveryGetValueI(devstr, "serverport");
                    long api = daq.discoveryGetValueI(devstr, "apilevel");

                    daq.init(host, Convert.ToUInt16(port), (ZIAPIVersion_enum)api);
                    daq.connectDevice(devstr, iface, "");

                    if (string.IsNullOrWhiteSpace(iface))
                    {
                        Console.WriteLine("Connection to MFLI failed, reconnecting..." + devstr);
                        return false;
                    }
                    else
                    {
                        connected = true;
                        Console.WriteLine("Connection to MFLI " + devstr + " was successful");

                        return true;
                    }
                }
                catch (Exception ee)
                {
                    connected = false;
                    Console.WriteLine("Problem connecting to MFLI " + ee);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        public bool close_conn()
        {
            if (connected)
            {
                try
                {
                    daq.disconnect();
                    return true;
                }
                catch (Exception ee)
                {
                    Console.WriteLine("Problem closing connection to MFLI " + ee);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        
        public double get_data(int CH)
        {
            if (auxmode)
            {
                String path = devstr + "/auxouts/" + CH.ToString() + "/value";
                return daq.getDouble(path);
            }
            else
            {
                String path = "/" + devstr + "/demods/" + CH.ToString() + "/sample";
                /*
                daq.subscribe(path);
                Lookup lookup = daq.poll(0.1, 5, 0, 1);

                /*
                Dictionary<String, Chunk[]> nodes = lookup.nodes; 
                Chunk[] chunks = lookup[path]; 
                Chunk chunk = lookup[path][0];  
                */

                //ZIDemodSample[] demodSamples = lookup[path][0].demodSamples;
                /*
                ZIDemodSample sample = lookup[path][0].demodSamples[0];
                */
                ZIDemodSample sample = daq.getDemodSample(path);

                double X = sample.x;
                double Y = sample.y;
                double R = Math.Sqrt(X * X + Y * Y);

                return R;
            }
        }

        public double[] get_data(int[] CH)
        {
            if (connected)
            {
                if (auxmode)
                {
                    String path = devstr + "/auxouts/*/  /*value";
                    return null;
                }
                else
                {
                    double[] R_CH = new double[CH.Length];

                    for (int i = 0; i < CH.Length; i++)
                    {
                        R_CH[i] = get_data(CH[i]);
                    }
                    return R_CH;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
