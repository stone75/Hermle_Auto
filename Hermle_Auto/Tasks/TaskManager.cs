﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using Hermle_Auto.Comm;

using HermleCS.Comm;
using HermleCS.Data;

using McProtocol.Mitsubishi;

using MoonLib.Logger;

namespace Hermle_Auto.Tasks
{
    /// <summary>
    /// UserControl 에 있는 기능을 가져오기 위해 사전 생성.
    /// </summary>
    public class TaskManager : IDisposable
    {
        private readonly static TaskManager __instance = new TaskManager ();
        public Action   AlarmAction     { get; set; }           // 2024/12/06 flagmoon
        public Action   M2000Action     { get; set; }           // 2024/12/06 flagmoon
        public Action   M2300Action     { get; set; }
        public Action   RobotLocationAction     { get; set; }   // 2024/12/08 flagmoon

        private Thread?  __thread       = null;
        private bool    __threadFlag    = false;
        private bool    __disposedValue = false;

        private TaskManager () 
        {
            init ();
        }


        protected virtual void Dispose (bool disposing)
        {
            if (!__disposedValue)
            {
                if (disposing)
                {
                }

                __disposedValue     = true;
                __threadFlag        = false;
            }
        }

        public void Dispose ()
        {
            Dispose (disposing: true);
            GC.SuppressFinalize (this);
        }

        private void init ()
        {
            try
            {
                M2000Action         += M2000EventHandler;
                AlarmAction         += AlarmEventHandler;

                __thread = new Thread (new ThreadStart (svc));
                __thread.IsBackground = true;
                __thread.Start ();
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString ());
            }
        }

