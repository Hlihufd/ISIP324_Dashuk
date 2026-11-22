using System.Linq;
using System.Windows.Controls;

namespace OnlineShopWinApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    public partial class ProfPage : Page
    {
        public ProfPage()
        {
            InitializeComponent();
            var user = CurrentUser.User;
            if (user == null) return;

            FullNameText.Text = "ФИО " + user.FullName;
            LoginText.Text = "Логин " + user.Login;
            EmailText.Text = "Email " + user.Email;

            using(var db = new CinemaDBEntities())
            {
                TicketsList.ItemsSource = db.Tickets.Where(t => t.UserId == user.Id)
                    .Select(t => new
                    {
                        Movie = t.Session.Movy.Title,
                        Hall = t.Session.Hall.Number,
                        Time = t.Session.StartTime,
                        t.RowNumber,
                        t.SeatNumber,
                        t.Price
                    }).ToList();
            }
        }
    }
}
