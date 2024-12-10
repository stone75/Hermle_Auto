using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hermle_Auto.Comm;

using MoonLib.Logger;

using Newtonsoft.Json;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hermle_Auto.Data
{   
    public enum ROBOT_STATE_ENUM    
    {
        AUTO_MODE           = 0x01,
        MANUAL_MODE         = 0x02,
        PGM_RUNNING         = 0x04,
        PGM_PAUSED          = 0x08,
        MOTION_HELD         = 0x10,
        FAULT               = 0x20,
        PEACH               = 0x40,
        TP_ENABLED          = 0x80,
        BUSY                = 0x0100,
        E_STOP              = 0x0200,
        E_STOP_REVERSE      = 0x0400,
    }

    public class RobotAxes
    {
        public int    X     { get; set; }
        public int    Y     { get; set; }
        public int    Z     { get; set; }
        public int    w     { get; set; }
        public int    p     { get; set; }
        public int    r     { get; set; }
    }

    public enum     ROBOT_AXES_ENUM
    {
        X   = 1, 
        Y   = 2, 
        Z   = 3,   
        W   = 4,
        P   = 5,
        R   = 6,
    }
    public class PocketInfo
    {
        public bool Sensor      { get; set; }
        public int  Layer       { get; set; }
        public int  Pocket      { get; set; }
        public int  Sequence    { get; set; }
        public ROBOT_AXES_ENUM  Axes        { get; set; }
    }
    public class Robot
    {
        public Dictionary<int, int>   RobotAxesDictionary { get; set; }
        public int      RobotStatus             { get; set; }       // M2300 To Integer

        public int[]    RobotLocation           { get; set; }       = new int[12];          // 2024/12/08 flagmoon, D2010 ~ D2021
        public bool     RobotLocationChanged    { get; set; }

        public Robot()
        {
            init ();
        }

        private void init ()
        {
            try
            {
                string json = File.ReadAllText (@".\CSV\robot.teaching.json");
                RobotAxesDictionary = JsonConvert.DeserializeObject<Dictionary<int, int>> (json);
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString ());
            }
        }

        // 2024/12/10 flagmoon :  Robot Teaching Data Send 
        public void SendTeachingData ()
        {
            int[]   data =  new int[1];
            int[]   data2;
            int     wait;
            try
            {
                foreach (KeyValuePair<int, int> kv in RobotAxesDictionary)
                {
                    data[0] = kv.Key;
                    CommPLC.Instance.Write ("D2022", data.Length, data);
                    data2 = CommPLC.Instance.ConvertIntToShort (kv.Value);
                    CommPLC.Instance.Write ("D2023", data2.Length, data2);

                    MoonLib.Logger.Log.Instance ().Info ($"Robot Teaching Data Send : {kv.Key.ToString ("X")}, {kv.Value.ToString ("X")}");

                    wait = 10;
                    while (wait > 0) 
                    {
                        CommPLC.Instance.Write ("D2022", data.Length, data);
                        if (data[0] == 0)
                        {
                            break;
                        }
                        wait--;
                        Thread.Sleep (10);
                    }

                    if (wait == 0)
                    {
                        Log.Instance ().Error ("Robot Teaching Data Sending Error!!!", "ERROR");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString ());
            }
        }

        public PocketInfo GetPocketInfo (int pocket)
        {
            PocketInfo  info = new PocketInfo ();

            try
            {
                info.Sensor     = ((0x8000 & pocket) == 0)  ? false : true;
                info.Layer      = (0x7000 & pocket) >> 12;
                info.Pocket     = (0x0f00 & pocket) >> 8;
                info.Sequence   = (0x00f0 & pocket) >> 4;
                info.Axes       = (ROBOT_AXES_ENUM)(0x000f & pocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString ());
            }
            return info;
        }

        public int GetPocketInfoKey (bool senser, int layer, int pocket, int sequence, ROBOT_AXES_ENUM axes)
        {
            int key = 0;

            try
            {
                if (senser == true) 
                {
                    key = 0x8000;
                }
                key         |= layer << 12;
                key         |= pocket << 8;
                key         |= sequence << 4;
                key         |= (int)(axes);
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString ());
            }

            return key;
        }
        public void BuildJson ()
        {
            string[]    all = File.ReadAllLines (@".\CSV\robot.teaching.csv");
            byte    layer;
            byte    number;
            byte    seq;
            byte    axes;
            int key;
            int value;

            RobotAxesDictionary = new Dictionary<int, int> ();
            foreach (string line in all) 
            {
                string[] sp = line.Split (",");
                string[] pocket = sp[0].Split ("번");
                
                // 234567, 8, 14
                if (pocket[1].Contains ("센서"))
                {
                    layer   = 0x08;    
                    pocket = pocket[0].Split ("-");
                    number   = byte.Parse (pocket[0]);
                    seq      = byte.Parse (pocket[1]);

                    for (int i = 1; i <=6; i++)
                    {
                        key         = (layer + 1) << 12;
                        key         |= number << 8;
                        key         |= seq << 4;
                        key         |= i;
                        value       = (int)(float.Parse (sp[i + 1]) * 1000);
                        RobotAxesDictionary.Add (key, value);
                    }
                    for (int i = 1; i <=6; i++)
                    {
                        key         = (layer + 2) << 12;
                        key         |= number << 8;
                        key         |= seq << 4;
                        key         |= i;
                        value       = (int)(float.Parse (sp[i + 1]) * 1000);
                        RobotAxesDictionary.Add (key, value);
                    }
                    for (int i = 1; i <=6; i++)
                    {
                        key         = (layer + 3) << 12;
                        key         |= number << 8;
                        key         |= seq << 4;
                        key         |= i;
                        value       = (int)(float.Parse (sp[i + 1]) * 1000);
                        RobotAxesDictionary.Add (key, value);
                    }
                }
                else
                {

                    layer   = 0x00;    
                    pocket  = pocket[0].Split ("-");
                    number   = byte.Parse (pocket[0]);
                    seq      = byte.Parse (pocket[1]);

                    for (int i = 1; i <=6; i++)
                    {
                        key         = (layer + 1) << 12;
                        key         |= number << 8;
                        key         |= seq << 4;
                        key         |= i;
                        value       = (int)(float.Parse (sp[i + 1]) * 1000);
                        RobotAxesDictionary.Add (key, value);
                    }
                    for (int i = 1; i <=6; i++)
                    {
                        key         = (layer + 2) << 12;
                        key         |= number << 8;
                        key         |= seq << 4;
                        key         |= i;
                        value       = (int)(float.Parse (sp[i + 1]) * 1000);
                        RobotAxesDictionary.Add (key, value);
                    }
                    for (int i = 1; i <=6; i++)
                    {
                        key         = (layer + 3) << 12;
                        key         |= number << 8;
                        key         |= seq << 4;
                        key         |= i;
                        value       = (int)(float.Parse (sp[i + 1]) * 1000);
                        RobotAxesDictionary.Add (key, value);
                    }
                }
            }
            string json = JsonConvert.SerializeObject (RobotAxesDictionary, Newtonsoft.Json.Formatting.Indented);

            File.WriteAllText (@".\CSV\robot.teaching" + ".json", json);
        }
    }
}
