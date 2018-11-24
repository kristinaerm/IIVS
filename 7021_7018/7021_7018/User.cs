using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _7021_7018
{
    public partial class User : Form
    {

        private SerialPort comport = new SerialPort();
        private bool getSpeed = false;
        private List<string> commands = new List<string>();
        private List<string> commandsWithData = new List<string>();
        private bool dataNeeded = false;
        private int countOf7018 = 0;
        private int countWithData = 4;
        private int currentWithData = 0;

        public User()
        {
            InitializeComponent();
            //18
            commands.Add("#");
            commands.Add("$2");
            commands.Add("$3");
            commands.Add("$6");
            countOf7018 = 4;
            //21
            commands.Add("$2");
            commands.Add("$5");
            commands.Add("$4");
            commands.Add("$6");
            commands.Add("$8");
            commands.Add("~4");
            commands.Add("~5");
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

            for (int i = 0; i < dataR.Length; i += 1)
                AddHistoryMessage(((char)dataR[i]).ToString());

            AddHistoryMessage("\n");
            comport.DiscardInBuffer();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string command = "";
            for (int i=0; i<commands.Count; i++)
            {
                try
                {
                    command += commands[i][0];
                    if (i < countOf7018)
                    {
                        command += textBox1.Text;
                    }
                    else
                    {
                        command += textBox2.Text;
                    }
                    for (int j=3; j<commands[i].Length; j++)
                    {
                        command += commands[i][j];
                    }
                    // записать команду в COM-порт (символ окончания команды – 0x0D)
                    comport.WriteLine(command + (char)0x0D);
                    // выдать сообщение в историю
                    AddHistoryMessage("Записана команда:" + command + "\n");
                }
                catch
                {
                    AddHistoryMessage("Проблемы с записью команды. \n");
                }
                command = "";
            }
            MessageBox.Show("Введите данные для команды считывания значения с канала N аналогового входа");
        }

        private void comboBoxPort_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
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

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string command = "";
            if (currentWithData < countWithData)
            {
                switch (currentWithData)
                {
                    case 0:
                        command += "#";
                        command += textBox1.Text;
                        command += textBox3.Text;
                        try
                        {
                            // записать команду в COM-порт (символ окончания команды – 0x0D)
                            comport.WriteLine(command + (char)0x0D);
                            // выдать сообщение в историю
                            AddHistoryMessage("Записана команда:" + command + "\n");
                            MessageBox.Show("Введите данные для включения определенных каналов аналогового ввода!");
                        }
                        catch
                        {
                            AddHistoryMessage("Проблемы с записью команды. \n");
                        }
                        currentWithData++;
                        break;
                    case 1:
                        command += "$";
                        command += textBox1.Text;
                        command += "5"+textBox3.Text;
                        try
                        {
                            // записать команду в COM-порт (символ окончания команды – 0x0D)
                            comport.WriteLine(command + (char)0x0D);
                            // выдать сообщение в историю
                            AddHistoryMessage("Записана команда:" + command + "\n");
                            MessageBox.Show("Введите данные для задания величины смещения для схемы компенсации холодного спая!");
                        }
                        catch
                        {
                            AddHistoryMessage("Проблемы с записью команды. \n");
                        }
                        currentWithData++;
                        break;
                    case 2:
                        command += "$";
                        command += textBox1.Text;
                        command += "9" + textBox3.Text;
                        try
                        {
                            // записать команду в COM-порт (символ окончания команды – 0x0D)
                            comport.WriteLine(command + (char)0x0D);
                            // выдать сообщение в историю
                            AddHistoryMessage("Записана команда:" + command + "\n");
                            MessageBox.Show("Введите данные для задания значения, устанавливаемого на аналоговом выводе модуля!");
                        }
                        catch
                        {
                            AddHistoryMessage("Проблемы с записью команды. \n");
                        }
                        currentWithData++;
                        break;
                    case 3:
                        command += "#";
                        command += textBox2.Text;
                        command += textBox3.Text;
                        try
                        {
                            // записать команду в COM-порт (символ окончания команды – 0x0D)
                            comport.WriteLine(command + (char)0x0D);
                            // выдать сообщение в историю
                            AddHistoryMessage("Записана команда:" + command + "\n");
                        }
                        catch
                        {
                            AddHistoryMessage("Проблемы с записью команды. \n");
                        }
                        currentWithData++;
                        break;
                }
            }
            else
            {
                MessageBox.Show("Тестирование завершено!");
            }
        }
    }
}
