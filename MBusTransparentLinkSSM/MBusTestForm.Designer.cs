namespace MBusTransparentLinkSSM
{
    partial class MBusTestForm
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.TransparentLintStart = new System.Windows.Forms.Button();
            this.seriaRD_OneEv = new System.Windows.Forms.TextBox();
            this.serialRD = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.udpRD = new System.Windows.Forms.TextBox();
            this.udpRD_OneEv = new System.Windows.Forms.TextBox();
            this.SSM_UdpLisenerStop = new System.Windows.Forms.Button();
            this.SSM_UdpLisenerStart = new System.Windows.Forms.Button();
            this.TST_ShowMemoryStram = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TransparentLintStart
            // 
            this.TransparentLintStart.Location = new System.Drawing.Point(24, 21);
            this.TransparentLintStart.Name = "TransparentLintStart";
            this.TransparentLintStart.Size = new System.Drawing.Size(142, 23);
            this.TransparentLintStart.TabIndex = 0;
            this.TransparentLintStart.Text = "TransparentLintStart";
            this.TransparentLintStart.UseVisualStyleBackColor = true;
            this.TransparentLintStart.Click += new System.EventHandler(this.OpenSerialPort_Click);
            // 
            // seriaRD_OneEv
            // 
            this.seriaRD_OneEv.Location = new System.Drawing.Point(196, 42);
            this.seriaRD_OneEv.Name = "seriaRD_OneEv";
            this.seriaRD_OneEv.Size = new System.Drawing.Size(349, 22);
            this.seriaRD_OneEv.TabIndex = 1;
            // 
            // serialRD
            // 
            this.serialRD.Location = new System.Drawing.Point(196, 70);
            this.serialRD.Multiline = true;
            this.serialRD.Name = "serialRD";
            this.serialRD.Size = new System.Drawing.Size(349, 347);
            this.serialRD.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(193, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Serial Read Data:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(575, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 16);
            this.label2.TabIndex = 6;
            this.label2.Text = "UDP Read Data:";
            // 
            // udpRD
            // 
            this.udpRD.Location = new System.Drawing.Point(578, 82);
            this.udpRD.Multiline = true;
            this.udpRD.Name = "udpRD";
            this.udpRD.Size = new System.Drawing.Size(349, 347);
            this.udpRD.TabIndex = 5;
            // 
            // udpRD_OneEv
            // 
            this.udpRD_OneEv.Location = new System.Drawing.Point(578, 47);
            this.udpRD_OneEv.Name = "udpRD_OneEv";
            this.udpRD_OneEv.Size = new System.Drawing.Size(349, 22);
            this.udpRD_OneEv.TabIndex = 4;
            // 
            // SSM_UdpLisenerStop
            // 
            this.SSM_UdpLisenerStop.Location = new System.Drawing.Point(12, 134);
            this.SSM_UdpLisenerStop.Name = "SSM_UdpLisenerStop";
            this.SSM_UdpLisenerStop.Size = new System.Drawing.Size(154, 23);
            this.SSM_UdpLisenerStop.TabIndex = 7;
            this.SSM_UdpLisenerStop.Text = "SSM_UdpLisenerStop";
            this.SSM_UdpLisenerStop.UseVisualStyleBackColor = true;
            this.SSM_UdpLisenerStop.Click += new System.EventHandler(this.SSM_UdpLisenerStop_Click);
            // 
            // SSM_UdpLisenerStart
            // 
            this.SSM_UdpLisenerStart.Location = new System.Drawing.Point(12, 163);
            this.SSM_UdpLisenerStart.Name = "SSM_UdpLisenerStart";
            this.SSM_UdpLisenerStart.Size = new System.Drawing.Size(154, 23);
            this.SSM_UdpLisenerStart.TabIndex = 8;
            this.SSM_UdpLisenerStart.Text = "SSM_UdpLisenerStart";
            this.SSM_UdpLisenerStart.UseVisualStyleBackColor = true;
            this.SSM_UdpLisenerStart.Click += new System.EventHandler(this.SSM_UdpLisenerStart_Click);
            // 
            // TST_ShowMemoryStram
            // 
            this.TST_ShowMemoryStram.Location = new System.Drawing.Point(221, 491);
            this.TST_ShowMemoryStram.Name = "TST_ShowMemoryStram";
            this.TST_ShowMemoryStram.Size = new System.Drawing.Size(196, 23);
            this.TST_ShowMemoryStram.TabIndex = 9;
            this.TST_ShowMemoryStram.Text = "TST_ShowMemoryStram";
            this.TST_ShowMemoryStram.UseVisualStyleBackColor = true;
            this.TST_ShowMemoryStram.Click += new System.EventHandler(this.TST_ShowMemoryStram_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(483, 491);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(164, 39);
            this.button1.TabIndex = 10;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MBusTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1445, 559);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.TST_ShowMemoryStram);
            this.Controls.Add(this.SSM_UdpLisenerStart);
            this.Controls.Add(this.SSM_UdpLisenerStop);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.udpRD);
            this.Controls.Add(this.udpRD_OneEv);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.serialRD);
            this.Controls.Add(this.seriaRD_OneEv);
            this.Controls.Add(this.TransparentLintStart);
            this.Name = "MBusTestForm";
            this.Text = "MBusTestForm";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button TransparentLintStart;
        private System.Windows.Forms.TextBox seriaRD_OneEv;
        private System.Windows.Forms.TextBox serialRD;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox udpRD;
        private System.Windows.Forms.TextBox udpRD_OneEv;
        private System.Windows.Forms.Button SSM_UdpLisenerStop;
        private System.Windows.Forms.Button SSM_UdpLisenerStart;
        private System.Windows.Forms.Button TST_ShowMemoryStram;
        private System.Windows.Forms.Button button1;
    }
}

