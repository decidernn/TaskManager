using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
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
using System.Xml.Linq;

namespace Task_Manager
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// 
    /// 
    /// </summary>
    public partial class Authorization : Page
    {
        TaskManagerEntities db = new TaskManagerEntities();
        function fn = new function();
        PasswordHasher Hasher = new PasswordHasher();
        public Authorization()
        {
            InitializeComponent();

            txtLogin.Text = "decider";
            txtPassword.Password = "123";

            //Смена цвета темы
            PaletteHelper paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();
            theme.SetPrimaryColor(Colors.Black);
            paletteHelper.SetTheme(theme);
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var login = txtLogin.Text;
            var password = txtPassword.Password;

            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrEmpty(txtLogin.Text))
            {
                errors.AppendLine("Введите логин!");
            }
            if (string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                errors.AppendLine("Введите пароль!");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Внимание!", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string hashedpassword = Hasher.HashPassword(txtPassword.Password);

            var account = db.User.AsNoTracking().FirstOrDefault(a => a.Login == txtLogin.Text && a.Password == hashedpassword);

            if (account == null)
            {
                MessageBox.Show("Пользователь с такими данными не найден!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                MessageBox.Show("Добро пожаловать в систему!", "Уведомление!", MessageBoxButton.OK, MessageBoxImage.Information);

                var user = db.User.FirstOrDefault(u => u.Login == txtLogin.Text);

                Menu m = new Menu(user.Id);
                Manager.MainFrame.Navigate(m);

                db.AddUserHistoryRecord(user.Id, 1, DateTime.Now);
            }
        }

        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new Registration());
        }
    }
}
