using MainUi.Views;
using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MainUi
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void PickImgBtn_Click(object sender, RoutedEventArgs e)
        {
            var tempFolder = ApplicationData.Current.TemporaryFolder;
        }

        private async void LoginFbBtn_Click(object sender, RoutedEventArgs e)
        {
            var ret = await FacebookAuthService.LoginAndGetTokenAsync();
        }

        private void GoToABtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Content is string pageName)
            {
                Type pageType = Type.GetType($"MainUi.Views.Page{pageName}");
                if (pageType != null)
                {
                    MyFrame.Navigate(pageType, null, new DrillInNavigationTransitionInfo());
                }
            }
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            if (MyFrame.CanGoBack)
            {
                MyFrame.GoBack();
            }
        }

        private void GoForward_Click(object sender, RoutedEventArgs e)
        {
            if (MyFrame.CanGoForward)
            {
                MyFrame.GoForward();
            }
        }
    }
}
