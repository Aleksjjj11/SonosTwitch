using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SonosTwitch.Models;
using SonosTwitch.ViewModels;

namespace SonosTwitch.Views
{
    /// <summary>
    /// Логика взаимодействия для VideoOffersWindow.xaml
    /// </summary>
    public partial class VideoOffersWindow : Window
    {
        public VideoOffersWindow(AppSetting setting, TwitchBot bot)
        {
            DataContext = new VideoOffersWindowVM(setting, bot);
            InitializeComponent();
        }

        private void VideoOffersWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
