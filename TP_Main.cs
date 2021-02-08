using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using TimerPomodoro.Properties;

namespace TimerPomodoro {
    public partial class TP_Main: Form {
        public TP_Main() {
            InitializeComponent();
            Icon = Resources.Pomodoro;

            StreamReader SR = null;
            try {
                SR = new StreamReader("TimerPomodoro.txt");
                _PomodoriStart = Int32.Parse(SR.ReadLine());
                _PomodoriToday = Int32.Parse(SR.ReadLine());
                _PomodoriYesterday = Int32.Parse(SR.ReadLine());
                _PomodoriDate = DateTime.Parse(SR.ReadLine());
            }
            catch {
            }
            finally {
                if (SR != null) {
                    SR.Close();
                }
            }
            if (_PomodoriDate.Date != DateTime.Now.Date) {
                _PomodoriYesterday = _PomodoriToday;
                _PomodoriToday = 0;
            }

            btnStop_Click(null, null);
        }

        private DateTime _DateTimeStop = DateTime.MinValue;
        private TimeSpan _TimeSpanStop = new TimeSpan(0, 0, 0);
        private DateTime _TimeSpanLastStop = DateTime.MinValue;
        private int _PomodoriStart = 0;
        private int _PomodoriToday = 0;
        private int _PomodoriYesterday = 0;
        private DateTime _PomodoriDate = DateTime.MinValue;

        private void btnStop_Click(object sender, EventArgs e) {
            Graphics G = lblTimer.CreateGraphics();
            SizeF S = G.MeasureString(" 25:00.0 ", lblTimer.Font);
            lblTimer.Font = new Font(lblTimer.Font.FontFamily, lblTimer.Font.Size * (lblTimer.Width / S.Width), lblTimer.Font.Style);

            TimerEnd.Enabled = false;
            TimerStop.Enabled = true;
            _DateTimeStop = DateTime.MinValue;
            _TimeSpanStop = new TimeSpan(0, 0, 0);
            _TimeSpanLastStop = DateTime.Now;
            lblTimer.Text = new TimeSpan(0, 25, 0).ToString("mm\\:ss\\.f");
        }

        private void btnStart_Click(object sender, EventArgs e) {
            _DateTimeStop = DateTime.Now.AddMinutes(25);
            TimerEnd.Enabled = true;
            TimerStop.Enabled = false;
            lblAvvio.Text = DateTime.Now.ToString("HH\\:mm\\:ss") + " - " + _PomodoriToday + " (" + _PomodoriYesterday + ") " + _PomodoriStart;
        }

        private void TimerEnd_Tick(object sender, EventArgs e) {
            DateTime Ora = DateTime.Now;
            if (Ora >= _DateTimeStop) {
                Rectangle R = Screen.GetWorkingArea(lblTimer);
                Left = (R.Width - Width) / 2;
                Top = (R.Height - Height) / 2;

                _PomodoriToday++;
                _PomodoriStart++;
                Save();
                btnStop_Click(null, null);

                BackColor = Color.Yellow;
                Application.DoEvents();
                Thread.Sleep(500);
                BackColor = Color.Blue;
                Application.DoEvents();
                Thread.Sleep(500);
                BackColor = Color.Red;
                Application.DoEvents();
                Thread.Sleep(500);
                BackColor = SystemColors.Control;
                Application.DoEvents();
            }
            else {
                lblTimer.Text = (_DateTimeStop.Subtract(Ora)).ToString("mm\\:ss\\.f");
            }
        }

        private void lblTimer_MouseMove(object sender, MouseEventArgs e) {
            MoveWindow();
        }

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
            DateTime Now = DateTime.Now;
            if ((Now - _TimeSpanLastStop).TotalMilliseconds < TimerStop.Interval * 5) {
                _TimeSpanStop = _TimeSpanStop.Add(Now - _TimeSpanLastStop);
            }
            if (_TimeSpanLastStop.DayOfYear != Now.DayOfYear) {
                _PomodoriYesterday = _PomodoriToday;
                _PomodoriToday = 0;
                Save();
            }
            _TimeSpanLastStop = Now;
            lblAvvio.Text = _TimeSpanStop.ToString("hh\\:mm\\:ss") + " - " + _PomodoriToday + " (" + _PomodoriYesterday + ") " + _PomodoriStart;
        }

        private void lblStart_MouseMove(object sender, MouseEventArgs e) {
            MoveWindow();
        }

        private void Save() {
            try {
                StreamWriter SW = new StreamWriter("TimerPomodoro.txt", false);
                SW.WriteLine(_PomodoriStart);
                SW.WriteLine(_PomodoriToday);
                SW.WriteLine(_PomodoriYesterday);
                SW.WriteLine(DateTime.Now);
                SW.Close();
            }
            catch {
            }
        }
    }
}
