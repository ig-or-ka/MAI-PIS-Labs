using System;
using System.ComponentModel;
using System.Windows;

namespace Laba1
{
    /// <summary>
    /// Логика взаимодействия для EditWindow.xaml
    /// </summary>
    public enum Posts {
        None = 0,
        Secretary = 1,
        AssociateDirector = 2,
        Director = 3 
    };
    public partial class EditWindow : Window
    {
        DB? connection;
        User? added_user = null;

        public EditWindow(DB connection, User? authed_user)
        {
            this.connection = connection;

            InitializeComponent();
            Closing += OnClose;

            usersGrid.ItemsSource = connection.Users;

            if(authed_user?.Post != Posts.Director)
            {
                usersGrid.ColumnFromDisplayIndex(3).Visibility = Visibility.Hidden;
                usersGrid.ColumnFromDisplayIndex(0).Visibility = Visibility.Hidden;
            }

            if(authed_user == null)
            {
                usersGrid.ColumnFromDisplayIndex(1).Visibility = Visibility.Hidden;
                usersGrid.ColumnFromDisplayIndex(5).Visibility = Visibility.Hidden;
                usersGrid.ColumnFromDisplayIndex(6).Visibility = Visibility.Hidden;
            }

            if(authed_user == null || authed_user.Post == Posts.Secretary || authed_user.Post == Posts.AssociateDirector)
            {
                AddItemButton.Visibility = Visibility.Hidden;
                RemoveButton.Visibility = Visibility.Hidden;
            }

            if (authed_user == null || authed_user.Post == Posts.Secretary)
            {
                SaveButton.Visibility = Visibility.Hidden;
            }
        }

        public void OnClose(object? sender, CancelEventArgs e)
        {
            Console.WriteLine("Closing wind");
            connection?.Close();
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            if (added_user != null)
            {
                MessageBox.Show("Сначала необходимо сохранить нового пользователя!");
            }
            else
            {
                added_user = connection!.NewUser();
                usersGrid.ItemsSource = null;
                usersGrid.ItemsSource = connection.Users;
            }            
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if(added_user != null)
            {
                if(added_user.Name == "")
                {
                    MessageBox.Show("Укажите имя!");
                    return;
                }

                if (added_user.Password == "")
                {
                    MessageBox.Show("Укажите пароль!");
                    return;
                }

                if (added_user.Post == Posts.None)
                {
                    MessageBox.Show("Укажите должность!");
                    return;
                }

                if (added_user.Address == "")
                {
                    MessageBox.Show("Укажите адрес!");
                    return;
                }

                if (added_user.Phone == "")
                {
                    MessageBox.Show("Укажите телефон!");
                    return;
                }

                added_user.Save();
                added_user = null;

                MessageBox.Show("Пользователь добавлен!");
            }

            int count_edited = 0;
            foreach(var usr in connection!.Users){
                if (usr.Save())
                {
                    count_edited++;
                }
            }

            usersGrid.ItemsSource = null;
            usersGrid.ItemsSource = connection.Users;

            if(count_edited > 0)
            {
                MessageBox.Show($"Изменено пользователей: {count_edited}");
            }            
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            int count_removed = 0;
            foreach (var usr in connection!.Users)
            {
                if (usr.Selected)
                {
                    if(usr == added_user)
                    {
                        added_user = null;
                    }
                    connection.Remove(usr);
                    count_removed++;
                }                
            }

            usersGrid.ItemsSource = null;
            usersGrid.ItemsSource = connection.Users;

            MessageBox.Show($"Удалено пользоватетелей: {count_removed}");
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            added_user = null;
            usersGrid.ItemsSource = null;
            connection!.Update();
            usersGrid.ItemsSource = connection.Users;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();            
        }
    }
}
