using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
using static System.Net.Mime.MediaTypeNames;
using static Task_Manager.Menu;

namespace Task_Manager
{
    /// <summary>
    /// Логика взаимодействия для Menu.xaml
    /// </summary>
    public partial class Menu : Page
    {
        TaskManagerEntities db = new TaskManagerEntities();
        function fn = new function();
        dbconnection dbconn = new dbconnection();
        List<Teams> teams1 = new List<Teams>();
        List<MySubtask> mySubtasks = new List<MySubtask>();
        int iduser;
        int mainid;
        string locationImg;

        public class DateFormatConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is DateTime date)
                {
                    return date.ToString("dd-MM-yyyy");
                }
                return value;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        public void UpdateUser(int userId, byte[] photo, string name, string surname, string login, string password, string phoneNumber, string email)
        {
            var user = db.User.Find(userId);
            if (user != null)
            {
                user.Photo = photo;
                user.Name = name;
                user.Surname = surname;
                user.Login = login;
                user.Password = password;
                user.PhoneNumber = phoneNumber;
                user.Email = email;

                db.SaveChanges();
            }
        }

        public BitmapImage GetImageByUserId(int userId)
        {
            var user = db.User.FirstOrDefault(u => u.Id == userId);
            if (user != null && user.Photo != null)
            {
                using (MemoryStream stream = new MemoryStream(user.Photo))
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = stream;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    return image;
                }
            }

