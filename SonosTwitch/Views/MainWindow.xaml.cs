using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.Serialization.Formatters.Binary;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;
using SonosTwitch.Models;
using SonosTwitch.ViewModels;
using TwitchLib.Api.Core.Models.Undocumented.Comments;
using TwitchLib.Client.Events;

namespace SonosTwitch
{
    //TODO https://github.com/aleksjjj11/SonosTwitch/releases/latest for UPDATE SYSTEM
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        //TODO Перенести Setting в ViewModel и всё, что с этим связано
        const string FileName = @"SavedSetting.bin";
        private DispatcherTimer _timerColor;
        public MainWindowVM ViewModel { get; set; }
        private delegate Task OnPlaySong(string nameCommand);
        private System.Windows.Forms.NotifyIcon notifyIcon = null;

        public MainWindow()
        {
            bool needUpdateListCommand = !LoadSaving();
            DataContext = ViewModel = new MainWindowVM(new TwitchBot(App.Setting.TwitchChannel, App.Setting.TwitchToken), App.Setting);
            SyncCommands();
            ViewModel.ClientBot.Notify += TwitchBotOnNotify;
           
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

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
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

        private void TwitchBotOnNotify(object sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Message.Contains(ViewModel.AppSetting.Prefix + ViewModel.AppSetting.SpeechCommand))
            {
                SpeechSynthesizer speech = new SpeechSynthesizer();
                speech.Speak(e.ChatMessage.Message.Remove(0, ViewModel.AppSetting.SpeechCommand.Length + 1));
                return;
            }

            if (ViewModel.AppSetting.IsGetOffer)
            {
                ViewModel.QueueOffers.Add(new Offer(DateTime.Now, e.ChatMessage.Message, e.ChatMessage.Username));
                return;
            }
            Dispatcher?.BeginInvoke(DispatcherPriority.Normal, new OnPlaySong(PlaySong), e.ChatMessage.Message.Remove(0, 1));
        }

        private Task PlaySong(string nameCommand)
        {
            try
            {
                string path = ViewModel.Sounds.First(x => x.Command == nameCommand).PathSound;
                MediaPlayer player = new MediaPlayer();
                player.Open(new Uri(path));
                player.Volume = ViewModel.Sounds.First(x => x.PathSound == path).Volume;
                player.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
            ViewModel.Sounds.Add(new Sound($"NewCommand{ViewModel.Sounds.Count}", "Opa"));
        }

        private void ButtonOpenAudioFile_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog {DefaultExt = ".wav", Filter = "(.wav)|*.wav|(.mp3)|*.mp3"};
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
                
                return true;
            }
            
            //App.Setting = new AppSetting(new Dictionary<string, string>());
            App.Setting = new AppSetting(new List<Sound>());
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
            App.Setting.DictionaryCommands = App.Setting.DictionaryCommands ?? new List<Sound>();
            App.Setting.DictionaryCommands.Clear();
            foreach (var sound in ViewModel.Sounds)
            {
                App.Setting.DictionaryCommands.Add(sound);
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

        private void CancelOnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.QueueOffers.Remove(ViewModel.QueueOffers.First(x => x.Id == ((Button) sender).Tag as string));
        }

        private void AcceptOnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = ViewModel.Sounds.First(x => x.Command == ViewModel.QueueOffers.First(y => y.Id == ((Button)sender).Tag as string).Message.Replace(ViewModel.AppSetting.Prefix, "")).PathSound;
                MediaElement player = new MediaElement();
                player.Source = new Uri(path);
                player.Volume = ViewModel.Sounds.First(x => x.PathSound == path).Volume;
                player.Play();
                ViewModel.QueueOffers.Remove(ViewModel.QueueOffers.First(x => x.Id == ((Button) sender).Tag as string));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
            try
            {
                ViewModel.Sounds.Remove(ViewModel.Sounds.First(x => x.Command == (string) ((Button) sender).DataContext));
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
            ViewModel.Sounds = ViewModel.Sounds ?? new ObservableCollection<Sound>();
            foreach (var pair in App.Setting.DictionaryCommands)
            {
                ViewModel.Sounds.Add(pair);
            }
        }

        private void Button_CloseApp(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void MainWindows_Initialized(object sender, EventArgs e)
        {
            System.Windows.Forms.ContextMenuStrip contextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            System.Windows.Forms.ToolStripMenuItem showItem = new System.Windows.Forms.ToolStripMenuItem("Show");
            showItem.Click += ShowItem_Click;
            System.Windows.Forms.ToolStripMenuItem exitItem = new System.Windows.Forms.ToolStripMenuItem("Exit");
            exitItem.Click += ExitItem_Click;
            contextMenuStrip.Items.Add(showItem);
            contextMenuStrip.Items.Add(exitItem);
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.DoubleClick += new EventHandler(notifyIcon_DoubleClick);
            notifyIcon.Text = "SoNoS";
            notifyIcon.ContextMenuStrip = contextMenuStrip;
            //notifyIcon.Icon = new System.Drawing.Icon("C:/Users/TaskeDes/YandexDisk/C#/SonosTwitch/SonosTwitch/Icons/presentation.ico");
            notifyIcon.Icon = SystemIcons.Application;
            notifyIcon.Visible = true;

        }

        private void ExitItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ShowItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }
    }
}