using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OnlineShopWinApp.Pages
{
    public partial class TicketPage : Page
    {
        private int sessionId, row, seat;
        private decimal price;

        public TicketPage(int sessionId, int row, int seat)
        {
            InitializeComponent();
            this.sessionId = sessionId;
            this.row = row;
            this.seat = seat;

            using (var db = new CinemaDBEntities())
            {
                var s = db.Sessions.First(x => x.Id == sessionId);
                price = s.Price;

                MovieText.Text = s.Movy.Title;
                HallText.Text = "Зал: " + s.Hall.Number;
                TimeText.Text = "Сеанс: " + s.StartTime.ToString("dd.MM.yyyy HH:mm");
                SeatText.Text = "Место: ряд " + row + ", место " + seat;
                PriceText.Text = "Цена: " + price.ToString("F0") + " ₽";
            }
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new CinemaDBEntities())
                {
                    bool busy = db.Tickets.Any(t => t.SessionId == sessionId
                    && t.RowNumber == row
                    && t.SeatNumber == seat);
                    if (busy)
                    {
                        MessageBox.Show("Это место уже занято, выберите другое");
                        NavigationService.GoBack();
                        return;
                    }

                    db.Tickets.Add(new Ticket
                    {
                        UserId = CurrentUser.User.Id,
                        SessionId = sessionId,
                        RowNumber = row,
                        SeatNumber = seat,
                        Price = price
                    });
                    db.SaveChanges();
                }

                MessageBox.Show("Билет оформлен!");
                MainWindow.Instance.GoHome();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не удалось оформить билет: " + ex.Message);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}