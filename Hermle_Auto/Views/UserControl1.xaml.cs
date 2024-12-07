using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using McProtocol;
using McProtocol.Mitsubishi;

using HermleCS.Comm;
using Hermle_Auto.ViewModels;
using HermleCS.Data;
using System.Net.Http;
using System.Security.Policy;
using System.Windows.Threading;
using Hermle_Auto.Comm;
using System.Text.Json;
using MoonLib.Logger;

public delegate void PLCCommHandler(int addr, string message);
public delegate Task PLCCommSender(McProtocolTcp conn, PlcDeviceType type, int addr, int value);

public delegate void RobotStatusLogger(string message);

namespace Hermle_Auto.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        UserControl1ViewModel userControl1ViewModel = new UserControl1ViewModel();
        private CommHTTPComponent httpclient = CommHTTPComponent.Instance;

        private CommPLC commPLC = CommPLC.Instance;
        private McProtocolTcp mcProtocolTcp;
        public event PLCCommHandler commHandler;
        public event PLCCommSender commSender;
        public event RobotStatusLogger logger;

        private Thread plcworker;
        private bool plcrunning;

        private Thread robotworker;
        private bool robotrunning;

        

        public UserControl1()
        {
            InitializeComponent();

            init ();        // 2024/12/06 flagmoon

            this.DataContext = userControl1ViewModel;

            workPieceView.WorkPieceChanged += userControl1ViewModel.OnWorkPieceUpdate;
            //workPieceView.Visibility = Visibility.Visible;



            D.Instance.ReadIniFile();



            httpclient.MessageReceived += (addr, message) =>
            {
                string log = "Log : " + addr + " / " + message;

                if (logText.Dispatcher.CheckAccess())
                {
                    logText.Text = log;
                }
                else
                {
                    logText.Dispatcher.Invoke(() =>
                    {
                        logText.Text = log;
                    });
                }
            };

            // 2024/112/03
            // 아래로 이동.
            // mcProtocolTcp = commPLC.mcProtocolTcp;
            //commSender += WritePLC;

            //작업시 제외
            StartPLC();
            StartRobotThread();

            Unloaded += UIUnloaded;

            logger += automatView.writelog;
            logger += infoView.writelog;

            operationView.logger += automatView.writelog;
            operationView.logger += infoView.writelog;

            automatView.logger += automatView.writelog;
            automatView.logger += infoView.writelog;

            // 2024/12/03
            commPLC.mcProtocolTcp = mcProtocolTcp;
            logger += UserControl1_logger;

            //---
        }

        // 2024/12/07 flagmoon add.
        private void UserControl1_logger (string message)
        {
            Log.Instance ().Info (message);
        }

        private void init ()
        {
            try
            {
                btnComm.Visibility  = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString ());
            }
        }


        // 2024/12/06 flagmoon
#if false
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
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString ());
            }
        }
#endif
        public void StartPLC()
        {
            try
            {
                mcProtocolTcp = new McProtocolTcp(C.PLC_IP, C.PLC_PORT, McFrame.MC3E);

                mcProtocolTcp.Open();

                // 2024/12/06 flagmooon 삭제.
#if false
                plcworker = new Thread(async () => await ReadThreadHandler(mcProtocolTcp));
                plcrunning = true;
                plcworker.Start();
#else
                // TaskManager.cs 로 이동
                //plcworker = new Thread (new ThreadStart (svc));
                //plcworker.IsBackground = true;
                //plcworker.Start ();
#endif
                logText.Text = "PLC Connect";
                logText.Foreground = Brushes.Green;
             

            }
            catch (Exception ex)
            {
                logText.Text = "PLC Fail Connect";
                logText.Foreground = Brushes.Red;
                //MessageBox.Show("PLC 연결 실패");
            }
        }

        public void StartRobotThread()
        {
            try
            {
                robotworker = new Thread(async () => await RobotThreadHandler());
                robotrunning = true;
                robotworker.Start();
            }
            catch (Exception ex)
            {
                logText.Text = "Robot Thread starting exception : " + ex.Message;
            }
        }

        //종료 함수 추가
        public void CloseUserControl()
        {
            plcrunning = false;
            robotrunning = false;
        }


        private void UIUnloaded(object sender, RoutedEventArgs e)
        {
            plcrunning = false;
            robotrunning = false;
        }


        /// <summary>
        /// PLC Read Thread Service
        /// 2024/12/06  flagmoon add
        /// Moce To TaskManager class
        /// </summary>
        /// 
