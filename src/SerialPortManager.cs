using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UB482
{
    class SerialPortManager
    {
        public ComboBox comboBox { get; set; }
        public TextBox[] textBoxes { get; set; }
        private SerialPort _serialPort;

        public bool isEnabled = false;
        public string readBuffer;

        public int receivedPacket;

        public SerialPortManager(SerialPort serialPort)
        {
            _serialPort = serialPort;
        }

        public string[] CheckSerialPort()
        {
            return SerialPort.GetPortNames();
        }

        public void OpenConnection(int baudRate)
        {
            isEnabled = true;
            _serialPort.BaudRate = baudRate;
            _serialPort.PortName = comboBox.Text;
            _serialPort.Open();
        }

        public void CloseConnection()
        {
            isEnabled = false;
            _serialPort.Close();
        }

        public void SplitBuffer(Data data)
        {
            string[] dataWithHeader = readBuffer.Split(';');
            Data.header = dataWithHeader[0];

            string[] splittedData = dataWithHeader[1].Split(',');

            int i = 0;

            foreach (var rawData in splittedData)
            {
                data.datas[i] = splittedData[i];

                i++;
            }
        }

        public async void ViewDataAsync(Data data)
        {
            await Task.Run(() =>
            {
                int i;
                for (i = 0; i < 49; i++)
                {
                    if (i < 41)
                    {
                        textBoxes[i].Text = data.datas[i];
                        data.writer.Write(data.datas[i] + ",");
                    }
                    else if (i < 48) //7 tane gidicez
                    {

                        textBoxes[i].Text = data.datas[i + 2];
                        data.writer.Write(data.datas[i + 2] + ",");
                    }
                    else // 2 tane atladik 1 tane aldik
                    {
                        textBoxes[i].Text = data.datas[i + 5];
                        data.writer.Write(data.datas[i + 5] + ",");
                    }
                }
                data.writer.WriteLine();
            });

        }
        public async void LogDataAsync(Data data)
        {
            await Task.Run(() =>
            {
                //    string[] datas = new string[]
                //{
                //data.gnss, length, year, month,
                //day, minute, second, rtkStatus, headingStatus,
                //numGpsStatus, numGloStatus, numBdsStatus, baselineN,
                //baselineE, baselineU, baselineNStd, baselineEsStd, baselineUStd,
                //heading, gpsPitch, gpsRoll, gpsSpeed, velN, velE, velUP, xigVx,
                //xigVy, xigVz, latitude, longitude, roverHei, ecefX, ecefY, ecefZ,
                //xigLat, xigLon, xigAlt, xigEcefX, xigEcefY, xigEcefZ, baseLat, baseLon,
                //baseAlt, secLat, secLon, secAlt, gpsWeekSecond, diffage, speedHeading,
                //undulation, remainFloat3, remainFloat4, numGalStatus, remainChar2,
                //remainChar3, remainChar4, crc
                //};
                foreach (var singleData in data.datas)
            {
                data.writer.Write(singleData + ",");
                
            }
            data.writer.WriteLine();
            });
        }
    }
}
