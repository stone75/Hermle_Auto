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
    /// ToolsView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ToolsView : UserControl
    {
        public ToolsView()
        {
            InitializeComponent();
        }


        private void IncreasePocket(object sender, RoutedEventArgs e)
        {
            //currentShelf++;

            //currentShelf = Math.Min(50, currentShelf);

            //WpOptionLineNum = currentLineNumber.ToString();
            //ShelfTextBox.Text = currentShelf.ToString();
        }
        private void DecreasePocket(object sender, RoutedEventArgs e)
        {
           /* currentShelf--;

            currentShelf = Math.Max(1, currentShelf);*/

            //ShelfTextBox.Text = currentShelf.ToString();

            //WpOptionLineNum = currentLineNumber.ToString();
        }
    }
}
