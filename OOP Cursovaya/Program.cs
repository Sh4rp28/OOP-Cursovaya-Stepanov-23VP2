using System;
using System.Windows.Forms;

namespace OOP_Cursovaya
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Показываем приветственную форму
            using (var welcomeForm = new WelcomeForm())
            {
                if (welcomeForm.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new ClientForm());
                }
            
            }
        }
    }
}