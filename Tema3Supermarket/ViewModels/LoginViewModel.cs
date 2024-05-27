using System.Linq;
using System.Windows;
using System.Windows.Input;
using Tema3Supermarket.Comands;
using Tema3Supermarket.Models;
using Tema3Supermarket.Views;

namespace Tema3Supermarket.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string username;
        public string Username
        {
            get { return username; }
            set { username = value; OnPropertyChanged("Username"); }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; OnPropertyChanged("Password"); }
        }

        public ICommand LoginCommand { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(param => Login());
        }

        private void Login()
        {
            using (var context = new SupermarketDbContext())
            {
                var user = context.Utilizatori
                    .FirstOrDefault(u => u.Nume == Username && u.Parola == Password);

                if (user != null)
                {
                    MessageBox.Show($"Login reușit pentru utilizatorul {user.Nume} cu rolul {user.TipUtilizator}.");

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (user.TipUtilizator == "Administrator")
                        {
                            var adminView = new AdminView();
                            adminView.Show();
                        }
                        else if (user.TipUtilizator == "Casier")
                        {
                            var casierView = new CasierView();
                            casierView.Show();
                        }

                        // Închide fereastra de login
                        foreach (Window window in Application.Current.Windows)
                        {
                            if (window is LoginView)
                            {
                                window.Close();
                                break;
                            }
                        }
                    });
                }
                else
                {
                    MessageBox.Show("Username sau parola incorectă.");
                }
            }
        }
    }
}
