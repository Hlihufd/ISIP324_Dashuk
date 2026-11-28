using System.Windows;
using System.Windows.Controls;
using OnlineShopWinApp.Pages;

namespace OnlineShopWinApp
{
    public partial class MainWindow : Window
    {
        private const string Placeholder = "🔍Введите название...";
        private MainPage mainPage;

        public static MainWindow Instance { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;

            mainPage = new MainPage();
            MainFrame.Navigate(mainPage);
        }

        public void GoHome()
        {
            if (MainFrame.Content != mainPage)
                MainFrame.Navigate(mainPage);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser.User == null)
                MainFrame.Navigate(new LoginPage());
            else
                MainFrame.Navigate(new ProfPage());
        }

        private void HomeItem_Click(object sender, RoutedEventArgs e)
        {
            GoHome();
        }

        // Подсказка в строке поиска
        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == Placeholder)
                SearchBox.Text = "";
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text.Trim() == "")
                SearchBox.Text = Placeholder;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool typed = SearchBox.IsKeyboardFocused && SearchBox.Text != "";
            ApplyFilter(typed);
        }

        private void SortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter(true);
        }

        private void ApplyFilter(bool goHome)
        {
            if (mainPage == null) return; // событие сработало при создании окна

            string text = SearchBox.Text == Placeholder ? "" : SearchBox.Text.Trim();
            mainPage.ApplyFilter(text, SortBox.SelectedIndex);

            if (goHome) GoHome();
        }
    }
}