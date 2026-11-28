using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace OnlineShopWinApp.Pages
{
    public class MovieCard
    {
        public Movy Movie { get; set; }
        public string Title { get; set; }
        public decimal Rating { get; set; }
        public DateTime StartDate { get; set; }
        public string AgeRating { get; set; }
        public string PosterPath { get; set; }
    }
    public partial class MainPage : Page
    {
        private List<MovieCard> allMovies;

        public MainPage()
        {
            InitializeComponent();

            using(var db = new CinemaDBEntities())
            {
                allMovies = db.Movies.ToList().Select(m => new MovieCard
                {
                    Movie = m,
                    Title = m.Title,
                    Rating = m.Rating,
                    StartDate = DateTime.Now,
                    AgeRating = m.AgeRating,
                    PosterPath = string.IsNullOrEmpty(m.ImagePath) ? null : "pack://application:,,,/" + m.ImagePath.TrimStart('/')
                }).ToList();
            }
            ApplyFilter("", 0);
        }

        public void ApplyFilter(string text, int sortIndex)
        {
            IEnumerable<MovieCard> result = allMovies;
            if(text != null) result = result.Where(m => m.Title.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) >= 0);

            if(sortIndex == 1) result = result.OrderByDescending(m => m.Rating);
            else result = result.OrderBy(m => m.Title);

            MoviesList.ItemsSource = result.ToList();
        }

       

        private void Card_Click(object sender, MouseButtonEventArgs e)
        {
            var card = (sender as FrameworkElement).DataContext as MovieCard;
            NavigationService.Navigate(new MoviePage(card.Movie));
        }
    }
}
