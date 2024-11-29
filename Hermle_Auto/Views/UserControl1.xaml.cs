﻿using System;
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

            mcProtocolTcp = commPLC.mcProtocolTcp;
            //commSender += WritePLC;

            //작업시 제외
            //StartPLC();
            //StartRobotThread();

            Unloaded += UIUnloaded;

            logger += automatView.writelog;
            logger += infoView.writelog;

            operationView.logger += automatView.writelog;
            operationView.logger += infoView.writelog;

            automatView.logger += automatView.writelog;
            automatView.logger += infoView.writelog;

        }

        public void StartPLC()
        {
            try
            {
                mcProtocolTcp = new McProtocolTcp(C.PLC_IP, C.PLC_PORT, McFrame.MC3E);

                mcProtocolTcp.Open();

                plcworker = new Thread(async () => await ReadThreadHandler(mcProtocolTcp));
                plcrunning = true;
                plcworker.Start();

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


        private void UIUnloaded(object sender, RoutedEventArgs e)
        {
            plcrunning = false;
            robotrunning = false;
        }

        private async Task ReadThreadHandler(McProtocolTcp conn)
        {
               
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
                        return;
                    }
                    
                }
                Thread.Sleep(500);
            }
            
        }

        private async Task RobotThreadHandler()
        {
            CommHTTPComponent http = CommHTTPComponent.Instance;
            string res;

            while (robotrunning)
            {
                try
                {
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnResume_Click(object sender, RoutedEventArgs e)
        {
            CommHTTPComponent http = CommHTTPComponent.Instance;
            CommPLC commplc = CommPLC.Instance;
            D d = D.Instance;

            logger?.Invoke("Resume Button is Clicked...");
            try
            {
                logger?.Invoke($"[Robot] Resume : " + C.ROBOT_SERVER + "/H_RESUME?task_str=" + d.CURRENT_JOBNAME);
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
                logger?.Invoke($"[Robot] Resume : " + C.ROBOT_SERVER + "/H_PAUSE?task_str=" + d.CURRENT_JOBNAME);
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
                logger?.Invoke($"[Robot] Resume : " + C.ROBOT_SERVER + "/H_RESET");
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
