using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OnlineShopWinApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    public partial class RegistrPage : Page
    {
        public RegistrPage()
        {
            InitializeComponent();
        }


        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullNameBox.Text.Trim();
            string email = EmailBox.Text.Trim();
            string login = LoginBox.Text.Trim();
            string password = PassBox.Password;

            if(fullName == "" ||  login == "" || password == "")
            {
                MessageBox.Show("Заполните ФИО, логин и пароль");
                return;
            }

            using (var db = new CinemaDBEntities()) {
                if (db.Users.Any(u => u.Login == login)) {
                    MessageBox.Show("Такой логин уже занят");
                    return;
                }
                var user = new User
                {
                    FullName = fullName,
                    Email = email,
                    Login = login,
                    Password = password
                };
                db.Users.Add(user);
                db.SaveChanges();

                CurrentUser.User = user;
            }

            MessageBox.Show("Регистрация прошла успешно");
            NavigationService.Navigate(new ProfPage());
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

    }
}
