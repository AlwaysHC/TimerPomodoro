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
                using TP_Main window = new();
                Application.Run(window);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or FormatException) {
                MessageBox.Show($"Could not load your counters.\n\n{exception.Message}", "Timer Pomodoro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
