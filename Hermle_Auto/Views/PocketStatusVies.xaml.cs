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


        int PocketCount = 0;
        int DrillCore = 0;
        int WorkPicec = 0;

        public PocketStatusVies()
        {
            InitializeComponent();

            PocketTable.ItemsSource = PocketList;
        }




        private void IncreasePocketCount(object sender, RoutedEventArgs e)
        {
            PocketCount++;

        }
        private void DecreasePocketCount(object sender, RoutedEventArgs e)
        {
            PocketCount--;

            PocketCount = Math.Max(1, PocketCount);
        }

        private void IncreaseWorkPicec(object sender, RoutedEventArgs e)
        {
            WorkPicec++;

        }
        private void DecreaseWorkPicec(object sender, RoutedEventArgs e)
        {
            WorkPicec--;

            WorkPicec = Math.Max(1, WorkPicec);
        }

        private void IncreaseDrillCore(object sender, RoutedEventArgs e)
        {
            DrillCore++;

        }
        private void DecreaseDrillCore(object sender, RoutedEventArgs e)
        {
            DrillCore--;

            DrillCore = Math.Max(1, DrillCore);
        }




        public class PocketData : INotifyPropertyChanged
        {
            public int Pocket { get; set; }
            public int WorkPiece { get; set; }
            public int Diameter { get; set; }
            public int Status { get; set; }
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
