using HermleCS.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Shapes;
using static Hermle_Auto.Views.WorkPiece2View;

namespace Hermle_Auto
{
    /// <summary>
    /// PassDlg.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PassDlg : Window
    {
        private const string CORRECT_PASSWORD = "1111";


        public PassDlg()
        {
            InitializeComponent();

            allLocationDataGrid.ItemsSource = AllLocationItem;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (passwordBox.Password == CORRECT_PASSWORD)
            {
                mainTabControl.Visibility = Visibility.Visible;
                //statusText.Text = "비밀번호가 맞습니다.";
                //statusText.Foreground = Brushes.Green;
            }
            else
            {
                mainTabControl.Visibility = Visibility.Collapsed;
                //statusText.Text = "비밀번호가 틀렸습니다.";
                //statusText.Foreground = Brushes.Red;
            }

        }

        private void DisPlayInformationClick(object sender, RoutedEventArgs e)
        {
            //ini 데이터 텍스트박스 적용
            ContryTextBox.Text           = D.Instance.iniFile.information.Country    ;
            FactoryTextBox.Text          = D.Instance.iniFile.information.factory    ;
            AutoMationNameTextBox.Text   = D.Instance.iniFile.information.AutoName   ;
            AutomationNumberTextBox.Text = D.Instance.iniFile.information.AutoNumber ;
            HermleNumberTextBox.Text     = D.Instance.iniFile.information.HermleNumber;
            HermleTypeTextBox.Text       = D.Instance.iniFile.information.HermleType ;
        }

        private void InformationSaveChangeClick(object sender, RoutedEventArgs e)
        {
            D.Instance.iniFile.information.Country      = ContryTextBox.Text          ;
            D.Instance.iniFile.information.factory      = FactoryTextBox.Text         ;
            D.Instance.iniFile.information.AutoName     = AutoMationNameTextBox.Text  ;
            D.Instance.iniFile.information.AutoNumber   = AutomationNumberTextBox.Text;
            D.Instance.iniFile.information.HermleNumber = HermleNumberTextBox.Text    ;
            D.Instance.iniFile.information.HermleType   = HermleTypeTextBox.Text      ;

            D.Instance.WriteIniFile();


        }

        private void SelectGripperTypeClick(object sender, RoutedEventArgs e)
        {

            //어디에 무었을 바꿀지 알아야됨

            Button button = sender as Button;


            if(button.Content == "HKS")
            {

            }
            else if (button.Content == "Drill")
            {

            }
            else if (button.Content == "Round")
            {

            }


            D.Instance.WriteIniFile();

        }
        

        private void DisplayConfigurationClick(object sender, RoutedEventArgs e)
        {
            /* ; ****Gripper * **
;
            ; style = 1  Single gripper
            ; style = 2  Double gripper
           ;**** AppToolType ***
            ;
            ;     Other = 0
            ;     HSK   = 1
            ;     Drill = 2
            ;     Round = 3
            ;
           */
            //확인 필요
            gipperType.Text     = D.Instance.iniFile.gripper.style == 1 ? "Single gripper" : "Double gripper";

            shelf1ToolType.Text = CheckToolType(D.Instance.iniFile.shelvs.first);
            shelf2ToolType.Text = CheckToolType(D.Instance.iniFile.shelvs.second);
            shelf3ToolType.Text = CheckToolType(D.Instance.iniFile.shelvs.third);

        }

        private void DisplayAllLocationsClick(object sender, RoutedEventArgs e)
        {

            var toolType = D.Instance.GetToolType();

            var ret = D.Instance.ReadGeneralLocations(toolType);

            //GeneralLocations[] location =  D.Instance.DrillGeneralLocations;
            GeneralLocations[] location = D.Instance.getGeneralLocations(toolType);

            AllLocationItem.Clear();


            foreach (GeneralLocations locationItem in location)
            {
                AllLocationItem.Add
                    (
                    new AllLocationData
                    {
                        GeneralLocation = locationItem.name,
                        x =  locationItem.x.ToString(),
                        y =  locationItem.y.ToString(),
                        z =  locationItem.z.ToString(),
                        Rx = locationItem.rx.ToString(),
                        Ry = locationItem.ry.ToString(),
                        Rz = locationItem.rx.ToString(),
                    });
            }

           
            //getGeneralLocations
            //allLocationDataGrid
        }

        public ObservableCollection<AllLocationData> AllLocationItem { get; set; } = new ObservableCollection<AllLocationData>();
        public class AllLocationData : INotifyPropertyChanged
        {
            public string GeneralLocation { get; set; }
            public string x  { get; set; }
            public string y  { get; set; }
            public string z  { get; set; }
            public string Rx { get; set; }
            public string Ry { get; set; }
            public string Rz { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            
            public void AllPropertyChanged()
            {
                OnPropertyChanged(null);
            }
        }
        //툴타입 체크 함수
        private string CheckToolType(int toolType) 
        {

            string result = "";


            switch (toolType)
            {
                case 0:
                    result = "Other";
                    break;
                case 1:
                    result = "HSK";
                    break;
                case 2:
                    result = "Drill";
                    break;
                case 3:
                    result = "Round";
                    break;
               
                default:
                    break;
            }


            return result;
        }


    }
}
