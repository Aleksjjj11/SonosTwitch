using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Win32;
using TwitchLib.Client.Events;

namespace SonosTwitch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        const string FileName = @"SavedSetting.bin";
        public static AppSetting Setting;
        private DispatcherTimer _timerColor;
        public TwitchBot Bot { get; set; }

        private delegate Task OnLog(DateTime time, string username, string nameCommand);
        private delegate Task OnPlaySong(string nameCommand);
        public MainWindow()
        {
            InitializeComponent();
            Bot = new TwitchBot();
            Bot.Notify += TwitchBotOnNotify;
            Closing += MainWindow_OnClosed;
            bool needUpdateListCommand = !LoadSaving();
            LoadInterface(needUpdateListCommand);
            _timerColor = new DispatcherTimer(DispatcherPriority.Background);
            _timerColor.Interval = new TimeSpan(0,0,0, 0, 50);
            _timerColor.Tick += Coloring;
            _timerColor.Tag = false;
            _timerColor.IsEnabled = true;
            _timerColor.Start();
        }

        private void Coloring(object sender, EventArgs eventArgs)
        {
            bool? incremente = _timerColor.Tag as bool?;
            if (LabelTwitchChannel.Opacity < 0.05)
                incremente = true;
            if (LabelTwitchChannel.Opacity > 0.85)
                incremente = false;
            if (incremente is true)
                LabelTwitchChannel.Opacity += 0.01;
            else 
                LabelTwitchChannel.Opacity -= 0.01;
            _timerColor.Tag = incremente;
        }
        private void LoadInterface(bool isEmpty)
        {
            if (isEmpty)
            {
                Setting = Setting ?? new AppSetting(new Dictionary<string, string>());
                Setting.DictionaryCommands.Add("command", "path");
                Setting.Prefix = "!";
            }
            foreach (var pair in Setting.DictionaryCommands)
            {
                AddNewCommandLine(pair.Key, pair.Value);
            }

            LabelTwitchChannel.Content = Setting.TwitchChannel;
            TextBoxPrefix.Text = Setting.Prefix;
            CheckBoxFollower.IsChecked = Setting.ReceiveFollower;
            CheckBoxSubscriber.IsChecked = Setting.ReceiveSubscriber;
            CheckBoxEveryOne.IsChecked = Setting.ReceiveEveryone;
        }

        private void TwitchBotOnNotify(object sender, OnMessageReceivedArgs e)
        {
            Dispatcher?.BeginInvoke(DispatcherPriority.Normal, new OnLog(AddNewReceivedLog), DateTime.Now, e.ChatMessage.Username, e.ChatMessage.Message);
            Dispatcher?.BeginInvoke(DispatcherPriority.Normal, new OnPlaySong(PlaySong), e.ChatMessage.Message.Remove(0, 1));
        }

        private Task PlaySong(string nameCommand)
        {
            if (CheckBoxGetOffer.IsChecked == false)
            {
                try
                {
                    string path = Setting.DictionaryCommands[nameCommand];
                    SoundPlayer player = new SoundPlayer(path);
                    player.Play();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Добавляет новую строку с командой и путём до файла, который должен воспроизводится
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAddCommand_OnClick(object sender, RoutedEventArgs e)
        {
            AddNewCommandLine();
        }

        private void ButtonResetAllCommands_OnClick(object sender, RoutedEventArgs e)
        {
            var children = GridCommands.Children.OfType<UIElement>().ToList();
            foreach (var element in children)
            {
                if (element is TextBox)
                {
                    if ((element as TextBox).Visibility == Visibility.Visible)
                        GridCommands.Children.Remove(element as TextBox);
                }

                if (element is Button)
                {
                    if ((element as Button).Visibility == Visibility.Visible)
                        GridCommands.Children.Remove(element as Button);
                }
            }
            while (GridCommands.RowDefinitions.Count > 1)
            {
                GridCommands.RowDefinitions.Clear();
                AddNewCommandLine();
            }
        }

        private void AddNewCommandLine(string textCommand = "command", string textPath = "path")
        {
            try
            {
                var command = new TextBox();
                command.Height = 30;
                command.Width = 150;
                command.Style = TextBoxCommand.Style;
                command.Text = textCommand;
                command.DataContext = "TextCommand";
                var commandMargin = command.Margin;
                commandMargin.Left = 50;
                commandMargin.Top = 10;
                command.Margin = commandMargin;
                
                var path = new TextBox();
                path.Height = 30;
                path.Width = 200;
                path.Text = textPath;
                path.Style = TextBoxPath.Style;
                path.DataContext = "TextPath";
                var pathMargin = path.Margin;
                pathMargin.Top = 10;
                path.Margin = pathMargin;
                
                var buttonDialog = new Button();
                buttonDialog.Width = 30;
                buttonDialog.HorizontalAlignment = HorizontalAlignment.Right;
                buttonDialog.VerticalAlignment = VerticalAlignment.Center;
                buttonDialog.Click += ButtonOpenAudioFile_OnClick;
                buttonDialog.Style = ButtonOpenAudioFileObj.Style;
                var buttonMargin = buttonDialog.Margin;
                buttonMargin.Left = 8;
                buttonMargin.Top = 6;
                buttonMargin.Right = 5;
                buttonDialog.Margin = buttonMargin;
                
                var buttonDeleteLine = new Button();
                buttonDeleteLine.Width = 30;
                buttonDeleteLine.Height = 30;
                buttonDeleteLine.Content = "Del";
                buttonDeleteLine.Click += ButtonDeleteCommand_OnClick;
                buttonDeleteLine.Style = ButtonDeleteCommand.Style;
                buttonDeleteLine.Margin = new Thickness(135, 10, 0, 0);
                
                var newRow = new RowDefinition();
                GridCommands.RowDefinitions.Add(newRow);
                Grid.SetRow(command, GridCommands.RowDefinitions.Count - 1);
                Grid.SetColumn(command, 0);
                
                Grid.SetRow(buttonDeleteLine, GridCommands.RowDefinitions.Count - 1);
                Grid.SetColumn(buttonDeleteLine, 0);
                
                Grid.SetRow(path, GridCommands.RowDefinitions.Count - 1);
                Grid.SetColumn(path, 1);
                
                Grid.SetRow(buttonDialog, GridCommands.RowDefinitions.Count - 1);
                Grid.SetColumn(buttonDialog, 1);
                
                GridCommands.Children.Add(command);
                GridCommands.Children.Add(path);
                GridCommands.Children.Add(buttonDialog);
                GridCommands.Children.Add(buttonDeleteLine);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void ButtonOpenAudioFile_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog {DefaultExt = ".wav", Filter = "(.wav)|*.wav"};
            var result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                var source = sender as Button;
                int row = Grid.GetRow(source);
                if (row >= 0)
                {
                    foreach (var element in GridCommands.Children)
                    {
                        if (element is TextBox)
                        {
                            if (Grid.GetRow(element as TextBox) == row && (element as TextBox).DataContext.ToString() == "TextPath")
                            {
                                (element as TextBox).Text = filename;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Возвращает true, если сохранения были загружены из файла, иначе возвращает false
        /// </summary>
        /// <returns></returns>
        private bool LoadSaving()
        {
            if (File.Exists(FileName))
            {
                Console.WriteLine("Reading saved");
                Stream openFileStream = File.OpenRead(FileName);
                BinaryFormatter deserializer = new BinaryFormatter();
                Setting = (AppSetting) deserializer.Deserialize(openFileStream);
                Setting.TimeLastLoaded = DateTime.Now;
                openFileStream.Close();
                
                Console.WriteLine(@"Downloaded:");
                foreach (var pair in Setting.DictionaryCommands)
                {
                    Console.WriteLine($"{pair.Key} : {pair.Value}");
                }
                
                return true;
            }
            
            Setting = new AppSetting(new Dictionary<string, string>());
            return false;
        }

        private void ButtonUploadIcon_OnClick(object sender, RoutedEventArgs e)
        {
            AuthorizeTwitch page = new AuthorizeTwitch();
            page.Owner = this;
            page.Show();
            Setting.Prefix = TextBoxPrefix.Text;
            SaveChangeListCommands();
            SaveInFile();
        }

        private void SaveChangeListCommands()
        {
            Setting.DictionaryCommands = Setting.DictionaryCommands ?? new Dictionary<string, string>();
            Setting.DictionaryCommands.Clear();
            string command = null, path = null, allCommand = "";
            foreach (var element in GridCommands.Children)
            {
                if (element as UIElement != null)
                    if ((element as UIElement)?.Visibility == Visibility.Collapsed) continue;
                if (element is TextBox)
                {
                   if ((element as TextBox).DataContext.ToString() == "TextPath")
                   {
                       path = (element as TextBox).Text;
                   }
                   if ((element as TextBox).DataContext.ToString() == "TextCommand")
                   {
                       command = (element as TextBox).Text;
                   }

                   if (command != null && path != null)
                   {
                       try
                       {
                           Setting.DictionaryCommands.Add(command, path);
                       }
                       catch (ArgumentException exception)
                       {
                           Setting.DictionaryCommands[command] = path;
                       }

                       allCommand += command + "\n";
                       command = null;
                       path = null;
                   }
                }
            }
        }

        private static void SaveInFile()
        {
            Stream saveFileStream = File.Create(FileName);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(saveFileStream, Setting);
            saveFileStream.Close();
        }
        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            SaveChangeListCommands();
            SaveInFile();
            Application.Current.Shutdown();
        }

        private Task AddNewReceivedLog(DateTime time, string username, string nameCommand)
        {
            if (CheckBoxGetOffer.IsChecked == false) return null;

            Grid lineLogging = new Grid();
            try
            {
                string name =
                    ("LineLog" + (time.Year * 365 + time.Month * 30 + time.Day) * 24 * 60 * 60*1000 + time.Hour*60*60*1000 + time.Minute*60*1000 + time.Second*1000 + time.Millisecond)
                    .Replace("-", "");
                RegisterName(name, lineLogging);
                lineLogging.Name = name;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
            lineLogging.ColumnDefinitions.Add(new ColumnDefinition{Width = GridLength.Auto});
            lineLogging.ColumnDefinitions.Add(new ColumnDefinition{Width = GridLength.Auto});
            lineLogging.RowDefinitions.Add(new RowDefinition());
            lineLogging.RowDefinitions.Add(new RowDefinition());

            StackPanel information = new StackPanel();
            information.Orientation = Orientation.Horizontal;
            Grid.SetColumn(information, 0);
            Grid.SetRow(information, 0);
            Grid.SetColumnSpan(information, 2);

            StackPanel buttons = new StackPanel();
            buttons.Orientation = Orientation.Horizontal;
            Grid.SetColumn(buttons, 0);
            Grid.SetRow(buttons, 1);
            Grid.SetColumnSpan(buttons, 2);

            Label labelTime = new Label();
            labelTime.Content = time.ToString("t");
            labelTime.Style = LabelTime0.Style;
            labelTime.Margin = new Thickness{ Left = 5, Top = 15};
            
            Label labelUserName = new Label();
            labelUserName.Content = username;
            labelUserName.Style = LabelUserName0.Style;
            labelUserName.Foreground = LabelUserName0.Foreground;
            labelUserName.Margin = new Thickness{Top = 10};
            
            TextBlock textBlockCommand = new TextBlock();
            textBlockCommand.Text = nameCommand;
            textBlockCommand.Style = LabelReceivedCommand.Style;
            textBlockCommand.Width = LabelReceivedCommand.Width;
            textBlockCommand.MaxWidth = 100;
            textBlockCommand.FontSize = 14;
            textBlockCommand.TextWrapping = TextWrapping.Wrap;
            textBlockCommand.Margin = new Thickness{Top = 15, Right = 5};

            information.Children.Add(labelTime);
            information.Children.Add(labelUserName);
            information.Children.Add(textBlockCommand);

            Button accept = new Button();
            accept.Content = "Accept";
            accept.DataContext = nameCommand.Remove(0, 1) + " " + lineLogging.Name;
            accept.Style = ButtonAccept.Style;
            accept.Margin = new Thickness{Left = 35, Top = 10, Right = 10};
            accept.Click += AcceptOnClick;
            
            Button cancel = new Button();
            cancel.Content = "Cancel";
            cancel.DataContext = nameCommand.Remove(0, 1) + " " + lineLogging.Name;
            cancel.Style = ButtonCancel.Style;
            cancel.Margin = new Thickness{Left = 10, Top = 10, Right = 35};
            cancel.Click += CancelOnClick;

            buttons.Children.Add(accept);
            buttons.Children.Add(cancel);
            
            lineLogging.Children.Add(information);
            lineLogging.Children.Add(buttons);
            
            StackPanelLogging.Children.Add(lineLogging);
            
            return Task.CompletedTask;
        }

        private void CancelOnClick(object sender, RoutedEventArgs e)
        {
            var dataContext = ((sender as Button)?.DataContext as string).Split(' ');
            var res = StackPanelLogging.FindName(dataContext.Last());
            try
            {
                if (res is Grid)
                {
                    StackPanelLogging.Children.Remove(res as Grid);
                    UnregisterName((res as Grid).Name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AcceptOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var dataContext = ((sender as Button)?.DataContext as string).Split(' ');
                string path = Setting.DictionaryCommands[dataContext.First()];
                SoundPlayer player = new SoundPlayer(path);
                player.Play();
                var res = StackPanelLogging.FindName(dataContext.Last());
                if (res is Grid)
                {
                    StackPanelLogging.Children.Remove(res as Grid);
                    UnregisterName((res as Grid).Name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CheckBoxEveryOne_OnClick(object sender, RoutedEventArgs e)
        {

            if (sender is CheckBox)
                Setting.ReceiveEveryone = (sender as CheckBox).IsChecked.Value;
            else
                MessageBox.Show("Объект не является переключателем");
        }

        private void CheckBoxSubscriber_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox)
                Setting.ReceiveSubscriber = (sender as CheckBox).IsChecked.Value;
            else
                MessageBox.Show("Объект не является переключателем");
        }

        private void CheckBoxFollower_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox)
                Setting.ReceiveFollower = (sender as CheckBox).IsChecked.Value;
            else
                MessageBox.Show("Объект не является переключателем");
        }

        private void CheckBoxGetOffer_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox)
                Setting.IsGetOffer = (sender as CheckBox).IsChecked.Value;
            else
                MessageBox.Show("Объект не является переключателем");
        }

        private void ButtonDeleteCommand_OnClick(object sender, RoutedEventArgs e)
        {
            var children = GridCommands.Children.OfType<UIElement>().ToList();
            var source = sender as Button;
            string command = "";
            int row = Grid.GetRow(source);
            try
            {
                if (row >= 0)
                {
                    foreach (var element in children)
                    {
                        if (element is TextBox)
                        {
                            if (Grid.GetRow(element as TextBox) == row)
                            {
                                if ((element as TextBox).DataContext.ToString() == "TextCommand")
                                {
                                    command = (element as TextBox).Text;
                                    Setting.DictionaryCommands.Remove(command);
                                }

                                GridCommands.Children.Remove(element);
                            }
                        }

                        if (element is Button)
                        {
                            if (Grid.GetRow(element as Button) == row)
                            {
                                GridCommands.Children.Remove(element);
                            }
                        }
                    }

                    //GridCommands.RowDefinitions.Remove(GridCommands.RowDefinitions[row]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + row);
                
            }
            
                
            
            
            
            /*var children = GridCommands.Children.OfType<UIElement>().ToList();
            foreach (var element in children)
            {
                if (element is TextBox)
                {
                    if ((element as TextBox).Visibility == Visibility.Visible)
                        GridCommands.Children.Remove(element as TextBox);
                }

                if (element is Button)
                {
                    if ((element as Button).Visibility == Visibility.Visible)
                        GridCommands.Children.Remove(element as Button);
                }
            }
            while (GridCommands.RowDefinitions.Count > 1)
            {
                GridCommands.RowDefinitions[row];
                AddNewCommandLine();
            }*/
        }
    }
}