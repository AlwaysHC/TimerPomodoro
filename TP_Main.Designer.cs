namespace TimerPomodoro {
    partial class TP_Main {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.lblTimer = new System.Windows.Forms.Label();
            this.TimerEnd = new System.Windows.Forms.Timer(this.components);
            this.btnAvvia = new System.Windows.Forms.Button();
            this.btnFerma = new System.Windows.Forms.Button();
            this.lblAvvio = new System.Windows.Forms.Label();
            this.TimerStop = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // lblTimer
            // 
            this.lblTimer.BackColor = System.Drawing.Color.Transparent;
            this.lblTimer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTimer.Location = new System.Drawing.Point(0, 0);
            this.lblTimer.Margin = new System.Windows.Forms.Padding(0);
            this.lblTimer.Name = "lblTimer";
            this.lblTimer.Size = new System.Drawing.Size(202, 73);
            this.lblTimer.TabIndex = 0;
            this.lblTimer.Text = "25:00";
            this.lblTimer.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.lblTimer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblTimer_MouseMove);
            // 
            // TimerFine
            // 
            this.TimerEnd.Tick += new System.EventHandler(this.TimerEnd_Tick);
            // 
            // btnAvvia
            // 
            this.btnAvvia.Location = new System.Drawing.Point(0, 0);
            this.btnAvvia.Margin = new System.Windows.Forms.Padding(0);
            this.btnAvvia.Name = "btnAvvia";
            this.btnAvvia.Size = new System.Drawing.Size(49, 20);
            this.btnAvvia.TabIndex = 1;
            this.btnAvvia.Text = "Start";
            this.btnAvvia.UseVisualStyleBackColor = true;
            this.btnAvvia.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnFerma
            // 
            this.btnFerma.Location = new System.Drawing.Point(154, 0);
            this.btnFerma.Margin = new System.Windows.Forms.Padding(0);
            this.btnFerma.Name = "btnFerma";
            this.btnFerma.Size = new System.Drawing.Size(49, 20);
            this.btnFerma.TabIndex = 2;
            this.btnFerma.Text = "Stop";
            this.btnFerma.UseVisualStyleBackColor = true;
            this.btnFerma.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // lblAvvio
            // 
            this.lblAvvio.Location = new System.Drawing.Point(51, 3);
            this.lblAvvio.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblAvvio.Name = "lblAvvio";
            this.lblAvvio.Size = new System.Drawing.Size(100, 19);
            this.lblAvvio.TabIndex = 3;
            this.lblAvvio.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblAvvio.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lblStart_MouseMove);
            // 
            // TimerFermo
            // 
            this.TimerStop.Interval = 1000;
            this.TimerStop.Tick += new System.EventHandler(this.TimerStop_Tick);
            // 
            // TP_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(202, 73);
            this.ControlBox = false;
            this.Controls.Add(this.btnFerma);
            this.Controls.Add(this.btnAvvia);
            this.Controls.Add(this.lblAvvio);
            this.Controls.Add(this.lblTimer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TP_Main";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Timer Pomodoro";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTimer;
        private System.Windows.Forms.Timer TimerEnd;
        private System.Windows.Forms.Button btnAvvia;
        private System.Windows.Forms.Button btnFerma;
        private System.Windows.Forms.Label lblAvvio;
        private System.Windows.Forms.Timer TimerStop;
    }
}

