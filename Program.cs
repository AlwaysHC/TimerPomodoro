using System;
using System.IO;
using System.Windows.Forms;

namespace TimerPomodoro {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            ApplicationConfiguration.Initialize();
            try {
                using TP_Main Window = new();
                Application.Run(Window);
            }
            catch (Exception Ex) when (Ex is IOException or UnauthorizedAccessException or FormatException) {
                MessageBox.Show($"Could not load your counters.\n\n{Ex.Message}", "Timer Pomodoro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
