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
using System.Xml.Linq;

namespace Task_Manager
{
    /// <summary>
    /// Логика взаимодействия для AddTeam.xaml
    /// </summary>
    public partial class AddTeam : Page
    {
        TaskManagerEntities db = new TaskManagerEntities();
        function fn = new function();
        int idmain;
        int tumpler;
        public class filldg
        {
            public int user_id { get; set; }
            public int member_id { get; set; }
            public string surname { get; set; }
            public string name { get; set; }
        }

        public AddTeam(int iduser, int flag)
        {
            InitializeComponent();
            idmain = iduser;

            tumpler = flag;
            if (flag > 0) 
            {
                var team = db.Teams.FirstOrDefault(t => t.Id == flag);
                if (team != null)
                {
                    txtNameTeam.Text = team.Title;
                    txtSpecification.Text = team.Specification;

                    // Получение участников команды
                    var members = db.Members
                        .Where(m => m.IdTeam == flag)
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
                        filldg filldg = new filldg();
                        filldg.user_id = member.UserId;
                        filldg.member_id = member.MemberId;
                        filldg.surname = member.Surname;
                        filldg.name = member.Name;

                        dgUsers.Items.Add(filldg);
                    }
                }
            }

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (tumpler <= 0)
            {
                if ((txtNameTeam.Text.Length > 0) && (txtSpecification.Text.Length > 0))
                {
                    Teams t = new Teams();
                    t.Title = txtNameTeam.Text;
                    t.Specification = txtSpecification.Text;
                    TaskManagerEntities.GetContext().Teams.Add(t);
                    TaskManagerEntities.GetContext().SaveChanges();

                    var team = db.Teams.FirstOrDefault(d => d.Title == txtNameTeam.Text);

                    int id = team.Id;

                    Members m = new Members();
                    m.IdRole = 1;
                    m.IdTeam = id;
                    m.IdUser = idmain;
                    TaskManagerEntities.GetContext().Members.Add(m);
                    TaskManagerEntities.GetContext().SaveChanges();

                    Manager.MainFrame.Navigate(new EditUsersInTeam(id, 0, 0, id, 0));

                    db.AddUserHistoryRecord(idmain, 4, DateTime.Now);
                }
            }
            else
            {
                if ((txtNameTeam.Text.Length > 0) && (txtSpecification.Text.Length > 0))
                {
                    var team = db.Teams.FirstOrDefault(t => t.Title == txtNameTeam.Text);

                    int id = team.Id;

                    Manager.MainFrame.Navigate(new EditUsersInTeam(id, 0, 0, tumpler, 0));
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new Menu(idmain));
        }
    }
}
