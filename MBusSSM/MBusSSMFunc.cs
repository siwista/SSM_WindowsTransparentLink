using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Forms;
using System.Threading;
using System.Timers;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MBusTransparentLinkSSM
{
    public class MBusSSMClass
    {
        //private static MBusSSMSlass instance;
        //public static MBusSSMSlass Instance { get { return instance; } }

        private string SSM_UDP_InterfaceLisener;
        private int SSM_UDP_PortLisener;
        private SerialPort mBusSerial;
        private static string SSM_UDP_DestinationAddress;
        private static int SSM_UDP_DestinationPort;

        private bool isModbusSerialOpenedBool = false;
        private bool isSSM_UDP_LisenerWorksBool = false;

        /// <summary>
        /// Class constructor
        /// </summary>
        public MBusSSMClass()
        {
            
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="mBusSerial">Serial Port</param>
        /// <param name="SSM_UDP_InterfaceLisener">IP addres for UDP server lisener and SSM destination (SSM Communication)</param>
        /// <param name="SSM_UDP_PortLisener">UDP port lisener (SSM Communication)</param>
        /// <param name="SSM_UDP_DestinationAddress">IP SSM destination Address</param>
        /// <param name="SSM_UDP_DestinationPort">SSM destination port</param>
        public MBusSSMClass(SerialPort mBusSerial, string SSM_UDP_InterfaceLisener, int SSM_UDP_PortLisener, string SSM_UDP_DestinationAddress, int SSM_UDP_DestinationPort)
        {
            this.SSM_UDP_InterfaceLisener = SSM_UDP_InterfaceLisener;
            this.SSM_UDP_PortLisener = SSM_UDP_PortLisener;
            SSM_DestinationUDPAddress = SSM_UDP_DestinationAddress;
            SSM_DestinationUDPPort = SSM_UDP_DestinationPort;
            //this.SSM_UDP_DestinationPort = SSM_UDP_DestinationPort;
            //this.SSM_UDP_DestinationAddress = SSM_UDP_DestinationAddress;
            this.mBusSerial = mBusSerial;
        }

        /// <summary>
        /// Getting / Setting ModBus serial instance
        /// </summary>
        public SerialPort ModBusSerial
        {
            get { return mBusSerial; }
            set { mBusSerial = value; }
        }

        /// <summary>
        /// Getting / Setting UDP(SSM) server port Lisener
        /// </summary>
        public int SSM_PortLisener
        {
            get { return SSM_UDP_PortLisener; }
            set { SSM_UDP_PortLisener = value; }
        }

        /// <summary>
        /// Getting / Setting UDP (SSM) server IP Address lisener
        /// </summary>
        public string SSM_InterfaceLisener
        {
            get { return SSM_UDP_InterfaceLisener; }
            set
            {
                SSM_UDP_InterfaceLisener = value;
            }
        }

        /// <summary>
        /// Getting / Setting UDP(SSM) Desination port
        /// </summary>
        public int SSM_DestinationUDPPort
        {
            get { return SSM_UDP_DestinationPort; }
            set
            {
                SSM_UDP_DestinationPort = value;
            }
        }

        /// <summary>
        /// Getting / Setting UDP(SSM) Destination IP Address
        /// </summary>
        public static string SSM_DestinationUDPAddress
        {
            get { return SSM_UDP_DestinationAddress; }
            set
            {
                SSM_UDP_DestinationAddress = value;
            }
        }

        /// <summary>
        /// Getting modbus serial port status
        /// </summary>
        public bool isModbusSerialOpened
        {
            get { return isModbusSerialOpened;} 
        }

        /// <summary>
        /// Getting UDP(SSM) server lisener status
        /// </summary>
        public bool isSSM_UDP_LisenerWorks
        {
            get { return isSSM_UDP_LisenerWorks;}   
        }

        public long mBusBufferedTimeOut
        {
            get { return silenceMbusComTimeOutMiliseconds; }
        }

        public double silenceMbusBuferredCountChar
        {
            get { return silenceMbusComChar; }
            set { silenceMbusComChar = value; }
        }

        public static uint counterMbusComRxBufferedFrame
        {
            get { return _counterMbusComRxBufferedFrame;}
            set { _counterMbusComRxBufferedFrame = value;}
        }

        public uint counterMbusComTxBufferedFrame
        {
            get { return _counterMbusComTxBufferedFrame; }
            set { _counterMbusComTxBufferedFrame = value; }
        }

        public uint counterUdpRxBufferedFrame
        {
            get { return _counterUdpRxBufferedFrame; }
            set { _counterUdpRxBufferedFrame = value; }
        }

        public static uint counterUdpTxBufferedFrame
        {
            get { return _counterUdpTxBufferedFrame; }
            set { _counterUdpTxBufferedFrame = value; }
        }



        private long      silenceMbusComTimeOutMiliseconds = 0;
        private double    oneMbusByteBitSize = 9;
        private double    silenceMbusComChar = 3.5;    //min 3, max 1000

        private static uint _counterMbusComRxBufferedFrame = 0;
        private uint _counterMbusComTxBufferedFrame = 0;

        private uint _counterUdpRxBufferedFrame = 0;
        private static uint _counterUdpTxBufferedFrame = 0;

        /// <summary>
        /// Run Modbus serial port - initialize
        /// </summary>
        /// <returns> Exception of wrong initialization if something wrong, or null if everything is fine </returns>
        public Exception StartModBusSerialLisener()
        {
            try
            {
                mBusSerial.Disposed += MBusSerial_Disposed;
                mBusSerial.Open();
                mBusSerial.DataReceived += MBusSerial_DataReceived;
                isModbusSerialOpenedBool = true;
                
                if (silenceMbusComChar<3)
                {
                    silenceMbusComChar = 3;
                }

                if (silenceMbusComChar > 1000)
                {
                    silenceMbusComChar = 1000;
                }

                if (mBusSerial.Parity != Parity.None)
                {
                    oneMbusByteBitSize = oneMbusByteBitSize + 1;
                }

                if (mBusSerial.StopBits == StopBits.One)
                {
                    oneMbusByteBitSize = oneMbusByteBitSize + 1;
                }
                else if (mBusSerial.StopBits == StopBits.Two)
                {
                    oneMbusByteBitSize = oneMbusByteBitSize + 2;
                }
                else if (mBusSerial.StopBits == StopBits.OnePointFive)
                {
                    oneMbusByteBitSize = oneMbusByteBitSize + 1.5;
                }
               
                silenceMbusComTimeOutMiliseconds = (long)(silenceMbusComChar * 1000000 * oneMbusByteBitSize / (double)mBusSerial.BaudRate);
                counterMbusComRxBufferedFrame = 0;
                counterMbusComTxBufferedFrame = 0;
                return null;
            }
            catch (Exception Ex)
            {
               isModbusSerialOpenedBool = false;
               return Ex;
            }
        }

        /****************** SERIAL DATA - START *****************************/
        private TextBox terminalSerial;
        
        /// <summary>
        /// Set reference to TextBox object for incoming serial port data ex. text terminal for incoming data
        /// </summary>
        /// <param name="terminalSerial">TextBox reference object</param>
        public void setSerialTerminalReference (TextBox terminalSerial)
        {
            this.terminalSerial = terminalSerial;
        }  
        /// <summary>
        /// Delegate for incomig serial port data
        /// </summary>
        /// <param name="readSerialBytes">Data bytes array for incoming serial data</param>
        private delegate void terminalRxSerialBytesDelegate(byte[] readSerialBytes);
        /// <summary>
        /// Delegate reference function for incoming serial data
        /// </summary>
        /// <param name="readSerialBytes">Data bytes array for incoming serial data</param>
        private void terminalRxSerialBytes (byte[] readSerialBytes)
        {
            terminalSerial.Text = BitConverter.ToString(readSerialBytes);
        }
        
        
        /// <summary>
        /// Event for serial port data recived
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MBusSerial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //throw new NotImplementedException();
            if (mBusSerial.BytesToRead > 0)
            {
                int bytesToReadCount = mBusSerial.BytesToRead;
                byte[] readSerialBytes = new byte[bytesToReadCount];
                mBusSerial.Read(readSerialBytes, 0, bytesToReadCount);

                while (isThProcessedSerialMbusData) { }

                WaitTimeoutForIncomingSerialDataThread.Abort();
                WaitTimeoutForIncomingSerialDataThread = new Thread(new ThreadStart(WaitTimeoutForIncomingSerialData));
                MbusStream.Write(readSerialBytes, 0, bytesToReadCount);
                long eventRxDataTimeStampStart = Stopwatch.GetTimestamp();
                endDelayWaitRxMbusData = eventRxDataTimeStampStart + (long)((double)(silenceMbusComTimeOutMiliseconds * Stopwatch.Frequency) / 1000000);
                WaitTimeoutForIncomingSerialDataThread.IsBackground = true;
                WaitTimeoutForIncomingSerialDataThread.Start();

                //MbusStream.Write(readSerialBytes, 0, bytesToReadCount);
                //SendToSSM_UdpData(readSerialBytes);

                try
                {
                    terminalSerial.BeginInvoke(new terminalRxSerialBytesDelegate(terminalRxSerialBytes), new object[] { readSerialBytes });
                }
                catch (Exception Ex)
                {
                    
                }
            }
        }



        private static MemoryStream MbusStream = new MemoryStream();
        static bool isThProcessedSerialMbusData = false;
        static bool terminationWaitTimeoutForIncomingSerialDataThread = false;
        static long endDelayWaitRxMbusData = 0;
        static Thread WaitTimeoutForIncomingSerialDataThread = new Thread(new ThreadStart(WaitTimeoutForIncomingSerialData));
        private static void WaitTimeoutForIncomingSerialData ()
        {
            isThProcessedSerialMbusData = false;
            while (Stopwatch.GetTimestamp() < endDelayWaitRxMbusData) 
            {
                if(terminationWaitTimeoutForIncomingSerialDataThread)
                {
                    break;
                }
            }
            if (!terminationWaitTimeoutForIncomingSerialDataThread)
            {
                isThProcessedSerialMbusData = true;
                byte[] mBusDataToUDP = MbusStream.ToArray();
                counterMbusComRxBufferedFrame = counterMbusComRxBufferedFrame + 1;
                SendToSSM_UdpData(mBusDataToUDP); //ONLY TEST !!!!!!
                MbusStream.Close();
                MbusStream = new MemoryStream();
            }
            isThProcessedSerialMbusData = false;
            terminationWaitTimeoutForIncomingSerialDataThread = false;
        }

        /// <summary>
        /// Send dato to serial port
        /// </summary>
        /// <param name="dataToSend">Data bytes array to send</param>
        /// <returns>Exception of wrong data send, or null if everything is fine</returns>
        private Exception MBusSendData(byte[] dataToSend)
        {
            try
            {
                counterMbusComTxBufferedFrame = counterMbusComTxBufferedFrame + 1;
                mBusSerial.Write(dataToSend, 0, dataToSend.Length);
                return null;
            }
            catch (Exception Ex)
            {
                return (Ex);
            }
        }

        /// <summary>
        /// Event for serial port disposed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MBusSerial_Disposed(object sender, EventArgs e)
        {
            isModbusSerialOpenedBool = false;
        }

        /// <summary>
        /// Stop modbus serial port
        /// </summary>
        /// <returns>Exception of wrong deinitialize port if something wrong, or null if everything is fine </returns>
        private Exception stopMBusSerial ()
        {
            try
            {
                mBusSerial.Disposed -= MBusSerial_Disposed;
            }
            catch (Exception Ex)
            {

            }

            try
            {
                mBusSerial.DataReceived -= MBusSerial_DataReceived; 
            }
            catch (Exception Ex)
            {   

            }

            try
            {
                mBusSerial.Close();
                isModbusSerialOpenedBool = false;
                return null;
            }
            catch (Exception Ex)
            {
                isModbusSerialOpenedBool = false;
                return Ex;
            }
        }
        /****************** SERIAL DATA - END *****************************/


        private UdpClient mySSM_UDP_Server = new UdpClient();
        /// <summary>
        /// Start UDP(SSM) Server Lisener thread
        /// </summary>
        public void Start_SSM_UDP_Server ()
        {
            Stop_SSM_UDP_Server();
            counterUdpRxBufferedFrame = 0;
            counterUdpTxBufferedFrame = 0;
            Thread reciveSSM_UdpServer = new Thread(new ThreadStart(SSM_UDP_Server_Thr));
            reciveSSM_UdpServer.IsBackground = true;
            reciveSSM_UdpServer.Start();
        }
        /// <summary>
        /// Stop UDP(SSM) server lisener thread
        /// </summary>
        public void Stop_SSM_UDP_Server()
        {
            try
            {
                mySSM_UDP_Server.Close();
                while (isSSM_UDP_LisenerWorksBool) { };
            }
            catch (Exception Ex)
            {

            }

        }
        private TextBox terminalSSM_UDP;
        /// <summary>
        /// Set reference to TextBox object for incoming UDP data ex. text terminal for incoming data
        /// </summary>
        /// <param name="terminalSSM_UDP">TextBox reference object</param>
        public void SetSSM_UDP_Terminal_Reference(TextBox terminalSSM_UDP)
        {
            this.terminalSSM_UDP = terminalSSM_UDP;
        }
        /// <summary>
        /// Delegate for incoming UDP(SSM) data
        /// </summary>
        /// <param name="readUdpSSM_Bytes">byte array incoming UDP data</param>
        private delegate void SSM_UDP_TerminalDelegate(byte[] readUdpSSM_Bytes);
        /// <summary>
        /// Delegate method for incoming UDP(SSM) data
        /// </summary>
        /// <param name="readUdpSSM_Bytes">byte array incoming UDP data</param>
        private void SSM_UDP_Terminal(byte[] readUdpSSM_Bytes)
        {
            terminalSSM_UDP.Text = (BitConverter.ToString(readUdpSSM_Bytes))+"\r\n";
        }
        /// <summary>
        /// thread for UDP(SSM) server
        /// </summary>
        private void SSM_UDP_Server_Thr ()
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                mySSM_UDP_Server = new UdpClient(SSM_UDP_PortLisener);
                isSSM_UDP_LisenerWorksBool = true;
            }
            catch (Exception Ex)
            {
                isSSM_UDP_LisenerWorksBool = false;
                mySSM_UDP_Server.Close();
            }
            if (isSSM_UDP_LisenerWorksBool)
            {
                while (true)
                {
                    try
                    {
                        counterUdpRxBufferedFrame = counterUdpRxBufferedFrame + 1;
                        byte[] readUdpSSMdata = mySSM_UDP_Server.Receive(ref iPEndPoint);
                        MBusSendData(readUdpSSMdata);//Only for tests
                        try
                        {
                            terminalSSM_UDP.BeginInvoke(new SSM_UDP_TerminalDelegate(SSM_UDP_Terminal), new object[] { readUdpSSMdata });
                        }
                        catch (Exception Ex)
                        {

                        }
                    }
                    catch (Exception Ex)
                    {
                        isSSM_UDP_LisenerWorksBool = false;
                        mySSM_UDP_Server.Close();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Method for send UDP data to SSM
        /// </summary>
        /// <param name="dataToSSM">Byte array to send</param>
        private static void SendToSSM_UdpData(byte[] dataToSSM)
        {
            UdpClient SSM_UdpClient = new UdpClient(SSM_DestinationUDPAddress, SSM_UDP_DestinationPort);
            counterUdpTxBufferedFrame = counterUdpTxBufferedFrame + 1;
            SSM_UdpClient.Send(dataToSSM, dataToSSM.Length);
            SSM_UdpClient.Close();
        }


        //TEST ONLY !!!!!!!!!! (Bellow)
        /*
        SerialPort SP = new SerialPort();
        public void TST_EmuSSM_UDP_ServerStart()
        {
            
            SP.PortName = "COM8";
            SP.Parity = Parity.None;
            SP.BaudRate = 19200;
            SP.StopBits = StopBits.One;
            SP.Open();

            Thread TST_EMU_SSM_UDP_Server = new Thread (new ThreadStart(TST_EmuSSM_UDP_ServerStart_Thr));
            TST_EMU_SSM_UDP_Server.IsBackground = true;
            TST_EMU_SSM_UDP_Server.Start(); 
        }

        UdpClient myEmuSSM_UDP_Server = new UdpClient();

        private void TST_EmuSSM_UDP_ServerStart_Thr()
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);
            myEmuSSM_UDP_Server = new UdpClient(SSM_UDP_DestinationPort);
            while (true)
            {
                try
                {   
                    Byte[] readUdpSSMdata = myEmuSSM_UDP_Server.Receive(ref iPEndPoint);
                    
                    UdpClient SSM_UdpClient = new UdpClient(SSM_InterfaceLisener, SSM_PortLisener);
                    SP.Write(readUdpSSMdata, 0, readUdpSSMdata.Length);
                    //SSM_UdpClient.Send(readUdpSSMdata, readUdpSSMdata.Length);
                }
                catch (Exception Ex)
                {

                    myEmuSSM_UDP_Server.Close();
                    break;
                }
            }
        }

        public void TST_ShowMemoryStream ()
        {
            byte[] BT_TST = MbusStream.ToArray();
            int k;
            //MbusSream.Read(BT_TST, 0, Convert.ToInt32(MbusSream.Length));
            //MessageBox.Show(MbusSream.Length.ToString()+ "    " + BT_TST.Length + "\r\n" + BitConverter.ToString(BT_TST));
            MessageBox.Show(MbusStream.Length.ToString() + "\r\n" + BitConverter.ToString(MbusStream.ToArray()));
            //MbusParaser(ref MbusStream);
            //MbusStream.Close();
            //MbusStream = new MemoryStream();
            //MessageBox.Show(MbusStream.Length.ToString() + "\r\n" + BitConverter.ToString(MbusStream.ToArray()));
        }*/


    }
}
