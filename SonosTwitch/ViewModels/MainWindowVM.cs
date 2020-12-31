using SonosTwitch.Commands;
using SonosTwitch.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SonosTwitch.ViewModels
{
    public class MainWindowVM : BaseVM
    {
        const string FileName = @"SavedSetting.bin";
        private TwitchBot clientBot;
        public TwitchBot ClientBot
        {
            get => clientBot;
            set
            {
                clientBot = value;
                OnPropertyChanged();
            }
        }
        private AppSetting appSetting;
        public AppSetting AppSetting
        {
            get => appSetting;
            set
            {
                appSetting = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Offer> QueueOffers { get; set; }
        //private ObservableCollection<Sound> sounds;
        //public ObservableCollection<Sound> Sounds
        //{
        //    get => sounds; 
        //    set
        //    {
        //        sounds = value;
        //        OnPropertyChanged();
        //    } 
        //}

        public MainWindowVM()
        {
            LoadSaving();
            appSetting = App.Setting;
            clientBot = new TwitchBot(appSetting.TwitchChannel, appSetting.TwitchToken, appSetting);
            QueueOffers = new ObservableCollection<Offer>();
            //sounds = new ObservableCollection<Sound>();
            //foreach (var el in appSetting.DictionaryCommands)
            //{
            //    sounds.Add(el);
            //}
        }
        private ICommand _addSound;
        public ICommand AddSoundCommand
        {
            get => _addSound ?? new RelayCommand(() =>
            {
                AppSetting.DictionaryCommands.Add(new Sound($"NewCommand{AppSetting.DictionaryCommands.Count}", "Path"));
            }, () => true);
        }
        private ICommand _resetAllSounds;
        public ICommand ResetAllSoundsCommand
        {
            get => _resetAllSounds ?? new RelayCommand(() => 
            {
                AppSetting.DictionaryCommands.Clear();
            }, () => true);
        }
        private ICommand _saveSetting;
        public ICommand SaveSettingCommand
        {
            get => _saveSetting ?? new RelayCommand(() => 
            {
                App.Setting = AppSetting;
                ClientBot.UpdateSetting(AppSetting);
                SaveInFile();   
            }, () => true);
        }
        private ICommand _hideApp;
        public ICommand HideAppCommand
        {
            get => _hideApp ?? new RelayCommand<Window>(x =>
            {
                x.Hide();
            }, x => true);
        }
        private ICommand _openTwitchEditWindow;
        public ICommand OpenTwitchEditWindow
        {
           get => _openTwitchEditWindow ?? new RelayCommand<Window>(x =>
           {
               AuthorizeTwitch page = new AuthorizeTwitch();
               page.Owner = x;
               page.Show();
           });
        }
        //private ICommand _deleteSound;
        //public ICommand DeleteSoundCommand
        //{
        //    get => _deleteSound ?? new RelayCommand<Sound>(x =>
        //    {
        //        try
        //        {
        //            //Sounds.Remove(x);
        //            MessageBox.Show("click cluck");
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.Message);
        //        }
        //    }, x => true);
        //}
        public static void SaveInFile()
        {
            Stream saveFileStream = File.Create(FileName);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(saveFileStream, App.Setting);
            saveFileStream.Close();
        }
        public bool LoadSaving()
        {
            if (File.Exists(FileName))
            {
                Stream openFileStream = File.OpenRead(FileName);
                BinaryFormatter deserializer = new BinaryFormatter();
                App.Setting = (AppSetting)deserializer.Deserialize(openFileStream);
                App.Setting.TimeLastLoaded = DateTime.Now;
                openFileStream.Close();

                return true;
            }

            App.Setting = new AppSetting();
            return false;
        }

        private ICommand _declineOfferCommand;
        public ICommand DeclineOfferCommand => _declineOfferCommand ?? new RelayCommand<TextBlock>(x =>
        {
            QueueOffers.Remove(QueueOffers.First(k => k.Id == (x.Tag as string)));
        }, x => true);


        private ICommand _acceptOfferCommand;
        public ICommand AcceptOfferCommand => _acceptOfferCommand ??  new RelayCommand<TextBlock>(x =>
        {
            try
            {
                var sound = AppSetting.DictionaryCommands.First(k => k.Command == QueueOffers.First(y => y.Id == x.Tag as string).Message.Replace(AppSetting.Prefix, ""));
                string path = sound.PathSound;
                MediaPlayer player = new MediaPlayer();
                player.Open(new Uri(path));
                player.Volume = sound.Volume;
                player.Play();
                QueueOffers.Remove(QueueOffers.First(y => y.Id == x.Tag as string));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }, x => true);

        private ICommand _deleteSoundCommand;

        public ICommand DeleteSoundCommand => _deleteSoundCommand ?? new RelayCommand<string>(x =>
        {
            //Delete sound by X's values from dictionary
            AppSetting.DictionaryCommands.Remove(AppSetting.DictionaryCommands.First(k => k.Command == x));
        }, x => true);
    }
}