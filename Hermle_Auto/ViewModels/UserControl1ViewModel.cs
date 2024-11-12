using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hermle_Auto.Core;
using Hermle_Auto.Views;
using HermleCS.Data;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Hermle_Auto.ViewModels
{
    public class UserControl1ViewModel : ObservableObject , INotifyPropertyChanged
    {

        private string _diameter = "Diameter";
        private string _tools = "Tools";
        private string _manualtyep = "Manual";
        private string _pocket = "Pocket";
        private string _tapSelect = "Work Piece";

        public string Diameter
        {
            get => _diameter;
            set
            {
                if (_diameter != value)
                {
                    _diameter = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Tools
        {
            get => _tools;
            set
            {
                if (_tools != value)
                {
                    _tools = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ManualType
        {
            get => _manualtyep;
            set
            {
                if (_manualtyep != value)
                {
                    _manualtyep = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Pocket
        {
            get => _pocket;
            set
            {
                if (_pocket != value)
                {
                    _pocket = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TapSelect
        {
            get => _tapSelect;
            set
            {
                if (_tapSelect != value)
                {
                    _tapSelect = value;
                    OnPropertyChanged();
                }
            }
        }


        private string _valueX = "-278.204";
        private string _valueY = "-8.2014";
        private string _valueZ = "-78.2044";

        public string ValueX
        {
            get => _valueX;
            set
            {
                if (_valueX != value)
                {
                    _valueX = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ValueY
        {
            get => _valueY;
            set
            {
                if (_valueY != value)
                {
                    _valueY = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ValueZ
        {
            get => _valueZ;
            set
            {
                if (_valueZ != value)
                {
                    _valueZ = value;
                    OnPropertyChanged();
                }
            }
        }



        private string _valueRx = "0.0128";
        private string _valueRy = "-0.7144";
        private string _valueRz = "73.3149";

        public string ValueRx
        {
            get => _valueRx;
            set
            {
                if (_valueRx != value)
                {
                    _valueRx = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ValueRy
        {
            get => _valueRy;
            set
            {
                if (_valueRy != value)
                {
                    _valueRy = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ValueRz
        {
            get => _valueRz;
            set
            {
                if (_valueRz != value)
                {
                    _valueRz = value;
                    OnPropertyChanged();
                }
            }
        }


        private string _valueWorkPiece = "1";
        private string _valueAmountLeft = "24";
        private string _valueNCProgram = "1355";

        public string ValueWorkPiece
        {
            get => _valueWorkPiece;
            set
            {
                if (_valueWorkPiece != value)
                {
                    _valueWorkPiece = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ValueAmountLeft
        {
            get => _valueAmountLeft;
            set
            {
                if (_valueAmountLeft != value)
                {
                    _valueAmountLeft = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ValueNCProgram
        {
            get => _valueNCProgram;
            set
            {
                if (_valueNCProgram != value)
                {
                    _valueNCProgram = value;
                    OnPropertyChanged();
                }
            }
        }


        private string _valueServe      = "OFF";
        private string _valueGripper    = "OPEN";
        private string _valueKeyState   = "Remote";

        private Brush _valueServeForeground = Brushes.Red;
        private Brush _valueGripperForeground = Brushes.Black;
        private Brush _valueKeyStateForeground = Brushes.Green;

        private FontWeight _valueServeFontWeight = FontWeights.Bold;
        private FontWeight _valueGripperFontWeight = FontWeights.Bold;
        private FontWeight _valueKeyStateFontWeight = FontWeights.Bold;

        public string ValueServe
        {
            get => _valueServe;
            set
            {
                if (_valueServe != value)
                {
                    _valueServe = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ValueGripper
        {
            get => _valueGripper;
            set
            {
                if (_valueGripper != value)
                {
                    _valueGripper = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ValueKeyState
        {
            get => _valueKeyState;
            set
            {
                if (_valueKeyState != value)
                {
                    _valueKeyState = value;
                    OnPropertyChanged();
                }
            }
        }

        public Brush ValueServeForeground
        {
            get => _valueServeForeground;
            set
            {
                if (_valueServeForeground != value)
                {
                    _valueServeForeground = value;
                    OnPropertyChanged();
                }
            }
        }

        public Brush ValueGripperForeground
        {
            get => _valueGripperForeground;
            set
            {
                if (_valueGripperForeground != value)
                {
                    _valueGripperForeground = value;
                    OnPropertyChanged();
                }
            }
        }

        public Brush ValueKeyStateForeground
        {
            get => _valueKeyStateForeground;
            set
            {
                if (_valueKeyStateForeground != value)
                {
                    _valueKeyStateForeground = value;
                    OnPropertyChanged();
                }
            }
        }

        public FontWeight ValueServeFontWeight
        {
            get => _valueServeFontWeight;
            set
            {
                if (_valueServeFontWeight != value)
                {
                    _valueServeFontWeight = value;
                    OnPropertyChanged();
                }
            }
        }

        public FontWeight ValueGripperFontWeight
        {
            get => _valueGripperFontWeight;
            set
            {
                if (_valueGripperFontWeight != value)
                {
                    _valueGripperFontWeight = value;
                    OnPropertyChanged();
                }
            }
        }

        public FontWeight ValueKeyStateFontWeight
        {
            get => _valueKeyStateFontWeight;
            set
            {
                if (_valueKeyStateFontWeight != value)
                {
                    _valueKeyStateFontWeight = value;
                    OnPropertyChanged();
                }
            }
        }



        private string _selectedMode;

        public string SelectedMode
        {
            get => _selectedMode;
            set
            {
                if (_selectedMode != value)
                {
                    _selectedMode = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand AutoCommand { get; }
        public ICommand SemiCommand { get; }
        public ICommand ManualCommand { get; }

        private void ExecuteAuto()
        {
            SelectedMode = "Auto";

            ManualType = "Auto";
            // 추가적인 Auto 모드 로직
        }

        private void ExecuteSemi()
        {
            SelectedMode = "Semi";

            ManualType = "Semi";
            // 추가적인 Semi 모드 로직
        }

        private void ExecuteManual()
        {
            SelectedMode = "Manual";

            ManualType = "Manual";
            // 추가적인 Manual 모드 로직
        }

        public ICommand StartCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand ResetCommand { get; }
        public ICommand ExitCommand { get; }

        private void ExitWindow()
        {
            Application.Current.Shutdown();
        }
        private void StartMode()
        {
            D data = D.Instance;
            Locations loc = data.getCurrentLocation();


            ValueX = loc.x.ToString("F4");
            ValueY = loc.y.ToString("F4");
            ValueZ = loc.z.ToString("F4");
            ValueRx = loc.rx.ToString("F4");
            ValueRy = loc.ry.ToString("F4");
            ValueRz = loc.rz.ToString("F4");

        }
        private void StopMode()
        {
        }
        private void ResetMode()
        {

            // 추가적인 Auto 모드 로직
        }


        public void OnWorkPieceUpdate(WorkPiece workPiece)
        {
            //Console.WriteLine($"WorkPiece received: {workPiece.Name}, ID: {workPiece.ID}");
            
            ValueWorkPiece = workPiece.wpnumber.ToString();
            ValueAmountLeft = workPiece.toolamount.ToString();
            ValueNCProgram = workPiece.ncprogram.ToString();

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private EViewName _selectedView;
        public EViewName SelectedView
        {
            get => _selectedView;
            set
            {
                SetProperty(ref _selectedView, value);

                OnPropertyChanged();
                //if (_selectedView == value)
                //    SetProperty(ref _selectedView, EViewName.None);
                //else
                //    SetProperty(ref _selectedView, value);
            }
        }

        public ICommand TabSelectCommand { get; }



        public UserControl1ViewModel()
        {
            TabSelectCommand = new RelayCommand<string?>(ChangeView);

            //SelectedView = EViewName.None;

            //SelectedView = EViewName.WorkPiece2;
            //SelectedView = EViewName.Diagnostic;

            //SelectedView = EViewName.Automat;
            SelectedView = EViewName.Automat;

            ChangeView(EViewName.Automat.ToString());


            AutoCommand = new UserControlRelayCommand(ExecuteAuto);
            SemiCommand = new UserControlRelayCommand(ExecuteSemi);
            ManualCommand = new UserControlRelayCommand(ExecuteManual);

            StartCommand = new RelayCommand(StartMode);
            PauseCommand = new RelayCommand(StopMode);
            ResetCommand = new RelayCommand(ResetMode);
            ExitCommand = new RelayCommand(ExitWindow);


        }

        private void ChangeView(string? viewName)
        {
            SelectedView = (Enum.TryParse(viewName ?? "None", out EViewName view)) ? view : EViewName.None;

            TapSelect = SelectedView.ToString();


            Console.WriteLine(SelectedView);
        }
    }

    public class UserControlRelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public UserControlRelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        public void Execute(object parameter) => _execute();

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

   


}
