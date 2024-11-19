using McProtocol.Mitsubishi;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Hermle_Auto
{
    /// <summary>
    /// CommunacationWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommunicationWindow : Window
    {
        private McProtocolTcp mcProtocolTcp;

        public delegate void CallPLCConnect();
        public event CallPLCConnect callPLCConnect;

        public CommunicationWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            putLog("McProtocol Init...");

            try
            {
                mcProtocolTcp = new McProtocolTcp();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void putLog(string message)
        {
            // RichTextBox의 Document에 있는 내용을 지우기
            richTextBox.Document.Blocks.Clear();


            // TextRange를 사용하여 시작과 끝 위치 지정 후, 메시지를 설정
            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            textRange.Text = message;
        }
     
        
        private async void Button_Connect_Click(object sender, RoutedEventArgs e)
        {

            int port = 0;
            string ip = textIp.Text.ToString();
            string portStr = textPort.Text.ToString();

            if (portStr.Length > 0)
            {
                port = int.Parse(portStr);
            }

            try
            {
                mcProtocolTcp = new McProtocolTcp(ip, port, McFrame.MC3E);

                int isConnected = await mcProtocolTcp.Open();

                if (isConnected == 0)
                {

                    putLog("연결 성공");

                }
                else
                {
                    putLog("연결 실패");
                }

            }
            catch (Exception ex)
            {
                putLog(ex.ToString());
            }


        }

        private async void Button_AUTOStart_Click(object sender, RoutedEventArgs e)
        {

            putLog("M2000 - AUTO MODE");

            int write_addr = 2000;
            int write_value = 1;

            int[] writebuf = new int[1];
            writebuf[0] = write_value;

            await mcProtocolTcp.SetBitDevice(PlcDeviceType.M, write_addr, 1, writebuf);

        }

        private async void Button_AUTOStop_Click(object sender, RoutedEventArgs e)
        {
            putLog("M2000 - Auto Stop");

            int write_addr = 2000;
            int write_value = 0;

            int[] writebuf = new int[1];
            writebuf[0] = write_value;

            await mcProtocolTcp.SetBitDevice(PlcDeviceType.M, write_addr, 1, writebuf);
        }

        private async void Button_AUTOPause_Click(object sender, RoutedEventArgs e)
        {
            putLog("M2003 - PAUSE");

            int write_addr = 2003;
            int write_value = 1;

            int[] writebuf = new int[1];
            writebuf[0] = write_value;

            await mcProtocolTcp.SetBitDevice(PlcDeviceType.M, write_addr, 1, writebuf);

        }

        private async void Button_AUTOResume_Click(object sender, RoutedEventArgs e)
        {
            putLog("M2004 - RESUME");

            int write_addr = 2004;
            int write_value = 1;

            int[] writebuf = new int[1];
            writebuf[0] = write_value;

            await mcProtocolTcp.SetBitDevice(PlcDeviceType.M, write_addr, 1, writebuf);

        }



        private void Button_PLCConnect_Click(object sender, RoutedEventArgs e)
        {
            callPLCConnect?.Invoke();
        }


        private bool CheckPLCActiv()
        {
            bool check = false;
            //PLC 체크 부분 추가

            if (check)
            {
                return true;
            }
            else
            {
                MessageBox.Show("PLC가 동작하지않습다.");
                return false;
            }


        }

    }
}