            return null;
        }

        public User GetUserById(int userId)
        {
            var user = db.User.FirstOrDefault(u => u.Id == userId);
            return user;
        }

        public class MySubtask
        {
            public int subtask_id { get; set; }
            public string subtask_title { get; set; }
            public DateTime DateOfEnd { get; set; }
            public string status_title { get; set; }
            public string task_title { get; set; }
        }

        public class MyProject
        {
            public int project_id { get; set; }
            public string project_title { get; set; }
            public DateTime DateOfEnd { get; set; }
            public string status_title { get; set; }
        }

        private int GetSelectedTaskId(DataGrid dataGrid)
        {
            if (dataGrid.SelectedItem != null)
            {
                MySubtask selectedTask = (MySubtask)dataGrid.SelectedItem;
                return selectedTask.subtask_id;
            }

            return -1; // Возвращаем -1, если ничего не выбрано
        }

        private int GetSelectedProjectId(DataGrid dataGrid)
        {
            if (dataGrid.SelectedItem != null)
            {
                MyProject selectedProject = (MyProject)dataGrid.SelectedItem;
                return selectedProject.project_id;
            }

            return -1; // Возвращаем -1, если ничего не выбрано
        }

        public Menu(int id)
        {
            InitializeComponent();

            //Смена цвета темы
            PaletteHelper paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();
            theme.SetPrimaryColor(Colors.Black);
            paletteHelper.SetTheme(theme);

            iduser = id;

            //Загружаем аватарку из базы данных
            BitmapImage userImage = GetImageByUserId(id);

            if (userImage != null)
            {
                UserPhoto.Source = userImage;
            }
            else
            {
                //Добавить аватарку пустому
            }

            var user = db.User.Where(u => u.Id == id).FirstOrDefault();

            //Заполняем данные (личные + меняем имя приложения)
            if (user != null)
            {
                System.Windows.Application.Current.MainWindow.Title = user.Name + " " + user.Surname;

                txtName.Text = user.Name;
                txtSurname.Text = user.Surname;
                txtPhone.Text = user.PhoneNumber;
                txtEmail.Text = user.Email;

            }

            var teamIds = db.Members.Where(m => m.IdUser == id).Select(m => m.IdTeam).ToList();
            var teams = db.Teams.Where(t => teamIds.Contains(t.Id)).ToList();

            dgTeams.ItemsSource = teams.ToList();

            var query_subtask = from subtask in db.Subtask
                                join status in db.Status on subtask.IdStatus equals status.Id
                                join task in db.Task on subtask.IdTask equals task.Id
                                join memberSubtask in db.MemberSubtask on subtask.Id equals memberSubtask.IdSubtask
                                join member in db.Members on memberSubtask.IdMember equals member.Id
                                where db.Members.Any(m => m.IdUser == id && m.IdTeam == task.IdTeam)
                                select new
                                {
                                    subtask_id = subtask.Id,
                                    subtask_title = subtask.Title,
                                    DateOfEnd = subtask.DateOfEnd,
                                    status_title = status.Title,
                                    task_title = task.Title
                                };

            foreach (var result in query_subtask)
            {
                MySubtask mySubtask = new MySubtask
                {
                    subtask_id = result.subtask_id,
                    subtask_title = result.subtask_title,
                    DateOfEnd = (DateTime)result.DateOfEnd,
                    status_title = result.status_title,
                    task_title = result.task_title
                };
                dgMyAssignments.Items.Add(mySubtask);
            }

            var query_project = from task in db.Task
                                join status in db.Status on task.IdStatus equals status.Id
                                where db.Members.Any(m => m.IdUser == id && m.IdTeam == task.IdTeam)
                                select new
                                {
                                    project_id = task.Id,
                                    project_title = task.Title,
                                    DateOfEnd = task.DateOfEnd,
                                    status_title = status.Title
                                };

            foreach (var result in query_project)
            {
                MyProject myProject = new MyProject
                {
                    project_id = result.project_id,
                    project_title = result.project_title,
                    DateOfEnd = (DateTime)result.DateOfEnd,
                    status_title = result.status_title
                };
                dgMyProject.Items.Add(myProject);
            }

            mainid = id;
        }

        private void btnCreateTeam_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddTeam(iduser, 0));
        }

        private void btnCreateProject_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new CreareProject(iduser, 0));
        }

        private void btnCreateAssignments_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new CreateSubtask(iduser, 0));
        }

        private void btnChangeUser_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены что хотите выйти из учетной записи?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Manager.MainFrame.Navigate(new Authorization());

                System.Windows.Application.Current.MainWindow.Title = "Task Manager";
            }
        }

        private void btnEditSubtask_Click(object sender, RoutedEventArgs e)
        {
            int selectedTaskId = GetSelectedTaskId(dgMyAssignments);
            if (selectedTaskId != -1)
            {
                Manager.MainFrame.Navigate(new CreateSubtask(mainid, selectedTaskId));
            }
            else
            {
                MessageBox.Show("Сначала выберите задачу!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnEdit2_Click(object sender, RoutedEventArgs e)
        {
            var query = db.Members
                .Where(m => m.IdRole == 1 && db.Teams.Select(t => t.Id).Distinct().Contains(m.IdTeam))
                .Select(m => new { m.IdUser, m.IdTeam });

            var result = query.ToList(); // Преобразование результата запроса в список

            int idUser = result.First().IdUser;

            if (iduser != idUser)
            {
                MessageBox.Show("Вы не можете редактировать этот проект, т.к. не являетесь его руководителем!", "Недостаточно прав!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                int selectedProjectId = GetSelectedProjectId(dgMyProject);
                if (selectedProjectId != -1)
                {
                    Manager.MainFrame.Navigate(new CreareProject(iduser, selectedProjectId));
                }
                else
                {
                    MessageBox.Show("Сначала выберите проект!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgTeams.SelectedIndex == -1)
            {
                MessageBox.Show("Сначала выберите команду!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                Teams teams = (Teams)dgTeams.Items[dgTeams.SelectedIndex];

                //Проверка является ли человек руководителем команды
                string teamTitle = teams.Title;
                var leaderId = (from m in db.Members
                              where m.IdRole == 1 && m.IdTeam == (from t in db.Teams where t.Title == teamTitle select t.Id).FirstOrDefault()
                              select m.IdUser).FirstOrDefault();
                if (iduser == Convert.ToInt32(leaderId))
                {
                    //получение id из datagrid
                    Manager.MainFrame.Navigate(new AddTeam(iduser, teams.Id));
                } else
                {
                    MessageBox.Show("Вы не можете редактировать команду т.к. не являетесь её руководителем!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);

                    return;
                }
            }
        }

        private void btnChangePhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter = "Image Files (*.jpg, *.jpeg, *.png) | *.jpg, *.jpeg, *.png";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (openFileDialog.ShowDialog() == true)
            {
                locationImg = openFileDialog.FileName.ToString();
                UserPhoto.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }

        }

        private void btnRefreshData_Click(object sender, RoutedEventArgs e)
        {
            byte[] imageBytes;
            using (var stream = new MemoryStream())
            {
                // Преобразуйте изображение Image в массив байтов
                BitmapImage image = (BitmapImage)UserPhoto.Source;
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(stream);
                imageBytes = stream.ToArray();
            }

            User user = GetUserById(iduser);

            string login = user.Login;
            string password = user.Password;

            UpdateUser(iduser, imageBytes, txtName.Text, txtSurname.Text, login, password, txtPhone.Text, txtEmail.Text);

            MessageBox.Show("Данные успешно изменены!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnUserHistory_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new UserHistory(iduser));
        }
    }
}
