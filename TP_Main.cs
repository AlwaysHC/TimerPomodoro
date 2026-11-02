using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TimerPomodoro.Properties;

namespace TimerPomodoro {
    public partial class TP_Main: Form {
        private static readonly TimeSpan SessionDuration = TimeSpan.FromMinutes(25);
        private readonly Stopwatch _sessionClock = new();
        private readonly Stopwatch _breakClock = new();
        private readonly System.Windows.Forms.Timer _flashTimer;
        private PomodoroState _state;
        private bool _needsSave;
        private TimeSpan _breakElapsed;
        private DateTime _sessionStartedAt;
        private int _flashStep;

        public TP_Main() {
            (_state, _needsSave) = PomodoroStore.Load();
            InitializeComponent();
            Icon = Resources.Pomodoro;
            _flashTimer = new(components) { Interval = 500 };
            _flashTimer.Tick += FlashTimer_Tick;
            AdvanceDate();
            ResetSession();
        }

        protected override void OnShown(EventArgs e) {
            base.OnShown(e);
            using Graphics graphics = lblTimer.CreateGraphics();
            SizeF size = graphics.MeasureString(" 25:00.0 ", lblTimer.Font);
            Font timerFont = new(lblTimer.Font.FontFamily, lblTimer.Font.Size * lblTimer.Width / size.Width, lblTimer.Font.Style);
            lblTimer.Font = timerFont;
            lblTimer.Disposed += (_, _) => timerFont.Dispose();
            if (_needsSave) {
                Save();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e) {
            base.OnFormClosing(e);
            if (!e.Cancel && _needsSave && !Save()) {
                e.Cancel = true;
            }
        }

        private bool AdvanceDate() {
            PomodoroState updated = _state.AdvanceTo(DateOnly.FromDateTime(DateTime.Today));
            if (updated == _state) {
                return false;
            }

            _state = updated;
            _needsSave = true;
            return true;
        }

        private void ShowStatus(string time) =>
            lblAvvio.Text = $"{time} - {_state.Today} ({_state.Yesterday}) {_state.Total}";

        private void ResetSession() {
            StopFlash();
            TimerEnd.Stop();
            _sessionClock.Reset();
            _breakElapsed = TimeSpan.Zero;
            _breakClock.Restart();
            TimerStop.Start();
            lblTimer.Text = SessionDuration.ToString(@"mm\:ss\.f");
            ShowStatus(_breakElapsed.ToString(@"hh\:mm\:ss"));
        }

        private void btnStop_Click(object sender, EventArgs e) {
            AdvanceDate();
            ResetSession();
            if (_needsSave) {
                Save();
            }
        }

        private void btnStart_Click(object sender, EventArgs e) {
            AdvanceDate();
            StopFlash();
            _sessionStartedAt = DateTime.Now;
            _sessionClock.Restart();
            _breakClock.Reset();
            TimerStop.Stop();
            TimerEnd.Start();
            lblTimer.Text = SessionDuration.ToString(@"mm\:ss\.f");
            ShowStatus(_sessionStartedAt.ToString("HH:mm:ss"));
            if (_needsSave) {
                Save();
            }
        }

        private void TimerEnd_Tick(object sender, EventArgs e) {
            if (!TimerEnd.Enabled) {
                return;
            }

            bool dateChanged = AdvanceDate();
            TimeSpan remaining = SessionDuration - _sessionClock.Elapsed;
            if (remaining > TimeSpan.Zero) {
                lblTimer.Text = remaining.ToString(@"mm\:ss\.f");
                ShowStatus(_sessionStartedAt.ToString("HH:mm:ss"));
                if (dateChanged) {
                    Save();
                }
                return;
            }

            _state = _state with { Total = _state.Total + 1, Today = _state.Today + 1 };
            _needsSave = true;
            ResetSession();
            Rectangle area = Screen.GetWorkingArea(this);
            Location = new(area.Left + (area.Width - Width) / 2, area.Top + (area.Height - Height) / 2);
            Save();
            BackColor = Color.Yellow;
            _flashTimer.Start();
        }

        private void StopFlash() {
            _flashTimer.Stop();
            _flashStep = 0;
            BackColor = SystemColors.Control;
        }

        private void FlashTimer_Tick(object sender, EventArgs e) {
            _flashStep++;
            if (_flashStep >= 3) {
                StopFlash();
                return;
            }
            BackColor = _flashStep == 1 ? Color.Blue : Color.Red;
        }

        private void lblTimer_MouseMove(object sender, MouseEventArgs e) => MoveWindow();

        private void MoveWindow() {
            Point Pos = PointToClient(MousePosition);
            int T = Top;
            int L = Left;
            int X4 = lblTimer.Width / 4;
            int Y3 = lblTimer.Height / 3;
            if (Pos.X < X4) {
                if (Pos.Y < Y3) {
                    T += 5;
                    L += 5;
                }
                else {
                    if (Pos.Y < Y3 * 2) {
                        L += 5;
                    }
                    else {
                        T -= 5;
                        L += 5;
                    }
                }
            }
            else {
                if (Pos.X < X4 * 3) {
                    if (Pos.Y < Y3) {
                        T += 5;
                    }
                    else {
                        if (Pos.Y > Y3 * 2) {
                            T -= 5;
                        }
                    }
                }
                else {
                    if (Pos.Y < Y3) {
                        T += 5;
                        L -= 5;
                    }
                    else {
                        if (Pos.Y < Y3 * 2) {
                            L -= 5;
                        }
                        else {
                            T -= 5;
                            L -= 5;
                        }
                    }
                }
            }
            Rectangle R = Screen.GetWorkingArea(lblTimer);
            if (T <= (R.Top + 5)) {
                T = R.Top + 5;
            }
            if (T + Height >= (R.Bottom - 5)) {
                T = (R.Bottom - Height) - 5;
            }
            if (L <= (R.Left + 5)) {
                L = R.Left + 5;
            }
            if (L + Width >= (R.Right - 5)) {
                L = (R.Right - Width) - 5;
            }
            Top = T;
            Left = L;
        }

        private void TimerStop_Tick(object sender, EventArgs e) {
            if (!TimerStop.Enabled) {
                return;
            }

            TimeSpan elapsed = _breakClock.Elapsed;
            _breakClock.Restart();
            if (elapsed.TotalMilliseconds < TimerStop.Interval * 5) {
                _breakElapsed += elapsed;
            }
            bool dateChanged = AdvanceDate();
            ShowStatus(_breakElapsed.ToString(@"hh\:mm\:ss"));
            if (dateChanged) {
                Save();
            }
        }

        private void lblStart_MouseMove(object sender, MouseEventArgs e) => MoveWindow();

        private bool Save() {
            try {
                PomodoroStore.Save(_state);
                _needsSave = false;
                return true;
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException) {
                bool sessionRunning = TimerEnd.Enabled;
                bool breakRunning = TimerStop.Enabled;
                TimerEnd.Stop();
                TimerStop.Stop();
                MessageBox.Show(this, $"Could not save your counters to '{PomodoroStore.FilePath}'.\n\n{exception.Message}",
                    "Timer Pomodoro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                TimerEnd.Enabled = sessionRunning;
                TimerStop.Enabled = breakRunning;
                return false;
            }
        }
    }
}
