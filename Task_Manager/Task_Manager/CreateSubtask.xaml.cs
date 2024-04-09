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
using static Task_Manager.Menu;

namespace Task_Manager
{
    /// <summary>
    /// Логика взаимодействия для CreateSubtask.xaml
    /// </summary>
    public partial class CreateSubtask : Page
    {
        TaskManagerEntities db = new TaskManagerEntities();
        int idsubtask;
        int idd;
        bool isCheck = false;
        public class filldg
        {
            public int user_id { get; set; }
            public int member_id { get; set; }
            public string surname { get; set; }
            public string name { get; set; }
        }
        public void UpdateSubtask(int idsubtask, string title, string specifications, DateTime dateofstart, DateTime dateofend, int idstatus, int idtask)
        {
            var subtask = db.Subtask.Find(idsubtask);
            if (subtask != null)
            {
                subtask.Title = title;
                subtask.Specification = specifications;
                subtask.DateOfStart = dateofstart;
                subtask.DateOfEnd = dateofend;
                subtask.IdStatus = idstatus;
                subtask.IdTask = idtask;

                db.SaveChanges();
            }
        }

        public int FindSubtask(string title, string specification, int idstatus, int idtask)
        {
            Subtask subtask = db.Subtask.FirstOrDefault(s => s.Title == title
                && s.Specification == specification
                && s.IdStatus == idstatus
                && s.IdTask == idtask);

            if (subtask != null)
            {
                return subtask.Id; // Возвращаем идентификатор найденной подзадачи
            }

            return -1; // Возвращаем -1 в случае, если подзадача не найдена
        }

