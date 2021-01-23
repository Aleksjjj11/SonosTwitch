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
using System.Windows.Documents;
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
        private DispatcherTimer _timerColor;
        public MainWindowVM ViewModel { get; set; }
        private delegate Task OnPlaySong(string nameCommand);

        private delegate Task OnAddOffer(Offer offer);
        private System.Windows.Forms.NotifyIcon notifyIcon = null;

        public MainWindow()
        {
            DataContext = ViewModel = new MainWindowVM();
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
            if (ViewModel.AppSetting.SpeechCommand != null &&
                e.ChatMessage.Message.Contains(ViewModel.AppSetting.Prefix + ViewModel.AppSetting.SpeechCommand))
            {
                SpeechSynthesizer speech = new SpeechSynthesizer();
                speech.Volume = ViewModel.AppSetting.VolumeTextSpeech;
                speech.Speak(e.ChatMessage.Message.Remove(0, ViewModel.AppSetting.SpeechCommand.Length + 1));
                return;
            }

            if (ViewModel.AppSetting.IsGetOffer)
            {
                Dispatcher?.BeginInvoke(DispatcherPriority.Normal, new OnAddOffer(AddOffer), new Offer(DateTime.Now, e.ChatMessage.Message, e.ChatMessage.Username));
                return;
            }
            Dispatcher?.BeginInvoke(DispatcherPriority.Normal, new OnPlaySong(PlaySong), e.ChatMessage.Message.Remove(0, 1));
        }

        private Task AddOffer(Offer offer)
        {
            ViewModel.QueueOffers.Add(offer);
            return Task.CompletedTask;
        }

        private Task PlaySong(string nameCommand)
        {
            try
            {
                string path = ViewModel.AppSetting.DictionaryCommands.First(x => x.Command == nameCommand).PathSound;
                MediaPlayer player = new MediaPlayer();
                player.Open(new Uri(path));
                player.Volume = ViewModel.AppSetting.DictionaryCommands.First(x => x.PathSound == path).Volume;
                player.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return Task.CompletedTask;
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

        private void SaveChangeListCommands()
        {
            /*App.Setting.DictionaryCommands = App.Setting.DictionaryCommands ?? new ObservableCollection<Sound>();
            App.Setting.DictionaryCommands.Clear();
            foreach (var sound in ViewModel.AppSetting.DictionaryCommands)
            {
                App.Setting.DictionaryCommands.Add(sound);
            }*/
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            /*App.Setting = ViewModel.AppSetting;*/
            /*SaveChangeListCommands();*/
            /*MainWindowVM.SaveInFile();*/
            Application.Current.Shutdown();
        }

        //Очень странные обработчики //TODO привязать их напрямую со свойствами

        //private void ButtonDeleteCommand_OnClick(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        ViewModel.AppSetting.DictionaryCommands.Remove(ViewModel.AppSetting.DictionaryCommands.First(x => x.Command == (string) ((Button) sender).DataContext));
        //    } catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}
        private void TextBoxTimeout_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9 ||
                 e.Key == Key.Back || e.Key == Key.Delete))
            {
                e.Handled = true;
            }
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

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start((sender as Hyperlink).NavigateUri.ToString());
        }
    }
}