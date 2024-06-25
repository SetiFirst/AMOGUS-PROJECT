using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            FirstNameTextBox.Text = string.Empty;
            LastNameTextBox.Text = string.Empty;
            BirthDayPicker.Text = string.Empty;
            LoginTextBox.Text = string.Empty;
            PasswordBox.Password = string.Empty;
            ConfirmPasswordBox.Password = string.Empty;
            PhoneTextBox.Text = string.Empty;
        }

        private bool checkPass(string password, string login)
        {
            // Проверка на схожесть логина и пароля
            if(login == password)
            {
                MessageBox.Show("Логин и пароль должны отличаться!");
                return false;
            }

            // Проверка длины пароля
            if (password.Length < 8)
            {
                MessageBox.Show("Пароль должен содержать не менее 8 символов.");
                return false;
            }

            // Проверка на комбинацию символов
            if (!Regex.IsMatch(password, @"[A-Z]") || !Regex.IsMatch(password, @"[a-z]") ||
                !Regex.IsMatch(password, @"\d") || !Regex.IsMatch(password, @"[!@#$%^&*()]"))
            {
                MessageBox.Show("Пароль должен содержать комбинацию букв верхнего и нижнего регистра, цифр и специальных символов.");
                return false;
            }

            // Проверка на слабые пароли
            string[] weakPasswords = { "password", "123456", "qwerty", "admin", "user", "letmein" };
            foreach (string weakPassword in weakPasswords)
            {
                if (password.Contains(weakPassword))
                {
                    MessageBox.Show("Пароль не должен содержать слабых паролей.");
                    return false;
                }
            }

            // Проверка на повторение символов
            if (Regex.IsMatch(password, @"(\w)\1\1"))
            {
                MessageBox.Show("Пароль не должен содержать повторяющиеся символы или последовательности символов.");
                return false;
            }

            // Проверка на словарь
            string[] dictionaryWords = { "apple", "banana", "orange", "computer", "internet", "password" };
            foreach (string dictionaryWord in dictionaryWords)
            {
                if (password.Contains(dictionaryWord))
                {
                    MessageBox.Show("Пароль не должен содержать общих слов или фраз.");
                    return false;
                }
            }
            return true;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string firstName = FirstNameTextBox.Text.ToString();
            string lastName = LastNameTextBox.Text.ToString();
            string birthday = BirthDayPicker.Text.ToString();
            string login = LoginTextBox.Text.ToString();
            string password = PasswordBox.Password.ToString();
            string confirm = ConfirmPasswordBox.Password.ToString();
            string phone = PhoneTextBox.Text.ToString();
            string pattern = @"^^\+7[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]|8[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]$";

            if (firstName == string.Empty || lastName == string.Empty || birthday == string.Empty || login == string.Empty || password == string.Empty || confirm == string.Empty || phone == string.Empty)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }
            if (password != confirm)
            {
                MessageBox.Show("Пароли не совадают!");
                return;
            }
            if (!Regex.IsMatch(phone, pattern))
            {

                MessageBox.Show($"Вы ввели не действительный номер телефона! Пример правильного ввода: +79991112233 или 89991112233");
                return;
            }

            if (!checkPass(password, login))
            {
                return;
            }

            DBConnect.Register(firstName, lastName, birthday, login, password, phone, this);
        }
    }
}
