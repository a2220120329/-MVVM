using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MySerialPortTestWPF
{
    public delegate void RxDataProcessedHandler();
    public class MySerialPortViewModel : BaseViewModel
    {
        #region Private Members
        Window mWindow;
        TextBox mRxTextBox;
        MySerialPort PortModel;
        #endregion

        #region events

        #endregion

        #region Public Properties

        #region Boolean Properties
        //接收16进制
        public bool IsRxAsciiChecked { get; set; } = true;
        //自送换行
        public bool IsRxAutoReturn { get; set; } = false;
        public bool IsRxShowCurrTime { get; set; } = false;
        //设置是否重复发送
        public bool IsSetTimer { get; set; } = false;
        public int RepeatTime { set; get; } = 1000;
        //串口打开ToggleButton标志
        public bool IsOpenBtnChecked { get; set; } = false;
        // 是否滚动至最新行
        public Boolean IsScrollToEnd { get; set; }
        
        #endregion

        #region TxData
        //发送数据
        public string TxData { get; set; }
        public long SendNums { get; set; } = 0;
        //定时发送状态
        public string TxDataState { get; set; } = "已停止";
        #endregion

        #region RxData
        public StringBuilder Rx_Sb { get; set; }
        public String Rx_Str { get; set; }
        private long Rx_count = 0;
        #endregion

        #region SerialNums
        public ObservableCollection<string> SerialPortNums { get; set; }
        public string SelectedPortNum { get; set; }
        #endregion

        #region SerialPortBaud
        public List<string> SerialPortBaud { get; set; }
        public string SelectedPortBaud { get; set; }
        #endregion

        #region SerialPortStopBits
        public List<string> SerialPortStopBits { get; set; }
        public string SelectedPortStopBit { get; set; }
        public Dictionary<string, StopBits> StopBitsDic { set; get; }
        #endregion

        #region SerialPortDataBit
        public List<int> SerialPortDataBits { get; set; }
        public int SelectedPortDataBit { get; set; }
        #endregion

        #region SerialPortParities
        public List<string> SerialPortParities { get; set; }
        public string SelectedPortParity { get; set; }
        public Dictionary<string, Parity> PortParityDic { get; set; }
        #endregion

        #endregion

        #region Commands
        /// <summary>
        /// When the serialport comobox is been clicked update the serialport numbers.
        /// </summary>
        public RelayCommand UpdateSerialPortNames { get; set; }

        public RelayCommand OpenPortCommand { get; set; }

        public RelayCommand SendTxDataCommand { get; set; }

        public RelayCommand ClearRxDataCommand { get; set; }
        #endregion

        #region Ctor
        /// <summary>
        /// Init the necessary items.
        /// </summary>
        /// <param name="window"></param>
        public MySerialPortViewModel(Window window,ref TextBox textBlock)
        {


            //Init the private members
            #region init the private members
            PortModel = new MySerialPort();
            MySerialPortServices services = new MySerialPortServices(ref PortModel);
            mWindow = window;
            mRxTextBox = textBlock;
            #endregion

            //init the public properties
            #region single properties
            //发送数据
            TxData = "";
            //接收数据处理缓存
            Rx_Sb = new StringBuilder();
            //接收数据
            Rx_Str = "";
            //是否滚动至最新
            IsScrollToEnd = true;

            #endregion

            #region PortNums
            SerialPortNums = new ObservableCollection<string>();
            if (SerialPort.GetPortNames().Length != 0)
                SelectedPortNum = SerialPort.GetPortNames()[0];
            else
            {
                //扫描有效串口  
                Task task = new Task(() => {
                    Boolean result = false;
                    for (int i = 0; i < 5; i++)
                    {
                        Thread.Sleep(1000);
                        if (SerialPort.GetPortNames().Length != 0)
                        {
                            SelectedPortNum = SerialPort.GetPortNames()[0];
                            result = true;
                            break;
                        }
                    }
                    if (result == false)
                        MessageBox.Show("未扫描到有效串口请检查串口连接或驱动。");
                });


            }
            #endregion

            #region init the Baud
            SerialPortBaud = new List<string> { "1200","2400","4800","9600","14400","19200","38400",
                                                "43000","57600","76800","115200","128000","230400",
                                                 "256000","460800","921600","1382400"};
            SelectedPortBaud = "115200";
            #endregion

            #region init the StopBits 
            SerialPortStopBits = new List<string> { "0", "1", "1.5", "2" };
            SelectedPortStopBit = "1";
            StopBitsDic = new Dictionary<string, StopBits>();
            StopBitsDic.Add("0", StopBits.None);
            StopBitsDic.Add("1", StopBits.One);
            StopBitsDic.Add("2", StopBits.Two);
            StopBitsDic.Add("1.5", StopBits.OnePointFive);
            #endregion

            #region init the DataBit
            SerialPortDataBits = new List<int> { 8, 7, 6, 5 };
            SelectedPortDataBit = 8;
            #endregion

            #region init the Parity
            SerialPortParities = new List<string> { "无", "奇校验", "偶校验", "校验位总为0", "校验位总为1" };
            SelectedPortParity = "无";
            PortParityDic = new Dictionary<string, Parity>();
            PortParityDic.Add("无", Parity.None);
            PortParityDic.Add("奇校验", Parity.Odd);
            PortParityDic.Add("偶校验", Parity.Even);
            PortParityDic.Add("校验位总为0", Parity.Space);
            PortParityDic.Add("校验位总为1", Parity.Mark);
            #endregion

            //Init the commands
            #region UpdateSerialPortNames Commands
            /// <summary>
            /// When the serialport comobox is been clicked update the serialport numbers.
            /// </summary>
            UpdateSerialPortNames = new RelayCommand(() => {
                //若串口被更改
                if (SerialPort.GetPortNames().ToString() != SerialPortNums.ToString())
                {
                    SerialPortNums.Clear();//清空串口数据
                    string[] NewSerialPortNums = SerialPort.GetPortNames();
                    for (int i = 0; i < NewSerialPortNums.Length; i++)
                    {
                        SerialPortNums.Add(NewSerialPortNums[i]);
                    }
                }

            });
            #endregion

            #region ClearRxDataCommand
            ClearRxDataCommand = new RelayCommand(async()=> {
                await Task.Run(()=> {
                    mWindow.Dispatcher.Invoke(()=> {
                        mRxTextBox.Text = "";
                    });
                });
            });
            #endregion

            #region SendTxDataCommand
            SendTxDataCommand = new RelayCommand(async () => {
                await Task.Run(async() =>
                {
                    do
                    {
                       SendNums += TxData.Length;
                        try
                        {
                            PortModel.SP.Write(TxData);
                        }
                        catch(Exception e)
                        {
                            MessageBox.Show("串口已关闭");
                        }
                        TxDataState = "运行中";
                        await Task.Delay(RepeatTime);
                    } while (IsSetTimer&&PortModel.SP.IsOpen);
                    TxDataState = "已停止";
                });
            });
            #endregion

            #region OpenPortCommand Commands
            /// <summary>
            /// When the openport button is clicked close or open the port
            /// </summary>
            OpenPortCommand = new RelayCommand(() => {
                if (!PortModel.SP.IsOpen)
                {
                    

                    //set the serialport basic properties
                    if (SelectedPortNum == null || SelectedPortNum == "")
                    {
                        IsOpenBtnChecked = false;
                        MessageBox.Show("串口打开失败，打开串口前请选择有效串口。");
                        return;
                    }
                    PortModel.SP.PortName = SelectedPortNum;
                    PortModel.SP.BaudRate = Convert.ToInt32(SelectedPortBaud);
                    PortModel.SP.StopBits = StopBitsDic[SelectedPortStopBit];
                    PortModel.SP.DataBits = SelectedPortDataBit;
                    PortModel.SP.Parity = PortParityDic[SelectedPortParity];
                    PortModel.SP.NewLine = "\r\n";
                    PortModel.SP.Encoding = System.Text.Encoding.GetEncoding("GB2312");

                    //打开串口
                    if (services.OpenPort(ref PortModel.SP) == true)
                    {//打开串口成功

                     //绑定接收数据事件
                        PortModel.SP.DataReceived += DataReceivedCallBack;
                        //开启数据处理线程
                        Task.Run(async () => {
                            while (PortModel.SP.IsOpen)
                            {
                                int count = PortModel.RxQue.Count;
                                StringBuilder temp_strb = new StringBuilder();
                                if (count > 0)
                                {
                                    //取出当前队列中数据
                                    for (int i = 0; i < count; i++)
                                    {
                                        temp_strb.Append(PortModel.RxQue.Dequeue());
                                    }
                                    mWindow.Dispatcher.Invoke(() => {
                                        mRxTextBox.AppendText(temp_strb.ToString());//ui更新数据
                                        if(IsScrollToEnd)
                                        mRxTextBox.ScrollToEnd();
                                    });
                                }
                                await Task.Delay(500);
                            }
                        });
                        IsOpenBtnChecked = true;
                    }
                    else
                    {
                        IsOpenBtnChecked = false;
                        MessageBox.Show("串口打开失败，请检查配置或连线是否正常。");
                    }
                }
                else
                {
                    MySerialPortServices.ClosePort(ref PortModel.SP);
                }

            });
            #endregion

            #region SerialPort Open Template Code
            //串口初始化项目
            //初始化，datarecived事件
            //PortModel.SP.ReceivedBytesThreshold = 1;
            //PortModel.SP.NewLine = "\r\n";
            //PortModel.SP.BaudRate = 115200;
            //PortModel.SP.StopBits = StopBits.One;
            //PortModel.SP.DataBits = 8;
            //PortModel.SP.Parity = Parity.None;
            //PortModel.SP.Encoding = System.Text.Encoding.GetEncoding("GB2312");
            #endregion

        }
        #endregion
        private void DataReceivedCallBack(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {

                SerialPort port = sender as SerialPort;
                byte[] DataBuff = new byte[port.BytesToRead];
                port.Read(DataBuff, 0, port.BytesToRead);
                string temp = "";
                if(IsRxAsciiChecked)
                {
                    temp=Encoding.Default.GetString(DataBuff);
                }
                else
                {
                    foreach(byte b in DataBuff)
                    {
                        Rx_Sb.Append(b.ToString("X2") + " ");
                    }
                    temp = Rx_Sb.ToString();
                }
                //显示时间戳
                if (IsRxShowCurrTime) temp += DateTime.Now.Hour.ToString() +":"+ DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString() + ":" + DateTime.Now.Millisecond.ToString();
                //自动回车
                if (IsRxAutoReturn) temp += "\r";
                PortModel.RxQue.Enqueue(temp);
                Rx_Sb.Clear();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("error:接受信息异常" + ex.ToString());
            }
        }
    }

        
    
}
