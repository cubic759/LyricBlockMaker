using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LyricBlockMaker
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>

    public partial class MainWindow : Window
    {
        HistoryRecorder recorder = new HistoryRecorder();
        public MainWindow()
        {
            InitializeComponent();
            recorder.setGrid(grid);
            string[] cmdLine = Environment.GetCommandLineArgs();
            if (cmdLine.Length > 1)
            {
                if (cmdLine[1].EndsWith(".txt") || cmdLine[1].EndsWith(".blk"))
                {
                    StreamReader sr = new StreamReader(cmdLine[1]);
                    string s = sr.ReadToEnd();
                    initiate(s, cmdLine[1]);
                    if (cmdLine.Length > 2)
                    {
                        for (int i = 2; i < cmdLine.Length; i++)
                        {
                            if (cmdLine[i].EndsWith(".txt") || cmdLine[i].EndsWith(".blk"))
                            {
                                sr = new StreamReader(cmdLine[i]);
                                string s1 = sr.ReadToEnd();
                                MainWindow window = new MainWindow(true);
                                window.initiate(s1, cmdLine[i]);
                                window.Show();
                            }
                            else
                            {
                                MessageBox.Show("不支持." + cmdLine[i].Split('.')[1] + "文件类型。仅支持文件类型为.txt、.blk。");
                            }
                        }
                    }
                    sr.Close();
                }
                else
                {
                    MessageBox.Show("不支持." + cmdLine[1].Split('.')[1] + "文件类型。仅支持文件类型为.txt、.blk。");
                }
            }
        }
        public MainWindow(bool isMultiple)
        {
            InitializeComponent();
            recorder.setGrid(grid);
        }

        #region B
        String beforeText = "";
        private void tb_PreviewTextInput(object sender, TextCompositionEventArgs e)//列方块输入的单个字符
        {
            isSaved = false;
            if (!Title.EndsWith("*"))
            {
                Title += "*";
            }
            TextBox box = (TextBox)sender;
            Regex re = new Regex("[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
            beforeText = box.Text;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)//检查全部数字
        {
            isSaved = false;
            if (!Title.EndsWith("*"))
            {
                Title += "*";
            }
            TextBox box = (TextBox)sender;
            String t = box.Text;
            if (t != null && t != "")
            {
                int n = int.Parse(t);
                StackPanel panel = (StackPanel)box.Parent;
                int number = panel.Children.Count - 1;
                if (n <= 100 && n > 0)
                {
                    if (n < number)//移除多余
                    {
                        for (int i = number; i > n; i--)
                        {
                            panel.Children.RemoveAt(i);
                        }
                    }
                    else if (n > number)
                    {
                        for (int i = 0; i < n - number; i++)
                        {
                            var a = new TextBox();//文字格
                            a.Width = 20;
                            a.Margin = new Thickness(5, 0, 0, 0);
                            a.HorizontalContentAlignment = HorizontalAlignment.Center;
                            a.VerticalContentAlignment = VerticalAlignment.Center;
                            a.TextChanged += A_TextChanged;
                            a.PreviewKeyDown += A_KeyDown;
                            a.PreviewMouseDown += A_PreviewMouseDown;
                            a.MouseMove += A_MouseMove;
                            a.LostFocus += A_LostFocus;
                            a.GotFocus += A_GotFocus;
                            panel.Children.Add(a);
                        }
                        StackPanel lastPanel = (StackPanel)grid.Children[grid.Children.Count - 1];
                        if (lastPanel.Children.Count != 1)
                        {
                            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
                            StackPanel panel1 = new StackPanel();
                            panel1.Orientation = Orientation.Horizontal;
                            panel1.Height = 20;
                            panel1.Margin = new Thickness(0, 10, 0, 0);
                            panel1.SetValue(Grid.RowProperty, grid.RowDefinitions.Count - 1);
                            grid.Children.Add(panel1);
                            var b = new TextBox();//数字格 列格
                            b.Width = 20;
                            b.Margin = new Thickness(10, 0, 10, 0);
                            b.HorizontalContentAlignment = HorizontalAlignment.Center;
                            b.VerticalContentAlignment = VerticalAlignment.Center;
                            b.PreviewTextInput += tb_PreviewTextInput;
                            b.TextChanged += TextBox_TextChanged;
                            b.PreviewKeyDown += B_PreviewKeyDown;
                            b.SetValue(InputMethod.IsInputMethodEnabledProperty, false);
                            panel1.Children.Add(b);
                        }
                    }
                }
                else
                {
                    box.Text = beforeText;
                    MessageBox.Show("请输入数字1-100");
                }
            }
        }

        private void B_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox box = (TextBox)sender;
            StackPanel panel = (StackPanel)box.Parent;
            if (e.Key == Key.Left)
            {
                e.Handled = true;
                if (grid.Children.IndexOf(panel) != 0)
                {
                    StackPanel previousPanel = (StackPanel)grid.Children[grid.Children.IndexOf(panel) - 1];
                    TextBox previousBox = (TextBox)previousPanel.Children[previousPanel.Children.Count - 1];
                    previousBox.Focus();
                    previousBox.SelectionStart = previousBox.Text.Length;
                }
            }
            else if (e.Key == Key.Right)
            {
                e.Handled = true;
                if (grid.Children.IndexOf(panel) != grid.Children.Count - 1)
                {
                    TextBox nextBox = (TextBox)panel.Children[panel.Children.IndexOf(box) + 1];
                    nextBox.Focus();
                    nextBox.SelectionStart = nextBox.Text.Length;
                }
            }
            else if (e.Key == Key.Up)
            {
                if (grid.Children.IndexOf(panel) != 0)
                {
                    StackPanel previousPanel = (StackPanel)grid.Children[grid.Children.IndexOf(panel) - 1];
                    TextBox nextBox = (TextBox)previousPanel.Children[panel.Children.IndexOf(box)];
                    nextBox.Focus();
                    nextBox.SelectionStart = nextBox.Text.Length;
                }
            }
            else if (e.Key == Key.Down)
            {
                if (grid.Children.IndexOf(panel) != grid.Children.Count - 1)
                {
                    StackPanel nextPanel = (StackPanel)grid.Children[grid.Children.IndexOf(panel) + 1];
                    TextBox nextBox = (TextBox)nextPanel.Children[panel.Children.IndexOf(box)];
                    nextBox.Focus();
                    nextBox.SelectionStart = nextBox.Text.Length;
                }
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.V) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                isSaved = false;
                if (!Title.EndsWith("*"))
                {
                    Title += "*";
                }
            }
            else if (e.Key == Key.Enter)//插入一行
            {
                isSaved = false;
                if (!Title.EndsWith("*"))
                {
                    Title += "*";
                }
                e.Handled = true;
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
                StackPanel panel1 = new StackPanel();
                panel1.Orientation = Orientation.Horizontal;
                panel1.Height = 20;
                panel1.Margin = new Thickness(0, 10, 0, 0);
                for (int i = grid.RowDefinitions.Count - 2; i > grid.Children.IndexOf(panel); i--)
                {
                    StackPanel panel2 = (StackPanel)grid.Children[i];
                    panel2.SetValue(Grid.RowProperty, i + 1);
                }
                panel1.SetValue(Grid.RowProperty, grid.Children.IndexOf(panel) + 1);
                grid.Children.Insert(grid.Children.IndexOf(panel) + 1, panel1);
                var b = new TextBox();//数字格 列格
                b.Width = 20;
                b.Margin = new Thickness(10, 0, 10, 0);
                b.HorizontalContentAlignment = HorizontalAlignment.Center;
                b.VerticalContentAlignment = VerticalAlignment.Center;
                b.PreviewTextInput += tb_PreviewTextInput;
                b.TextChanged += TextBox_TextChanged;
                b.PreviewKeyDown += B_PreviewKeyDown;
                b.SetValue(InputMethod.IsInputMethodEnabledProperty, false);
                panel1.Children.Add(b);
            }
        }
        #endregion B

        #region A
        private void A_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!isSelected)
            {
                e.Handled = true;//接管
                TextBox box = (TextBox)sender;
                box.CaretBrush = System.Windows.Media.Brushes.Black;
            }
        }

        private void A_LostFocus(object sender, RoutedEventArgs e)
        {
            if (isSelecting)
            {
                e.Handled = true;//接管
                TextBox box = (TextBox)sender;
                box.CaretBrush = System.Windows.Media.Brushes.Transparent;
            }
        }

        bool isSelected = false;
        bool isFirstSelected = true;
        bool isSelectedOneLine = true;
        int selectedRow = 0;
        TextBox firstSelectedBox;
        private void A_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isClicked)
            {
                if (isSelecting)
                {
                    e.Handled = true;
                    TextBox box = (TextBox)sender;//this
                    if (isFirstSelected)
                    {
                        firstSelectedBox = box;
                        isFirstSelected = false;
                        box.Focus();
                        box.SelectAll();
                        isSelected = true;
                    }
                    else
                    {
                        StackPanel stackPanel = (StackPanel)box.Parent;//this
                        int columnIndex = stackPanel.Children.IndexOf(box);//this
                        int rowIndex = grid.Children.IndexOf(stackPanel);//this
                        StackPanel firstSelectdStackPanel = (StackPanel)firstSelectedBox.Parent;//firstSelectd
                        int firstColumnIndex = firstSelectdStackPanel.Children.IndexOf(firstSelectedBox);//firstSelectd
                        int firstRowIndex = grid.Children.IndexOf(firstSelectdStackPanel);//firstSelectd
                        int selectStartPoint = 0;//0=无，1=this，2=first
                        bool isEnd = false;//selectEndPoint passed
                        for (int i = 0; i < grid.RowDefinitions.Count - 1; i++)//row
                        {
                            StackPanel panel = (StackPanel)grid.Children[i];
                            for (int j = 0; j < panel.Children.Count; j++)//column
                            {
                                if (j != 0)
                                {
                                    TextBox box1 = (TextBox)panel.Children[j];
                                    if (selectStartPoint == 0)
                                    {
                                        if (columnIndex == j && rowIndex == i)//this first
                                        {
                                            box1.Focus();
                                            box1.SelectAll();
                                            selectStartPoint = 1;
                                        }
                                        else if (firstColumnIndex == j && firstRowIndex == i)//first first
                                        {
                                            box1.Focus();
                                            box1.SelectAll();
                                            selectStartPoint = 2;
                                        }
                                        else
                                        {
                                            box1.Select(box1.Text.Length, 0);
                                        }
                                    }
                                    else
                                    {
                                        if (!isEnd)
                                        {
                                            if (rowIndex != firstRowIndex)
                                            {
                                                isSelectedOneLine = false;
                                            }
                                            else
                                            {
                                                isSelectedOneLine = true;
                                                selectedRow = rowIndex;
                                            }
                                            box1.Focus();
                                            box1.SelectAll();
                                            if (selectStartPoint == 1)
                                            {
                                                if (firstColumnIndex == j && firstRowIndex == i)//first last
                                                {
                                                    isEnd = true;
                                                }
                                            }
                                            else
                                            {
                                                if (columnIndex == j && rowIndex == i)//this last
                                                {
                                                    isEnd = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            box1.Select(box1.Text.Length, 0);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        bool isClicked = false;
        private void A_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            TextBox box = (TextBox)sender;
            box.Focus();
            box.SelectionStart = box.Text.Length;
            isClicked = true;
        }

        private void A_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox box = (TextBox)sender;
            StackPanel panel = (StackPanel)box.Parent;
            if (e.Key == Key.Space)//插入一格
            {
                isSaved = false;
                if (!Title.EndsWith("*"))
                {
                    Title += "*";
                }
                e.Handled = true;
                int index = panel.Children.IndexOf(box);
                TextBox newBox = new TextBox();//a.Width
                newBox.Width = 20;
                newBox.Margin = new Thickness(5, 0, 0, 0);
                newBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                newBox.VerticalContentAlignment = VerticalAlignment.Center;
                newBox.TextChanged += A_TextChanged;
                newBox.PreviewKeyDown += A_KeyDown;
                newBox.PreviewMouseDown += A_PreviewMouseDown;
                newBox.MouseMove += A_MouseMove;
                newBox.LostFocus += A_LostFocus;
                newBox.GotFocus += A_GotFocus;
                panel.Children.Insert(index + 1, newBox);
                TextBox firstBox = (TextBox)panel.Children[0];
                firstBox.Text = (int.Parse(firstBox.Text) + 1).ToString();
                newBox.Focus();
            }
            else if (e.Key == Key.Enter)//插入一行
            {
                isSaved = false;
                if (!Title.EndsWith("*"))
                {
                    Title += "*";
                }
                e.Handled = true;
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
                StackPanel panel1 = new StackPanel();
                panel1.Orientation = Orientation.Horizontal;
                panel1.Height = 20;
                panel1.Margin = new Thickness(0, 10, 0, 0);
                for (int i = grid.RowDefinitions.Count - 2; i > grid.Children.IndexOf(panel); i--)
                {
                    StackPanel panel2 = (StackPanel)grid.Children[i];
                    panel2.SetValue(Grid.RowProperty, i + 1);
                }
                panel1.SetValue(Grid.RowProperty, grid.Children.IndexOf(panel) + 1);
                grid.Children.Insert(grid.Children.IndexOf(panel) + 1, panel1);
                var b = new TextBox();//数字格 列格
                b.Width = 20;
                b.Margin = new Thickness(10, 0, 10, 0);
                b.HorizontalContentAlignment = HorizontalAlignment.Center;
                b.VerticalContentAlignment = VerticalAlignment.Center;
                b.PreviewTextInput += tb_PreviewTextInput;
                b.TextChanged += TextBox_TextChanged;
                b.PreviewKeyDown += B_PreviewKeyDown;
                b.SetValue(InputMethod.IsInputMethodEnabledProperty, false);
                panel1.Children.Add(b);
            }
            else if (e.Key == Key.Back)//删除文字
            {
                isSaved = false;
                if (!Title.EndsWith("*"))
                {
                    Title += "*";
                }
                if (box.Text.Length <= 1)
                {
                    e.Handled = true;
                    if (isSelected)
                    {
                        bool isFirst = true;
                        for (int i = 0; i < grid.RowDefinitions.Count - 1; i++)
                        {
                            StackPanel stackPanel = (StackPanel)grid.Children[i];
                            for (int j = 0; j < stackPanel.Children.Count; j++)
                            {
                                if (j != 0)
                                {
                                    TextBox textBox = (TextBox)stackPanel.Children[j];
                                    if (textBox.SelectedText != "")//selected
                                    {
                                        if (isFirst)
                                        {
                                            textBox.Focus();
                                            isFirst = false;
                                        }
                                        textBox.Text = "";
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        box.Text = "";
                        int index = panel.Children.IndexOf(box);
                        if (index != 1)
                        {
                            TextBox previousBox = (TextBox)panel.Children[index - 1];
                            previousBox.Focus();
                            previousBox.SelectionStart = previousBox.Text.Length;
                        }
                    }

                }
            }
            else if (e.Key == Key.Delete)//删除一格
            {
                isSaved = false;
                if (!Title.EndsWith("*"))
                {
                    Title += "*";
                }
                e.Handled = true;
                int index = panel.Children.IndexOf(box);
                panel.Children.RemoveAt(index);
                TextBox firstBox = (TextBox)panel.Children[0];
                if (int.Parse(firstBox.Text) > 1)
                {
                    firstBox.Text = (int.Parse(firstBox.Text) - 1).ToString();
                    TextBox nextBox;
                    if (index > panel.Children.Count - 1)
                    {
                        nextBox = (TextBox)panel.Children[index - 1];
                    }
                    else
                    {
                        nextBox = (TextBox)panel.Children[index];
                    }
                    nextBox.Focus();
                    nextBox.SelectionStart = nextBox.Text.Length;
                }
                else
                {
                    firstBox.Text = "";
                }

            }
            else if (e.Key == Key.Left)
            {
                int index = panel.Children.IndexOf(box);
                e.Handled = true;
                if (index != 0)
                {
                    TextBox previousBox = (TextBox)panel.Children[index - 1];
                    previousBox.Focus();
                    previousBox.SelectionStart = previousBox.Text.Length;
                }
                else
                {
                    StackPanel panel1 = (StackPanel)grid.Children[grid.Children.IndexOf(panel) - 1];
                    TextBox previousBox = (TextBox)panel1.Children[panel1.Children.Count - 1];
                    previousBox.Focus();
                    previousBox.SelectionStart = previousBox.Text.Length;
                }
            }
            else if (e.Key == Key.Right)
            {
                int index = panel.Children.IndexOf(box);
                e.Handled = true;
                if (index != panel.Children.Count - 1)
                {
                    TextBox nextBox = (TextBox)panel.Children[index + 1];
                    nextBox.Focus();
                    nextBox.SelectionStart = nextBox.Text.Length;
                }
                else
                {
                    StackPanel panel1 = (StackPanel)grid.Children[grid.Children.IndexOf(panel) + 1];
                    TextBox nextBox = (TextBox)panel1.Children[0];
                    nextBox.Focus();
                    nextBox.SelectionStart = nextBox.Text.Length;
                }
            }
            else if (e.Key == Key.Up)
            {
                if (grid.Children.IndexOf(panel) != 0)
                {
                    StackPanel previousPanel = (StackPanel)grid.Children[grid.Children.IndexOf(panel) - 1];
                    if (panel.Children.IndexOf(box) <= previousPanel.Children.Count - 1)
                    {
                        TextBox nextBox = (TextBox)previousPanel.Children[panel.Children.IndexOf(box)];
                        nextBox.Focus();
                        nextBox.SelectionStart = nextBox.Text.Length;
                    }
                    else
                    {
                        TextBox nextBox = (TextBox)previousPanel.Children[previousPanel.Children.Count - 1];
                        nextBox.Focus();
                        nextBox.SelectionStart = nextBox.Text.Length;
                    }
                }
            }
            else if (e.Key == Key.Down)
            {
                if (grid.Children.IndexOf(panel) != grid.Children.Count - 1)
                {
                    StackPanel nextPanel = (StackPanel)grid.Children[grid.Children.IndexOf(panel) + 1];
                    if (panel.Children.IndexOf(box) <= nextPanel.Children.Count - 1)
                    {
                        TextBox nextBox = (TextBox)nextPanel.Children[panel.Children.IndexOf(box)];
                        nextBox.Focus();
                        nextBox.SelectionStart = nextBox.Text.Length;
                    }
                    else
                    {
                        TextBox nextBox = (TextBox)nextPanel.Children[nextPanel.Children.Count - 1];
                        nextBox.Focus();
                        nextBox.SelectionStart = nextBox.Text.Length;
                    }
                }
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.C) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                if (isSelected)
                {
                    string result = "";
                    for (int i = 0; i < grid.Children.Count; i++)
                    {
                        StackPanel stackPanel = (StackPanel)grid.Children[i];
                        for (int j = 0; j < stackPanel.Children.Count; j++)
                        {
                            TextBox textBox = (TextBox)stackPanel.Children[j];
                            if (textBox.SelectedText != "")
                            {
                                result += textBox.Text;
                            }
                        }
                    }
                    Clipboard.SetDataObject(result);
                }
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.X) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                if (isSelected)
                {
                    isSaved = false;
                    if (!Title.EndsWith("*"))
                    {
                        Title += "*";
                    }
                    string result = "";
                    for (int i = 0; i < grid.Children.Count; i++)
                    {
                        StackPanel stackPanel = (StackPanel)grid.Children[i];
                        for (int j = 0; j < stackPanel.Children.Count; j++)
                        {
                            TextBox textBox = (TextBox)stackPanel.Children[j];
                            if (textBox.SelectedText != "")
                            {
                                result += textBox.Text;
                                textBox.Text = "";
                            }
                        }
                    }
                    Clipboard.SetDataObject(result);
                }
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.Z) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                isSaved = false;
                if (!Title.EndsWith("*"))
                {
                    Title += "*";
                }
                e.Handled = true;
            }
            else if (e.KeyStates == Keyboard.GetKeyStates(Key.Y) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                isSaved = false;
                if (!Title.EndsWith("*"))
                {
                    Title += "*";
                }
                e.Handled = true;
            }
        }

        private void A_TextChanged(object sender, TextChangedEventArgs e)
        {
            isSaved = false;
            if (!Title.EndsWith("*"))
            {
                Title += "*";
            }
            TextBox box = (TextBox)sender;
            StackPanel stackPanel = (StackPanel)box.Parent;
            String t = box.Text;
            if (IsAllChineseLetter(t))
            {
                if (t.Length > 1)
                {
                    int index = stackPanel.Children.IndexOf(box);
                    int total = stackPanel.Children.Count;
                    if (index == total - 1)
                    {
                        box.Text = t.Substring(0, 1);
                    }
                    else
                    {
                        TextBox nextBox = (TextBox)stackPanel.Children[index + 1];
                        nextBox.Focus();
                        nextBox.Text = t.Substring(1);
                        nextBox.SelectionStart = nextBox.Text.Length;
                        box.Text = t.Substring(0, 1);
                    }
                }
            }
        }

        protected bool IsAllChineseLetter(string input)
        {
            if (input != "" && input != null)
            {
                bool result = true;
                for (int i = 0; i < input.Length; i++)
                {
                    int code = 0;
                    int chfrom = Convert.ToInt32("4e00", 16);    //范围（0x4e00～0x9fff）转换成int（chfrom～chend）
                    int chend = Convert.ToInt32("9fff", 16);
                    code = Char.ConvertToUtf32(input, i);    //获得字符串input中指定索引index处字符unicode编码
                    if (code != 45 && (code < chfrom || code > chend))
                    {
                        result = false;
                        break;
                    }
                }
                return result;
            }
            else
            {
                return false;
            }
        }
        #endregion A

        #region 命令
        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!isSaved)
            {
                if (MessageBox.Show("文件未保存，你确定要退出吗", "警告", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Close();
                }
            }
            else
            {
                if (MessageBox.Show("你确定要退出吗", "警告", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Close();
                }
            }
            
        }

        private void Export_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".txt"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension
            dlg.InitialDirectory = @"D:\LyricBlock";
            if (dlg.ShowDialog() == true)
            {
                String result = "";
                for (int i = 0; i < grid.RowDefinitions.Count; i++)
                {
                    StackPanel panel = (StackPanel)grid.Children[i];
                    for (int j = 0; j < panel.Children.Count; j++)
                    {
                        if (j != 0)
                        {
                            TextBox box = (TextBox)panel.Children[j];
                            result += box.Text;
                        }
                    }
                    if (i != grid.RowDefinitions.Count - 1)
                    {
                        result += "\r\n";
                    }
                }
                WriteFile(dlg.FileName, result);
                MessageBox.Show("导出成功");
            }
        }

        public static void WriteFile(string Path, string Strings)
        {
            System.IO.FileStream f = System.IO.File.Create(Path);
            f.Close();
            f.Dispose();
            System.IO.StreamWriter f2 = new System.IO.StreamWriter(Path, true, System.Text.Encoding.UTF8);
            f2.Write(Strings);
            f2.Close();
            f2.Dispose();
        }

        private void New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!isSaved)
            {
                if (MessageBox.Show("文件未保存，要保存吗", "警告", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Save_Executed(sender, e);
                }
            }
            isSaved = false;
            isFirstSave = true;
            Title = "歌词本-未命名.blk*";
            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
            StackPanel panel1 = new StackPanel();
            panel1.Orientation = Orientation.Horizontal;
            panel1.Height = 20;
            panel1.Margin = new Thickness(0, 10, 0, 0);
            panel1.SetValue(Grid.RowProperty, grid.RowDefinitions.Count - 1);
            grid.Children.Add(panel1);
            var b = new TextBox();//数字格 列格
            b.Width = 20;
            b.Margin = new Thickness(10, 0, 10, 0);
            b.HorizontalContentAlignment = HorizontalAlignment.Center;
            b.VerticalContentAlignment = VerticalAlignment.Center;
            b.PreviewTextInput += tb_PreviewTextInput;
            b.TextChanged += TextBox_TextChanged;
            b.PreviewKeyDown += B_PreviewKeyDown;
            b.SetValue(InputMethod.IsInputMethodEnabledProperty, false);
            panel1.Children.Add(b);
        }

        private void UnderLine_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (isSelectedOneLine)
            {
                if (isSelected)
                {
                    isSaved = false;
                    if (!Title.EndsWith("*"))
                    {
                        Title += "*";
                    }
                    bool isFirst = true;
                    bool isStarted = false;
                    bool isClosed = false;
                    StackPanel panel = (StackPanel)grid.Children[selectedRow];
                    for (int j = 0; j < panel.Children.Count; j++)
                    {
                        if (j != 0)
                        {
                            TextBox box = (TextBox)panel.Children[j];
                            if (box.SelectedText != "")//selected
                            {
                                isStarted = true;
                                if (isFirst)
                                {
                                    box.BorderThickness = new Thickness(2, 2, 0, 2);
                                    isFirst = false;
                                }
                                else
                                {
                                    box.BorderThickness = new Thickness(0, 2, 0, 2);
                                }
                            }
                            else
                            {
                                if (isStarted)
                                {
                                    TextBox previousBox = (TextBox)panel.Children[j - 1];
                                    previousBox.BorderThickness = new Thickness(0, 2, 2, 2);
                                    isClosed = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (!isClosed)//if the last one is selected
                    {
                        TextBox box = (TextBox)panel.Children[panel.Children.Count - 1];
                        box.BorderThickness = new Thickness(0, 2, 2, 2);
                    }
                }
            }
            else
            {
                MessageBox.Show("标记功能应能提高文本辨识度，因此不应选择多于一行、少于两个字或包含空格的文字", "提示");
            }
        }

        private void DisunderLine_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (isSelected)
            {
                isSaved = false;
                if (!Title.EndsWith("*"))
                {
                    Title += "*";
                }
                for (int i = 0; i < grid.Children.Count; i++)
                {
                    StackPanel panel = (StackPanel)grid.Children[i];
                    for (int j = 0; j < panel.Children.Count; j++)
                    {
                        if (j != 0)
                        {
                            TextBox box = (TextBox)panel.Children[j];
                            if (box.SelectedText != "")
                            {
                                box.BorderThickness = new Thickness(1, 1, 1, 1);
                            }
                        }
                    }
                }
            }
        }

        private void DeleteLine_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            isSaved = false;
            if (!Title.EndsWith("*"))
            {
                Title += "*";
            }
            bool delete = false;
            int row = 0;
            for (int i = 0; i < grid.RowDefinitions.Count - 1; i++)
            {
                StackPanel panel = (StackPanel)grid.Children[i];
                for (int j = 0; j < panel.Children.Count; j++)
                {
                    TextBox box = (TextBox)panel.Children[j];
                    if (box.IsFocused)
                    {
                        delete = true;
                        row = i;
                        break;
                    }
                }
            }
            if (delete)
            {
                grid.Children.RemoveAt(row);
                for (int i = row; i < grid.RowDefinitions.Count - 1; i++)
                {
                    grid.Children[i].SetValue(Grid.RowProperty, i);
                }
                grid.RowDefinitions.RemoveAt(grid.RowDefinitions.Count - 1);
            }
        }

        bool isSaved = false;
        bool isFirstSave = true;
        string fileName = "";
        string name = "";
        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (isFirstSave)
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "Lyric"; // Default file name
                dlg.DefaultExt = ".blk"; // Default file extension
                dlg.Filter = "Lyric Blocks (.blk)|*.blk"; // Filter files by extension
                dlg.InitialDirectory = @"D:\LyricBlock";

                // Process save file dialog box results
                if (dlg.ShowDialog() == true)
                {
                    // Save document
                    fileName = dlg.FileName;
                    name = dlg.SafeFileName;
                    System.IO.FileStream f = System.IO.File.Create(fileName);
                    f.Close();
                    f.Dispose();
                    string result = "";
                    for (int i = 0; i < grid.RowDefinitions.Count - 1; i++)
                    {
                        StackPanel panel = (StackPanel)grid.Children[i];
                        for (int j = 0; j < panel.Children.Count; j++)
                        {
                            TextBox box = (TextBox)panel.Children[j];
                            int row = i;
                            int column = j;
                            string text = box.Text;
                            int selectedPosition = 0;
                            if (box.BorderThickness == new Thickness(2, 2, 0, 2))
                            {
                                selectedPosition = 1;
                            }
                            else if (box.BorderThickness == new Thickness(0, 2, 0, 2))
                            {
                                selectedPosition = 2;
                            }
                            else if (box.BorderThickness == new Thickness(0, 2, 2, 2))
                            {
                                selectedPosition = 3;
                            }
                            result += i + "," + j + "," + text + "," + selectedPosition + "," + "\r\n";
                        }
                    }
                    System.IO.StreamWriter f2 = new System.IO.StreamWriter(fileName, true, System.Text.Encoding.UTF8);
                    f2.Write(result);
                    f2.Close();
                    f2.Dispose();
                    Title = "歌词本-" + name;
                    isSaved = true;
                    isFirstSave = false;
                }
            }
            else
            {
                System.IO.FileStream f = System.IO.File.Create(fileName);
                f.Close();
                f.Dispose();
                string result = "";
                for (int i = 0; i < grid.RowDefinitions.Count - 1; i++)
                {
                    StackPanel panel = (StackPanel)grid.Children[i];
                    for (int j = 0; j < panel.Children.Count; j++)
                    {
                        TextBox box = (TextBox)panel.Children[j];
                        int row = i;
                        int column = j;
                        string text = box.Text;
                        int selectedPosition = 0;
                        if (box.BorderThickness == new Thickness(2, 2, 0, 2))
                        {
                            selectedPosition = 1;
                        }
                        else if (box.BorderThickness == new Thickness(0, 2, 0, 2))
                        {
                            selectedPosition = 2;
                        }
                        else if (box.BorderThickness == new Thickness(0, 2, 2, 2))
                        {
                            selectedPosition = 3;
                        }
                        result += i + "," + j + "," + text + "," + selectedPosition + "," + "\r\n";
                    }
                }
                System.IO.StreamWriter f2 = new System.IO.StreamWriter(fileName, true, System.Text.Encoding.UTF8);
                f2.Write(result);
                f2.Close();
                f2.Dispose();
                Title = "歌词本-" + name;
                isSaved = true;
            }
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!isSaved)
            {
                if (MessageBox.Show("文件未保存，要保存吗", "警告", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Save_Executed(sender, e);
                }
            }
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Lyric Blocks (.blk)|*.blk|Text (.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                StreamReader sr = new StreamReader(openFileDialog.FileName);
                string s = sr.ReadToEnd();
                initiate(s, openFileDialog.FileName);
            }
        }
        #endregion

        #region viewer
        bool isSelecting = false;
        private void viewer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isSelecting = true;
            if (isSelected)//取消所有被选择的
            {
                for (int i = 0; i < grid.Children.Count; i++)
                {
                    StackPanel panel = (StackPanel)grid.Children[i];
                    for (int j = 0; j < panel.Children.Count; j++)
                    {
                        TextBox box = (TextBox)panel.Children[j];
                        if (box.SelectedText != "")
                        {
                            box.Select(box.Text.Length, 0);
                        }
                    }
                }
                isSelected = false;
                isFirstSelected = true;
                firstSelectedBox = null;
                isSelectedOneLine = false;
            }
        }

        private void viewer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isSelecting = false;
            isClicked = false;
        }
        #endregion viewer

        public void initiate(string fileContent, string fileName)
        {
            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            if (fileName.EndsWith(".txt"))
            {
                fileContent = fileContent.Replace("\r\n", "\n");
                string[] array = fileContent.Split('\n');
                StackPanel panel1 = new StackPanel();
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].Length > 0)
                    {
                        grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
                        panel1 = new StackPanel();
                        panel1.Orientation = Orientation.Horizontal;
                        panel1.Height = 20;
                        panel1.Margin = new Thickness(0, 10, 0, 0);
                        panel1.SetValue(Grid.RowProperty, grid.RowDefinitions.Count - 1);
                        grid.Children.Add(panel1);
                        var b = new TextBox();//数字格 列格
                        b.Width = 20;
                        b.Margin = new Thickness(10, 0, 10, 0);
                        b.HorizontalContentAlignment = HorizontalAlignment.Center;
                        b.VerticalContentAlignment = VerticalAlignment.Center;
                        b.Text = array[i].Length.ToString();
                        b.PreviewTextInput += tb_PreviewTextInput;
                        b.TextChanged += TextBox_TextChanged;
                        b.PreviewKeyDown += B_PreviewKeyDown;
                        b.SetValue(InputMethod.IsInputMethodEnabledProperty, false);
                        panel1.Children.Add(b);
                        int number = array[i].Length;
                        for (int j = 0; j < number; j++)
                        {
                            var aBox = new TextBox();//文字格
                            aBox.Width = 20;
                            aBox.Margin = new Thickness(5, 0, 0, 0);
                            aBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                            aBox.VerticalContentAlignment = VerticalAlignment.Center;
                            aBox.Text = array[i].Substring(0, 1);
                            aBox.TextChanged += A_TextChanged;
                            aBox.PreviewKeyDown += A_KeyDown;
                            aBox.PreviewMouseDown += A_PreviewMouseDown;
                            aBox.MouseMove += A_MouseMove;
                            aBox.LostFocus += A_LostFocus;
                            aBox.GotFocus += A_GotFocus;
                            panel1.Children.Add(aBox);
                            array[i] = array[i].Replace(array[i].Substring(0, 1), "");
                        }
                    }
                }
            }
            else if (fileName.EndsWith(".blk"))
            {
                fileContent = fileContent.Replace("\r\n", "\n");
                string[] array = fileContent.Split('\n');
                int line = -1;
                StackPanel panel1 = new StackPanel();
                for (int i = 0; i < array.Length - 1; i++)
                {
                    string[] a = array[i].Split(',');
                    if (int.Parse(a[0]) != line)
                    {
                        grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });
                        panel1 = new StackPanel();
                        panel1.Orientation = Orientation.Horizontal;
                        panel1.Height = 20;
                        panel1.Margin = new Thickness(0, 10, 0, 0);
                        panel1.SetValue(Grid.RowProperty, grid.RowDefinitions.Count - 1);
                        grid.Children.Add(panel1);
                        line++;
                    }
                    if (a[1] == "0")
                    {
                        var b = new TextBox();//数字格 列格
                        b.Width = 20;
                        b.Margin = new Thickness(10, 0, 10, 0);
                        b.HorizontalContentAlignment = HorizontalAlignment.Center;
                        b.VerticalContentAlignment = VerticalAlignment.Center;
                        b.Text = a[2];
                        b.PreviewTextInput += tb_PreviewTextInput;
                        b.TextChanged += TextBox_TextChanged;
                        b.PreviewKeyDown += B_PreviewKeyDown;
                        b.SetValue(InputMethod.IsInputMethodEnabledProperty, false);
                        panel1.Children.Add(b);
                    }
                    else
                    {
                        var aBox = new TextBox();//文字格
                        aBox.Width = 20;
                        aBox.Margin = new Thickness(5, 0, 0, 0);
                        aBox.HorizontalContentAlignment = HorizontalAlignment.Center;
                        aBox.VerticalContentAlignment = VerticalAlignment.Center;
                        aBox.Text = a[2];
                        aBox.TextChanged += A_TextChanged;
                        aBox.PreviewKeyDown += A_KeyDown;
                        aBox.PreviewMouseDown += A_PreviewMouseDown;
                        aBox.MouseMove += A_MouseMove;
                        aBox.LostFocus += A_LostFocus;
                        aBox.GotFocus += A_GotFocus;
                        if (a[3] == "1")
                        {
                            aBox.BorderThickness = new Thickness(2, 2, 0, 2);
                        }
                        else if (a[3] == "2")
                        {
                            aBox.BorderThickness = new Thickness(0, 2, 0, 2);
                        }
                        else if (a[3] == "3")
                        {
                            aBox.BorderThickness = new Thickness(0, 2, 2, 2);
                        }
                        panel1.Children.Add(aBox);
                    }
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("插入一格\tSpace\r\n删除一格\tDelete\r\n删除字符\tBackspace\r\n添加一行\tEnter", "帮助", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("版本0.0.1", "关于", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = "https://github.com/cubic759/LyricBlockMaker";
            proc.Start();
        }

        private void Undo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            recorder.Undo();
        }

        private void Redo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            recorder.Redo();
        }
    }
}
