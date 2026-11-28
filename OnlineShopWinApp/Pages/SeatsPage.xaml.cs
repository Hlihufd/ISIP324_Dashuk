using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OnlineShopWinApp.Pages
{
    public partial class SeatsPage : Page
    {
        private int sessionId;
        private int rows, seatsPerRow;
        private HashSet<string> taken = new HashSet<string>();
        private List<Button> takenButtons = new List<Button>();

        private Button selectedBtn;
        private int selectedRow, selectedSeat;

        private static Brush FreeBrush = Make("#d4c18f");
        private static Brush TakenBrush = Make("#7a6f5a");
        private static Brush SelectedBrush = Make("#7fa83a");

        private static Brush Make(string color)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
        }

        public SeatsPage(int sessionId)
        {
            InitializeComponent();
            this.sessionId = sessionId;

            using (var db = new CinemaDBEntities())
            {
                var s = db.Sessions.First(x => x.Id == sessionId);
                InfoText.Text = s.Movy.Title;
                InfoText2.Text = "Зал " + s.Hall.Number + ", " + s.StartTime.ToString("dd.MM.yyyy HH:mm")
                + ", цена " + s.Price.ToString("F0") + " руб";
                rows = s.Hall.RowsCount;
                seatsPerRow = s.Hall.SeatsPerRow;

                var tickets = db.Tickets.Where(t => t.SessionId == sessionId)
                .Select(t => new { t.RowNumber, t.SeatNumber }).ToList();
                foreach (var t in tickets)
                    taken.Add(t.RowNumber + "-" + t.SeatNumber);
            }

            BuildSeats();

            Loaded += (s, e) =>
            {
                if (CurrentUser.User == null)
                    NavigationService.Navigate(new LoginPage());
            };
        }

        private void BuildSeats()
        {
            for (int r = 1; r <= rows; r++)
            {
                var rowPanel = new StackPanel { Orientation = Orientation.Horizontal };
                rowPanel.Children.Add(new TextBlock
                {
                    Text = "Ряд " + r,
                    Width = 50,
                    VerticalAlignment = VerticalAlignment.Center
                });

                for (int n = 1; n <= seatsPerRow; n++)
                {
                    var btn = new Button
                    {
                        Content = n,
                        Width = 36,
                        Height = 32,
                        Margin = new Thickness(2),
                        Padding = new Thickness(0),
                        Tag = r + "-" + n
                    };

                    if (taken.Contains(r + "-" + n))
                    {
                        btn.Background = TakenBrush;
                        btn.IsHitTestVisible = false; // нельзя
                        takenButtons.Add(btn);
                    }
                    else
                    {
                        btn.Background = FreeBrush;
                        btn.Click += Seat_Click;
                    }
                    rowPanel.Children.Add(btn);
                }
                SeatsPanel.Children.Add(rowPanel);
            }
        }

        private void Seat_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;

            if (selectedBtn != null)
                selectedBtn.Background = FreeBrush;

            selectedBtn = btn;
            btn.Background = SelectedBrush;

            var parts = ((string)btn.Tag).Split('-');
            selectedRow = int.Parse(parts[0]);
            selectedSeat = int.Parse(parts[1]);
            SelectedText.Text = "Выбрано: ряд " + selectedRow + ", место " + selectedSeat;
        }

        private void HideBox_Changed(object sender, RoutedEventArgs e)
        {
            var v = HideBox.IsChecked == true ? Visibility.Hidden : Visibility.Visible;
            foreach (var b in takenButtons)
                b.Visibility = v;
        }

        private void BuyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedBtn == null)
            {
                MessageBox.Show("Сначала выберите место");
                return;
            }
            NavigationService.Navigate(new TicketPage(sessionId, selectedRow, selectedSeat));
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}