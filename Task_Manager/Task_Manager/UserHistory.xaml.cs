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

namespace Task_Manager
{
    /// <summary>
    /// Логика взаимодействия для UserHistory.xaml
    /// </summary>
    public partial class UserHistory : Page
    {
        TaskManagerEntities db = new TaskManagerEntities();
        int id;
        public UserHistory(int accountid)
        {
            InitializeComponent();

            id = accountid;

            var statusTitles = db.Action
                .Select(s => s.Title)
                .ToList();

            comboSearch.Items.Add("Все");

            foreach (var title in statusTitles)
            {
                comboSearch.Items.Add(title);
            }

            dgAction.ItemsSource = db.GetUserHistory(accountid).ToList();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new Menu(id));
        }

        private void comboSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var query = from UserHistory in TaskManagerEntities.GetContext().UserHistory
                        join Action in TaskManagerEntities.GetContext().Action
                        on UserHistory.IdAction equals Action.Id
                        select new { Action.Title, UserHistory.DateAction };

            if (comboSearch.SelectedValue.ToString() == "Все")
            {
                dgAction.ItemsSource = db.GetUserHistory(id).ToList();
            }
            else
            {
                dgAction.ItemsSource = query.Where(item => item.Title == comboSearch.SelectedValue.ToString() || item.Title.Contains(comboSearch.SelectedValue.ToString())).ToList();
            }
        }
    }
}
