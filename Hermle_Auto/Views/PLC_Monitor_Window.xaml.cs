using System;
using System.Collections.Generic;
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

namespace Hermle_Auto.Views
{
    /// <summary>
    /// PLC_Monitor_Window.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PLC_Monitor_Window : Window
    {

        private List<MachineStatus> leftData;
        private List<MachineStatus> rightData;


        public PLC_Monitor_Window()
        {
            InitializeComponent();
            LoadMachineData();

            // 테스트를 위한 타이머 설정 (실제로는 필요에 따라 변경)
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += Timer_Tick;
            timer.Start();


            //SetLampState("M2106", true);

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // UI 스레드에서 실행되도록 보장
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var item in leftData.Concat(rightData))
                {
                    item.IsOn = !item.IsOn;
                }
            });
        }


        // 특정 주소의 램프 상태를 변경하는 메서드
        public void SetLampState(string address, bool isOn)
        {
            var leftItem = leftData?.FirstOrDefault(x => x.Address == address);
            if (leftItem != null)
            {
                leftItem.IsOn = isOn;
                return;
            }

            var rightItem = rightData?.FirstOrDefault(x => x.Address == address);
            if (rightItem != null)
            {
                rightItem.IsOn = isOn;
            }
        }

        public void CheckLamp(string address)
        {

          /*  var leftItem = ((List<MachineStatus>)LeftMachineStatusGrid.ItemsSource).FirstOrDefault(x => x.Address == address);
            if (leftItem != null)
            {
                leftItem.LampColor = Colors.Green;
            }*/


            /*   var leftItem = ((List<MachineStatus>)LeftMachineStatusGrid.ItemsSource).FirstOrDefault(x => x.Address == "M2106");
               if (leftItem != null)
               {
                   leftItem.LampColor = Colors.Green;
               }*/

            /*   var rightItem = ((List<MachineStatus>)RightMachineStatusGrid.ItemsSource)
                   .FirstOrDefault(x => x.Address == "M2206");
               if (rightItem != null)
               {
                   rightItem.LampColor = Colors.Red;
               }*/
        }

        private void LoadMachineData()
        {

            // M21xx 데이터 (왼쪽 그리드)
            leftData = new List<MachineStatus>();

            string[,] m21Data = {
                { "M2106", "is toggled whenever new data are received", "새로운 데이터가 수신될 때마다 토글" },
                { "M2107", "is toggled when data are changed", "데이터가 변경될 때마다 토글" },
                { "M2112", "record identification - machine -> PLC : always E1", "기록식별 - 기계 -> PLC : 항상 E1" },
                { "M2116", "machine ready for loading chuck", "M/C 로딩 준비 완료" },
                { "M2117", "-.validity (machine ready for loading chuck)", "(M/C 로딩 준비 완료)" },
                { "M2118", "nc programm finished", "NC 프로그램 종료" },
                { "M2119", "-.validity (nc programm finished)", "(NC 프로그램 종료)" },
                { "M2120", "nc programm run", "NC 프로그램 실행 중" },
                { "M2121", "-.validity (nc programm run)", "(NC 프로그램 실행 중)" },
                { "M2122", "blow off is deactivated", "Blow off 비활성화" },
                { "M2123", "-.validity (blow off is deactivated)", "(Blow off 비활성화)" },
                { "M2124", "machine chuck is open", "척 클램프 풀림" },
                { "M2125", "-.validity (machine chuck is open)", "(척 클램프 풀림)" },
                { "M2126", "machine connection to automation is turned on", "자동화에 대한 M/C 연결됨" },
                { "M2127", "-.validity (machine connection to automation is turned on)", "(자동화에 대한 M/C 연결됨)" },
                { "M2128", "zero point number is taken over", "원점 번호 수신완료" },
                { "M2129", "-.validity (zero point number is taken over)", "(원점 번호 수신완료)" },
                { "M2130", "nc-programm is taken over", "NC 프로그램 번호 수신완료" },
                { "M2131", "-.validity (nc-programm is taken over)", "(NC 프로그램 번호 수신완료)" },
                { "M2132", "work piece is finished (no error while processing)", "가공작업 완료(문제없음)" },
                { "M2133", "-.validity (work piece is finished (no error while processing))", "(가공작업 완료(문제없음))" },
                { "M2134", "Anlagekontrolle", "시스템 제어" },
                { "M2135", "-.validity (Anlagekontrolle)", "(시스템 제어)" },
                { "M2136", "tool broken", "공구 파손됨" },
                { "M2137", "-.validity (tool broken)", "(공구 파손됨)" },
                { "M2138", "machine chuck is closed", "척 클램프 잠김" },
                { "M2139", "-.validity (machine chuck is closed)", "(척 클램프 잠김)" },
                { "M2146", "machine has an error", "M/C 알람발생" },
                { "M2147", "-.validity (machine has an error)", "(M/C 알람발생)" }
            // ... [기존 M21xx 데이터]
        };

            // M22xx 데이터 (오른쪽 그리드)
            rightData = new List<MachineStatus>();

            string[,] m22Data = {
               { "M2206", "is toggled whenever new data are received", "새로운 데이터가 수신될 때마다 토글" },
                { "M2207", "is toggled when data are changed", "데이터가 변경될 때마다 토글" },
                { "M2208", "A1 – byte 2 to 5 contains the nc program number", "값이 A1 일대 바이트 2~5에 nc 프로그램 번호" },
                { "M2209", "A2 – byte 2 to 5 contains the nc zero point program number", "값이 A2 일대 바이트 2~5에 nc 제로 포인트 프로그램 번호" },
                { "M2210", "A3 – byte 2 to 5 contains commands to the machine", "값이 A3 일대 바이트 2~5에 기계에 대한 명령이 전송" },
                { "M2216", "request to close the chuck", "척 클램프 잠김 요청" },
                { "M2217", "-validity (request to close the chuck)", "(척 클램프 잠김 요청)" },
                { "M2218", "request to open the chuck", "척 클램프 풀림 요청" },
                { "M2219", "-validity (request to open the chuck)", "(척 클램프 풀림 요청)" },
                { "M2220", "request for loading machine", "로딩 요청" },
                { "M2221", "-validity (request for loading machine)", "(로딩 요청)" },
                { "M2222", "synchronize command (order to send actual status)", "동기화 요청(현재 상태 전송 명령)" },
                { "M2223", "-validity (synchronize command)", "(동기화 요청)" },
                { "M2224", "no work peace loaded", "로딩된 제품 없음" },
                { "M2225", "-validity (no work peace loaded)", "(로딩된 제품 없음)" },
                { "M2226", "request start nc program", "NC 프로그램 시작 요청" },
                { "M2227", "-validity (request start nc program)", "(NC 프로그램 시작 요청)" },
                { "M2228", "zero point number is valid", "제로 포인트 번호 유효" },
                { "M2229", "-validity (zero point number is valid)", "(제로 포인트 번호 유효)" },
                { "M2230", "nc program number is valid", "NC 프로그램 번호 유효" },
                { "M2231", "-validity (nc program number is valid)", "(NC 프로그램 번호 유효)" },
                { "M2236", "turn off machine", "turn off M/C" },
                { "M2237", "-validity (turn off machine)", "(turn off M/C)" },
                { "M2238", "deactivate blow off", "Blow off 비활성화" },
                { "M2239", "-validity (deactivate blow off)", "(Blow off 비활성화)" },
                { "M2246", "machine has an error (not used)", "M/C 이상" },
                { "M2247", "-validity (machine has an error (not used))", "(M/C 이상)" }
            // ... [M22xx 데이터]
        };

            // 데이터 로드 함수
            LoadGridData(leftData, m21Data);
            LoadGridData(rightData, m22Data);

            // DataGrid에 데이터 바인딩
            LeftMachineStatusGrid.ItemsSource = leftData;
            RightMachineStatusGrid.ItemsSource = rightData;
        }

        private void LoadGridData(List<MachineStatus> list, string[,] data)
        {
            for (int i = 0; i < data.GetLength(0); i++)
            {
                bool isValidity = data[i, 1].Contains("-validity");
                list.Add(new MachineStatus
                {
                    Address = data[i, 0],
                    DescriptionEng = data[i, 1],
                    DescriptionKor = data[i, 2],
                    //LampColor = Colors.Gray,
                    IsValidity = isValidity,
                     IsOn = false  // 초기 상태는 꺼짐
                });
            }
        }
    }
}


public class MachineStatus : INotifyPropertyChanged
{
    private bool _isOn;
    private Color _lampColor = Colors.Gray;

    public string Address { get; set; }
    public string DescriptionEng { get; set; }
    public string DescriptionKor { get; set; }
    public bool IsValidity { get; set; }

    public bool IsOn
    {
        get => _isOn;
        set
        {
            if (_isOn != value)
            {
                _isOn = value;
                LampColor = _isOn ? Colors.LimeGreen : Colors.Gray;
                OnPropertyChanged(nameof(IsOn));
            }
        }
    }

    public Color LampColor
    {
        get => _lampColor;
        private set
        {
            if (_lampColor != value)
            {
                _lampColor = value;
                OnPropertyChanged(nameof(LampColor));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}