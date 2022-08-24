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

//problem with send-command, particularly in stream.Write() where it loses established connection. possibly due to accessing same ports for client and device. 
//fixes: no client/data streaming, only use API functions
//fixes: find a different port to connect the client?

namespace MFLI_namespace
{
    public class MFLI
    {
        public string devstr;
        public ziDotNET daq;
        public bool connected;

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

                    ziDotNET daq = new ziDotNET();
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

        /*
        public bool testconnection()
        {
            if (!connected)
            {
                try
                {
                    daq.init("192.168.85.194", 8004, (ZIAPIVersion_enum)6);
                    daq.connectDevice(devstr, "PCIe", "");

                    if (client == null)
                    {
                        client = new TcpClient("192.168.85.194", 8004);
                        stream = client.GetStream();
                        sr = new StreamReader(stream);
                        stream.ReadTimeout = 100;
                    }
                    Thread.Sleep(1000);
                    connected = true;
                    sr.DiscardBufferedData();
                    sendcommand("ls" + nl); //problem
                    string str = readBack();
                    str += readBack();
                    str += readBack();

                    connected = false;

                    if (str.Contains(devstr))
                    {
                        connected = true;
                        Console.WriteLine("Connection to MFLI " + devstr + " was successful");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Connection to MFLI failed, reconnecting..." + devstr);
                        return false;
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
                    sr.Dispose();
                    client.Close();
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

        public bool sendcommand(String send)
        {
            if (connected)
            {
                send += nl;
                byte[] data = Encoding.ASCII.GetBytes(send);
                stream.Write(data, 0, data.Length); //problem****
                return true;
            }
            else
            {
                return false;
            }
        }

        public string readBack()
        {
            while (!connected)
            {
                connected = testconnection();
                return "";
            }
            string returnstring = "";
            try
            {
                string line;
                line = sr.ReadLine();
                returnstring += line + nl;

                return returnstring;
            }
            catch (IOException ee)
            {
                return returnstring;
            }
            catch (Exception ee)
            {
                Console.WriteLine("Problem reading data back from MFLI " + ee);
                return "";
            }
        }

        public string readBack_demod()
        {
            while (!connected)
            {
                connected = testconnection();
                return "";
            }
            string returnstring = "";
            string line = "";
            try
            {
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    if (line.Contains("demod"))
                        returnstring += line + nl;
                }
                return returnstring;
            }
            catch (IOException ee)
            {
                return returnstring;
            }
            catch (Exception ee)
            {
                Console.WriteLine("Problem reading data back from MFLI " + ee);
                Console.WriteLine(nl + "returnstring = " + nl + returnstring);
                return "";
            }
        }

        public double get_data(int CH)
        {
            Match match;

            if (auxmode)
            {
                sendcommand(devstr + "/auxouts/" + CH.ToString() + "/value ?");
                string str = readBack();
                match = Regex.Match(str, @" [-+]?([0-9]*\.[0-9]+|[0-9]+)", RegexOptions.IgnoreCase);
                return Convert.ToDouble(match.Value.Substring(1));
            }
            else
            {
                sr.DiscardBufferedData();
                sendcommand(devstr + "/demods/" + CH.ToString() + "/sample ?");
                string str = readBack();
                if (str.IndexOf("X=") == -1)
                    return 0;
                match = Regex.Match(str, @"X=[-+]?([0-9]*\.[0-9]+|[0-9]+)", RegexOptions.IgnoreCase);
                double X = Convert.ToDouble(match.Value.Substring(2));
                match = Regex.Match(str, @"Y=[-+]?([0-9]*\.[0-9]+|[0-9]+)", RegexOptions.IgnoreCase);
                double Y = Convert.ToDouble(match.Value.Substring(2));
                double R = Math.Sqrt(X * X + Y * Y);
                return R;
            }
        }

        public double[] get_data(int[] CH)
        {
            Match match, matchX, matchY;

            if (connected)
            {
                if (auxmode)
                {
                    sendcommand(devstr + "/auxouts/*/  /*value ?");
                    string str = readBack();
                    match = Regex.Match(str, @" [-+]?([0-9]*\.[0-9]+|[0-9]+)", RegexOptions.IgnoreCase);
                    return null;
                }
                else
                {
                    double[] R = new double[4];
                    double[] R_CH = new double[CH.Length];
                    double X, Y;
                    string matchstr;
                    sr.DiscardBufferedData(); //Discard previous data so we make sure we are getting the latest values.
                    sendcommand(devstr + "/demods/*/  /*sample ?");
                    string str = readBack();
                    if (str.IndexOf("X=") == -1)
                        return null;
                    for (int i = 0; i < 4; i++)
                    {
                        matchstr = "demods/" + i.ToString() + ".+\n";
                        match = Regex.Match(str, matchstr, RegexOptions.IgnoreCase);
                        matchX = Regex.Match(match.Value, @"X=[-+]?([0-9]*\.[0-9]+|[0-9]+)", RegexOptions.IgnoreCase);
                        X = Convert.ToDouble(matchX.Value.Substring(2));

                        matchY = Regex.Match(match.Value, @"Y=[-+]?([0-9]*\.[0-9]+|[0-9]+)", RegexOptions.IgnoreCase);
                        Y = Convert.ToDouble(matchY.Value.Substring(2));
                        R[i] = Math.Sqrt(X * X + Y * Y);
                    }
                    for (int ii = 0; ii < R_CH.Length; ii++)
                    {
                        R_CH[ii] = R[CH[ii]];
                    }
                    return R_CH;
                }
            }
            else
            {
                return null;
            }
        }
        */
    }
}
