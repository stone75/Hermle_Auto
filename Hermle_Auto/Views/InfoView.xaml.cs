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
    /// InfoView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InfoView : UserControl
    {
        public InfoView()
        {
            InitializeComponent();
        }
        private void IncreaseShelf(object sender, RoutedEventArgs e)
        {
            //currentShelf++;

            //currentShelf = Math.Min(50, currentShelf);

            //WpOptionLineNum = currentLineNumber.ToString();
            //ShelfTextBox.Text = currentShelf.ToString();
        }
        private void DecreaseShelf(object sender, RoutedEventArgs e)
        {
            //currentShelf--;

            //currentShelf = Math.Max(1, currentShelf);

            //ShelfTextBox.Text = currentShelf.ToString();

            //WpOptionLineNum = currentLineNumber.ToString();
        }

        private void IncreasePocket(object sender, RoutedEventArgs e)
        {
            //currentPocket++;

            //currentPocket = Math.Min(50, currentPocket);

            //WpOptionLineNum = currentLineNumber.ToString();
            //PocketTextBox.Text = currentPocket.ToString();
        }
        private void DecreasePocket(object sender, RoutedEventArgs e)
        {
            //currentPocket--;

            // currentPocket = Math.Max(1, currentPocket);

            //PocketTextBox.Text = currentPocket.ToString();

            //WpOptionLineNum = currentLineNumber.ToString();
        }

        private void IncreaseDrillCode(object sender, RoutedEventArgs e)
        {
            //currentDrillCode++;

            //currentDrillCode = Math.Min(50, currentDrillCode);

            //WpOptionLineNum = currentLineNumber.ToString();
            //DrillCodeTextBox.Text = currentDrillCode.ToString();
        }
        private void DecreaseDrillCode(object sender, RoutedEventArgs e)
        {
            //currentDrillCode--;

            //currentDrillCode = Math.Max(1, currentDrillCode);

            //DrillCodeTextBox.Text = currentDrillCode.ToString();

            //WpOptionLineNum = currentLineNumber.ToString();
        }


        private void IncreaseDrillCore(object sender, RoutedEventArgs e)
        {
            //currentDrillCode++;

            //currentDrillCode = Math.Min(50, currentDrillCode);

            //WpOptionLineNum = currentLineNumber.ToString();
            //DrillCodeTextBox.Text = currentDrillCode.ToString();
        }
        private void DecreaseDrillCore(object sender, RoutedEventArgs e)
        {
            //currentDrillCode--;

            //currentDrillCode = Math.Max(1, currentDrillCode);

            //DrillCodeTextBox.Text = currentDrillCode.ToString();

            //WpOptionLineNum = currentLineNumber.ToString();
        }


        private void IncreasePocketCount(object sender, RoutedEventArgs e)
        {
            //currentDrillCode++;

            //currentDrillCode = Math.Min(50, currentDrillCode);

            //WpOptionLineNum = currentLineNumber.ToString();
            //DrillCodeTextBox.Text = currentDrillCode.ToString();
        }
        private void DecreasePocketCount(object sender, RoutedEventArgs e)
        {
            //currentDrillCode--;

            //currentDrillCode = Math.Max(1, currentDrillCode);

            //DrillCodeTextBox.Text = currentDrillCode.ToString();

            //WpOptionLineNum = currentLineNumber.ToString();
        }
    }
}
