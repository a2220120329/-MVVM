using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MySerialPortTestWPF
{
    public delegate void RxDataHandler();
    public  class MySerialPortServices
    {
        #region Private Members
        private   MySerialPort mPort;
        #endregion

        #region Public Properties

        public event RxDataHandler RxDataEvent;
        #endregion

        #region public Methods

        #region OpenPort
        /// <summary>
        /// Open the SerialPort
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public bool OpenPort(ref SerialPort sp)
        {
            if (sp.IsOpen) return false;
            else
            {
                try
                {
                    sp.Open();
                    sp.DataReceived += new SerialDataReceivedEventHandler(SerialPortDataRecived);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        #endregion

        #region ClosePort
        /// <summary>
        /// Close the SerialPort
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public static bool ClosePort(ref SerialPort sp)
        {
            if (!sp.IsOpen) return true;
            else
            {
                try
                {
                    sp.Close();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        #endregion

        #region recive data
        /// <summary>
        /// Recive the data from serialport callback
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="temp"></param>
        private  void SerialPortDataRecived(object sender, SerialDataReceivedEventArgs temp)
        {
            try
            {
                SerialPort port = sender as SerialPort;
                byte[] DataBuff = new byte[port.BytesToRead];
                port.Read(DataBuff, 0, port.BytesToRead);
                mPort.RxDataBuffer = Encoding.Default.GetString(DataBuff);
                RxDataEvent();
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine("error:接受信息异常" + ex.ToString());
            }
        }
        #endregion

        #endregion


        #region Ctor
        //ctor
        public MySerialPortServices(ref MySerialPort port)
        {
            mPort = port;
        }

        #endregion

    }
}
