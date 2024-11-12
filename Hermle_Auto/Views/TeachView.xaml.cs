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
            //ShelfTextBox.Text = currentShelf.ToString();
        }
        private void DecreaseShelf(object sender, RoutedEventArgs e)
        {
            currentShelf--;

            currentShelf = Math.Max(1, currentShelf);

            //ShelfTextBox.Text = currentShelf.ToString();

            //WpOptionLineNum = currentLineNumber.ToString();
        }

        private void IncreasePocket(object sender, RoutedEventArgs e)
        {
            currentPocket++;

            //currentPocket = Math.Min(50, currentPocket);

            //WpOptionLineNum = currentLineNumber.ToString();
            //PocketTextBox.Text = currentPocket.ToString();
        }
        private void DecreasePocket(object sender, RoutedEventArgs e)
        {
            currentPocket--;

            currentPocket = Math.Max(1, currentPocket);

            //PocketTextBox.Text = currentPocket.ToString();

            //WpOptionLineNum = currentLineNumber.ToString();
        }

        private void IncreaseDrillCode(object sender, RoutedEventArgs e)
        {
            currentDrillCode++;

            //currentDrillCode = Math.Min(50, currentDrillCode);

            //WpOptionLineNum = currentLineNumber.ToString();
            //DrillCodeTextBox.Text = currentDrillCode.ToString();
        }
        private void DecreaseDrillCode(object sender, RoutedEventArgs e)
        {
            currentDrillCode--;

            currentDrillCode = Math.Max(1, currentDrillCode);

            //DrillCodeTextBox.Text = currentDrillCode.ToString();

            //WpOptionLineNum = currentLineNumber.ToString();
        }


        private void IncreaseDrillCore(object sender, RoutedEventArgs e)
        {
            currentDrillCode++;

            //currentDrillCode = Math.Min(50, currentDrillCode);

            //WpOptionLineNum = currentLineNumber.ToString();
            //DrillCodeTextBox.Text = currentDrillCode.ToString();
        }
        private void DecreaseDrillCore(object sender, RoutedEventArgs e)
        {
            currentDrillCode--;

            currentDrillCode = Math.Max(1, currentDrillCode);

            //DrillCodeTextBox.Text = currentDrillCode.ToString();

            //WpOptionLineNum = currentLineNumber.ToString();
        }


        private void IncreasePocketCount(object sender, RoutedEventArgs e)
        {
            currentDrillCode++;

            //currentDrillCode = Math.Min(50, currentDrillCode);

            //WpOptionLineNum = currentLineNumber.ToString();
            //DrillCodeTextBox.Text = currentDrillCode.ToString();
        }
        private void DecreasePocketCount(object sender, RoutedEventArgs e)
        {
            currentDrillCode--;

            currentDrillCode = Math.Max(1, currentDrillCode);

            //DrillCodeTextBox.Text = currentDrillCode.ToString();

            //WpOptionLineNum = currentLineNumber.ToString();
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
