using System;
using System.Windows.Forms;

namespace OOP_Cursovaya
{
    /// <summary>
    /// Форма приветствия, которая появляется при запуске приложения.
    /// Предоставляет информацию о программе и автоматически закрывается через 10 секунд.
    /// </summary>
    public partial class WelcomeForm : Form
    {
        /// <summary>
        /// Инициализирует новый экземпляр формы приветствия.
        /// </summary>
        public WelcomeForm()
        {
            InitializeComponent();
            InitializeWelcomeForm();
        }

        /// <summary>
        /// Настраивает внешний вид и поведение формы приветствия.
        /// Добавляет метку с описанием, кнопку "Далее" и таймер для автоматического закрытия.
        /// </summary>
        private void InitializeWelcomeForm()
        {
            // Настройка формы
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.ControlBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Width = 400;
            this.Height = 200;
            this.Text = "Степанов Р.Д. 23ВП2, курсовой проект ООП";

            // Создание метки с описанием
            var label = new Label
            {
                Text = "Добро пожаловать. Это приложение для работы с базой данных 'Мебель'.\n\nЧтобы продолжить нажмите 'Далее' или подождите 10 секунд",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Создание кнопки "Далее"
            var button = new Button
            {
                Text = "Далее",
                Width = 100,
                DialogResult = DialogResult.OK
            };

            // Панель для размещения кнопки внизу формы
            var panel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50
            };
            panel.Controls.Add(button);
            button.Left = (panel.Width - button.Width) / 2;
            button.Top = (panel.Height - button.Height) / 2;

            // Добавление элементов на форму
            this.Controls.Add(label);
            this.Controls.Add(panel);

            // Таймер для автоматического закрытия через 10 секунд
            var timer = new System.Windows.Forms.Timer { Interval = 10000 };
            timer.Tick += (sender, e) =>
            {
                timer.Stop();
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            timer.Start();

            // Обработчик нажатия на кнопку "Далее"
            button.Click += (sender, e) => this.Close();
        }
    }
}