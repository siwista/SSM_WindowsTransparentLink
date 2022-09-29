using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace MBusTransparentLinkSSM
{
    public partial class MBusTestForm : Form
    {
        public MBusTestForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        MBusSSMClass SSM_Class = new MBusSSMClass();

        private void OpenSerialPort_Click(object sender, EventArgs e)
        {
            SerialPort SP = new SerialPort();
            SP.PortName = "COM7";
            SP.Parity = Parity.None;
            SP.BaudRate = 19200;
            SP.StopBits = StopBits.One;
            SSM_Class = new MBusSSMClass(SP, "127.0.0.1", 5000, "127.0.0.1", 5001);
            seriaRD_OneEv.TextChanged += SeriaRD_OneEv_TextChanged;
            udpRD_OneEv.TextChanged += UdpRD_OneEv_TextChanged;
            
            SSM_Class.setSerialTerminalReference(seriaRD_OneEv);
            SSM_Class.SetSSM_UDP_Terminal_Reference(udpRD_OneEv);

            
            SSM_Class.StartModBusSerialLisener();
            SSM_Class.Start_SSM_UDP_Server();
            SSM_Class.TST_EmuSSM_UDP_ServerStart();
        }

        private void UdpRD_OneEv_TextChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            string tmp = udpRD_OneEv.Text;
            tmp = tmp.Replace('-', ' ');
            if (udpRD.Text.Length > 900) udpRD.Text = "";
            udpRD.Text = udpRD.Text + tmp;
            //throw new NotImplementedException();
        }

        private void SeriaRD_OneEv_TextChanged(object sender, EventArgs e)
        {
            string tmp = seriaRD_OneEv.Text;
            tmp = tmp.Replace('-', ' ');
            if (serialRD.Text.Length > 900) serialRD.Text = "";
            serialRD.Text = serialRD.Text + tmp;
            //throw new NotImplementedException();
        }

        private void SSM_UdpLisenerStop_Click(object sender, EventArgs e)
        {
            
            SSM_Class.Stop_SSM_UDP_Server();
        }

        private void SSM_UdpLisenerStart_Click(object sender, EventArgs e)
        {
            //SSM_Class.StartModBusSerialLisener();
            SSM_Class.Start_SSM_UDP_Server();
        }
        

        

        private void TST_ShowMemoryStram_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
