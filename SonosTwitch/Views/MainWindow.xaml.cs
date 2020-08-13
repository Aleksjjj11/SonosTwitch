using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;
using SonosTwitch.ViewModels;
using TwitchLib.Client.Events;

namespace SonosTwitch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        //TODO Перенести Setting в ViewModel и всё, что с этим связано
        const string FileName = @"SavedSetting.bin";
        private DispatcherTimer _timerColor;
        public MainWindowVM ViewModel { get; set; }
        public ObservableCollection<Sound> Sounds { get; set; }
        //public TwitchBot Bot { get; set; }

        private delegate Task OnLog(DateTime time, string username, string nameCommand);
        private delegate Task OnPlaySong(string nameCommand);
        public MainWindow()
        {
            bool needUpdateListCommand = !LoadSaving();
            SyncCommands();
            try
            {
                ViewModel = new MainWindowVM(new TwitchBot(App.Setting.TwitchChannel, App.Setting.TwitchToken), App.Setting);
                ViewModel.ClientBot.Notify += TwitchBotOnNotify;
            }
            catch (Exception ex)
            {
                LabelTwitchChannel.Foreground = new SolidColorBrush(Colors.Red);
            }
            InitializeComponent();
            Closing += MainWindow_OnClosed;
            _timerColor = new DispatcherTimer(DispatcherPriority.Background)
            {
                Interval = new TimeSpan(0, 0, 0, 0, 50)
            };
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
        /*private void LoadInterface(bool isEmpty)
        {
            if (isEmpty)
            {
                App.Setting = App.Setting ?? new AppSetting(new Dictionary<string, string>());
                App.Setting.DictionaryCommands.Add("command", "path");
                App.Setting.Prefix = "!";
            }
            foreach (var pair in App.Setting.DictionaryCommands)
            {
                AddNewCommandLine(pair.Key, pair.Value);
            }

            LabelTwitchChannel.Content = App.Setting.TwitchChannel;
            TextBoxPrefix.Text = App.Setting.Prefix;
            TextBoxTimeout.Text = App.Setting.Timeout.ToString();
            CheckBoxFollower.IsChecked = App.Setting.ReceiveFollower;
            CheckBoxSubscriber.IsChecked = App.Setting.ReceiveSubscriber;
            CheckBoxEveryOne.IsChecked = App.Setting.ReceiveEveryone;
        }*/

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
                    string path = App.Setting.DictionaryCommands[nameCommand];
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
            Sounds.Add(new Sound($"NewCommand{Sounds.Count}", "Opa"));
            //AddNewCommandLine();
        }

        /*private void ButtonResetAllCommands_OnClick(object sender, RoutedEventArgs e)
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
        }*/

        /*private void AddNewCommandLine(string textCommand = "command", string textPath = "path")
        {
            try
            {
                var command = new TextBox();
                command.Height = 30;
                command.Width = 160;
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
                buttonMargin.Left = 0;
                buttonMargin.Top = 6;
                buttonMargin.Right = 30;
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
        }*/

        private void ButtonOpenAudioFile_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog {DefaultExt = ".wav", Filter = "(.wav)|*.wav"};
            var result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                ((TextBox) sender).Text = filename;
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
                App.Setting = (AppSetting) deserializer.Deserialize(openFileStream);
                App.Setting.TimeLastLoaded = DateTime.Now;
                openFileStream.Close();
                
                Console.WriteLine(@"Downloaded:");
                foreach (var pair in App.Setting.DictionaryCommands)
                {
                    Console.WriteLine($"{pair.Key} : {pair.Value}");
                }
                
                return true;
            }
            
            App.Setting = new AppSetting(new Dictionary<string, string>());
            return false;
        }

        private void ButtonUploadIcon_OnClick(object sender, RoutedEventArgs e)
        {
            App.Setting = ViewModel.AppSetting;
            App.Setting.Timeout = Convert.ToUInt32(TextBoxTimeout.Text);
            SaveChangeListCommands();
            SaveInFile();
        }

        private void SaveChangeListCommands()
        {
            App.Setting.DictionaryCommands = App.Setting.DictionaryCommands ?? new Dictionary<string, string>();
            App.Setting.DictionaryCommands.Clear();
            foreach (var sound in Sounds)
            {
                App.Setting.DictionaryCommands.Add(sound.Command, sound.PathSound);
            }
        }

        private static void SaveInFile()
        {
            Stream saveFileStream = File.Create(FileName);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(saveFileStream, App.Setting);
            saveFileStream.Close();
        }
        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            App.Setting = ViewModel.AppSetting;
            App.Setting.Timeout = Convert.ToUInt32(TextBoxTimeout.Text);
            SaveChangeListCommands();
            SaveInFile();
            Application.Current.Shutdown();
        }

        private Task AddNewReceivedLog(DateTime time, string username, string nameCommand)
        {
            if (CheckBoxGetOffer.IsChecked == false) return null;
            ViewModel.QueueOffers.Add(new Offer(time, nameCommand, username));
            /*
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
            */
            return Task.CompletedTask;
        }

        private void CancelOnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.QueueOffers.Remove(ViewModel.QueueOffers.First(x => x.Id == ((Button) sender).Tag as string));
            /*var dataContext = ((sender as Button)?.DataContext as string).Split(' ');
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
            }*/
        }

        private void AcceptOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = App.Setting.DictionaryCommands[
                    ViewModel.QueueOffers.First(x => x.Id == ((Button) sender).Tag as string).Message.Replace(ViewModel.AppSetting.Prefix, "")];
                SoundPlayer player = new SoundPlayer(path);
                player.Play();
                ViewModel.QueueOffers.Remove(ViewModel.QueueOffers.First(x => x.Id == ((Button) sender).Tag as string));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            /*try
            {
                var dataContext = ((sender as Button)?.DataContext as string).Split(' ');
                string path = App.Setting.DictionaryCommands[dataContext.First()];
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
            }*/
        }

        private void CheckBoxEveryOne_OnClick(object sender, RoutedEventArgs e)
        {

            if (sender is CheckBox)
                App.Setting.ReceiveEveryone = (sender as CheckBox).IsChecked.Value;
            else
                MessageBox.Show("Объект не является переключателем");
        }

        private void CheckBoxSubscriber_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox)
                App.Setting.ReceiveSubscriber = (sender as CheckBox).IsChecked.Value;
            else
                MessageBox.Show("Объект не является переключателем");
        }

        private void CheckBoxFollower_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox)
                App.Setting.ReceiveFollower = (sender as CheckBox).IsChecked.Value;
            else
                MessageBox.Show("Объект не является переключателем");
        }

        private void CheckBoxGetOffer_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox)
                App.Setting.IsGetOffer = (sender as CheckBox).IsChecked.Value;
            else
                MessageBox.Show("Объект не является переключателем");
        }

        private void ButtonDeleteCommand_OnClick(object sender, RoutedEventArgs e)
        {
            Sounds.Remove(Sounds.First(x => x.Command == ((Button) sender).DataContext));
        }

        private void ButtonEditTwitchChannel_OnClick(object sender, RoutedEventArgs e)
        {
            AuthorizeTwitch page = new AuthorizeTwitch();
            page.Owner = this;
            page.Show();
        }

        private void TextBoxTimeout_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 ||
                 e.Key == Key.Back || e.Key == Key.Delete))
            {
                e.Handled = true;
            }
        }

        private void SyncCommands()
        {
            Sounds = Sounds ?? new ObservableCollection<Sound>();
            foreach (var pair in App.Setting.DictionaryCommands)
            {
                Sounds.Add(new Sound(pair.Key, pair.Value));
            }
        }

        private void TextBox_Command_OnLostFocus(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show();
            
        }
    }
}