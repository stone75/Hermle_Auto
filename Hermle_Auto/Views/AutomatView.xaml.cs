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
            CommPLC commplc = CommPLC.Instance;
            D d = D.Instance;

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