        private void AlarmEventHandler ()
        {
            try
            {
                // 알람처리
                //---


                D.Instance.M2080Changed     = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString ());
            }
        }

        private void M2000EventHandler ()
        {
            try
            {
                if (D.Instance.M2000[7] == 1)
                {
                    CommPLC.Instance.Set ("M2351");
                    D.Instance.SendHold = true;
                }

                if (D.Instance.M2000[20] == 1 || D.Instance.M2000[22] == 1)
                {
                    CommPLC.Instance.Set ("M2351");
                    D.Instance.SendHold = true;
                }
                
                D.Instance.M2000Changed     = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString ());
            }
        }

        public static TaskManager Instance ()
        {
            return __instance;
        }

        private void svc ()
        {
            uint step       = 0;
            
            __threadFlag    = true;


            while (__threadFlag)
            {
                try
                {
                    if (step % 2  == 0) 
                    {
                        readM2000 ();
                    }


                    if (step % 10 == 3)
                    {

                    }

                    if (step % 10 == 6)
                    {

                    }

                    if (step % 50 == 10)
                    {
                        if (CommPLC.Instance.mcProtocolTcp.Connected == false)
                        {
                            Thread thd = new Thread (async () => 
                            {
                                try
                                {
                                    CommPLC.Instance.mcProtocolTcp = new McProtocolTcp(C.PLC_IP, C.PLC_PORT, McFrame.MC3E);
                                    CommPLC.Instance.mcProtocolTcp.Open ();   
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine (ex.ToString ());
                                }
                            });
                            thd.IsBackground = true;
                            thd.Start ();
                        }
                    }
                    if (step % 50 == 10)
                    {
                        readM2080 ();
                    }

                    if (step % 50 == 20)
                    {
                        readRobotLocation ();
                    }

                    if (step % 50 == 30)
                    {
                        readM2300 ();

                    }

                    if (step % 50 == 40)
                    {
                        read2100 ();
                    }

                    // Hold 명령 전송 상태
                    if (D.Instance.SendHold == true)
                    {
                        // 로봇이 동작상태가 아니면. Hold 신호 Clear.
                        if (D.Instance.M2300[2] == 0)
                        {
                            CommPLC.Instance.Clear ("M2352");  
                            D.Instance.SendHold     = false;
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine (ex.ToString ());
                }
                finally
                {
                    Thread.Sleep (10);
                    step++;
                }
            }
        }

        private static int[] setBit (int[] data)
        {
            int     length  = (data.Length / 16 + (data.Length % 16 > 0 ? 1 : 0));
            int[]   result  = new int[length];

            try
            {
                //for (int n = 0; n < length; n++) 
                //{
                //    result[n]   = 0;
                //}
                Array.Fill (result, 0);
                
                for (int i = 0; i < data.Length; i++) 
                {
                    int idx         = (int)(i / 16);
                    int pos         = i % 16;
                    result[idx]     |= data[i] << pos;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return result;
        }

        private async void readM2000 ()
        {
            try
            {
                if (CommPLC.Instance.mcProtocolTcp.Connected == false)
                {
                    return;
                }

                int[]   data = new int[D.Instance.M2000.Length];

                await CommPLC.Instance.mcProtocolTcp.GetBitDevice ("M2000", data.Length, data);

                for (int i  = 0; i < data.Length; i++) 
                {
                    switch (i)
                    {
                        case 7 :
                        case 20 :
                        case 22 :
                        case 24 :
                        case 25 :
                        case 28 :
                            if (D.Instance.M2000[i] != data[i])
                            {
                                D.Instance.M2000[i] = data[i];
                                D.Instance.M2000Changed = true;
                            }
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString ());
            }
            finally
            {
                if (D.Instance.M2000Changed == true)
                {
                    if (M2000Action != null)
                    {
                        M2000Action.Invoke ();
                    }
                }

            }
        }

        // 2024/12/06 flagmoon Add : Alarm.
        private async void readM2080 ()
        {
            try
            {
                if (CommPLC.Instance.mcProtocolTcp.Connected == false)
                {
                    return;
                }

                int[]   data = new int[D.Instance.M2080.Length];

                await CommPLC.Instance.mcProtocolTcp.GetBitDevice ("M2080", data.Length, data);

                for (int i = 0; i < data.Length; i++)
                {
                    if (D.Instance.M2080[i] != data[i])
                    {
                        D.Instance.M2080[i]     = data[i];
                        D.Instance.M2080Changed = true;
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString ());
            }
            finally
            {
                if (D.Instance.M2080Changed == true)
                {
                    if (AlarmAction != null)
                    {
                        AlarmAction.Invoke ();
                    }
                }
            }
        }

        private async void readM2300 ()
        {
            try
            {
                if (CommPLC.Instance.mcProtocolTcp.Connected == false)
                {
                    return;
                }

                int[]   data = new int[D.Instance.M2300.Length];

                await CommPLC.Instance.mcProtocolTcp.GetBitDevice ("M2300", data.Length, data);


                for (int i = 0; i < data.Length; i++)
                {
                    if (D.Instance.M2300[i] != data[i])
                    {
                        D.Instance.M2300[i]     = data[i];
                        D.Instance.M2300Changed = true;
                    }
                }

                if (D.Instance.M2300Changed == true)
                {
                    // M2300 ~ M2315
                    data    = setBit (data);
                    D.Instance.Robot.RobotStatus = data[0];
                    //---
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString ());
            }
            finally
            {
                if (D.Instance.M2300Changed == true)
                {
                    Console.WriteLine ($"Robot => PC : {D.Instance.Robot.RobotStatus.ToString ("X")}");
                    if (M2300Action != null)
                    {
                        M2300Action.Invoke ();
                    }
                }
            }
        }

        private async void read2100 ()
        {
            try
            {
                if (CommPLC.Instance.mcProtocolTcp.Connected == false)
                {
                    return;
                }
                
                int length;
                int[]   data;
                    // M2100
                length  = D.Instance.M2100.Length;
                data    = new int[length];

                await CommPLC.Instance.mcProtocolTcp.GetBitDevice ("M2100", data.Length, data);
                for (int i = 0; i < D.Instance.M2100.Length; i++)
                {
                    if (D.Instance.M2100[i] != data[i])
                    {
                        D.Instance.M2100[i]     = data[i];    
                        D.Instance.M2100Changed = true;
                    }
                }
                //---

                // M2200
                length  = D.Instance.M2200.Length;
                data    = new int[length];

                await CommPLC.Instance.mcProtocolTcp.GetBitDevice ("M2200", data.Length, data);
                for (int i = 0; i < D.Instance.M2200.Length; i++)
                {
                    if (D.Instance.M2200[i] != data[i])
                    {
                        D.Instance.M2200[i]     = data[i];    
                        D.Instance.M2200Changed = true;

                    }
                }
                //---
 
            }
            catch (Exception ex)
            {
                MoonLib.Logger.Log.Instance ().Error (ex.ToString (), "ERROR");
                throw;
            }
        }

        /// <summary>
        /// Robot Location Read
        /// 2024/12/09
        /// </summary>
        private async void readRobotLocation ()
        {
            try
            {
                if (CommPLC.Instance.mcProtocolTcp.Connected == false)
                {
                    return;
                }
                
                int     length;
                int[]   data;
                
                length  = D.Instance.Robot.RobotLocation.Length;
                data    = new int[length];

                await CommPLC.Instance.mcProtocolTcp.ReadDeviceBlock ("D2010", data.Length, data);
                for (int i = 0; i < D.Instance.Robot.RobotLocation.Length; i++)
                {
                    if (D.Instance.Robot.RobotLocation[i] != data[i])
                    {
                        D.Instance.Robot.RobotLocation[i]     = data[i];    
                        D.Instance.Robot.RobotLocationChanged = true;
                    }
                }
                //---
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString ());
                Log.Instance ().Error (ex.ToString (), "ERROR");
            }
            finally
            {
                if (D.Instance.Robot.RobotLocationChanged == true)
                {
                    if (RobotLocationAction != null)
                    {
                        RobotLocationAction.Invoke ();
                    }
                }
            }
        }

        // 2024/12/09 flagmoon
        // Robot Location 정보 변환을 위해 추가 구현.
        public static int[] ConvertShortToInt (int[] f, int length)
        {
            int[]       result  = new int[length];
            byte[]      cvt     = new byte[4];
            byte[]      ordData = new byte[2];

            try
            {
                for (int i = 0; i < length; i++) 
                {
                    ordData     = BitConverter.GetBytes (f[i * 2]);

                    cvt[0] = ordData[0];
                    cvt[1] = ordData[1];

                    ordData     = BitConverter.GetBytes (f[i * 2 + 1]);
                    cvt[2] = ordData[0];
                    cvt[3] = ordData[1];

                    result[i]    = BitConverter.ToInt32(cvt, 0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString ());
            }
            
            return result;
        }

    }
}