#if false
        private void svc ()
        {
            uint step   = 0;

            plcrunning  = true;

            while (plcrunning == true)
            {
                try
                {

                    // 20ms Task
                    if (step % 2 == 0) 
                    {
                        readM2000 ();
                    }

                    // 500ms Task
                    if (step % 50 == 5)
                    {
                        readM2080 ();
                    }

                    if (step % 50 == 15)
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
#endif

        // 2024/12/03 flagmoon
        // 이 쓰레드는 PLC Monitor Window로 이동 해야함.
        // 정보 표출 할 상태가 아닌데 계속 수행할 이유가 없슴.
        private async Task ReadThreadHandler(McProtocolTcp conn)
        {

            uint     step    = 0;
               
             int[] addrs =
             {
                2007, 
                2020, 2022, 2024, 2025, 2028,

                2106, 2107,
                2116, 2118, 2120, 2122,
                2124, 2126, 2128, 2130,
                2132, 2134, 2136, 2138,
                2146,

                2206, 2207,
                2216, 2218, 2220, 2222,
                2224, 2226, 2228, 2230,
                            2236, 2238,
                2246
            };
            int[] getData = new int[1];

            while (plcrunning)
            {
                
                for (int i = 0; i < addrs.Length; i++)
                {

                    try
                    {
                                                    
                        // 2024/12/03
                        if (conn.Connected == false)
                        {
                            continue;
                        }
                        //---

                        await conn.GetBitDevice(PlcDeviceType.M, addrs[i], 1, getData);

                        if (getData[0] == 1)
                        {
                            Console.WriteLine($" Addr {addrs[i]} is onnnn!");
                            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                            {
                                logText.Text = $" Addr {addrs[i]} is on!";
                            }));

                            if (addrs[i] == 2007)
                            {
                                //MessageBox.Show("E.Stop was pressed");
                            }

                            if(plcMointerWindow != null)
                            {
                                plcMointerWindow.SetLampState(addrs[i], true);
                            }
                        }
                        else
                        {
                            Console.WriteLine($" Addr {addrs[i]} is off");
                            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                            {
                                logText.Text = $" Addr {addrs[i]} is off!";
                            }));

                            if (plcMointerWindow != null)
                            {
                                plcMointerWindow.SetLampState(addrs[i], false);
                            }
                        }

                    }
                    catch
                    {
                        Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            logText.Text = "PLC Fail Connect";
                            logText.Foreground = Brushes.Red;
                        }));

                        //MessageBox.Show("PLC 연결 실패");
                        // 2024/12/03 flagmoon 코멘트처리 함.
                        // return;
                        //---
                    }
                    
                }

