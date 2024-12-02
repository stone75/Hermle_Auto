using HermleCS.Comm;
using HermleCS.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
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
    /// PocketStatusVies.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PocketStatusVies : UserControl
    {
        public ObservableCollection<PocketData> PocketList { get; set; } = new ObservableCollection<PocketData>();


        int PocketCount = 100;
        int ShelfCount = 1;
        //int WorkPicec = 0;

        public PocketStatusVies()
        {
            InitializeComponent();

            PocketTable.ItemsSource = PocketList;
        }




        private void IncreasePocketCount(object sender, RoutedEventArgs e)
        {
            PocketCount++;

            //PocketCount = Math.Min(50, PocketCount);

            PocketTextBox.Text = PocketCount.ToString();
        }
        private void DecreasePocketCount(object sender, RoutedEventArgs e)
        {
            PocketCount--;

            PocketCount = Math.Max(1, PocketCount);

            PocketTextBox.Text = PocketCount.ToString();
        }

        private void IncreaseWorkPicec(object sender, RoutedEventArgs e)
        {
            ShelfCount++;

            ShelfTextBox.Text = ShelfCount.ToString();
        }
        private void DecreaseWorkPicec(object sender, RoutedEventArgs e)
        {
            ShelfCount--;

            ShelfCount = Math.Max(1, ShelfCount);

            ShelfTextBox.Text = ShelfCount.ToString();
        }

        private void ChangeWorkPiecePocketButton(object sender, RoutedEventArgs e)
        {

        }

        private void ChangeSinglePocketButton(object sender, RoutedEventArgs e)
        {

        }

        private void ResetButton(object sender, RoutedEventArgs e)
        {
            PocketList.Clear();
        }

        private void RefreshButton(object sender, RoutedEventArgs e)
        {

            string str = D.Instance.GetToolType();

            if(str == "")
            {
                C.log("GetToolType is not Type");
                return;
            }

           
            D.Instance.ReadStatus(str);

            Status[,,] status =  D.Instance.GetStatus(str);

            if(status == null)
            {
                C.log("Status is Null");
                return;
            }

            //status[ShelfCount, 0, 0];

            PocketList.Clear();

            int count = ShelfCount - 1;

            for (int i = 0; i < C.DRILL_STATUS_POCKET_COUNT; i++)
            {
                PocketData data = new PocketData();


                data.Pocket = status[count, 0, i].name;
                data.WorkPiece = status[count, 0, i].workpiece;
                data.Diameter = status[count, 0, i].diameter;

                data.Status = D.Instance.PocketStatusConberter(status[count, 0, i].status);

                data.Program = status[count, 0, i].programnumber;

                PocketList.Add(data);

                //Console.WriteLine(status[count, 0, i].status);
            }

            //PocketList


        }


        /*        private void IncreaseDrillCore(object sender, RoutedEventArgs e)
                {
                    DrillCore++;

                }
                private void DecreaseDrillCore(object sender, RoutedEventArgs e)
                {
                    DrillCore--;

                    DrillCore = Math.Max(1, DrillCore);
                }*/




        public class PocketData : INotifyPropertyChanged
        {
            public string Pocket { get; set; }
            public int WorkPiece { get; set; }
            public int Diameter { get; set; }
            public string Status { get; set; }
            public int Program { get; set; }
            //public int ToolAmountLeft { get; set; }


            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            /*            public void AllPropertyChanged()
                        {
                            OnPropertyChanged(nameof(ToolType)); // 속성 변경 알림
                            OnPropertyChanged(nameof(WorkPiece2)); // 속성 변경 알림
                            OnPropertyChanged(nameof(NCProgram)); // 속성 변경 알림
                            OnPropertyChanged(nameof(ToolDiameter)); // 속성 변경 알림
                            OnPropertyChanged(nameof(ToolAmount)); // 속성 변경 알림
                            OnPropertyChanged(nameof(LineNumber)); // 속성 변경 알림
                        }*/

            public void AllPropertyChanged()
            {
                OnPropertyChanged(null);
            }

        }
    }
}
