using HermleCS.Comm;
using HermleCS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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
using static System.Net.WebRequestMethods;

namespace Hermle_Auto.Views
{
    /// <summary>
    /// OperationView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class OperationView : UserControl
    {
        public event RobotStatusLogger logger;

        public OperationView()
        {
            InitializeComponent();
        }

        private void btnParkingPosition_Click(object sender, RoutedEventArgs e)
        {
            logger?.Invoke("Parking Position Button Clicked...");

            D d = D.Instance;
            CommHTTPComponent http = CommHTTPComponent.Instance;

            // 1. TP 파일 이름 셋팅
            //d.CURRENT_JOBNAME = "PARKING";

            string res;

            try
            {
                d.CURRENT_JOBNAME = "PARKING";
                res = http.GetAPI(C.ROBOT_SERVER + "/H_COMMAND?task_str=" + d.CURRENT_JOBNAME);

                HTTPResponse httpresponse = JsonSerializer.Deserialize<HTTPResponse>(res);
                if (httpresponse.result != 0)
                {
                    MessageBox.Show("Command Error : " + httpresponse.msg);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Robot HTTP Communication Exception : " + ex.Message);
            }
        }
        private void btnExchangeGripperPosition_Click(object sender, RoutedEventArgs e)
        {

            logger?.Invoke("Exchange Gripper Position Button Clicked...");

            D d = D.Instance;
            CommHTTPComponent http = CommHTTPComponent.Instance;

            // 1. TP 파일 이름 셋팅
            //d.CURRENT_JOBNAME = "EXCHANGE_GRIPPER";

            string res;

            try
            {
                d.CURRENT_JOBNAME = "EXCHANGE_GRIPPER";
                res = http.GetAPI(C.ROBOT_SERVER + "/H_COMMAND?task_str=" + d.CURRENT_JOBNAME);

                HTTPResponse httpresponse = JsonSerializer.Deserialize<HTTPResponse>(res);
                if (httpresponse.result != 0)
                {
                    MessageBox.Show("Command Error : " + httpresponse.msg);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Robot HTTP Communication Exception : " + ex.Message);
            }
        }
        private void btnRetractPosition_Click(object sender, RoutedEventArgs e)
        {
            logger?.Invoke("Retract Position Button Clicked...");

            D d = D.Instance;
            CommHTTPComponent http = CommHTTPComponent.Instance;

            // 1. TP 파일 이름 셋팅
            //d.CURRENT_JOBNAME = "CURRENT_RETRACT";

            string res;

            try
            {
                d.CURRENT_JOBNAME = "CURRENT_RETRACT";
                res = http.GetAPI(C.ROBOT_SERVER + "/H_COMMAND?task_str=" + d.CURRENT_JOBNAME);

                HTTPResponse httpresponse = JsonSerializer.Deserialize<HTTPResponse>(res);
                if (httpresponse.result != 0)
                {
                    MessageBox.Show("Command Error : " + httpresponse.msg);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Robot HTTP Communication Exception : " + ex.Message);
            }
        }
    }
}