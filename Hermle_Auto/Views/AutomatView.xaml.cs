using Hermle_Auto.Comm;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using HermleCS.Data;
using HermleCS.Comm;
using System.Net.Sockets;


namespace Hermle_Auto.Views
{
    /// <summary>
    /// Interaction logic for AutomatView.xaml
    /// </summary>
    public partial class AutomatView : UserControl
    {
        public event RobotStatusLogger logger;

        public AutomatView()
        {
            InitializeComponent();

            WorkPieceToggle.MouseDown += WorkPieceToggle_MouseDown;
            OneToolToggle.MouseDown += WorkPieceToggle_MouseDown;
            NightModeToggle.MouseDown += WorkPieceToggle_MouseDown;
        }

        // 2024/12/03 flagmoon
        /// 미구현.
        private void WorkPieceToggle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ToggleButton textBox = sender as ToggleButton;


            if(textBox.Name == "WorkPiece")
            {
                C.log($"{textBox.Name} : {textBox.IsChecked}");
            }
            else if(textBox.Name == "One Tool")
            {
                C.log($"{textBox.Name} : {textBox.IsChecked}");
            }
            else if (textBox.Name == "Night Mode")
            {
                C.log($"{textBox.Name} : {textBox.IsChecked}");
            }


           
        }

        public void writelog(string msg)
        {
            string formattedTime = DateTime.Now.ToString("HH:mm:ss");
            txtRobotStatus.Text = $"{formattedTime} : {msg}\r\n" + txtRobotStatus.Text;
        }


        private void btnStartAutomat_Click(object sender, RoutedEventArgs e)
        {
            // 2024.12.05 로봇 통신을 PLC로 변경작업
            //CommPLC commplc = CommPLC.Instance;
            //D d = D.Instance;

            try
            {
                // 마지막 로봇과의 통신 시점에 전달하는 것으로 이동.
                //CommPLC.Instance.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2009, 1);
                //CommPLC.Instance.WritePLCBlock(2000, d.WorkPiecesList[d.CurrentWorkPieceIndex].ncprogram);

                // 1. 첫번째 워크피스 NC 프로그램 번호 전송 D2000
                CommPLC.Instance.WritePLCBlock(2000, D.Instance.WorkPiecesList[0].ncprogram);

                // 2. ToolType 값 전송 
                // To Do..
                int tooltype = 0;
                if (D.Instance.WorkPiecesList[0].wptooltype == "DRILL")
                {
                    tooltype = 1;
                } else if (D.Instance.WorkPiecesList[0].wptooltype == "HSK")
                {
                    tooltype = 2;
                } else if (D.Instance.WorkPiecesList[0].wptooltype == "ROUND")
                {
                    tooltype = 3;
                } else
                {
                    tooltype = 4;
                }
                CommPLC.Instance.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2009, tooltype);

                // 3. AmountLeft 값 전송
                // To Do..
                CommPLC.Instance.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2009, D.Instance.WorkPiecesList[0].toolamountleft);

                // 4. Diameter 값 전송
                // To Do..
                CommPLC.Instance.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2009, D.Instance.WorkPiecesList[0].tooldiameter);

                // 5. 도구 감지 센서 사용
                // To Do..
                CommPLC.Instance.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2009, D.Instance.WorkPiecesList[0].tooldiameter);

                // 6. ToolType 이 HSK 가 아닐 경우, Offset 전송
                if (tooltype != 2)
                {
                    string param = "";
                    param += "?AbovePocket=" + D.Instance.iniFile.offsets.AbovePocket;
                    param += "&AboveChuck=" + D.Instance.iniFile.offsets.AboveChuck;
                    param += "&ChuckStopper=" + D.Instance.iniFile.offsets.ChuckStopper;
                    param += "&ChuckDepth=" + D.Instance.iniFile.offsets.ChuckDepth;
                    param += "&PocketStopper=" + D.Instance.iniFile.offsets.PocketStopper;
                    param += "&KioskStopper=" + D.Instance.iniFile.offsets.KioskStopper;

                    CommHTTPComponent.Instance.GetAPI(C.ROBOT_SERVER + "/H_DRILL_OFST_WRITE" + param);
                }

                // 7. Speed 값 전송
                int speed = (int)speedslider.Value;
                CommPLC.Instance.WritePLCBlock(2005, speed);

                // 8. 로봇, PLC 전송
                D.Instance.CURRENT_JOBNAME = "AUTO_START_TEST";
                CommHTTPComponent.Instance.GetAPI(C.ROBOT_SERVER + "/H_COMMAND?task_str=" + D.Instance.CURRENT_JOBNAME);
                CommPLC.Instance.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2009, 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }
        }
                             
        // 2024/12/03 
        // 미구현
        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;

            //speedLabel.Content(slider.Value.ToString("F0"));
            if(speedLabel != null)
            {
                speedLabel.Content = slider.Value.ToString("F0") + "%";
            }

            //C.log($"{slider.Name} : {slider.Value}");
        }
    }
}
