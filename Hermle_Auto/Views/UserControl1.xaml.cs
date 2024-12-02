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

public delegate void PLCCommHandler(int addr, string message);
public delegate Task PLCCommSender(McProtocolTcp conn, PlcDeviceType type, int addr, int value);

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

        private Thread plcworker;
        private bool plcrunning;




        public UserControl1()
        {
            InitializeComponent();

            try
            {
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

            StartPLC();


                Unloaded += UIUnloaded;

            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString ());
            }
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
                2106, 2107,
                2116, 2118, 2120, 2122,
                2124, 2126, 2128, 2130,
                2132, 2134, 2136, 2138,
                2146,

                2206, 2207,
                2216, 2218, 2220, 2222,
                2224, 2226, 2228, 2230,
                            2336, 2338,
                2246
            };
            int[] getData = new int[1];

            while (plcrunning)
            {

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

                Thread.Sleep(500);
            }
            
        }

        public int[] GetBit (int[] data, int length)
        {
            int[]     result  = new int[length];
            try
            {
                for (int n = 0; n < length; n++) 
                {
                    result[n]   = 0;
                }
                for (int i = 0; i < result.Length; i++) 
                {
                    int idx         = (int)(i / 16);
                    int pos         = i % 16;
                    result[i]       = (data[idx] >> pos) & 0x01;;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString ());
            }

            return result;
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
            CommPLC commplc = CommPLC.Instance;
            D d = D.Instance;

            try
            {
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2004, 1);
                commplc.WritePLCBlock(2000, d.WorkPiecesList[d.CurrentWorkPieceIndex].ncprogram);
            }
            catch (Exception ex)
            {
                MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            CommPLC commplc = CommPLC.Instance;
            D d = D.Instance;

            try
            {
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2003, 1);
                commplc.WritePLCBlock(2000, d.WorkPiecesList[d.CurrentWorkPieceIndex].ncprogram);
            }
            catch (Exception ex)
            {
                MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            CommPLC commplc = CommPLC.Instance;
            D d = D.Instance;

            try
            {
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2005, 1);
                commplc.WritePLCBlock(2000, d.WorkPiecesList[d.CurrentWorkPieceIndex].ncprogram);
            }
            catch (Exception ex)
            {
                MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }
        }

        private void btnManualMode_Checked(object sender, RoutedEventArgs e)
        {
            CommPLC commplc = CommPLC.Instance;

            try
            {
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2002, 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }
        }

        private void btnAutoMode_Checked(object sender, RoutedEventArgs e)
        {
            CommPLC commplc = CommPLC.Instance;

            try
            {
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2000, 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }
        }

        private void btnSemiMode_Checked(object sender, RoutedEventArgs e)
        {
            CommPLC commplc = CommPLC.Instance;

            try
            {
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2001, 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CommPLC commplc = CommPLC.Instance;

            try
            {
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2006, 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }
        }
    }

   
}
