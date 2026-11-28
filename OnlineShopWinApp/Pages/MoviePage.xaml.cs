using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace OnlineShopWinApp.Pages
{
    public partial class MoviePage : Page
    {
        private Movy movie;

        public MoviePage(Movy selectedMovie)
        {
            InitializeComponent();
            movie = selectedMovie;
            TitleText.Text = movie.Title;
            AgeText.Text = "Возраст: " + movie.AgeRating;
            RatingText.Text = "Рейтинг: " + movie.Rating;
            GenreText.Text = "Жанр: " + movie.Genres;
            DiscriptionText.Text =  movie.Description;

            LoadPoster();
            LoadSessions();
        }

        public void LoadPoster()
        {
            try
            {
                string path = "pack://application:,,,/" + movie.ImagePath.TrimStart('/');
                PosterImage.Source = new BitmapImage(new Uri(path));
            }
            catch  { }
        }

        public void LoadSessions()
        {
            using(var db = new CinemaDBEntities())
            {
                SessionsList.ItemsSource = db.Sessions
                    .Where(s => s.MovieId == movie.Id)
                    .OrderBy(s => s.StartTime)
                    .Select(s => new
                    {
                        s.Id,
                        Time = s.StartTime,
                        HallNumber = s.Hall.Number,
                        HallRating = s.Hall.Rating,
                        s.Price
                    }).ToList();
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void SelectSessionBtn_Click(object sender, RoutedEventArgs e)
        {
            int sessionId = (int)((Button)sender).Tag;

            if (CurrentUser.User == null) {
                MessageBox.Show("Для выбора места нужно войти в аккаунт");
                NavigationService.Navigate(new LoginPage());
                return;
            }

            NavigationService.Navigate(new SeatsPage(sessionId));
        }
    }
}
