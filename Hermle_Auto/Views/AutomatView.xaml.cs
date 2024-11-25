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


namespace Hermle_Auto.Views
{
    /// <summary>
    /// Interaction logic for AutomatView.xaml
    /// </summary>
    public partial class AutomatView : UserControl
    {
        public AutomatView()
        {
            InitializeComponent();
//            logger += writelog;
        }

        private void AutomatView_logger(string message)
        {
            throw new NotImplementedException();
        }

        public void writelog(string msg)
        {
            string formattedTime = DateTime.Now.ToString("HH:mm:ss");
            txtRobotStatus.Text = $"{formattedTime} : {msg}\r\n" + txtRobotStatus.Text;
        }


        private void btnStartAutomat_Click(object sender, RoutedEventArgs e)
        {
            CommPLC commplc = CommPLC.Instance;
            D d = D.Instance;
//            logger?.Invoke("Start Automat Button Cliecked...");

            try
            {
                commplc.WritePLC(McProtocol.Mitsubishi.PlcDeviceType.M, 2009, 1);
                commplc.WritePLCBlock(2000, d.WorkPiecesList[d.CurrentWorkPieceIndex].ncprogram);
            }
            catch (Exception ex)
            {
                MessageBox.Show("PLC 예외상황 : " + ex.Message);
            }
        }
    }
}