#if false
                // 2024/11/19 flagmoon
                if (conn.Connected == true)
                {
                    int length      = 2100 - 2080;
                    int len         = (length % 16) + (length % 16) > 0 ? 1 : 0;
                    int[]   data    = new int[len];
                    int[]   temp;

                    // M2080
                    await conn.ReadDeviceBlock ("M2080", data.Length, data);

                    temp    = GetBit (data, length);

                    for (int i = 0; i < D.Instance.M2080.Length; i++)
                    {
                        D.Instance.M2080[i]     = temp[i];    
                    }
                    //---

                    // M2000
                    length  = 2040 - 2000;
                    len     = (length % 16) + (length % 16) > 0 ? 1 : 0;
                    data    = new int[len];

                    await conn.ReadDeviceBlock ("M2000", data.Length, data);
                    temp    = GetBit (data, length);
                    for (int i = 0; i < D.Instance.M2000.Length; i++)
                    {
                        D.Instance.M2000[i]     = temp[i];    
                    }
                    //---

                    // M2100
                    length  = D.Instance.M2100.Length;
                    len     = (length % 16) + (length % 16) > 0 ? 1 : 0;
                    data    = new int[len];

                    await conn.ReadDeviceBlock ("M2100", data.Length, data);
                    temp        = GetBit (data, length);
                    for (int i = 0; i < D.Instance.M2100.Length; i++)
                    {
                        if (D.Instance.M2100[i] != temp[i])
                        {
                            D.Instance.M2100[i]     = temp[i];    
                            D.Instance.M2100Changed = true;
                        }
                    }
                    //---

                    // M2200
                    length  = D.Instance.M2200.Length;
                    len     = (length % 16) + (length % 16) > 0 ? 1 : 0;
                    data    = new int[len];

                    await conn.ReadDeviceBlock ("M2200", data.Length, data);
                    temp        = GetBit (data, length);
                    for (int i = 0; i < D.Instance.M2200.Length; i++)
                    {
                        if (D.Instance.M2200[i] != temp[i])
                        {
                            D.Instance.M2200[i]     = temp[i];    
                            D.Instance.M2200Changed = true;
                        }
                    }
                    //---
                }
                //---
#endif

                Thread.Sleep(500);
            }
            
        }

        // Moce To TaskManager
