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
    /// TestsView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TestsView : UserControl
    {
        public TestsView()
        {
            InitializeComponent();
        }

        private void IncreaseShelf(object sender, RoutedEventArgs e)
        {
            //currentShelf++;

            //currentShelf = Math.Min(50, currentShelf);

            //WpOptionLineNum = currentLineNumber.ToString();
            //ShelfTextBox.Text = currentShelf.ToString();
        }
        private void DecreaseShelf(object sender, RoutedEventArgs e)
        {
            //currentShelf--;

            //currentShelf = Math.Max(1, currentShelf);

            //ShelfTextBox.Text = currentShelf.ToString();

            //WpOptionLineNum = currentLineNumber.ToString();
        }

        private void IncreasePocket(object sender, RoutedEventArgs e)
        {
            //currentPocket++;

            //currentPocket = Math.Min(50, currentPocket);

            //WpOptionLineNum = currentLineNumber.ToString();
            //PocketTextBox.Text = currentPocket.ToString();
        }
        private void DecreasePocket(object sender, RoutedEventArgs e)
        {
            //currentPocket--;

           // currentPocket = Math.Max(1, currentPocket);

            //PocketTextBox.Text = currentPocket.ToString();

            //WpOptionLineNum = currentLineNumber.ToString();
        }

        private void IncreaseDrillCode(object sender, RoutedEventArgs e)
        {
            //currentDrillCode++;

            //currentDrillCode = Math.Min(50, currentDrillCode);

            //WpOptionLineNum = currentLineNumber.ToString();
            //DrillCodeTextBox.Text = currentDrillCode.ToString();
        }
        private void DecreaseDrillCode(object sender, RoutedEventArgs e)
        {
            //currentDrillCode--;

            //currentDrillCode = Math.Max(1, currentDrillCode);

            //DrillCodeTextBox.Text = currentDrillCode.ToString();

            //WpOptionLineNum = currentLineNumber.ToString();
        }


        private void IncreaseDrillCore(object sender, RoutedEventArgs e)
        {
            //currentDrillCode++;

            //currentDrillCode = Math.Min(50, currentDrillCode);

            //WpOptionLineNum = currentLineNumber.ToString();
            //DrillCodeTextBox.Text = currentDrillCode.ToString();
        }
        private void DecreaseDrillCore(object sender, RoutedEventArgs e)
        {
            //currentDrillCode--;

            //currentDrillCode = Math.Max(1, currentDrillCode);

            //DrillCodeTextBox.Text = currentDrillCode.ToString();

            //WpOptionLineNum = currentLineNumber.ToString();
        }


        private void IncreasePocketCount(object sender, RoutedEventArgs e)
        {
            //currentDrillCode++;

            //currentDrillCode = Math.Min(50, currentDrillCode);

            //WpOptionLineNum = currentLineNumber.ToString();
            //DrillCodeTextBox.Text = currentDrillCode.ToString();
        }
        private void DecreasePocketCount(object sender, RoutedEventArgs e)
        {
            //currentDrillCode--;

            //currentDrillCode = Math.Max(1, currentDrillCode);

            //DrillCodeTextBox.Text = currentDrillCode.ToString();

            //WpOptionLineNum = currentLineNumber.ToString();
        }

        private void btnResetAllTests_Click(object sender, RoutedEventArgs e)
        {
            CommHTTPComponent http = CommHTTPComponent.Instance;

            try
            {
                http.GetAPI(C.ROBOT_SERVER + "/H_TEST_KIOSK_TO_POCKET");
                http.GetAPI(C.ROBOT_SERVER + "/H_TEST_POCKET_TO_KIOSK");
                http.GetAPI(C.ROBOT_SERVER + "/H_TEST_POCKET_TO_CHUCK");
                http.GetAPI(C.ROBOT_SERVER + "/H_TEST_CHUCK_TO_POCKET");
                http.GetAPI(C.ROBOT_SERVER + "/H_TEST_POCKET_TO_POCKET");
                http.GetAPI(C.ROBOT_SERVER + "/H_TEST_ALL_POCKETS");
            }
            catch (Exception ex)
            {
                MessageBox.Show("ROBOT 예외상황 : " + ex.Message);
            }
        }

        private string CurrentJob;
        private void btnKioskToPocket_Click(object sender, RoutedEventArgs e)
        {
            CurrentJob = "/H_TEST_KIOSK_TO_POCKET";
        }

        private void btnPocketToKiosk_Click(object sender, RoutedEventArgs e)
        {
            CurrentJob = "/H_TEST_POCKET_TO_KIOSK";
        }

        private void btnPocketToChunk_Click(object sender, RoutedEventArgs e)
        {
            CurrentJob = "/H_TEST_POCKET_TO_CHUNK";
        }

        private void btnChuckToPocket_Click(object sender, RoutedEventArgs e)
        {
            CurrentJob = "/H_TEST_CHUNK_TO_POCKET";
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            CommHTTPComponent http = CommHTTPComponent.Instance;

            try
            {
                http.GetAPI(C.ROBOT_SERVER + CurrentJob);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ROBOT 예외상황 : " + ex.Message);
            }

        }

        private string CurrentJob2;
        private void btnStationToSpindle_Click(object sender, RoutedEventArgs e)
        {
            CurrentJob2 = "/H_TEST_STATION_TO_SPINDLE";
        }

        private void btnSpindleToStation_Click(object sender, RoutedEventArgs e)
        {
            CurrentJob2 = "/H_TEST_SPINDLE_TO_STATION";
        }

        private void btnStart1_Click(object sender, RoutedEventArgs e)
        {
            CommHTTPComponent http = CommHTTPComponent.Instance;

            try
            {
                http.GetAPI(C.ROBOT_SERVER + CurrentJob2);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ROBOT 예외상황 : " + ex.Message);
            }

        }
    }
}
