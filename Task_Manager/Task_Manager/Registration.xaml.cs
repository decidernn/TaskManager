using System;
using System.Collections.Generic;
using System.Data;
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

namespace Task_Manager
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Page
    {
        TaskManagerEntities db = new TaskManagerEntities();
        function fn = new function();
        Validator validator = new Validator();
        public Registration()
        {
            InitializeComponent();
        }

        private void btnReg_Click(object sender, RoutedEventArgs e)
        {
            User user = new User();
            StringBuilder errors = new StringBuilder();
            PasswordHasher hasher = new PasswordHasher();

            string query = "SELECT * FROM [User] WHERE Login = '" + txtLogin.Text + "'";

            user.Surname = txtSurname.Text;
            user.Name = txtName.Text;
            user.Login = txtLogin.Text;
            user.Password = hasher.HashPassword(txtPassword.Text);
            user.PhoneNumber = txtPhone.Text;
            user.Email = txtEmail.Text;

            DataSet ds = fn.getData(query);

            if (!(validator.check_email(txtEmail.Text)))
            {
                MessageBox.Show("Почта введена некорретно! Повторите ввод!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }
            if (!(validator.check_phone(txtPhone.Text)))
            {
                MessageBox.Show("Номер телефона введен некорретно! Повторите ввод!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }
            if (!(validator.check_password(txtPassword.Text)))
            {
                MessageBox.Show("Пароль введен некорретно! Повторите ввод!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }
            if (!(validator.check_login(txtLogin.Text)))
            {
                MessageBox.Show("Логин введен некорретно! Повторите ввод!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            if (string.IsNullOrEmpty(txtLogin.Text))
            {
                errors.AppendLine("Введите логин!");
            }
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                errors.AppendLine("Введите пароль!");
            }
            if (string.IsNullOrWhiteSpace(txtSurname.Text))
            {
                errors.AppendLine("Введите фамилию!");
            }
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                errors.AppendLine("Введите имя!");
            }
            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                errors.AppendLine("Введите номер телефона!");
            }
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                errors.AppendLine("Введите email!");
            }
            if (txtPassword.Text != txtPasswordRepeat.Text)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Внимание!", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            } else
            {
                db.User.Add(user);
                db.SaveChanges();
                MessageBox.Show("Вы успешно зарегистрировались!", "Уведомление.", MessageBoxButton.OK, MessageBoxImage.Information);

                Manager.MainFrame.Navigate(new Authorization());
            }

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new Authorization());
        }
    }
}
