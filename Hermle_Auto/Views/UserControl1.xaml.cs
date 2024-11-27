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
            StartPLC();


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
                MessageBox.Show("PLC 연결 실패");
            }
        }


        private void UIUnloaded(object sender, RoutedEventArgs e)
        {
            plcrunning = false;
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
                                MessageBox.Show("E.Stop was pressed");
                            }
                        }
                        else
                        {
                            Console.WriteLine($" Addr {addrs[i]} is off");
                            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                            {
                                logText.Text = $" Addr {addrs[i]} is off!";
                            }));
                        }
                    }
                    catch
                    {
                        Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            logText.Text = "PLC Fail Connect";
                            logText.Foreground = Brushes.Red;
                        }));

                        MessageBox.Show("PLC 연결 실패");
                        return;
                    }
                    
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


        private void OpenPLC_CHECK_Click(object sender, RoutedEventArgs e)
        {
            // Password.xaml 창을 불러오기
            //CommunicationWindow Window = new CommunicationWindow();
            //Window.ShowDialog(); // 모달 창으로 실행

            PLC_Monitor_Window Window = new PLC_Monitor_Window();
            Window.ShowDialog(); // 모달 창으로 실행

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
                MessageBox.Show("PLC 예외상황 : " + ex.Message);
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
                MessageBox.Show("PLC 예외상황 : " + ex.Message);
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
                MessageBox.Show("PLC 예외상황 : " + ex.Message);
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
                MessageBox.Show("PLC 예외상황 : " + ex.Message);
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
                MessageBox.Show("PLC 예외상황 : " + ex.Message);
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
                MessageBox.Show("PLC 예외상황 : " + ex.Message);
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
                MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }
        }

        // RESTOP Button Down/UP
        private void RESTOPButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {

                MessageBox.Show("Signal Set!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during MouseDown: {ex.Message}");
            }
        }
        // RESTOP Button Down/UP
        private void RESTOPButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {

                MessageBox.Show("Signal Clear!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during MouseDown: {ex.Message}");
            }
        }

        // Auto Mode 이벤트 핸들러
        private void btnAutoMode_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            if (sender is RadioButton radioButton)
            {
                radioButton.IsChecked = true; // 강제로 IsChecked 설정
            }

            try
            {

                MessageBox.Show("Auto Mode: Button Pressed!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during MouseDown: {ex.Message}");
            }
        }

        private void btnAutoMode_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            
            try
            {

                MessageBox.Show("Auto Mode: Button Released!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during MouseDown: {ex.Message}");
            }
        }

        // Semi Mode 이벤트 핸들러
        private void btnSemiMode_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                radioButton.IsChecked = true; // 강제로 IsChecked 설정
            }
            try
            {

                MessageBox.Show("Semi Mode: Button Pressed!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during MouseDown: {ex.Message}");
            }
        }

        private void btnSemiMode_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            
            try
            {

                MessageBox.Show("Semi Mode: Button Released!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during MouseDown: {ex.Message}");
            }
        }

        // Manual Mode 이벤트 핸들러
        private void btnManualMode_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                radioButton.IsChecked = true; // 강제로 IsChecked 설정
            }
            try
            {

                MessageBox.Show("Manual Mode: Button Pressed!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during MouseDown: {ex.Message}");
            }
        }

        private void btnManualMode_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {

                MessageBox.Show("Manual Mode: Button Released!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during MouseDown: {ex.Message}");
            }
           
        }
        // RESUME 버튼 이벤트 핸들러
        private void btnResume_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Resume Button Pressed!");
        }

        private void btnResume_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Resume Button Released!");
        }

        // PAUSE 버튼 이벤트 핸들러
        private void btnPause_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Pause Button Pressed!");
        }

        private void btnPause_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Pause Button Released!");
        }

        // RESET 버튼 이벤트 핸들러
        private void btnReset_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Reset Button Pressed!");
        }

        private void btnReset_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Reset Button Released!");
        }
    }

   
}
