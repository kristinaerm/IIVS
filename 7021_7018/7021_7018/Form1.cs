using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace _7021_7018
{
    public partial class Form1 : Form
    {
        private SerialPort comport = new SerialPort();
        private bool GetSpeed = false;
        private bool getAnswer = false;
        private bool needAnswer = false;

        public Form1()
        {
            InitializeComponent();
            comport.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            this.Invoke(new EventHandler(AddRecieve));
        }

        private void AddRecieve(object s, EventArgs e)
        {
            // задержка
            System.Threading.Thread.Sleep(100);
            // буфер для чтения данных из COM-порта
            byte[] dataR = new byte[comport.BytesToRead];
            // прочитать данные
            comport.Read(dataR, 0, dataR.Length);
            // добавить ответ в историю команд
            AddHistoryMessage("Получен ответ:");
            if (GetSpeed)
                for (int i = 0; i < dataR.Length; i += 1)
                    textBoxAnswer.AppendText(((char)dataR[i]).ToString());
            else
                for (int i = 0; i < dataR.Length; i += 1)
                    AddHistoryMessage(((char)dataR[i]).ToString());

            AddHistoryMessage("\n");
            comport.DiscardInBuffer();
            needAnswer = false;
            getAnswer = true;
        }

        private void AddHistoryMessage(string msg)
        {
            // добавить сообщение в историю выполненных команд
            richTextBoxHistory.Invoke(
            new EventHandler(delegate
            {
                // добавить текст в историю
                richTextBoxHistory.AppendText(msg);
                // в списке истории выделить последнюю запись
                richTextBoxHistory.ScrollToCaret();
            }
            )
            );
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void comboBoxPort_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // записать команду в COM-порт (символ окончания команды – 0x0D)
                comport.WriteLine(textBoxCommand.Text + (char)0x0D);
                // выдать сообщение в историю
                AddHistoryMessage("Записана команда:" + textBoxCommand.Text + "\n");
                needAnswer = true;
                getAnswer = false;

            }
            catch
            {
                AddHistoryMessage("Проблемы с записью команды. \n");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comport.IsOpen) comport.Close();
            else
            {
                // порт ранее открыт не был
                // название COM-порта
                // comport.PortName = "COM" ;
                comport.PortName = comboBoxPort.Text;
                // скорость работы COM-порта
                // int baudRate = 9600;
                // int.TryParse(textBoxSpeed.Text, out baudRate);
                // comport.BaudRate = baudRate;
                comport.BaudRate = Convert.ToInt32(comboBoxSpeed.Text);
                // число бит данных
                comport.DataBits = 8;
                // число стоповых бит - один
                comport.StopBits = StopBits.One;
                // бит паритета - нет
                comport.Parity = Parity.None;
                // квитировать установление связи - нет
                comport.Handshake = Handshake.None;
                // число принимаемых бит
                comport.ReceivedBytesThreshold = 8;
                // размер буфера для записи
                comport.WriteBufferSize = 20;
                // размер буфера для чтения
                comport.ReadBufferSize = 20;
                // время таймаута чтения - по умолчанию
                comport.ReadTimeout = -1;
                // время таймаута записи - по умолчанию
                comport.WriteTimeout = -1;
                // сигнал готовности терминала к передаче данных - не установлен
                comport.DtrEnable = false;
                // открыть порт
                try
                {
                    comport.Open();
                    // запрос передатчика - установлен
                    comport.RtsEnable = true;
                    // задержка
                    System.Threading.Thread.Sleep(100);
                    AddHistoryMessage("Порт " + comboBoxPort.Text + " открыт. \n");
                }
                catch
                {
                    AddHistoryMessage("Проблемы с открытием порта. \n");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                // освободить выходной буфер
                comport.DiscardOutBuffer();
                // освободить входной буфер
                comport.DiscardInBuffer();
                // закрыть порт
                if (comport.IsOpen) comport.Close();
                AddHistoryMessage("Порт закрыт. \n");
            }
            catch
            {
                AddHistoryMessage("Проблемы с закрытием порта. \n");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //int a = (int)Double.Parse(textBox1.Text) * 100;
            //string s = Convert.ToString(a, 8);
        }

        private void comboBoxPort_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void comboBoxSpeed_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (needAnswer && !getAnswer)
            {
                MessageBox.Show("Введена неверная команда, ответ не получен");
            }
        }
    }
}
