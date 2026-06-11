using Microsoft.EntityFrameworkCore;
using PasswordManager.Data;
using PasswordManager.Models;
using PasswordManager.Services;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PasswordManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadPasswordsAsync();
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            // Ne engedjünk üres adatokat menteni
            if (string.IsNullOrWhiteSpace(ServiceTextBox.Text) || string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                MessageBox.Show("Kérem töltse ki a Szolgáltatás és a Jelszó mezőket is!", "Figyelmeztetés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var db = new AppDbContext();

                var entry = new PasswordEntry
                {
                    Value = ServiceTextBox.Text,
                    Password = EncryptionService.Encrypt(PasswordBox.Password)
                };

                db.Passwords.Add(entry);

                // Aszinkron mentés
                await db.SaveChangesAsync();

                await LoadPasswordsAsync();

                ServiceTextBox.Clear();
                PasswordBox.Clear();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Hiba történt a mentés során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async System.Threading.Tasks.Task LoadPasswordsAsync()
        {
            using var db = new AppDbContext();
            // Aszinkron betöltés
            PasswordsList.ItemsSource = await db.Passwords.ToListAsync();
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordsList.SelectedItem is not PasswordEntry entry)
            {
                MessageBox.Show("Először válasszon ki egy sort.", "Password Manager - Hiba", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Biztosan törölni szeretné ezt a bejegyzést?", "Password Manager - Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                using var db = new AppDbContext();

                var item = await db.Passwords.FirstOrDefaultAsync(x => x.Id == entry.Id);

                if (item == null) return;

                db.Passwords.Remove(item);
                await db.SaveChangesAsync();

                await LoadPasswordsAsync();

                MessageBox.Show("Sikeresen törölve.", "Password Manager - Siker", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Hiba történt a törlés során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ToggleVisibility_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Parent is Grid grid && btn.Tag is PasswordEntry entry)
            {
                var textBlock = grid.Children.OfType<TextBlock>().FirstOrDefault(x => x.Name == "VisiblePasswordText");

                if (textBlock != null)
                {
                    if (textBlock.Visibility == Visibility.Collapsed)
                    {
                        try
                        {
                            textBlock.Text = EncryptionService.Decrypt(entry.Password);
                            textBlock.Visibility = Visibility.Visible;
                            btn.Content = "🙈 Elrejt"; // Gomb szövegének cseréje
                            btn.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(243, 61, 33)); // Pirosas szín
                        }
                        catch
                        {
                            MessageBox.Show("Hiba a jelszó visszafejtésekor!", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    // Ha már látható, akkor újra elrejtjük
                    else
                    {
                        textBlock.Visibility = Visibility.Collapsed;
                        textBlock.Text = string.Empty;
                        btn.Content = "👁️ Mutat";
                        btn.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(52, 129, 190)); // Vissza kékre
                    }
                }
            }
        }

        private void CopyToClipboard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is PasswordEntry entry)
            {
                try
                {
                    string decrypted = EncryptionService.Decrypt(entry.Password);
                    Clipboard.SetText(decrypted);

                    MessageBox.Show($"A(z) '{entry.Value}' jelszava a vágólapra került!", "Sikeres másolás", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch
                {
                    MessageBox.Show("Nem sikerült visszafejteni a jelszót.", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) 
            {
                DragMove();
            }    
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}