using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySerialPortTestWPF
{
 public   class MySerialPort:BaseViewModel
    {
        #region Public Properties
        /// <summary>
        /// The object of the serialport
        /// </summary>
        public SerialPort SP;
        public string RxDataBuffer;
        #endregion
        #region Ctor
        public MySerialPort()
        {
            SP = new SerialPort();
        }
        #endregion
    }
}
