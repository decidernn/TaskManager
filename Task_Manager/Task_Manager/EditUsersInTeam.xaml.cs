using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Contexts;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using static Task_Manager.Menu;

namespace Task_Manager
{
    /// <summary>
    /// Логика взаимодействия для EditUsersInTeam.xaml
    /// </summary>
    public partial class EditUsersInTeam : Page
    {
        TaskManagerEntities db = new TaskManagerEntities();
        function fn = new function();
        int idteam;
        List<Members> members = new List<Members>();
        int tumberr;
        int tumblerprojectt;
        int tumblerteam;
        int tumblersub;
        int idmem;
        int tumberr2;
        int idt;
        int newidsubtask;
        List<TumberUsers> selectedUsersList = new List<TumberUsers>();
        List<TumberUsers> allUsersList = new List<TumberUsers>();

        public class TumberUsers
        {
            public int user_id { get; set; }
            public string user_name { get; set; }
            public string user_surname { get; set; }
        }
        public EditUsersInTeam(int id, int tumblersubtask, int tumblerproject, int tumblerteams, int idcreatenewsubtask)
        {
            InitializeComponent();

            tumberr2 = tumblersubtask;
            tumblerprojectt = tumblerproject;
            tumblerteam = tumblerteams;
            tumblersub = tumblersubtask;
            tumberr = idcreatenewsubtask;
            newidsubtask = idcreatenewsubtask;

            if (tumblersubtask > 0) //Заполняем dg из editsubtask
            {
                dgAllUsers2.Visibility = Visibility.Hidden;
                dgSelectedUsers2.Visibility = Visibility.Hidden;

                var members = db.User
                .Join(db.Members, u => u.Id, m => m.IdUser, (u, m) => new { User = u, Member = m })
                .Join(db.MemberSubtask, u_m => u_m.Member.Id, ms => ms.IdMember, (u_m, ms) => new { UserMember = u_m, MemberSubtask = ms })
                .Join(db.Subtask, u_ms => u_ms.MemberSubtask.IdSubtask, s => s.Id, (u_ms, s) => new { UserMemberSubtask = u_ms, Subtask = s })
                .Where(u_ms_s => u_ms_s.Subtask.Id == tumblersubtask)
                .OrderBy(u_ms_s => u_ms_s.UserMemberSubtask.UserMember.User.Id)
                .Select(u_ms_s => new
                {
                    UserId = u_ms_s.UserMemberSubtask.UserMember.User.Id,
                    MemberId = u_ms_s.UserMemberSubtask.UserMember.Member.Id,
                    user_surname = u_ms_s.UserMemberSubtask.UserMember.User.Surname,
                    user_name = u_ms_s.UserMemberSubtask.UserMember.User.Name
                })
                .ToList();

                foreach (var member in members)
                {
                    TumberUsers tumberUsers = new TumberUsers();
                    {
                        tumberUsers.user_name = member.user_name;
                        tumberUsers.user_surname = member.user_surname;
                    };
                    dgSelectedUsers.Items.Add(tumberUsers);
                }

                var allUsersQuery = from user in db.User
                            from member in user.Members
                            where db.Task.Any(task => task.IdTeam == member.IdTeam &&
                                                             task.Subtask.Any(subtask => subtask.Id == tumblersubtask))
                            select new
                            {
                                user.Id,
                                user.Name,
                                user.Surname
                            };

                foreach (var userData in allUsersQuery)
                {
                    TumberUsers tumberUsers = new TumberUsers();
                    tumberUsers.user_id = userData.Id;
                    tumberUsers.user_surname = userData.Surname;
                    tumberUsers.user_name = userData.Name;

                    allUsersList.Add(tumberUsers);
                }

                dgAllUsers.ItemsSource = allUsersList;
                
            }
            else if (tumberr > 0)
            {
                var allUsersQuery = from user in db.User
                where db.Members.Any(member => member.IdUser == user.Id && member.IdTeam == db.Task.Where(task => task.Id == db.Subtask.Where(subtask => subtask.Id == tumberr).Select(subtask => subtask.IdTask).FirstOrDefault()).Select(task => task.IdTeam).FirstOrDefault())
                select new
                {
                    user.Id,
                    user.Surname,
                    user.Name
                };

                foreach (var userData in allUsersQuery)
                {
                    TumberUsers tumberUsers = new TumberUsers();
                    tumberUsers.user_id = userData.Id;
                    tumberUsers.user_surname = userData.Surname;
                    tumberUsers.user_name = userData.Name;

                    dgAllUsers2.Items.Add(tumberUsers);
                }

                var selectedUsersQuery = from user in db.User
                                         join member in db.Members on user.Id equals member.IdUser
                                         join memberSubtask in db.MemberSubtask on member.Id equals memberSubtask.IdMember
                                         where memberSubtask.IdSubtask == tumberr && db.Task.Any(task => task.Id == db.Subtask.Where(subtask => subtask.Id == tumblersubtask).Select(subtask => subtask.IdTask).FirstOrDefault() && member.IdTeam == task.IdTeam)
                orderby user.Id
                select new
                {
                    user.Id,
                    user.Surname,
                    user.Name
                };

                foreach (var userData in selectedUsersQuery)
                {
                    TumberUsers tumberUsers = new TumberUsers();
                    tumberUsers.user_id = userData.Id;
                    tumberUsers.user_surname = userData.Surname;
                    tumberUsers.user_name = userData.Name;

                    dgSelectedUsers2.Items.Add(tumberUsers);
                }
            } else
            {
                dgAllUsers2.Visibility = Visibility.Hidden;
                dgSelectedUsers2.Visibility = Visibility.Hidden;

                dgAllUsers.ItemsSource = TaskManagerEntities.GetContext().User.ToList();

                idteam = id;
            }

            if (tumblerproject > 0) //Заполнение из editproject
            {
                dgAllUsers2.Visibility = Visibility.Hidden;
                dgSelectedUsers2.Visibility = Visibility.Hidden;

                var selectedUsersQuery = (from user in db.User
                                         join member in db.Members on user.Id equals member.IdUser
                                         join team in db.Teams on member.IdTeam equals team.Id
                                         join task in db.Task on team.Id equals task.IdTeam
                                         where task.Id == tumblerproject
                                         select new
                                         {
                                             user.Id,
                                             user.Surname,
                                             user.Name
                                         }).Distinct();

                var allUsersQuery = from user in db.User
                                    select new
                                    {
                                        user.Id,
                                        user.Surname,
                                        user.Name
                                    };

                foreach (var userData in selectedUsersQuery)
                {
                    TumberUsers tumberUsers = new TumberUsers();
                    tumberUsers.user_id = userData.Id;
                    tumberUsers.user_surname = userData.Surname;
                    tumberUsers.user_name = userData.Name;

                    dgSelectedUsers.Items.Add(tumberUsers);
                }

                foreach (var userData in allUsersQuery)
                {
                    TumberUsers tumberUsers = new TumberUsers();
                    tumberUsers.user_id = userData.Id;
                    tumberUsers.user_surname = userData.Surname;
                    tumberUsers.user_name = userData.Name;

                    allUsersList.Add(tumberUsers);
                }

                dgAllUsers.ItemsSource = allUsersList;
            }

            if (tumblerteam > 0) //Заполняем из editteams
            {
                dgSelectedUsers2.Visibility = Visibility.Hidden;
                dgAllUsers2.Visibility = Visibility.Hidden;

                var members = db.Members
                    .Where(m => m.IdTeam == tumblerteam)
                    .Select(m => new
                    {
                        UserId = m.User.Id,
                        MemberId = m.Id,
                        Surname = m.User.Surname,
                        Name = m.User.Name
                    })
                    .OrderBy(m => m.MemberId)
                    .ToList();

                foreach (var member in members)
                {
                    TumberUsers tumberUsers = new TumberUsers();
                    tumberUsers.user_id = member.UserId;
                    tumberUsers.user_surname = member.Surname;
                    tumberUsers.user_name = member.Name;

                    dgSelectedUsers.Items.Add(tumberUsers);
                }

                var allUsersQuery = from user in db.User
                                    select new
                                    {
                                        user.Id,
                                        user.Surname,
                                        user.Name
                                    };

                foreach (var userData in allUsersQuery)
                {
                    TumberUsers tumberUsers = new TumberUsers();
                    tumberUsers.user_id = userData.Id;
                    tumberUsers.user_surname = userData.Surname;
                    tumberUsers.user_name = userData.Name;

                    allUsersList.Add(tumberUsers);
                }
                dgAllUsers.ItemsSource = allUsersList;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tumberr2 > 0) // Добавление из editproject
                {
                    TumberUsers user = (TumberUsers)dgAllUsers.Items[dgAllUsers.SelectedIndex];

                    int idmember = db.Members
                      .Where(m => m.IdUser == user.user_id)
                      .Select(m => m.Id)
                      .FirstOrDefault();

                    if (idmember != 0)
                    {
                        MemberSubtask m = new MemberSubtask
                        {
                            IdMember = idmember,
                            IdSubtask = tumberr2
                        };

                        db.MemberSubtask.Add(m);
                        db.SaveChanges();

                        dgSelectedUsers.Items.Add(dgAllUsers.SelectedItem);

                        MessageBox.Show("Пользователь добавлен!");
                    }
                    else
                    {
                        MessageBox.Show("Пользователь не найден в базе данных!");
                    }
                }
                if (tumberr > 0) // Создание задачи
                {
                    if (dgAllUsers2.SelectedItem != null)
                    {
                        TumberUsers user = (TumberUsers)dgAllUsers2.Items[dgAllUsers2.SelectedIndex];

                        int idmember = db.Members
                          .Where(m => m.IdUser == user.user_id)
                          .Select(m => m.Id)
                          .FirstOrDefault();

                        if (idmember != 0)
                        {
                            MemberSubtask m = new MemberSubtask
                            {
                                IdMember = idmember,
                                IdSubtask = tumberr // Предполагается, что tumberr у вас уже определен
                            };

                            db.MemberSubtask.Add(m);
                            db.SaveChanges();

                            dgSelectedUsers2.Items.Add(dgAllUsers2.SelectedItem);

                            MessageBox.Show("Пользователь добавлен!");
                        }
                        else
                        {
                            MessageBox.Show("Пользователь не найден в базе данных!");
                        }
                    }
                }
                else
                {
                    if (dgAllUsers2.SelectedItem != null)
                    {
                        MemberSubtask m = new MemberSubtask();

                        dgSelectedUsers2.Items.Add(dgAllUsers2.SelectedItem);

                        User user = (User)dgAllUsers2.Items[dgAllUsers2.SelectedIndex];

                        m.IdSubtask = newidsubtask;
                        m.IdMember = user.Id;

                        using (var context = new TaskManagerEntities())
                        {
                            context.MemberSubtask.Add(m);
                            context.SaveChanges();
                        }

                        MessageBox.Show("Пользователь добавлен в команду!");
                    }
                }

                if (tumblerprojectt > 0) //Добавление из editproject
                {

                    Members m = new Members();

                    TumberUsers user = (TumberUsers)dgAllUsers.Items[dgAllUsers.SelectedIndex];

                    var team = db.Task.Where(u => u.Id == tumblerprojectt).Select(u => u.IdTeam).FirstOrDefault();

                    string check = Convert.ToString(db.CheckIdInMembersForTeam(user.user_id, tumblerprojectt).FirstOrDefault());

                    if (check == "False")
                    {
                        dgSelectedUsers.Items.Add(dgAllUsers.SelectedItem);

                        string idmember = "SELECT Id FROM Members WHERE IdUser = " + user.user_id;

                        DataSet ds_idmember = fn.getData(idmember);

                        string idteam = "SELECT IdTeam FROM Task WHERE Id = " + tumblerprojectt;

                        DataSet ds_idteam = fn.getData(idteam);

                        m.IdRole = 2;
                        m.IdTeam = Convert.ToInt32(ds_idteam.Tables[0].Rows[0][0].ToString());
                        m.IdUser = user.user_id;

                        TaskManagerEntities.GetContext().Members.Add(m);

                        TaskManagerEntities.GetContext().SaveChanges();

                        MessageBox.Show("Пользователь добавлен!");
                    } else
                    {
                        MessageBox.Show("Пользователь уже есть в этом прооекте!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);

                        return;
                    }

                }

                if (tumblerteam > 0) // Добавление пользователя в команду (Teams)
                {
                    try
                    {
                        Members m = new Members();

                        TumberUsers user = (TumberUsers)dgAllUsers.Items[dgAllUsers.SelectedIndex];

                        var title = db.Teams.Where(u => u.Id == tumblerteam).Select(u => u.Title).FirstOrDefault();

                        string check = Convert.ToString(db.CheckIdInMembers(user.user_id, title).FirstOrDefault());

                        if (check == "False")
                        {
                            dgSelectedUsers.Items.Add(dgAllUsers.SelectedItem);

                            string idmember = "SELECT Id FROM Members WHERE IdUser = " + user.user_id;

                            DataSet ds_idmember = fn.getData(idmember);

                            m.IdRole = 2;
                            m.IdTeam = tumblerteam;
                            m.IdUser = user.user_id;

                            TaskManagerEntities.GetContext().Members.Add(m);
                            TaskManagerEntities.GetContext().SaveChanges();

                            MessageBox.Show("Пользователь добавлен!");
                        }
                        else
                        {
                            MessageBox.Show("Пользователь уже есть в этой команде!", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);

                            return;
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            } catch
            {
                MessageBox.Show("Действие не может быть выполнено, повторите попытку!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var removing = dgSelectedUsers.SelectedItems;

            if (MessageBox.Show($"Вы точно хотите удалить {removing.Count} элемент(а)?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    if (tumberr > 0)
                    {
                        TumberUsers m = (TumberUsers)dgSelectedUsers2.Items[dgSelectedUsers2.SelectedIndex];

                        string query_idmember = "SELECT Id FROM Members WHERE IdUser = " + m.user_id;

                        DataSet ds_idmember = fn.getData(query_idmember);

                        int id = Convert.ToInt32(ds_idmember.Tables[0].Rows[0][0].ToString());

                        string query = "DELETE FROM MemberSubtask WHERE IdMember = " + id + "AND IdSubtask = " + tumberr;

                        DataSet ds_delete = fn.getData(query);

                        dgSelectedUsers2.Items.Remove(dgSelectedUsers2.SelectedItem);

                        TaskManagerEntities.GetContext().SaveChanges();

                        MessageBox.Show("Пользователь удален из команды!", "Уведомление!", MessageBoxButton.OK, MessageBoxImage.Information);
                        

                    } else if (tumblerprojectt > 0) // Удаление из составва команды в проекте
                    {
                        
                        TumberUsers m = (TumberUsers)dgSelectedUsers.Items[dgSelectedUsers.SelectedIndex];

                        string query_idmember = "SELECT Id FROM Members WHERE IdUser = " + m.user_id;

                        DataSet ds_idmember = fn.getData(query_idmember);

                        int id = Convert.ToInt32(ds_idmember.Tables[0].Rows[0][0].ToString());

                        string idteam = "SELECT IdTeam FROM Task WHERE Id = " + tumblerprojectt;

                        DataSet ds_idteam = fn.getData(idteam);

                        idt = Convert.ToInt32(ds_idteam.Tables[0].Rows[0][0].ToString());

                        string query2 = "DELETE FROM MemberSubtask WHERE IdMember = " + id + " AND IdSubtask IN (SELECT Id FROM Subtask WHERE IdTask = " + tumblerprojectt + ");";

                        DataSet ds_delete2 = fn.getData(query2);

                        string query = "DELETE FROM Members WHERE IdUser = " + id + "AND IdTeam = " + idt;

                        DataSet ds_delete = fn.getData(query);

                        dgSelectedUsers.Items.Remove(dgSelectedUsers.SelectedItem);

                        MessageBox.Show("Пользователь удален из команды!", "Уведомление!", MessageBoxButton.OK, MessageBoxImage.Information);
                        

                    } else if (tumblerteam > 0) // Удаление пользователя из команды (Teams)
                    {
                        
                        TumberUsers m = (TumberUsers)dgSelectedUsers.Items[dgSelectedUsers.SelectedIndex];

                        string query = "DELETE FROM Members WHERE IdUser = " + m.user_id + "AND IdTeam = " + tumblerteam;

                        DataSet ds_delete = fn.getData(query);

                        dgSelectedUsers.Items.Remove(dgSelectedUsers.SelectedItem);

                        MessageBox.Show("Пользователь удален из команды!", "Уведомление!", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                    }
                    else
                    {
                        TumberUsers m = (TumberUsers)dgSelectedUsers.Items[dgSelectedUsers.SelectedIndex];

                        string query1 = "SELECT Id FROM [User] WHERE [Name] = '" + m.user_name + "' AND [Surname] = '" + m.user_surname + "';";

                        DataSet ds1 = fn.getData(query1);

                        int id1 = Convert.ToInt32(ds1.Tables[0].Rows[0][0].ToString());
                        
                        string queryidteam = "SELECT IdTeam FROM Task WHERE Id IN (SELECT IdTask FROM Subtask WHERE Id = " + tumblersub + ")";

                        DataSet ds2 = fn.getData(queryidteam);

                        int idteam1 = Convert.ToInt32(ds2.Tables[0].Rows[0][0].ToString());

                        string query = "SELECT Id FROM Members WHERE IdUser = " + id1 + " AND IdTeam = " + idteam1 + ";";

                        DataSet ds_delete = fn.getData(query);

                        int id2 = Convert.ToInt32(ds_delete.Tables[0].Rows[0][0].ToString());

                        var memberSubtaskToDelete = db.MemberSubtask.FirstOrDefault(ms => ms.IdMember == id2 && ms.IdSubtask == tumblersub);

                        if (memberSubtaskToDelete != null)
                        {
                            // Удалить найденную запись
                            db.MemberSubtask.Remove(memberSubtaskToDelete);
                            db.SaveChanges();
                            dgSelectedUsers.Items.Remove(dgSelectedUsers.SelectedItem);

                            MessageBox.Show("Запись успешно удалена!", "Уведомление!", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Не удалось найти запись для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.GoBack();
        }

        private void txtSeartch_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Поиск по фамилии
            if ((tumblerteam > 0) || (tumblerprojectt > 0) || (tumblersub > 0))
            {
                string searchText = txtSeartch.Text.ToLower(); // Получение введенного текста для поиска

                var filteredUsers = allUsersList.Where(user => user.user_surname.ToLower().Contains(searchText)).ToList();

                // Отображение результатов в соответствующем датагриде
                dgAllUsers.ItemsSource = filteredUsers;
            }
        }
    }
}
