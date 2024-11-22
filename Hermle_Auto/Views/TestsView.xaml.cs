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

        private int pockets_PocketCount_L = 100;
        private int pockets_PocketCount_R = 100;
        private int pockets_DrillCode = 1;
        private int pockets_Shelf = 1;
        private int pockets_Diameter = 1;
        private int load_PocketCount = 100;
        private int load_DrillCode = 1;
        private int saw_station = 1;

        public TestsView()
        {
            InitializeComponent();
        }

        private void IncreasePocketCount_L(object sender, RoutedEventArgs e)
        {
            pockets_PocketCount_L++;

            PocketLTextBox.Text = pockets_PocketCount_L.ToString();
        }
        private void DecreasePocketCount_L(object sender, RoutedEventArgs e)
        {
            pockets_PocketCount_L--;

            pockets_PocketCount_L = Math.Max(1, pockets_PocketCount_L);

            PocketLTextBox.Text = pockets_PocketCount_L.ToString();
        }


        private void IncreasePocketCount_R(object sender, RoutedEventArgs e)
        {
            pockets_PocketCount_R++;

            PocketRTextBox.Text = pockets_PocketCount_R.ToString();
        }
        private void DecreasePocketCount_R(object sender, RoutedEventArgs e)
        {
            pockets_PocketCount_R--;

            pockets_PocketCount_R = Math.Max(1, pockets_PocketCount_R);

            PocketRTextBox.Text = pockets_PocketCount_R.ToString();
        }


        private void IncreasePocketsDrillCode(object sender, RoutedEventArgs e)
        {
            pockets_DrillCode++;

            PocketDrillCodeTextBox.Text = pockets_DrillCode.ToString();
        }
        private void DecreasePocketsDrillCode(object sender, RoutedEventArgs e)
        {
            pockets_DrillCode--;

            pockets_DrillCode = Math.Max(1, pockets_DrillCode);

            PocketDrillCodeTextBox.Text = pockets_DrillCode.ToString();
        }


        private void IncreasePocketsShelf(object sender, RoutedEventArgs e)
        {
            pockets_Shelf++;

            PocketsShelfTextBox.Text = pockets_Shelf.ToString();
        }
        private void DecreasePocketsShelf(object sender, RoutedEventArgs e)
        {
            pockets_Shelf--;

            pockets_Shelf = Math.Max(1, pockets_Shelf);

            PocketsShelfTextBox.Text = pockets_Shelf.ToString();
        }


        private void IncreasePocketsDiameter(object sender, RoutedEventArgs e)
        {
            pockets_Diameter++;

            DiameterTextBox.Text = pockets_Diameter.ToString();
        }
        private void DecreasePocketsDiameter(object sender, RoutedEventArgs e)
        {
            pockets_Diameter--;

            pockets_Diameter = Math.Max(1, pockets_Diameter);

            DiameterTextBox.Text = pockets_Diameter.ToString();
        }


        private void IncreaseLoadPocket(object sender, RoutedEventArgs e)
        {
            load_PocketCount++;

            LoadPocketTextBox.Text = load_PocketCount.ToString();
        }
        private void DecreaseLoadPocket(object sender, RoutedEventArgs e)
        {
            load_PocketCount--;

            load_PocketCount = Math.Max(1, load_PocketCount);

            LoadPocketTextBox.Text = load_PocketCount.ToString();
        }



        private void IncreaseLoadDrillCode(object sender, RoutedEventArgs e)
        {
            load_DrillCode++;

            LoadDrillCodeTextBox.Text = load_DrillCode.ToString();
        }
        private void DecreaseLoadDrillCode(object sender, RoutedEventArgs e)
        {
            load_DrillCode--;

            load_DrillCode = Math.Max(1, load_DrillCode);

            LoadDrillCodeTextBox.Text = load_DrillCode.ToString();
        }



        private void IncreaseSawStation(object sender, RoutedEventArgs e)
        {
            saw_station++;

            SawStationTextBox.Text = saw_station.ToString();
        }
        private void DecreaseSawStation(object sender, RoutedEventArgs e)
        {
            saw_station--;

            saw_station = Math.Max(1, saw_station);

            SawStationTextBox.Text = saw_station.ToString();
        }


        private void btnAllPocketsStartTest_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void btnPocketStartTest_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btnStopLoop_Click(object sender, RoutedEventArgs e)
        {

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
