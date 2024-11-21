using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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

using HermleCS.Comm;
using HermleCS.Data;
using static Hermle_Auto.Views.WorkPiece2View;

namespace Hermle_Auto.Views
{
    /// <summary>
    /// WorkPiece2View.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WorkPiece2View : UserControl
    {
        D d;
        
        // WorkPiece 값을 인자로 받아 처리하는 델리게이트 정의
        public Action<HermleCS.Data.WorkPiece> WorkPieceChanged { get; set; }


/*        public string WpOptionLineNum
        {
            get => _wpOptionLineNum;
            set
            {
                if (_wpOptionLineNum != value)
                {
                    _wpOptionLineNum = value;
                    //OnPropertyChanged();
                }
            }
        }*/

        private int currentLineNumber = 1;
        //private string _wpOptionLineNum = "0";

        private void IncreaseLineNum(object sender, RoutedEventArgs e)
        {
            currentLineNumber++;

            currentLineNumber = Math.Min(50, currentLineNumber);

            //WpOptionLineNum = currentLineNumber.ToString();
            WPLineNumberTextBox.Text = currentLineNumber.ToString();
        }
        private void DecreaseLineNum(object sender, RoutedEventArgs e)
        {
            currentLineNumber--;

            currentLineNumber = Math.Max(1, currentLineNumber);

            WPLineNumberTextBox.Text = currentLineNumber.ToString();

            //WpOptionLineNum = currentLineNumber.ToString();
        }

        // Data model for Work Piece
        public class WorkPiece: INotifyPropertyChanged
        {
            public string ToolType { get; set; }
            public int WorkPiece2 { get; set; }
            public int NCProgram { get; set; }
            public int ToolDiameter { get; set; }
            public int ToolAmount { get; set; }
            //public int ToolAmountLeft { get; set; }
            public int LineNumber { get; set; }


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


        // Observable collection to hold the data for DataGrid
        public ObservableCollection<WorkPiece> WorkPieces2 { get; set; } = new ObservableCollection<WorkPiece>();
        public WorkPiece2View()
        {
            InitializeComponent();

            //this.DataContext = this;

            WorkPieceTable.ItemsSource = WorkPieces2;

            d = D.Instance;





            WorkPieceLoadCSV();

            /*  foreach (HermleCS.Data.WorkPiece piece in d.WorkPiecesList)
              {
                  C.log($"WorkPiecesList :{piece.wpnumber} : {piece.ncprogram}");
              }*/

            //C.log($"WorkPiecesList :{d.getWorkPiecesList(d.WorkPiecesList)}");

        }


        public void DeleteLineNum(object sender, RoutedEventArgs e)
        {
            int index = currentLineNumber - 1;

            if (index == 0)
            {
                MessageBox.Show("Line 1 번 삭제");
            }
            if (index == WorkPieces2.Count)
            {
                return;
            }

            WorkPieces2.RemoveAt(index);

            UpdateWorkPiecesLineNum();
            SetWorkPiece();

        }
        public void UpLineNum(object sender, RoutedEventArgs e)
        {
            int index = currentLineNumber-1;

            if(index == 0)
            {
                return;
            }

            // 인덱스가 범위 내에 있고 첫 번째 요소가 아니어야만 위로 이동 가능
            if (index < WorkPieces2.Count)
            {
                // 현재 요소와 이전 요소 교환
                var temp = WorkPieces2[index];
                WorkPieces2[index] = WorkPieces2[index - 1];
                WorkPieces2[index - 1] = temp;

                UpdateWorkPiecesLineNum();
                SetWorkPiece();
            }
        }
        public void DownLineNum(object sender, RoutedEventArgs e)
        {
            int index = currentLineNumber - 1;

            if (index == WorkPieces2.Count)
            {
                return;
            }
           

            // 인덱스가 범위 내에 있고 첫 번째 요소가 아니어야만 위로 이동 가능
            if (index < WorkPieces2.Count)
            {
                // 현재 요소와 이전 요소 교환
                var temp = WorkPieces2[index];
                WorkPieces2[index] = WorkPieces2[index + 1];
                WorkPieces2[index + 1] = temp;

                UpdateWorkPiecesLineNum();
                SetWorkPiece();
            }
        }
        private void UpdateWorkPiecesLineNum()
        {
            int i = 1;
            foreach (WorkPiece item in WorkPieces2)
            {
                item.LineNumber = i;
                i++;

                item.AllPropertyChanged();
            }
        }

        private void SetWorkPiece()
        {



            int i = 0;

            for (i = 0; i < WorkPieces2.Count; i++)
            { 
                d.WorkPiecesList[i].wpnumber = 0;
                d.WorkPiecesList[i].ncprogram = 0;
                d.WorkPiecesList[i].toolamount = 0;
                d.WorkPiecesList[i].toolamountleft = 0;
                d.WorkPiecesList[i].tooldiameter = 0;
                d.WorkPiecesList[i].wptooltype = "";
            }

            i = 0;
            foreach (WorkPiece item in WorkPieces2)
            {
                d.WorkPiecesList[i].wpnumber        = item.WorkPiece2;
                d.WorkPiecesList[i].ncprogram       = item.NCProgram;
                d.WorkPiecesList[i].toolamount      = item.ToolAmount; 
                //d.WorkPiecesList[i].toolamountleft  = item.Tool;
                d.WorkPiecesList[i].tooldiameter    = item.ToolDiameter;
                d.WorkPiecesList[i].wptooltype      = item.ToolType;

                i++;
            }

            d.WriteWorkPieceList();

        }
        
        
        private void WorkPieceLoadCSV()
        {
            try
            {
                int readlines = 0;

                C.ApplicationPath = ".\\";

                d.ReadWorkPieceList();

                WorkPieces2.Clear();

                foreach (HermleCS.Data.WorkPiece piece in d.WorkPiecesList)
                {
                    WorkPiece wp = new WorkPiece();

                    wp.LineNumber = readlines + 1;
                    wp.WorkPiece2 = piece.wpnumber;
                    wp.NCProgram = piece.ncprogram;
                    wp.ToolAmount = piece.toolamount;
                    //wp.ToolAmountLeft = piece.toolamountleft;
                    wp.ToolDiameter = piece.tooldiameter;
                    wp.ToolType = piece.wptooltype;

                    readlines++;

                    WorkPieces2.Add(wp);

                   //C.log($"WorkPiecesList :{piece.wptooltype}");


                }

            }
            catch (Exception)
            {

                throw;
            }
        }


        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            WorkPieceLoadCSV();
        }

            // Event handler for the button click
       private void AddWorkPieceButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a new WorkPiece object and populate with the values from the TextBoxes and ComboBox

            try
            {
                if(lineNumberTextBox.Text == "" || WorkPieces2.Count < GetTextBoxTextToInt(lineNumberTextBox.Text))
                {
                    var newWorkPiece = new WorkPiece
                    {
                        ToolType = (toolTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                        WorkPiece2 = GetTextBoxTextToInt(workPieceTextBox.Text),
                        NCProgram = GetTextBoxTextToInt(ncProgramTextBox.Text),
                        ToolDiameter = GetTextBoxTextToInt(toolDiameterTextBox.Text),
                        ToolAmount = GetTextBoxTextToInt(toolAmountTextBox.Text),
                        //ToolAmountLeft = toolAmountTextBoxLeft.Text,
                        //LineNumber = GetTextBoxTextToInt(lineNumberTextBox.Text)
                        LineNumber = WorkPieces2.Count+1
                    };

                    // Add the new WorkPiece to the ObservableCollection
                    WorkPieces2.Add(newWorkPiece);

                   
                }
                else
                {
                    int index = GetTextBoxTextToInt(lineNumberTextBox.Text);


                    int value = GetTextBoxTextToInt(toolDiameterTextBox.Text);

                    if ((toolTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() == "Drill")
                    {
                        if (value < 1 || value > 7)
                        {
                            MessageBox.Show("1 ~ 7 사이 정수만 넣어주세요");
                            return;
                        }


                    }
                    else if((toolTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() == "HSK")
                    {

                        if (value != 100 && value != 200 && value != 300)
                        {
                            MessageBox.Show("100, 200, 300 만 넣어주세요");
                            return;
                        }
                    }
                    else if((toolTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() == "Round")
                    {
                        if (value < 1 || value > 8)
                        {
                            MessageBox.Show("1 ~ 8 사이 정수만 넣어주세요");
                            return;
                        }

                    }


                    WorkPieces2[index-1].ToolType = (toolTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                    WorkPieces2[index-1].WorkPiece2 = GetTextBoxTextToInt(workPieceTextBox.Text);
                    WorkPieces2[index-1].NCProgram = GetTextBoxTextToInt(ncProgramTextBox.Text);
                    WorkPieces2[index-1].ToolDiameter = GetTextBoxTextToInt(toolDiameterTextBox.Text);
                    WorkPieces2[index-1].ToolAmount = GetTextBoxTextToInt(toolAmountTextBox.Text);
                    WorkPieces2[index-1].AllPropertyChanged();

                }

                d.WriteWorkPieceList();

                // Clear the input fields after adding
                ClearInputFields();
                SetWorkPiece();
            }
            catch (Exception)
            {

                throw;
            }
            
        }
        private int GetTextBoxTextToInt(string tb)
        {
            if (!int.TryParse(tb, out int result))
            {
                // 정수가 아니면 0으로 설정
                result = 0;
            }
            return result;
        }



        private bool CheckToolAmount()
        {

            int ToolAmountCount = 0;

            for (int i = 0; i < WorkPieces2.Count; i++)
            {
                ToolAmountCount += WorkPieces2[i].ToolAmount;
            }


            int TotalDRILL = 0;
            int TotalHSK = 0;
            int TotalROUND = 0;

            if ((toolTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() == "Drill")
            {
                if (ToolAmountCount > (3 * TotalDRILL))
                {
                    return false;
                }
                else
                {
                    return true ;
                }
            }
            else if ((toolTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() == "HSK")
            {

                if (ToolAmountCount > (3 * TotalHSK))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else if ((toolTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() == "Round")
            {
                if (ToolAmountCount > (3 * TotalROUND))
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }



            return false;
        }



        // Method to clear the input fields
        private void ClearInputFields()
        {
            toolTypeComboBox.SelectedIndex = -1;
            workPieceTextBox.Clear();
            ncProgramTextBox.Clear();
            toolDiameterTextBox.Clear();
            toolAmountTextBox.Clear();
            lineNumberTextBox.Clear();
        }

        private void SetInputField(int index)
        {
            toolTypeComboBox.SelectedIndex = WorkPieces2[index].ToolType == "Drill" ? 0 : WorkPieces2[index].ToolType == "HSK" ? 1  : WorkPieces2[index].ToolType == "Round" ? 2 : -1;
            workPieceTextBox.Text =         WorkPieces2[index].WorkPiece2 == null ? "" : WorkPieces2[index].WorkPiece2.ToString();
            ncProgramTextBox.Text =         WorkPieces2[index].NCProgram == null ? "" : WorkPieces2[index].NCProgram.ToString();
            toolDiameterTextBox.Text =      WorkPieces2[index].ToolDiameter == null ? "" : WorkPieces2[index].ToolDiameter.ToString();
            toolAmountTextBox.Text =        WorkPieces2[index].ToolAmount == null ? "" : WorkPieces2[index].ToolAmount.ToString();
            lineNumberTextBox.Text =        WorkPieces2[index].LineNumber == null ? "" : WorkPieces2[index].LineNumber.ToString();

        }

        private void myDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            // 선택된 항목 가져오기
            var selectedItem = WorkPieceTable.SelectedItem;

            if (selectedItem != null)
            {
                // 선택된 행의 인덱스 가져오기
                int rowIndex = WorkPieceTable.Items.IndexOf(selectedItem);
                d.CurrentWorkPieceIndex = rowIndex;

                // 행 인덱스 출력
                //C.log($"Selected Row Index: {rowIndex}");
                SetInputField(rowIndex);

                HermleCS.Data.WorkPiece wp = new HermleCS.Data.WorkPiece();

                wp.ncprogram = WorkPieces2[rowIndex].NCProgram;
                wp.wpnumber = WorkPieces2[rowIndex].WorkPiece2;
                wp.wptooltype = WorkPieces2[rowIndex].ToolType;
                wp.tooldiameter = WorkPieces2[rowIndex].ToolDiameter;
                wp.toolamount = WorkPieces2[rowIndex].ToolAmount;


                WorkPieceChanged?.Invoke(wp);


            }

         
        }
    }
}

