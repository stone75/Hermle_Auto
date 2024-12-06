using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hermle_Auto.Comm;
using HermleCS.Data;

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
                        readM2080 ();
                    }

                    if (step % 50 == 30)
                    {
                        readM2300 ();

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

    }
}