        public CreateSubtask(int id, int flag)
        {
            InitializeComponent();

            idsubtask = flag;
            idd = id;

            var taskTitles = db.Task
            .Where(t => db.Members.Any(m => m.IdUser == id && m.IdTeam == t.IdTeam))
            .Select(t => t.Title)
            .ToList();

            foreach (var title in taskTitles)
            {
                comboProject.Items.Add(title);
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
                var subtask = db.Subtask.FirstOrDefault(s => s.Id == flag);
                if (subtask != null)
                {
                    txtName.Text = subtask.Title;
                    txtSpecification.Text = subtask.Specification;

                    var projectTitle = db.Task.Where(t => t.Id == subtask.IdTask).Select(t => t.Title).FirstOrDefault();
                    comboProject.SelectedValue = projectTitle;

                    var statusTitle = db.Status.Where(s => s.Id == subtask.IdStatus).Select(s => s.Title).FirstOrDefault();
                    comboStatus.SelectedValue = statusTitle;

                    DatePickerStart.SelectedDate = subtask.DateOfStart;
                    DatePickerEnd.SelectedDate = subtask.DateOfEnd;
                    var members = db.User
                        .Join(db.Members, u => u.Id, m => m.IdUser, (u, m) => new { User = u, Member = m })
                        .Join(db.MemberSubtask, u_m => u_m.Member.Id, ms => ms.IdMember, (u_m, ms) => new { UserMember = u_m, MemberSubtask = ms })
                        .Join(db.Subtask, u_ms => u_ms.MemberSubtask.IdSubtask, s => s.Id, (u_ms, s) => new { UserMemberSubtask = u_ms, Subtask = s })
                        .Where(u_ms_s => u_ms_s.Subtask.Id == idsubtask)
                        .OrderBy(u_ms_s => u_ms_s.UserMemberSubtask.UserMember.User.Id)
                        .Select(u_ms_s => new
                        {
                            UserId = u_ms_s.UserMemberSubtask.UserMember.User.Id,
                            MemberId = u_ms_s.UserMemberSubtask.UserMember.Member.Id,
                            Surname = u_ms_s.UserMemberSubtask.UserMember.User.Surname,
                            Name = u_ms_s.UserMemberSubtask.UserMember.User.Name
                        })
                        .ToList();

                    foreach (var member in members)
                    {
                        filldg filldg = new filldg()
                        {
                            user_id = member.UserId,
                            member_id = member.MemberId,
                            surname = member.Surname,
                            name = member.Name
                        };
                        dgMembers.Items.Add(filldg);
                    }
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            Subtask subtask = new Subtask();

            if (string.IsNullOrEmpty(txtName.Text))
            {
                errors.AppendLine("Укажите название задачи!");
            }
            if (string.IsNullOrEmpty(txtSpecification.Text))
            {
                errors.AppendLine("Укажите описание задачи!");
            }
            if (comboProject.SelectedIndex == -1)
            {
                errors.AppendLine("Выберите название проекта!");
            }
            if (comboStatus.SelectedIndex == -1)
            {
                errors.AppendLine("Выберите статус задачи!");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (idsubtask <= 0)
                {
                    subtask.Title = txtName.Text;
                    subtask.Specification = txtSpecification.Text;
                    subtask.DateOfStart = DatePickerStart.SelectedDate.Value;
                    subtask.DateOfEnd = DatePickerEnd.SelectedDate.Value;

                    var statusId = db.Status
                    .Where(s => s.Title == comboStatus.SelectedValue.ToString())
                    .Select(s => s.Id)
                    .FirstOrDefault();

                    var taskId = db.Task
                        .Where(t => t.Title == comboProject.SelectedValue.ToString())
                        .Select(t => t.Id)
                        .FirstOrDefault();

                    subtask.IdStatus = statusId;
                    subtask.IdTask = taskId;

                    db.Subtask.Add(subtask);

                    db.SaveChanges();

                    MessageBox.Show("Данные сохранены! Теперь выберите исполнителей!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Information);

                    isCheck = true;

                    db.AddUserHistoryRecord(idd, 6, DateTime.Now);
                } else
                {
                    var statusId = db.Status
                        .Where(s => s.Title == comboStatus.SelectedValue.ToString())
                        .Select(s => s.Id)
                        .FirstOrDefault();

                    // Получение Id команды по названию из comboTeam
                    var taskId = db.Task
                        .Where(t => t.Title == comboProject.SelectedValue.ToString())
                        .Select(t => t.Id)
                        .FirstOrDefault();

                    UpdateSubtask(idsubtask, txtName.Text, txtSpecification.Text, DatePickerStart.SelectedDate.Value, DatePickerEnd.SelectedDate.Value, statusId, taskId);

                    MessageBox.Show("Данные подзадачи успешно изменены и сохранены!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Information);

                    db.AddUserHistoryRecord(idd, 9, DateTime.Now);
                }
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (isCheck)
            {
                var statusId = db.Status
                    .Where(s => s.Title == comboStatus.SelectedValue.ToString())
                    .Select(s => s.Id)
                    .FirstOrDefault();

                // Получение Id команды по названию из comboTeam
                var taskId = db.Task
                    .Where(t => t.Title == comboProject.SelectedValue.ToString())
                    .Select(t => t.Id)
                    .FirstOrDefault();

                int newidsubtask = FindSubtask(txtName.Text, txtSpecification.Text, statusId, taskId);

                Manager.MainFrame.Navigate(new EditUsersInTeam(idsubtask, 0, 0, 0, newidsubtask));
            }
            else
            {
                if (idsubtask <= 0)
                {
                    StringBuilder errors = new StringBuilder();

                    if (string.IsNullOrEmpty(txtName.Text))
                    {
                        errors.AppendLine("Укажите название задачи!");
                    }
                    if (string.IsNullOrEmpty(txtSpecification.Text))
                    {
                        errors.AppendLine("Укажите описание задачи!");
                    }
                    if (comboProject.SelectedIndex == -1)
                    {
                        errors.AppendLine("Выберите название проекта!");
                    }
                    if (comboStatus.SelectedIndex == -1)
                    {
                        errors.AppendLine("Выберите статус задачи!");
                    }

                    if (errors.Length > 0)
                    {
                        MessageBox.Show(errors.ToString(), "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {

                        Subtask subtask1 = new Subtask();
                        var statusId = db.Status
                            .Where(s => s.Title == comboStatus.SelectedValue.ToString())
                            .Select(s => s.Id)
                            .FirstOrDefault();

                        // Получение Id команды по названию из comboTeam
                        var taskId = db.Task
                            .Where(t => t.Title == comboProject.SelectedValue.ToString())
                            .Select(t => t.Id)
                            .FirstOrDefault();

                        subtask1.Title = txtName.Text;
                        subtask1.Specification = txtSpecification.Text;
                        subtask1.DateOfStart = DatePickerStart.SelectedDate.Value;
                        subtask1.DateOfEnd = DatePickerEnd.SelectedDate.Value;
                        subtask1.IdStatus = statusId;
                        subtask1.IdTask = taskId;

                        db.Subtask.Add(subtask1);

                        db.SaveChanges();

                        int newidsubtask = FindSubtask(txtName.Text, txtSpecification.Text, statusId, taskId);

                        Manager.MainFrame.Navigate(new EditUsersInTeam(idsubtask, 0, 0, 0, newidsubtask));
                    }
                }
                else
                {
                    Manager.MainFrame.Navigate(new EditUsersInTeam(idsubtask, idsubtask, 0, 0, 0));
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            //Проверка на то, чтобы у задачи были исполнители
            var subtaskId = db.Subtask
                          .Where(s => s.Title == txtName.Text && s.Specification == txtSpecification.Text)
                          .Select(s => s.Id)
                          .FirstOrDefault();

            var memberSubtaskCount = db.MemberSubtask
                                          .Count(ms => ms.IdSubtask == subtaskId);

            if (memberSubtaskCount == 0)
            {
                MessageBox.Show("Необходимо выбрать исполнителей!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            } else
            {
                Manager.MainFrame.Navigate(new Menu(idd));
            }
        }
    }
}
