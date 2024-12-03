using HermleCS.Comm;
using HermleCS.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
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
using System.Windows.Media.Animation;
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

        // 미구현
        private void ChangeWorkPiecePocketButton(object sender, RoutedEventArgs e)
        {

        }

        private void ChangeSinglePocketButton(object sender, RoutedEventArgs e)
        {

            var toolType = D.Instance.GetToolType();
            int pocketNumber;
            int diameter = 0;
            int status;
            int ret;
            PocketStatus oldStatus;

            //ShelfCount
            try
            { 
                /*''check if the shelf tooltype is the same as the gripper tool type.
                   If AppShelvs(shelf).ShelfToolType<> AppToolType Then
                       ret = MsgBox("the shelf number is incorrect." & vbCrLf _
                           & "the shelf type is not the same as the gripper type" _
                           , vbCritical _
                           , "BtnSingleStatus_Click()")
                        Exit Sub
                    End If
                  if (AppShelves[shelf].ShelfToolType != AppToolType)
                    {
                        MessageBox.Show("The shelf number is incorrect.\n" +
                                        "The shelf type is not the same as the gripper type.",
                                        "BtnSingleStatus_Click()",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                        return;
                    }*/
                /*if (string.IsNullOrEmpty(TxtSinglePocketNumber.Text))
                    throw new ArgumentException("Pocket number is empty.");*/

                
                //TxtSingleToolDiameter -> 어디있는 값인지 확인 필요
                //if (toolType == "HSK" && string.IsNullOrEmpty(TxtSingleToolDiameter.Text))
                //    throw new ArgumentException("Tool diameter is empty for HSK type.");


                if (SinglePocketStatusListComboBox.Text == "Single Status")
                    throw new ArgumentException("Status not selected.");

                // Validate numeric input
                if (!int.TryParse(PocketTextBox.Text, out pocketNumber))
                    throw new ArgumentException("Pocket number is not numeric.");


                //TxtSingleToolDiameter -> 어디있는 값인지 확인 필요
                /*if (toolType == "HSK" && !int.TryParse(TxtSingleToolDiameter.Text, out diameter))
                    throw new ArgumentException("Tool diameter is not numeric.");*/


                // Get user inputs
                status = SinglePocketStatusListComboBox.SelectedIndex + 1;
                pocketNumber = int.Parse(PocketTextBox.Text);

                // Adjust pocket number and diameter based on tool type
                if (toolType == "HSK")
                {
                    if (pocketNumber >= 101 && pocketNumber <= 110)
                     { 
                        pocketNumber = pocketNumber - 100; 
                    }
                    else if (pocketNumber >= 201 && pocketNumber <= 210)
                    { 
                        pocketNumber = pocketNumber - 200; 
                    }
                    else if (pocketNumber >= 301 && pocketNumber <= 310)
                    {
                        pocketNumber = pocketNumber - 300;
                    }

                    diameter = 0;
                }
                else if (toolType == "Drill" || toolType == "Round")
                {
                    if (pocketNumber >= 101 && pocketNumber <= 112)
                    {
                        pocketNumber = pocketNumber - 100;
                    }
                    else if (pocketNumber >= 201 && pocketNumber <= 212)
                    {
                        pocketNumber = pocketNumber - 200;
                    }
                    else if (pocketNumber >= 301 && pocketNumber <= 312)
                    {
                        pocketNumber = pocketNumber - 300;
                    }

                    //TxtSingleToolDiameter -> 어디있는 값인지 확인 필요
                    //diameter = int.Parse(TxtSingleToolDiameter.Text);
                }

                // Get the current pocket status
                oldStatus = D.Instance.GetPocketStatus(ShelfCount, pocketNumber);

                if (oldStatus == PocketStatus.Mask)
                {
                    ret = (int)MessageBox.Show("The current status is: MASK\n" +
                                               "Do you want to continue?",
                                               "Change status for single pocket",
                                               MessageBoxButton.YesNo,
                                               MessageBoxImage.Question);

                    if (ret == (int)MessageBoxResult.No)
                        return;

                    SetPocketStatus(status, int.Parse(PocketTextBox.Text));
                }

                // Update pocket status
                SetPocketStatus(status, int.Parse(PocketTextBox.Text));



                RefreshButton(null, null);

            }
            catch (Exception ex)  
            {
                C.log(ex.ToString());
            }
        }

        

        private void SetPocketStatus(int status, int pocketNumber)
        {
            // Mocked method for setting pocket status
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
