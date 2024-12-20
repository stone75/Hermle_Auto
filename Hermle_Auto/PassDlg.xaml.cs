﻿using Hermle_Auto.Comm;
using HermleCS.Comm;
using HermleCS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
using static System.Net.WebRequestMethods;

namespace Hermle_Auto
{
    /// <summary>
    /// PassDlg.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PassDlg : Window
    {
        private const string CORRECT_PASSWORD = "1111";

        public event RobotStatusLogger logger;
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
            ContryTextBox.Text = D.Instance.iniFile.information.Country;
            FactoryTextBox.Text = D.Instance.iniFile.information.factory;
            AutoMationNameTextBox.Text = D.Instance.iniFile.information.AutoName;
            AutomationNumberTextBox.Text = D.Instance.iniFile.information.AutoNumber;
            HermleNumberTextBox.Text = D.Instance.iniFile.information.HermleNumber;
            HermleTypeTextBox.Text = D.Instance.iniFile.information.HermleType;
        }

        private void InformationSaveChangeClick(object sender, RoutedEventArgs e)
        {
            D.Instance.iniFile.information.Country = ContryTextBox.Text;
            D.Instance.iniFile.information.factory = FactoryTextBox.Text;
            D.Instance.iniFile.information.AutoName = AutoMationNameTextBox.Text;
            D.Instance.iniFile.information.AutoNumber = AutomationNumberTextBox.Text;
            D.Instance.iniFile.information.HermleNumber = HermleNumberTextBox.Text;
            D.Instance.iniFile.information.HermleType = HermleTypeTextBox.Text;

            D.Instance.WriteIniFile();


        }

        private void SelectGripperTypeClick(object sender, RoutedEventArgs e)
        {

            //어디에 무었을 바꿀지 알아야됨

            Button button = sender as Button;

            // 미구현.
            /*
             * 2024.12.04
             * INI 파일 저장 구현
             * @ To Do..
             * 다른 곳에서도 Gripper Type 을 INI 에서 가져오는지 확인 필요
             */
            // *2024.12.06 - 구현 완료(INI 파일 저장 및 불러오기)

            string buttonContent = button.Content as string; // Content를 string으로 캐스팅

            if (buttonContent == "HSK")
            {
                gipperType.Text = "HSK";
                D.Instance.iniFile.gripper.style = 1;
            }
            else if (buttonContent == "Drill")
            {
                gipperType.Text = "Drill";
                D.Instance.iniFile.gripper.style = 2;
            }
            else if (buttonContent == "Round")
            {
                gipperType.Text = "Round";
                D.Instance.iniFile.gripper.style = 3;
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
            /* Gripper Text 는
             * HSK / Drill / Round 
             * 셋 중에 하나 인것 같음... 동영상 참고.
             */
            //gipperType.Text     = D.Instance.iniFile.gripper.style == 1 ? "Single gripper" : "Double gripper";
            if (D.Instance.iniFile.gripper.style == 1)
            {
                gipperType.Text = "HSK";
            }
            else if (D.Instance.iniFile.gripper.style == 2)
            {
                gipperType.Text = "Drill";
            }
            else
            {
                gipperType.Text = "Round";
            }

            shelf1ToolType.Text = CheckToolType(D.Instance.iniFile.shelvs.first);
            shelf2ToolType.Text = CheckToolType(D.Instance.iniFile.shelvs.second);
            shelf3ToolType.Text = CheckToolType(D.Instance.iniFile.shelvs.third);

        }

        private void grippersendRobotClick(object sender, RoutedEventArgs e)
        {

            //어디에 무었을 바꿀지 알아야됨

            Button button = sender as Button;


            string buttonContent = button.Content as string; // Content를 string으로 캐스팅

            if (buttonContent == "HSK")
            {
                grippervalue.Text = "7";
                // Send To Robot 값 설정
            }
            else if (buttonContent == "Round Small")
            {
                grippervalue.Text = "6";
                // Send To Robot 값 설정
            }
            else if (buttonContent == "Round Big")
            {
                grippervalue.Text = "5";
                // Send To Robot 값 설정
            }


        }

        private void sendRobotClick(object sender, RoutedEventArgs e)
        {

            // 2024.12.06.
            //H_명령어 추가 필요
            //HSK = 5, Round Small = 6, Round Big = 7
            //H_TOOL 커맨드 정의 필요

            logger?.Invoke("Gripper Send to Robot...");

            D d = D.Instance;
            CommHTTPComponent http = CommHTTPComponent.Instance;


            string res;


            try
            {
                // 1. TP 파일 이름 셋팅
                d.CURRENT_JOBNAME = "TOOL";     //TOOL.TP

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
                        x = locationItem.x.ToString(),
                        y = locationItem.y.ToString(),
                        z = locationItem.z.ToString(),
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
            public string x { get; set; }
            public string y { get; set; }
            public string z { get; set; }
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
