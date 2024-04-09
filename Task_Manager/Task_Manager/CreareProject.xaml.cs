using System;
using System.Collections.Generic;
using System.Data;
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

namespace Task_Manager
{
    /// <summary>
    /// Логика взаимодействия для CreareProject.xaml
    /// </summary>
    public partial class CreareProject : Page
    {
        TaskManagerEntities db = new TaskManagerEntities();
        function fn = new function();
        int idproject;
        int idd;
        int flagid;
        public class filldg
        {
            public int user_id { get; set; }
            public int member_id { get; set; }
            public string surname { get; set; }
            public string name { get; set; }
        }

        public CreareProject(int id, int flag)
        {
            InitializeComponent();

            idproject = flag;
            flagid = flag;
            idd = id;

            // Получение названий команд, в которых состоит пользователь с определенным id
            var teamTitles = db.Teams
                .Where(t => db.Members.Any(m => m.IdUser == id && m.IdTeam == t.Id))
                .Select(t => t.Title)
                .ToList();

            foreach (var title in teamTitles)
            {
                comboTeam.Items.Add(title);
            }

            // Получение названий статусов
            var statusTitles = db.Status
                .Select(s => s.Title)
                .ToList();

            foreach (var title in statusTitles)
            {
                comboStatus.Items.Add(title);
            }

            if (flag > 0)
            {
                var task = db.Task.FirstOrDefault(t => t.Id == flag);
                var projectTitle = db.Teams.FirstOrDefault(t => t.Id == task.IdTeam)?.Title;
                var statusTitle = db.Status.FirstOrDefault(s => s.Id == task.IdStatus)?.Title;

                txtName.Text = task.Title;
                txtSpecification.Text = task.Specification;
                comboTeam.SelectedValue = projectTitle;
                comboStatus.SelectedValue = statusTitle;
                DatePickerStart.SelectedDate = task.DateOfStart;
                DatePickerEnd.SelectedDate = task.DateOfEnd;

                var users = from user in db.User
                                         join member in db.Members on user.Id equals member.IdUser
                                         join team in db.Teams on member.IdTeam equals team.Id
                                         join tt in db.Task on team.Id equals tt.IdTeam
                                         where tt.Id == idproject
                                         select new
                                         {
                                             user.Id,
                                             user.Surname,
                                             user.Name
                                         };

                foreach (var user in users)
                {
                    filldg filldg = new filldg();
                    filldg.surname = user.Surname;
                    filldg.name = user.Name;

                    dgMembers.Items.Add(filldg);
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (flagid > 0)
            {
                var project = db.Task.FirstOrDefault(p => p.Id == flagid);

                if (project != null)
                {
                    StringBuilder errors = new StringBuilder();
                    Task task = new Task();

                    if (string.IsNullOrEmpty(txtName.Text))
                    {
                        errors.AppendLine("Укажите название проекта!");
                    }
                    if (string.IsNullOrEmpty(txtSpecification.Text))
                    {
                        errors.AppendLine("Укажите описание проекта!");
                    }
                    if (comboTeam.SelectedIndex == -1)
                    {
                        errors.AppendLine("Выберите название команды!");
                    }
                    if (comboStatus.SelectedIndex == -1)
                    {
                        errors.AppendLine("Выберите статус проекта!");
                    }

                    if (errors.Length > 0)
                    {
                        MessageBox.Show(errors.ToString(), "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        project.Title = txtName.Text;
                        project.Specification = txtSpecification.Text;
                        project.DateOfStart = DatePickerStart.SelectedDate.Value;
                        project.DateOfEnd = DatePickerEnd.SelectedDate.Value;

                        var statusId = db.Status
                        .Where(s => s.Title == comboStatus.SelectedValue.ToString())
                        .Select(s => s.Id)
                        .FirstOrDefault();

                        var teamId = db.Teams
                            .Where(t => t.Title == comboTeam.SelectedValue.ToString())
                            .Select(t => t.Id)
                            .FirstOrDefault();

                        project.IdStatus = statusId;
                        project.IdTeam = teamId;

                        db.SaveChanges();

                        MessageBox.Show("Данные изменены!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Information);

                        db.AddUserHistoryRecord(idd, 8, DateTime.Now);
                    }
                }
            }
            else
            {
                StringBuilder errors = new StringBuilder();
                Task task = new Task();

                if (string.IsNullOrEmpty(txtName.Text))
                {
                    errors.AppendLine("Укажите название проекта!");
                }
                if (string.IsNullOrEmpty(txtSpecification.Text))
                {
                    errors.AppendLine("Укажите описание проекта!");
                }
                if (comboTeam.SelectedIndex == -1)
                {
                    errors.AppendLine("Выберите название команды!");
                }
                if (comboStatus.SelectedIndex == -1)
                {
                    errors.AppendLine("Выберите статус проекта!");
                }

                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString(), "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    task.Title = txtName.Text;
                    task.Specification = txtSpecification.Text;
                    task.DateOfStart = DatePickerStart.SelectedDate.Value;
                    task.DateOfEnd = DatePickerEnd.SelectedDate.Value;

                    var statusId = db.Status
                    .Where(s => s.Title == comboStatus.SelectedValue.ToString())
                    .Select(s => s.Id)
                    .FirstOrDefault();

                    var teamId = db.Teams
                        .Where(t => t.Title == comboTeam.SelectedValue.ToString())
                        .Select(t => t.Id)
                        .FirstOrDefault();

                    task.IdStatus = statusId;
                    task.IdTeam = teamId;

                    db.Task.Add(task);
                    db.SaveChanges();
                    MessageBox.Show("Данные сохранены!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Information);

                    db.AddUserHistoryRecord(idd, 5, DateTime.Now);
                }
            }
        }

        private void btnEditMembers_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new EditUsersInTeam(idd, 0, idproject, 0, 0));
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new Menu(idd));
        }
    }
}
