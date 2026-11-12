using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TimerPomodoro.Properties;

namespace TimerPomodoro {
    public partial class TP_Main: Form {
        private static readonly TimeSpan SESSIONDURATION = TimeSpan.FromMinutes(25);
        private readonly Stopwatch _SessionClock = new();
        private readonly Stopwatch _BreakClock = new();
        private readonly System.Windows.Forms.Timer _FlashTimer;
        private PomodoroState _State;
        private bool _NeedsSave;
        private TimeSpan _BreakElapsed;
        private DateTime _SessionStartedAt;
        private int _FlashStep;

        public TP_Main() {
            (_State, _NeedsSave) = PomodoroStore.Load();
            InitializeComponent();
            Icon = Resources.Pomodoro;
            _FlashTimer = new(components) { Interval = 500 };
            _FlashTimer.Tick += FlashTimer_Tick;
            AdvanceDate();
            ResetSession();
        }

        protected override void OnShown(EventArgs e) {
            base.OnShown(e);
            using Graphics Graphics = lblTimer.CreateGraphics();
            SizeF Size = Graphics.MeasureString(" 25:00.0 ", lblTimer.Font);
            Font TimerFont = new(lblTimer.Font.FontFamily, lblTimer.Font.Size * lblTimer.Width / Size.Width, lblTimer.Font.Style);
            lblTimer.Font = TimerFont;
            lblTimer.Disposed += (_, _) => TimerFont.Dispose();
            if (_NeedsSave) {
                Save();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e) {
            base.OnFormClosing(e);
            if (!e.Cancel && _NeedsSave && !Save()) {
                e.Cancel = true;
            }
        }

        private bool AdvanceDate() {
            PomodoroState Updated = _State.AdvanceTo(DateOnly.FromDateTime(DateTime.Today));
            if (Updated == _State) {
                return false;
            }

            _State = Updated;
            _NeedsSave = true;
            return true;
        }

        private void ShowStatus(string time) =>
            lblAvvio.Text = $"{time} - {_State.Today} ({_State.Yesterday}) {_State.Total}";

        private void ResetSession() {
            StopFlash();
            TimerEnd.Stop();
            _SessionClock.Reset();
            _BreakElapsed = TimeSpan.Zero;
            _BreakClock.Restart();
            TimerStop.Start();
            lblTimer.Text = SESSIONDURATION.ToString(@"mm\:ss\.f");
            ShowStatus(_BreakElapsed.ToString(@"hh\:mm\:ss"));
        }

        private void btnStop_Click(object sender, EventArgs e) {
            AdvanceDate();
            ResetSession();
            if (_NeedsSave) {
                Save();
            }
        }

        private void btnStart_Click(object sender, EventArgs e) {
            AdvanceDate();
            StopFlash();
            _SessionStartedAt = DateTime.Now;
            _SessionClock.Restart();
            _BreakClock.Reset();
            TimerStop.Stop();
            TimerEnd.Start();
            lblTimer.Text = SESSIONDURATION.ToString(@"mm\:ss\.f");
            ShowStatus(_SessionStartedAt.ToString("HH:mm:ss"));
            if (_NeedsSave) {
                Save();
            }
        }

        private void TimerEnd_Tick(object sender, EventArgs e) {
            if (!TimerEnd.Enabled) {
                return;
            }

            bool DateChanged = AdvanceDate();
            TimeSpan Remaining = SESSIONDURATION - _SessionClock.Elapsed;
            if (Remaining > TimeSpan.Zero) {
                lblTimer.Text = Remaining.ToString(@"mm\:ss\.f");
                ShowStatus(_SessionStartedAt.ToString("HH:mm:ss"));
                if (DateChanged) {
                    Save();
                }
                return;
            }

            _State = _State with { Total = _State.Total + 1, Today = _State.Today + 1 };
            _NeedsSave = true;
            ResetSession();
            Rectangle Area = Screen.GetWorkingArea(this);
            Location = new(Area.Left + (Area.Width - Width) / 2, Area.Top + (Area.Height - Height) / 2);
            Save();
            BackColor = Color.Yellow;
            _FlashTimer.Start();
        }

        private void StopFlash() {
            _FlashTimer.Stop();
            _FlashStep = 0;
            BackColor = SystemColors.Control;
        }

        private void FlashTimer_Tick(object sender, EventArgs e) {
            _FlashStep++;
            if (_FlashStep >= 3) {
                StopFlash();
                return;
            }
            BackColor = _FlashStep == 1 ? Color.Blue : Color.Red;
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

            TimeSpan Elapsed = _BreakClock.Elapsed;
            _BreakClock.Restart();
            if (Elapsed.TotalMilliseconds < TimerStop.Interval * 5) {
                _BreakElapsed += Elapsed;
            }
            bool DateChanged = AdvanceDate();
            ShowStatus(_BreakElapsed.ToString(@"hh\:mm\:ss"));
            if (DateChanged) {
                Save();
            }
        }

        private void lblStart_MouseMove(object sender, MouseEventArgs e) => MoveWindow();

        private bool Save() {
            try {
                PomodoroStore.Save(_State);
                _NeedsSave = false;
                return true;
            }
            catch (Exception Ex) when (Ex is IOException or UnauthorizedAccessException) {
                bool SessionRunning = TimerEnd.Enabled;
                bool BreakRunning = TimerStop.Enabled;
                TimerEnd.Stop();
                TimerStop.Stop();
                string Error = $"{Ex.GetType().Name} (0x{Ex.HResult:X8}): {Ex.Message}";
                MessageBox.Show(this, $"Could not save your counters to '{PomodoroStore.FilePath}'.\n\n{Error}",
                    "Timer Pomodoro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                TimerEnd.Enabled = SessionRunning;
                TimerStop.Enabled = BreakRunning;
                return false;
            }
        }
    }
}
