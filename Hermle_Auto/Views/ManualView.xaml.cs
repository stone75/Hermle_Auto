using HermleCS.Comm;
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

namespace Hermle_Auto.Views
{
    /// <summary>
    /// ManualView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ManualView : UserControl
    {
        private double currentStepValue = 0.1; // 기본값
        //private CommHTTPComponent httpclient = CommHTTPComponent.Instance;
        private bool isMoving = false;

    
        public ManualView()
        {
            InitializeComponent();
            // 기본값으로 0.1 선택
            var defaultButton = StepPanel.Children[0] as RadioButton;
            if (defaultButton != null)
            {
                defaultButton.IsChecked = true;
            }

            /*
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
            */
        }

        private void StepButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton && radioButton.Tag != null)
            {
                if (double.TryParse(radioButton.Tag.ToString(), out double stepValue))
                {
                    currentStepValue = stepValue;
                    // 여기서 stepValue를 사용하여 필요한 작업 수행
                }
            }
        }

        private void Jogo_Lobo_Click(object sender, RoutedEventArgs e)
        {
            //GlobalDataLists.data[1].DataValue = "a";
            Button button = sender as Button;

            //plc 체크 선행


            if (button.Content.ToString() == "X+")
            {

            }
            else if (button.Content.ToString() == "X-")
            {

            }
            else if (button.Content.ToString() == "Y-")
            {

            }
            else if (button.Content.ToString() == "Y+")
            {

            }
            else if (button.Content.ToString() == "Z-")
            {

            }
            else if (button.Content.ToString() == "Z+")
            {

            }
            else if (button.Content.ToString() == "Rx-")
            {

            }
            else if (button.Content.ToString() == "Rx+")
            {

            }
            else if (button.Content.ToString() == "Ry-")
            {

            }
            else if (button.Content.ToString() == "Ry+")
            {

            }
            else if (button.Content.ToString() == "Rz-")
            {

            }
            else if (button.Content.ToString() == "Rz+")
            {

            }

        }



        // 현재 선택된 스텝 값을 가져오는 속성
        public double CurrentStepValue
        {
            get { return currentStepValue; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
/*
            if (isMoving)
            {
                // httpclient.commandMove();
                robotcmd.Text = "AUTO_START_TEST.TP";
                string url = robotaddr.Text + robotcmd.Text;

                httpclient.commandCommand(url);
            }
            else
            {
                robotcmd.Text = "CHUCK_TO_KIOSK.TP";
                string url = robotaddr.Text + robotcmd.Text;

                httpclient.commandCommand(url);
            }

            isMoving = !isMoving;
*/
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
/*
            robotcmd.Text = "t.php";
            string url = robotaddr.Text + robotcmd.Text;

            httpclient.commandCommand(url);
*/
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
/*
            robotcmd.Text = "CHUCK_TO_POCKET.TP";
            string url = robotaddr.Text + robotcmd.Text;

            httpclient.commandCommand(url);
*/
        }

        private void CustomBtn_Click(object sender, RoutedEventArgs e)
        {
/*
            string url = robotaddr.Text + custCmd.Text;

            httpclient.commandCommand(url);
*/
        }
    }
}
