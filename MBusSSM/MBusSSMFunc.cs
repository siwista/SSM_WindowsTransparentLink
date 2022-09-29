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
        private string SSM_UDP_DestinationAddress;
        private int SSM_UDP_DestinationPort;

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
            this.SSM_UDP_DestinationPort = SSM_UDP_DestinationPort;
            this.SSM_UDP_DestinationAddress = SSM_UDP_DestinationAddress;
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
        public string SSM_DestinationUDPAddress
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


        uint silenceMbusComTimeMiliseconds = 0;
        uint silenceMbusComChar = 3;

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
                silenceMbusComTimeMiliseconds = (uint)((double)silenceMbusComChar * 1000000 * 10 / (double)mBusSerial.BaudRate);
                return null;
            }
            catch (Exception Ex)
            {
                isModbusSerialOpenedBool = false;
                return Ex;
            }
        }



        /****************** SERIAL MODBUS PARASE - START *****************************/
        const byte  MBUS_DEVICE_ADDRESS = 0;        //byte index for modbus device address
        const byte  MBUS_FUNC_NB = 1;               //byte index for modbus function number
        const byte  MBUS_DATA_ADDR_HI = 2;          //byte index for data address (Hi)
        const byte  MBUS_DATA_ADDR_LO = 3;          //byte index for data address (Lo)
        const byte  MBUS_DATA_LENGTH_HI = 4;        //byte index for modbus data length (Hi)
        const byte  MBUS_DATA_LENGTH_LO = 5;        //byte index for modbus data length (Lo)

        /// <summary>
        /// CRC for Modbus
        /// </summary>
        /// <param name="inpData">data from serial stream</param>
        /// <param name="startIdx">start index</param>
        /// <param name="length">input data length to Modbus CRC calculation</param>
        /// <returns></returns>
        private UInt16 GetModbusCRC(byte[] inpData, uint startIdx, uint length)
        {
            UInt16 crc_P = 0xFFFF;
            UInt16 crc_P1;
            uint k;
            byte i;
            for (k = startIdx; k < length; k++)
            {
                crc_P = Convert.ToUInt16(crc_P ^ inpData[k]);
                for (i = 8; i != 0; i--)
                {
                    // CRC_P = Convert.ToUInt16(CRC_P >> 1);

                    if ((crc_P & 1) != 0)
                    {
                        crc_P = Convert.ToUInt16(crc_P >> 1);
                        crc_P = Convert.ToUInt16(crc_P ^ 0xA001);
                    }
                    else
                    {
                        crc_P = Convert.ToUInt16(crc_P >> 1);
                    }
                }
            }
            crc_P1 = Convert.ToUInt16((crc_P << 8) & 0xFF00);
            crc_P = Convert.ToUInt16(((crc_P >> 8) & 0x00FF) | crc_P1);
            return (crc_P);
        }


        private TextBox terminalRxModbusSerial;
        /// <summary>
        /// Set reference to TextBox object for incoming serial modus frame (parased)
        /// </summary>
        /// <param name="terminalRxModbusSerial">TextBox reference Object</param>
        public void SetModbusTerminalReference(TextBox terminalRxModbusSerial)
        {
            this.terminalRxModbusSerial = terminalRxModbusSerial;
        }
        /// <summary>
        /// Delegate to parased Rx modbus frames (serial port)
        /// </summary>
        /// <param name="parasedModusRxFrame"></param>
        private delegate void modubsRxParaseTerminalDataDelegate(byte[] parasedModusRxFrame);
        /// <summary>
        /// Delegate reference function for incoming Rx Modbus flames (serial port)
        /// </summary>
        /// <param name="parasedModusRxFrame"></param>
        private void modubsRxParaseTerminalData(byte[] parasedModusRxFrame)
        {
            terminalRxModbusSerial.Text = BitConverter.ToString(parasedModusRxFrame);
        }
        /// <summary>
        /// Parase (check) data from serial port and get modbus structures
        /// </summary>
        /// <param name="inpSerialDataStream"> Data stream from serial port</param>
        private void MbusParaser (ref MemoryStream inpSerialDataStream)
        {
            byte[] serialBytes = MbusStream.ToArray();
            uint offset = 0;
            uint dataLen = (uint)serialBytes.Length;
            uint currentDataIdxPosition = 0;
            uint mBusPayloadLength;
            
            if (dataLen < 8)
            {
                return;
            }

            /*mBusPayloadLength = serialBytes[MBUS_DATA_LENGTH_HI];
            mBusPayloadLength = (mBusPayloadLength << 8) & 0xFF00;
            mBusPayloadLength |= serialBytes[MBUS_DATA_LENGTH_LO];*/

            uint crcIdxHi = 6;
            uint crcIdxLo = 7;
            uint sizeToCrcCalculate = 6;
            
            UInt16 readCRC = serialBytes[crcIdxHi];
            readCRC = (UInt16) (((readCRC << 8) & 0xFF00) | serialBytes[crcIdxLo]);
            UInt16 calculationCRC = GetModbusCRC(serialBytes, currentDataIdxPosition, sizeToCrcCalculate);
            //if (calculationCRC == readCRC)



            //Each modbus frame has more than 8 bytes

            inpSerialDataStream.Close();
            inpSerialDataStream = new MemoryStream();
            MbusStream.Write(serialBytes, (int)offset, (int)dataLen);
        }
        /****************** SERIAL MODBUS PARASE - END *****************************/

        /****************** SERIAL DATA - START *****************************/
        private TextBox terminalSerial;
        private MemoryStream MbusStream = new MemoryStream();
        Thread Th;
        double timeSpanSendUdpSerialRx = 0;
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
                //MbusSream.Close();
                //MbusSream = new MemoryStream();
                Th.Abort();
                double timeStamp1 = Stopwatch.GetTimestamp();

                int bytesToReadCount = mBusSerial.BytesToRead;
                byte[] readSerialBytes = new byte[bytesToReadCount];
                mBusSerial.Read(readSerialBytes, 0, bytesToReadCount);
                MbusStream.Write(readSerialBytes, 0, bytesToReadCount);

                timeSpanSendUdpSerialRx = timeStamp1 + silenceMbusComTimeMiliseconds;
                
                

                //MbusParaser(ref MbusStream);

                SendToSSM_UdpData(readSerialBytes); //ONLY TEST !!!!!!
                try
                {
                    terminalSerial.BeginInvoke(new terminalRxSerialBytesDelegate(terminalRxSerialBytes), new object[] { readSerialBytes });
                }
                catch (Exception Ex)
                {
                    
                }
            }
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
            //throw new NotImplementedException();
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
            terminalSSM_UDP.Text = BitConverter.ToString(readUdpSSM_Bytes);
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
        private void SendToSSM_UdpData(byte[] dataToSSM)
        {
            UdpClient SSM_UdpClient = new UdpClient(SSM_DestinationUDPAddress, SSM_UDP_DestinationPort);
            SSM_UdpClient.SendAsync(dataToSSM, dataToSSM.Length);
            SSM_UdpClient.Close();
        }


        //TEST ONLY !!!!!!!!!! (Bellow)

        public void TST_EmuSSM_UDP_ServerStart()
        {
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
                    SSM_UdpClient.SendAsync(readUdpSSMdata, readUdpSSMdata.Length);
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
            MbusParaser(ref MbusStream);
            //MbusStream.Close();
            //MbusStream = new MemoryStream();
            //MessageBox.Show(MbusStream.Length.ToString() + "\r\n" + BitConverter.ToString(MbusStream.ToArray()));
        }


    }
}
