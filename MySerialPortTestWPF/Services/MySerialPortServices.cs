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
