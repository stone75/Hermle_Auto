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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hermle_Auto.Views
{
    /// <summary>
    /// Interaction logic for TeachView.xaml
    /// </summary>
    public partial class TeachView : UserControl
    {

        private int currentShelf = 1;
        private int currentPocket = 1;
        private int currentDrillCode = 1;


        private int currentViewPocketShelf = 1;
        private int currentViewPocket = 100;


        private ObservableCollection<CoordinatePoint> coordinates;
        public CoordinatePoint FirstPoint { get; private set; }
        public CoordinatePoint LastPoint { get; private set; }


        public TeachView()
        {
            InitializeComponent();

            //this.DataContext = this;

            InitializeCoordinates();
        }
        private void InitializeCoordinates()
        {
            coordinates = new ObservableCollection<CoordinatePoint>();

            FirstPoint = new CoordinatePoint
            {
                PointType = "First Point:",
                X = 829.482,
                Y = -828.355,
                Z = 885.123
            };

            LastPoint = new CoordinatePoint
            {
                PointType = "Last Point:",
                X = -646.712,
                Y = -976.163,
                Z = 886.123
            };

            coordinates.Add(FirstPoint);
            coordinates.Add(LastPoint);

            CoordinateGrid.ItemsSource = coordinates;
        }
        // 좌표 업데이트 메서드
        public void UpdateFirstPoint(double x, double y, double z)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                FirstPoint.X = x;
                FirstPoint.Y = y;
                FirstPoint.Z = z;
            });
        }

        public void UpdateLastPoint(double x, double y, double z)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                LastPoint.X = x;
                LastPoint.Y = y;
                LastPoint.Z = z;
            });
        }


        private void IncreaseShelf(object sender, RoutedEventArgs e)
        {
            currentShelf++;

            //currentShelf = Math.Min(50, currentShelf);

            //WpOptionLineNum = currentLineNumber.ToString();
            ShelfTextBox.Text = currentShelf.ToString();
        }
        private void DecreaseShelf(object sender, RoutedEventArgs e)
        {
            currentShelf--;

            currentShelf = Math.Max(1, currentShelf);

            ShelfTextBox.Text = currentShelf.ToString();

            //WpOptionLineNum = currentLineNumber.ToString();
        }

        private void IncreasePocket(object sender, RoutedEventArgs e)
        {
            currentPocket++;

            //currentPocket = Math.Min(50, currentPocket);

            //WpOptionLineNum = currentLineNumber.ToString();
            PocketTextBox.Text = currentPocket.ToString();
        }
        private void DecreasePocket(object sender, RoutedEventArgs e)
        {
            currentPocket--;

            currentPocket = Math.Max(1, currentPocket);

            PocketTextBox.Text = currentPocket.ToString();

            //WpOptionLineNum = currentLineNumber.ToString();
        }

        private void IncreaseDrillCode(object sender, RoutedEventArgs e)
        {
            currentDrillCode++;

            //currentDrillCode = Math.Min(50, currentDrillCode);

            //WpOptionLineNum = currentLineNumber.ToString();
            DrillCodeTextBox.Text = currentDrillCode.ToString();
        }
        private void DecreaseDrillCode(object sender, RoutedEventArgs e)
        {
            currentDrillCode--;

            currentDrillCode = Math.Max(1, currentDrillCode);

            DrillCodeTextBox.Text = currentDrillCode.ToString();

            //WpOptionLineNum = currentLineNumber.ToString();
        }


        private void IncreaseViewPocketShelf(object sender, RoutedEventArgs e)
        {
            currentViewPocketShelf++;

            //currentDrillCode = Math.Min(50, currentDrillCode);

            //WpOptionLineNum = currentLineNumber.ToString();
            ViewPocketShelfTextBox.Text = currentViewPocketShelf.ToString();
        }
        private void DecreaseViewPocketShelf(object sender, RoutedEventArgs e)
        {
            currentViewPocketShelf--;

            currentViewPocketShelf = Math.Max(1, currentViewPocketShelf);

            ViewPocketShelfTextBox.Text = currentViewPocketShelf.ToString();

            //WpOptionLineNum = currentLineNumber.ToString();
        }


        private void IncreaseViewPocketPocket(object sender, RoutedEventArgs e)
        {
            currentViewPocket++;

            //currentDrillCode = Math.Min(50, currentDrillCode);

            //WpOptionLineNum = currentLineNumber.ToString();
            ViewPocketPocketTextBox.Text = currentViewPocket.ToString();
        }
        private void DecreaseViewPocketPocket(object sender, RoutedEventArgs e)
        {
            currentViewPocket--;

            currentViewPocket = Math.Max(1, currentViewPocket);

            ViewPocketPocketTextBox.Text = currentViewPocket.ToString();

            //WpOptionLineNum = currentLineNumber.ToString();
        }




        Dictionary<string, string> DicNumName = new Dictionary<string, string>()
            {
                { "10","Spindle" },
                { "11","Kiosk" },
                { "12","Chuck" },
                { "120","Station 1" },
                { "121","Station 2" },
            };

        private void RefreshTeachGeneralLocations()
        {
            PocketTable.ItemsSource = null;


            var toolType = D.Instance.GetToolType();
            //var toolType = "DRILL";
            //var toolType = "HSK";
            //var toolType = "ROUND";
            //var toolType = "";

            var ret = D.Instance.ReadGeneralLocations(toolType);
            if (ret < 0)
            {
                // Error
                return;
            }

            var locations = D.Instance.getGeneralLocations(toolType);
            if (locations == null)
            {
                // No locations
                return;
            }

            var data = new[]
            {
                new { Number="", Name="", X=0.0, Y=0.0, Z=0.0, },
            }.ToList();
            data.Clear();

            foreach (var l in locations)
            {
                var number = l.name;
                if (DicNumName.ContainsKey(number))
                {
                    var name = DicNumName[number];
                    data.Add(new { Number = number, Name = name, X = l.x, Y = l.y, Z = l.z, });
                }
            }

            PocketTable.ItemsSource = data;
        }

        private void TeachPositionButton_Click(object sender, RoutedEventArgs e)
        {
            TeachPositionLocations();

        }
        private void TeachPositionLocations()
        {
            //1 - Robot : CurrentPosition recv
            //2 GeneralLcation input


            if (SelectLocation.Text == "Kiosk")
            {
                // Save position to the memory of PC.
                /*             GeneralLocation[11].X = TempPosition[0];
                               GeneralLocation[11].Y = TempPosition[1];
                               GeneralLocation[11].Z = TempPosition[2];
                               GeneralLocation[11].Rx = TempPosition[3];
                               GeneralLocation[11].Ry = TempPosition[4];
                               GeneralLocation[11].Rz = TempPosition[5]; */
                // GeneralLocation = Robot Current Position 
                // 3-1 : PC -> Robot Write Position(Robot Current Position )
                // 3-2 : PC -> Robot H_COMMAND(calc_kiosk_points.TP)
            }
            else if (SelectLocation.Text == "Chuck")
            {
                /*                GeneralLocation[12].X = TempPosition[0];
                                GeneralLocation[12].Y = TempPosition[1];
                                GeneralLocation[12].Z = TempPosition[2];
                                GeneralLocation[12].Rx = TempPosition[3];
                                GeneralLocation[12].Ry = TempPosition[4];
                                GeneralLocation[12].Rz = TempPosition[5]; */
                // GeneralLocation = Robot Current Position 
                // 3-1 : PC -> Robot Write Position(Robot Current Position )
                // 3-2 :PC -> Robot H_COMMAND(calc_chuck_points.TP)

            }
            else if (SelectLocation.Text == "Spindle")
            {
                /*               GeneralLocation[10].X = TempPosition[0];
                               GeneralLocation[10].Y = TempPosition[1];
                               GeneralLocation[10].Z = TempPosition[2];
                               GeneralLocation[10].Rx = TempPosition[3];
                               GeneralLocation[10].Ry = TempPosition[4];
                               GeneralLocation[10].Rz = TempPosition[5];
                */
                // 3-1 : GeneralLocation = Robot Current Position 
                // 3-2 : PC -> Robot Write Position(Robot Current Position )

            }
            else if (SelectLocation.Text == "Station 1")
            {
                /*               GeneralLocation[120].X = TempPosition[0];
                                GeneralLocation[120].Y = TempPosition[1];
                                GeneralLocation[120].Z = TempPosition[2];
                                GeneralLocation[120].Rx = TempPosition[3];
                                GeneralLocation[120].Ry = TempPosition[4];
                                GeneralLocation[120].Rz = TempPosition[5];
                */
                // 3-1 : GeneralLocation = Robot Current Position 
                // 3-2 : PC -> Robot Write Position(Robot Current Position )

            }
            else if (SelectLocation.Text == "Station 2")
            {
                /*          GeneralLocation[121].X = TempPosition[0];
                            GeneralLocation[121].Y = TempPosition[1];
                            GeneralLocation[121].Z = TempPosition[2];
                            GeneralLocation[121].Rx = TempPosition[3];
                            GeneralLocation[121].Ry = TempPosition[4];
                            GeneralLocation[121].Rz = TempPosition[5];
                */
                // 3-1 : GeneralLocation = Robot Current Position 
                // 3-2 : PC -> Robot Write Position(Robot Current Position )
            }
            else if (SelectLocation.Text == "Select Location")
            {
                MessageBox.Show("Please select a valid location.", "Alert", MessageBoxButton.OK, MessageBoxImage.Warning);
            }


            var toolType = D.Instance.GetToolType();

            D.Instance.WriteGeneralLocations(toolType);



        }
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshTeachGeneralLocations();
        }

        private Dictionary<string, List<Locations>> ShelvesLocations { get; set; }
            = new Dictionary<string, List<Locations>>();

        private void TeachFirstButton_Click(object sender, RoutedEventArgs e)
        {
            var shelf = ShelfTextBox.Text;
            if (!ShelvesLocations.Keys.Contains(shelf))
                ShelvesLocations.Add(shelf, new List<Locations>());

            var locations = ShelvesLocations[shelf];

            var loc = D.Instance.getCurrentLocation();
            if (locations.Count < 1)
                locations.Add(loc);
            else
                locations[0] = loc;

            // Data
            CoordinateGrid.ItemsSource = null;

            var data = new[] {
                new { X=0.0, Y=0.0, Z=0.0, },
            }.ToList();
            data.Clear();

            foreach (var l in locations)
            {
                data.Add(new { X = l.x, Y = l.y, Z = l.z, });
            }

            CoordinateGrid.ItemsSource = data;
        }

        private void TeachLastButton_Click(object sender, RoutedEventArgs e)
        {
            var shelf = ShelfTextBox.Text;
            if (!ShelvesLocations.Keys.Contains(shelf))
                ShelvesLocations.Add(shelf, new List<Locations>());

            var locations = ShelvesLocations[shelf];

            var loc = D.Instance.getCurrentLocation();

            if (locations.Count < 1)
                locations.Add(loc);

            if (locations.Count < 2)
                locations.Add(loc);
            else
                locations[1] = loc;

            // Data
            CoordinateGrid.ItemsSource = null;

            var data = new[] {
                new { X=0.0, Y=0.0, Z=0.0, },
            }.ToList();
            data.Clear();

            foreach (var l in locations)
            {
                data.Add(new { X = l.x, Y = l.y, Z = l.z, });
            }

            CoordinateGrid.ItemsSource = data;
        }

        private void CalcButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ShelvesRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            CoordinateGrid.ItemsSource = null;

            var shelf = ShelfTextBox.Text;
            if (!ShelvesLocations.Keys.Contains(shelf))
                ShelvesLocations.Add(shelf, new List<Locations>());

            var locations = ShelvesLocations[shelf];

            CoordinateGrid.ItemsSource = locations;
        }

        private void TeachSinglePocketButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ShowPocketLocationButton_Click(object sender, RoutedEventArgs e)
        {
            var shelf = () => int.TryParse(
                ViewPocketShelfTextBox.Text, out int n) ? n : 0;
            var pocket = () => int.TryParse(
                ViewPocketPocketTextBox.Text, out int n) ? n : 0;

            var toolname = "HSK";

            // Tool Type 변수 설정 필요

            //var toolname = D.Instance.GetToolType;

            List<Locations> locations = D.Instance.GetPocketLocation(toolname);

            // Data
            CoordinateGrid.ItemsSource = null;

            var data = new[]{
                new {PocketNumber = "", X=0.0, Y=0.0, Z=0.0, Rx=0.0, Ry=0.0, Rz=0.0,},
            }.ToList();
            data.Clear();

            int pocketNumber = 1; // 초기 값 설정

            foreach (var l in locations)
            {

                data.Add(new { PocketNumber = pocketNumber.ToString(), X = l.x, Y = l.y, Z = l.z, Rx = l.rx, Ry = l.ry, Rz = l.rz, });

                pocketNumber++;
            }

            PocketTable2.ItemsSource = data;
        }
    }

    public class CoordinatePoint : INotifyPropertyChanged
    {
        private double _x;
        private double _y;
        private double _z;

        public string PointType { get; set; }

        public double X
        {
            get => _x;
            set
            {
                if (_x != value)
                {
                    _x = value;
                    OnPropertyChanged(nameof(X));
                }
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                if (_y != value)
                {
                    _y = value;
                    OnPropertyChanged(nameof(Y));
                }
            }
        }

        public double Z
        {
            get => _z;
            set
            {
                if (_z != value)
                {
                    _z = value;
                    OnPropertyChanged(nameof(Z));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
