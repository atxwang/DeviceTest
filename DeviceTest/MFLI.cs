using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


//need to implement check if data server is connected to PC (1Gbe) or MF (pcie)
namespace MFLI_namespace
{
    public class MFLI
    {
        public TcpClient client;
        public NetworkStream stream;
        public StreamReader sr;
        public bool connected;
        public string devstr;
        public bool auxmode = false;
        //public string data_controller = "MFLI";
        string nl = System.Environment.NewLine;

        public bool testconnection()
        {
            if (!connected)
            {
                try
                {
                    if (client == null)
                    {
                        client = new TcpClient("localhost", 8005);
                        stream = client.GetStream();
                        sr = new StreamReader(stream);
                        stream.ReadTimeout = 100;
                    }
                    Thread.Sleep(1000);
                    //devstr = Properties.Settings.Default.MFLI_dev;
                    //sendcommand(devstr + "/auxouts/0/value ?");
                    connected = true;
                    sr.DiscardBufferedData();
                    sendcommand("ls" + nl);
                    string str = readBack();
                    str += readBack();
                    str += readBack(); //CLEAN UP THIS CODE

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
                        //Console.WriteLine("str = {0}", str);
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
                return true;
        }

        public bool sendcommand(string send)
        {
            //while (!connected)
            //{
            //    if (Properties.Settings.Default.data_controller == "MFLI")
            //        connected = testconnection();
            //    return false;
            //}
            if (connected)
            {
                send += nl;
                byte[] data = Encoding.ASCII.GetBytes(send);
                stream.Write(data, 0, data.Length);
                return true;
            }
            else
            {
                return false;
            }
        }

        public string readBack()
        {
            //var buffer = new StringBuilder();
            while (!connected)
            {
                //if (Properties.Settings.Default.data_controller == "MFLI")
                connected = testconnection();
                return "";
            }
            string returnstring = "";
            try
            {
                //    if (sr.Peek() > -1)
                //        buffer.Append(sr.ReadLine() + nl);
                //    return buffer.ToString();
                //

                string line;
                line = sr.ReadLine();
                returnstring += line + nl;

                //while (line != "")
                //{
                //    line = sr.ReadLine();
                //}
                return returnstring;
            }
            catch (IOException ee)
            {
                //Console.WriteLine("Problem reading data back from MFLI " + ee);
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
            //var buffer = new StringBuilder();
            while (!connected)
            {
                //if (Properties.Settings.Default.data_controller == "MFLI")
                connected = testconnection();
                return "";
            }
            string returnstring = "";
            string line = "";
            try
            {
                //    if (sr.Peek() > -1)
                //        buffer.Append(sr.ReadLine() + nl);
                //    return buffer.ToString();
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
                //Console.WriteLine("Problem reading data back from MFLI " + ee);
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
            //sendcommand("cd " + devstr);
            //sendcommand("auxins/" + CH.ToString() +"/averaging 32768");
            Match match;

            if (auxmode)
            {
                sendcommand(devstr + "/auxouts/" + CH.ToString() + "/value ?");
                //sendcommand("auxins/" + CH.ToString() + "/averaging 32768");
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
                //sendcommand("unsubs demods/" + CH.ToString() + "/sample");
                return R;
            }
        }
        public double[] get_data(int[] CH)
        {
            //sendcommand("cd " + devstr);
            //sendcommand("auxins/" + CH.ToString() +"/averaging 32768");
            Match match, matchX, matchY;

            if (connected)
            {
                if (auxmode)
                {
                    sendcommand(devstr + "/auxouts/*/  /*value ?");
                    //sendcommand("auxins/" + CH.ToString() + "/averaging 32768");
                    string str = readBack();
                    match = Regex.Match(str, @" [-+]?([0-9]*\.[0-9]+|[0-9]+)", RegexOptions.IgnoreCase);
                    //return Convert.ToDouble(match.Value.Substring(1));
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

                        //str = str.Substring(matchY.Index + matchY.Length);
                    }
                    for (int ii = 0; ii < R_CH.Length; ii++)
                    {
                        R_CH[ii] = R[CH[ii]];
                    }
                    //sendcommand("unsubs demods/" + CH.ToString() + "/sample");
                    return R_CH;
                }
            }
            else
            {
                return null;
            }
        }

        /*
        public double[,] subs_data(int[] CH, double interval)
        {
            if (connected)
            {
                AlignmentHelper.CommTimer tmrComm = new AlignmentHelper.CommTimer(); //find an equivalent?
                sendcommand("subs " + devstr + "/demods/" + CH[0].ToString() + "/sample");
                tmrComm.Start(interval);
                while (tmrComm.timedout == false)
                {
                    //    Application.DoEvents();
                }
                sendcommand("unsubs " + devstr + "/demods/" + CH[0].ToString() + "/sample");
                //List<double> TS;
                //List<double> Amplitude;
                //Tuple<List<double>, List<double>> returnvalue;
                double[,] returnvalue;
                returnvalue = readBack_demod(); //why is this a double array
                return returnvalue;
            }
            else
            {
                return null;
            }
        }
        */

        //Private Sub TimerReceive_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerReceive.Tick
        //    Dim str As String
        //    str = readBack()
        //    If str.IndexOf("Ch0=") = -1 Then Return

        //    TextBoxCh0.Text = parseAux0(str).ToString
        //    TextBoxCh1.Text = parseAux1(str).ToString

        //End Sub

        //public double parseAux(string s, int CH)
        //{
        //    string toconvert;
        //    try
        //    {
        //        s = s.Replace(",", "");
        //        s = s.Replace(".", ",");
        //        toconvert = s.Substring(s.IndexOf("Ch"+CH.ToString()+"=") + 4, 9);
        //        return double.Parse(toconvert);
        //    }
        //    catch
        //    {
        //        Console.WriteLine("Problem reading data from MFLI");
        //        return 0;
        //    }
        //}
    }
}
