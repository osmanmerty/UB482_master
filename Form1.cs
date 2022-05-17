using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UB482
{
    public partial class Form1 : Form
    {

        #region .. Double Buffered function ..
        public static void SetDoubleBuffered(System.Windows.Forms.Control c)
        {
            if (System.Windows.Forms.SystemInformation.TerminalServerSession)
                return;
            System.Reflection.PropertyInfo aProp = typeof(System.Windows.Forms.Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            aProp.SetValue(c, true, null);
        }
        #endregion

        //object defining
        SerialPortManager serialPortManager;
        SerialPortManager serialPortManager1;
        Data data;
        public TextBox[] textBoxes;

        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            //object instantiating
            serialPortManager = new SerialPortManager(serialPort1);
            serialPortManager1 = new SerialPortManager(serialPort2);
            data = new Data();

            textBoxes = new TextBox[]
            {
                //no header first byte
                gnssTextBox, msgLenTextBox, yearTextBox, monthTextBox,
                dayTextBox, hourTextBox, minTextBox, secTextBox, rtkStatTextBox,
                headingStatTextBox, gpsStatTextBox, gloStatTextBox,
                bdsStatTextBox, baselineNTextBox, baselineETextBox,
                baselineUTextBox, baselineNStdTextBox, baselineEStdTextBox,
                baselineUStdTextBox, headingTextBox, gpsPitchTextBox, gpsRollTextBox,
                gpsSpeedTextBox, velNTextBox, velETextBox, velUpTextBox, 
                xigVxTextBox, xigVyTextBox, xigVzTextBox, latTextBox, lonTextBox,
                roverHeiTextBox, ecefXTextBox, ecefYTextBox, ecefZTextBox, xigLatTextBox,
                xigLonTextBox, xigAltTextBox, xigEcefXTextBox, xigEcefYTextBox,
                xigEcefZTextBox, /*baseline konumu yok*/secLatTextBox, secLonTextBox,
                secAltTextBox, gpsWeekSecTextBox, diffageTextBox, speedHeadingTextBox, 
                undulationTextBox, /*remain float yok*/galStatTextBox
            };

            serialPortManager.textBoxes = textBoxes;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(serialPortManager.CheckSerialPort());
            comboBox2.Items.AddRange(serialPortManager1.CheckSerialPort());
            try
            {
                comboBox1.SelectedIndex = 0;
                comboBox2.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            foreach (var textBox in textBoxes)
            {
                SetDoubleBuffered(textBox);
            }
        }
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPortManager.readBuffer = serialPort1.ReadLine();
                rawByteMonitor.Text += serialPortManager.readBuffer + "\n";
                serialPortManager.SplitBuffer(data);
                serialPortManager.ViewDataAsync(data);
                serialPortManager.receivedPacket++;
                //serialPortManager.LogDataAsync(data);
            }
        }

        private void serialPort2_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            if (serialPort2.IsOpen)
            {
                serialPort1.Write(serialPort2.ReadLine());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (!serialPortManager.isEnabled)
            {
                serialPortManager.comboBox = comboBox1;
                serialPortManager.OpenConnection(230400);
                timer1.Start();
            }
            else
            {
                serialPortManager.CloseConnection();
                comboBox1.Items.AddRange(serialPortManager.CheckSerialPort());
                timer1.Stop();
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!serialPortManager1.isEnabled)
            {
                serialPortManager1.comboBox = comboBox2;
                serialPortManager1.OpenConnection(9600);
            }
            else
            {
                serialPortManager1.CloseConnection();
                comboBox2.Items.AddRange(serialPortManager1.CheckSerialPort());
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timeIntervalTextBox.Text = serialPortManager.receivedPacket.ToString();
            serialPortManager.receivedPacket = 0;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            data.CloseFile();
        }

        private void label58_Click(object sender, EventArgs e)
        {

        }

        private void label3169_Click(object sender, EventArgs e)
        {

        }
    }
}