#if false
        // 2024/12/06 flagmoon Add
        private async void readM2000 ()
        {
            try
            {
                if (mcProtocolTcp.Connected == false)
                {
                    return;
                }

                int[]   data = new int[D.Instance.M2000.Length];

                await mcProtocolTcp.GetBitDevice ("M2000", data.Length, data);

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
                if (mcProtocolTcp.Connected == false)
                {
                    return;
                }

                int[]   data = new int[D.Instance.M2080.Length];

                await mcProtocolTcp.GetBitDevice ("M2080", data.Length, data);

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
                if (mcProtocolTcp.Connected == false)
                {
                    return;
                }

                int[]   data = new int[D.Instance.M2300.Length];

                await mcProtocolTcp.GetBitDevice ("M2300", data.Length, data);

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
#endif

        private async Task RobotThreadHandler()
        {
            CommHTTPComponent http = CommHTTPComponent.Instance;
            string res;

            while (robotrunning)
            {
                try
                {
                    //C.log(C.ROBOT_SERVER + "/H_LOCATION");
                    res = http.GetAPI(C.ROBOT_SERVER + "/H_LOCATION");
                    //res = http.GetAPI("http://t.odinox.com/position.php");

                    using var document = JsonDocument.Parse(res,
                        new JsonDocumentOptions
                        {
                            AllowTrailingCommas = true // Tailing Comma 허용
                        }
                    );

                    var root = document.RootElement;

                    int result = root.GetProperty("result").GetInt32();
                    string msg = root.GetProperty("msg").GetString();
                    if (result == 0)
                    {
                        userControl1ViewModel.ValueX = root.GetProperty("location").GetProperty("x").GetDouble().ToString("F3");
                        userControl1ViewModel.ValueY = root.GetProperty("location").GetProperty("y").GetDouble().ToString("F3");
                        userControl1ViewModel.ValueZ = root.GetProperty("location").GetProperty("z").GetDouble().ToString("F3");
                        userControl1ViewModel.ValueRx = root.GetProperty("location").GetProperty("rx").GetDouble().ToString("F3");
                        userControl1ViewModel.ValueRy = root.GetProperty("location").GetProperty("ry").GetDouble().ToString("F3");
                        userControl1ViewModel.ValueRz = root.GetProperty("location").GetProperty("rz").GetDouble().ToString("F3");
                    }
                }
                catch (Exception e)
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        logText.Text = "Robot HTTP Comm. Exception : " + e.Message ;
                        logText.Foreground = Brushes.Yellow;
                    }));
                }

                Thread.Sleep(500);
            }

        }


        // 2024/12/07 flagmoon
        public void M2300EventHandler ()
        {
            try
            {
                if (D.Instance.M2300[0] == 1)
                {
                    userControl1ViewModel.ValueKeyState     = "AUTO";          
                }

                if (D.Instance.M2300[1] == 1)
                {
                    userControl1ViewModel.ValueKeyState     = "MANUAL";          
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                D.Instance.M2300Changed     = false;
            }


        }

        private void OpenPasswordWindow_Click(object sender, RoutedEventArgs e)
        {
            // Password.xaml 창을 불러오기
            PassDlg passwordWindow = new PassDlg();
            passwordWindow.ShowDialog(); // 모달 창으로 실행
            
        }

        private void OpenCommunicationWindow_Click(object sender, RoutedEventArgs e)
        {
            // Password.xaml 창을 불러오기
            CommunicationWindow Window = new CommunicationWindow();

            Window.callPLCConnect += StartPLC;

            Window.ShowDialog(); // 모달 창으로 실행
            
        }


        PLC_Monitor_Window plcMointerWindow;

        private void OpenPLC_CHECK_Click(object sender, RoutedEventArgs e)
        {
            // Password.xaml 창을 불러오기
            //CommunicationWindow Window = new CommunicationWindow();
            //Window.ShowDialog(); // 모달 창으로 실행

            var result = plcMointerWindow = new PLC_Monitor_Window();

            //Delegatet설정

            plcMointerWindow.ShowDialog(); // 모달 창으로 실행

            plcMointerWindow.Closed += (s, args) =>
            {
                plcMointerWindow = null; // 창이 닫힐 때 변수 초기화
            };


            /*// Check the result and reset the variable
            if (result == DialogResult.OK || result == DialogResult.Cancel)
            {
                someVariable = null; // Set the variable to null when dialog closes
                MessageBox.Show("Variable is reset to null.");
            }*/



        }

       
        private void btnResume_Click(object sender, RoutedEventArgs e)
        {
            CommHTTPComponent http = CommHTTPComponent.Instance;
            CommPLC commplc = CommPLC.Instance;
            D d = D.Instance;

            logger?.Invoke("Resume Button is Clicked...");
            try
            {
                /*
                 * Operation Tab 에서 셋팅한 d.CURRENT_JOBNAME 실행
                 */
                logger?.Invoke($"[Robot] Resume : " + C.ROBOT_SERVER + "/H_RESUME?task_str=" + d.CURRENT_JOBNAME);
                C.log(C.ROBOT_SERVER + "/H_RESUME?task_str=" + d.CURRENT_JOBNAME);
                string res = http.GetAPI(C.ROBOT_SERVER + "/H_RESUME?task_str=" + d.CURRENT_JOBNAME);
                HTTPResponse httpresponse = JsonSerializer.Deserialize<HTTPResponse>(res);
                if (httpresponse.result == 0)
                {
                    logger?.Invoke("[Robot] Resume API Success...");
                } else 
                {
                    logger?.Invoke("HTTP Response MSG : " + httpresponse.msg);
                    return;
                }


                logger?.Invoke("[PLC] M2004 set 1");
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2004, 1);
                
                logger?.Invoke("[PLC] D2000 set " + d.WorkPiecesList[d.CurrentWorkPieceIndex].ncprogram);
                commplc.WritePLCBlock(2000, d.WorkPiecesList[d.CurrentWorkPieceIndex].ncprogram);
            }
            catch (Exception ex)
            {
                logger?.Invoke("예외상황 : " + ex.Message);
                //MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            CommHTTPComponent http = CommHTTPComponent.Instance;
            CommPLC commplc = CommPLC.Instance;
            D d = D.Instance;

            logger?.Invoke("Pause Button is Clicked...");
            try
            {
                /*
                 * Operation Tab 에서 셋팅한 d.CURRENT_JOBNAME PAUSE
                 */
                logger?.Invoke($"[Robot] Resume : " + C.ROBOT_SERVER + "/H_PAUSE?task_str=" + d.CURRENT_JOBNAME);
                C.log(C.ROBOT_SERVER + "/H_PAUSE?task_str=" + d.CURRENT_JOBNAME);
                string res = http.GetAPI(C.ROBOT_SERVER + "/H_PAUSE?task_str=" + d.CURRENT_JOBNAME);
                HTTPResponse httpresponse = JsonSerializer.Deserialize<HTTPResponse>(res);
                if (httpresponse.result == 0)
                {
                    logger?.Invoke("[Robot] Pause API Success...");
                }
                else
                {
                    logger?.Invoke("HTTP Response MSG : " + httpresponse.msg);
                    return;
                }


                logger?.Invoke("[PLC] M2003 set 1");
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2003, 1);

                logger?.Invoke("[PLC] D2000 set " + d.WorkPiecesList[d.CurrentWorkPieceIndex].ncprogram);
                commplc.WritePLCBlock(2000, d.WorkPiecesList[d.CurrentWorkPieceIndex].ncprogram);
            }
            catch (Exception ex)
            {
                logger?.Invoke("예외상황 : " + ex.Message);
                //MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            CommHTTPComponent http = CommHTTPComponent.Instance;
            CommPLC commplc = CommPLC.Instance;
            D d = D.Instance;

            logger?.Invoke("Reset Button is Clicked...");
            try
            {
                /*
                 * Operation Reset 은 별도의 Current Jobname 없이 진행
                 */
                logger?.Invoke($"[Robot] Resume : " + C.ROBOT_SERVER + "/H_RESET");
                C.log(C.ROBOT_SERVER + "/H_RESET");
                string res = http.GetAPI(C.ROBOT_SERVER + "/H_RESET");
                HTTPResponse httpresponse = JsonSerializer.Deserialize<HTTPResponse>(res);
                if (httpresponse.result == 0)
                {
                    logger?.Invoke("[Robot] Reset API Success...");
                }
                else
                {
                    logger?.Invoke("HTTP Response MSG : " + httpresponse.msg);
                    return;
                }


                logger?.Invoke("[PLC] M2005 set 1");
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2005, 1);

                //logger?.Invoke("[PLC] D2000 set " + d.WorkPiecesList[d.CurrentWorkPieceIndex].ncprogram);
                //commplc.WritePLCBlock(2000, d.WorkPiecesList[d.CurrentWorkPieceIndex].ncprogram);
            }
            catch (Exception ex)
            {
                logger?.Invoke("예외상황 : " + ex.Message);
                //MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }
        }

        private void btnManualMode_Checked(object sender, RoutedEventArgs e)
        {
            CommHTTPComponent http = CommHTTPComponent.Instance;
            CommPLC commplc = CommPLC.Instance;
            D d = D.Instance;

            /*
             * @To Do... 
             * Operation Tab 의 UI 변경 작업 필요
             * Operation Tab 의 모든 UI Disable
             *  - SemiAutomat Group Disable
             *  - Manual Operation Group Enable
             */

            logger?.Invoke("Manual mode...");
            try
            {
                logger?.Invoke("[PLC] 2002 set 1");
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2002, 1);
            }
            catch (Exception ex)
            {
                logger?.Invoke("예외상황 : " + ex.Message);
                //MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }
        }

        private void btnAutoMode_Checked(object sender, RoutedEventArgs e)
        {
            CommHTTPComponent http = CommHTTPComponent.Instance;
            CommPLC commplc = CommPLC.Instance;
            D d = D.Instance;

            /*
             * @To Do... 
             * Operation Tab 의 UI 변경 작업 필요
             * Operation Tab 의 모든 UI Disable
             *  - SemiAutomat Group Disable
             *  - Manual Operation Group Disable
             */

            logger?.Invoke("Auto mode...");
            try
            {
                logger?.Invoke("[PLC] 2000 set 1");
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2000, 1);
            }
            catch (Exception ex)
            {
                logger?.Invoke("예외상황 : " + ex.Message);
                //MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }
        }

        private void btnSemiMode_Checked(object sender, RoutedEventArgs e)
        {
            CommHTTPComponent http = CommHTTPComponent.Instance;
            CommPLC commplc = CommPLC.Instance;
            D d = D.Instance;

            /*
             * @To Do... 
             * Operation Tab 의 UI 변경 작업 필요
             * Operation Tab 의 모든 UI Disable
             *  - SemiAutomat Group Enable
             *  - Manual Operation Group Disable
             */

            logger?.Invoke("Semi Auto mode...");
            try
            {
                logger?.Invoke("[PLC] 2001 set 1");
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2001, 1);
            }
            catch (Exception ex)
            {
                logger?.Invoke("예외상황 : " + ex.Message);
                //MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }
        }

        
        //RESTOP Click Event 
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CommHTTPComponent http = CommHTTPComponent.Instance;
            CommPLC commplc = CommPLC.Instance;
            D d = D.Instance;

            logger?.Invoke("R.E.Stop Button is Clicked...");
            try
            {
                logger?.Invoke("[PLC] 2006 set 1");
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2006, 1);
            }
            catch (Exception ex)
            {
                logger?.Invoke("예외상황 : " + ex.Message);
                //MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }
        }

        // RESTOP Button Down/UP
        private void RESTOPButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //CommHTTPComponent http = CommHTTPComponent.Instance;
            //D d = D.Instance;
            CommPLC commplc = CommPLC.Instance;

            logger?.Invoke("R.E.Stop Button is Set...");
            try
            {
                logger?.Invoke("[PLC] 2006 set 1");
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2006, 1);
            }
            catch (Exception ex)
            {
                logger?.Invoke("예외상황 : " + ex.Message);
                //MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }

            /*
            try
            {

                MessageBox.Show("Signal Set!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during MouseDown: {ex.Message}");
            }
            */
            }
        // RESTOP Button Down/UP
        private void RESTOPButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            CommPLC commplc = CommPLC.Instance;

            logger?.Invoke("R.E.Stop Button is Cleared...");
            try
            {
                logger?.Invoke("[PLC] 2006 set 1");
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2006, 0);
            }
            catch (Exception ex)
            {
                logger?.Invoke("예외상황 : " + ex.Message);
                //MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }

            /*
            try
            {

                MessageBox.Show("Signal Clear!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during MouseDown: {ex.Message}");
            }
            */
        }

        // Auto Mode 이벤트 핸들러
        private void btnAutoMode_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            if (sender is RadioButton radioButton)
            {
                radioButton.IsChecked = true; // 강제로 IsChecked 설정
            }

            CommHTTPComponent http = CommHTTPComponent.Instance;
            CommPLC commplc = CommPLC.Instance;

            logger?.Invoke("Auto mode...");
            try
            {
                logger?.Invoke("[PLC] 2000 set 1");
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2000, 1);
            }
            catch (Exception ex)
            {
                logger?.Invoke("예외상황 : " + ex.Message);
                //MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }

            /*
            try
            {

                MessageBox.Show("Auto Mode: Button Pressed!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during MouseDown: {ex.Message}");
            }
            */
        }

        private void btnAutoMode_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            CommPLC commplc = CommPLC.Instance;

            operationView.setMode(2);
            logger?.Invoke("Auto mode...");
            try
            {
                logger?.Invoke("[PLC] 2000 is cleared");
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2000, 0);
            }
            catch (Exception ex)
            {
                logger?.Invoke("예외상황 : " + ex.Message);
                //MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }

            /*
            try
            {

                MessageBox.Show("Auto Mode: Button Released!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during MouseDown: {ex.Message}");
            }
            */
        }

        // Semi Mode 이벤트 핸들러
        private void btnSemiMode_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                radioButton.IsChecked = true; // 강제로 IsChecked 설정
            }

            CommPLC commplc = CommPLC.Instance;

            logger?.Invoke("Semi Auto mode...");
            try
            {
                logger?.Invoke("[PLC] 2001 set 1");
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2001, 1);
            }
            catch (Exception ex)
            {
                logger?.Invoke("예외상황 : " + ex.Message);
                //MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }
            /*
            try
            {

                MessageBox.Show("Semi Mode: Button Pressed!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during MouseDown: {ex.Message}");
            }
            */
        }

        private void btnSemiMode_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            CommPLC commplc = CommPLC.Instance;

            operationView.setMode(0);
            logger?.Invoke("Semi Auto mode Cleared...");
            try
            {
                logger?.Invoke("[PLC] 2001 set 0");
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2001, 0);
            }
            catch (Exception ex)
            {
                logger?.Invoke("예외상황 : " + ex.Message);
                //MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }

            /*
            try
            {
                MessageBox.Show("Semi Mode: Button Released!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during MouseDown: {ex.Message}");
            }
            */
        }

        // Manual Mode 이벤트 핸들러
        private void btnManualMode_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                radioButton.IsChecked = true; // 강제로 IsChecked 설정
            }

            CommHTTPComponent http = CommHTTPComponent.Instance;
            CommPLC commplc = CommPLC.Instance;
            D d = D.Instance;

            logger?.Invoke("Manual mode is set 1...");
            try
            {
                logger?.Invoke("[PLC] 2002 set 1");
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2002, 1);
            }
            catch (Exception ex)
            {
                logger?.Invoke("예외상황 : " + ex.Message);
                //MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }

            /*
            try
            {

                MessageBox.Show("Manual Mode: Button Pressed!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during MouseDown: {ex.Message}");
            }
            */
        }

        private void btnManualMode_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            CommHTTPComponent http = CommHTTPComponent.Instance;
            CommPLC commplc = CommPLC.Instance;
            D d = D.Instance;

            operationView.setMode(1);
            logger?.Invoke("Manual mode 2002 is released...");
            try
            {
                logger?.Invoke("[PLC] 2002 set 1");
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2002, 0);
            }
            catch (Exception ex)
            {
                logger?.Invoke("예외상황 : " + ex.Message);
                //MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }


            /*
            try
            {

                MessageBox.Show("Manual Mode: Button Released!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during MouseDown: {ex.Message}");
            }
            */
        }
        // RESUME 버튼 이벤트 핸들러
        private void btnResume_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            CommHTTPComponent http = CommHTTPComponent.Instance;
            CommPLC commplc = CommPLC.Instance;
            D d = D.Instance;

            logger?.Invoke("Resume Button is Clicked...");
            try
            {
                logger?.Invoke($"[Robot] Resume : " + C.ROBOT_SERVER + "/H_RESUME?task_str=" + d.CURRENT_JOBNAME);
                C.log(C.ROBOT_SERVER + "/H_RESUME?task_str=" + d.CURRENT_JOBNAME);
                string res = http.GetAPI(C.ROBOT_SERVER + "/H_RESUME?task_str=" + d.CURRENT_JOBNAME);
                HTTPResponse httpresponse = JsonSerializer.Deserialize<HTTPResponse>(res);
                if (httpresponse.result == 0)
                {
                    logger?.Invoke("[Robot] Resume API Success...");
                }
                else
                {
                    logger?.Invoke("HTTP Response MSG : " + httpresponse.msg);
                    return;
                }


                logger?.Invoke("[PLC] M2004 set 1");
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2004, 1);

                logger?.Invoke("[PLC] D2000 set " + d.WorkPiecesList[d.CurrentWorkPieceIndex].ncprogram);
                commplc.WritePLCBlock(2000, d.WorkPiecesList[d.CurrentWorkPieceIndex].ncprogram);
            }
            catch (Exception ex)
            {
                logger?.Invoke("예외상황 : " + ex.Message);
                //MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }

            //MessageBox.Show("Resume Button Pressed!");
        }

        private void btnResume_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            CommHTTPComponent http = CommHTTPComponent.Instance;
            CommPLC commplc = CommPLC.Instance;
            D d = D.Instance;

            logger?.Invoke("Resume Button is Cleared...");
            try
            {
                logger?.Invoke("[PLC] M2004 set 0");
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2004, 0);

                logger?.Invoke("[PLC] D2000 set 0");
                commplc.WritePLCBlock(2000, 0);
            }
            catch (Exception ex)
            {
                logger?.Invoke("예외상황 : " + ex.Message);
                //MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }


            //MessageBox.Show("Resume Button Released!");
        }

        // PAUSE 버튼 이벤트 핸들러
        private void btnPause_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            CommHTTPComponent http = CommHTTPComponent.Instance;
            CommPLC commplc = CommPLC.Instance;
            D d = D.Instance;

            logger?.Invoke("Pause Button is Clicked...");
            try
            {
                // Test
                /*
                string res_t = http.GetAPI("http://t.odinox.com/position.php");
                HTTPResponse httpresponse_t = JsonSerializer.Deserialize<HTTPResponse>(res_t);
                if (httpresponse_t.result == 0)
                {
                    logger?.Invoke("[Robot] Reset API Success...");
                }
                else
                {
                    logger?.Invoke("HTTP Response MSG : " + httpresponse_t.msg);
                    return;
                }

                return;
                */
                logger?.Invoke($"[Robot] Resume : " + C.ROBOT_SERVER + "/H_PAUSE?task_str=" + d.CURRENT_JOBNAME);
                C.log(C.ROBOT_SERVER + "/H_PAUSE?task_str=" + d.CURRENT_JOBNAME);
                string res = http.GetAPI(C.ROBOT_SERVER + "/H_PAUSE?task_str=" + d.CURRENT_JOBNAME);
                HTTPResponse httpresponse = JsonSerializer.Deserialize<HTTPResponse>(res);
                if (httpresponse.result == 0)
                {
                    logger?.Invoke("[Robot] Pause API Success...");
                }
                else
                {
                    logger?.Invoke("HTTP Response MSG : " + httpresponse.msg);
                    return;
                }

                logger?.Invoke("[PLC] M2003 set 1");
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2003, 1);

                logger?.Invoke("[PLC] D2000 set " + d.WorkPiecesList[d.CurrentWorkPieceIndex].ncprogram);
                commplc.WritePLCBlock(2000, d.WorkPiecesList[d.CurrentWorkPieceIndex].ncprogram);
            }
            catch (Exception ex)
            {
                logger?.Invoke("예외상황 : " + ex.Message);
                //MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }

            //MessageBox.Show("Pause Button Pressed!");
        }

        private void btnPause_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            CommHTTPComponent http = CommHTTPComponent.Instance;
            CommPLC commplc = CommPLC.Instance;
            D d = D.Instance;

            logger?.Invoke("Pause Button is Clicked...");
            try
            {
                logger?.Invoke("[PLC] M2003 set 0");
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2003, 0);

                logger?.Invoke("[PLC] D2000 set 0");
                commplc.WritePLCBlock(2000, 0);
            }
            catch (Exception ex)
            {
                logger?.Invoke("예외상황 : " + ex.Message);
                //MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }


            //MessageBox.Show("Pause Button Released!");
        }

        // RESET 버튼 이벤트 핸들러
        private void btnReset_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            CommHTTPComponent http = CommHTTPComponent.Instance;
            CommPLC commplc = CommPLC.Instance;
            D d = D.Instance;

            logger?.Invoke("Reset Button is Set...");
            try
            {
                logger?.Invoke($"[Robot] Resume : " + C.ROBOT_SERVER + "/H_RESET");
                C.log(C.ROBOT_SERVER + "/H_RESET");
                string res = http.GetAPI(C.ROBOT_SERVER + "/H_RESET");
                HTTPResponse httpresponse = JsonSerializer.Deserialize<HTTPResponse>(res);
                if (httpresponse.result == 0)
                {
                    logger?.Invoke("[Robot] Reset API Success...");
                }
                else
                {
                    logger?.Invoke("HTTP Response MSG : " + httpresponse.msg);
                    return;
                }


                logger?.Invoke("[PLC] M2005 set 1");
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2005, 1);
            }
            catch (Exception ex)
            {
                logger?.Invoke("예외상황 : " + ex.Message);
                //MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }


            //MessageBox.Show("Reset Button Pressed!");
        }

        private void btnReset_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            CommPLC commplc = CommPLC.Instance;

            logger?.Invoke("Reset Button is Cleared...");
            try
            {
                logger?.Invoke("[PLC] M2005 clear 0");
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2005, 0);
            }
            catch (Exception ex)
            {
                logger?.Invoke("예외상황 : " + ex.Message);
                //MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }


            //MessageBox.Show("Reset Button Released!");
        }
    }   
}
