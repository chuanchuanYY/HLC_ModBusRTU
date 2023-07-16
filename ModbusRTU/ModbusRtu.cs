using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace ModbusRTU
{
    public class ModbusRtu : IDisposable
    {
        private readonly SerialPort _port;

        private bool ReceivedStatus = false;

        private byte[] ReceivedData;

        public ModbusRtu(string protName = "COM1", int baudRate = 9600, Parity parity = Parity.None, int DataBit = 8, StopBits stopBits = StopBits.One)
        {
            try
            {
                _port = new SerialPort(protName, baudRate, parity, DataBit, stopBits);
                _port.Open();
                _port.DataReceived += new SerialDataReceivedEventHandler(_port_DataReceived);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ModbusRtu(ModbusRtuConfig config)
        {
            try
            {
                _port = new SerialPort(config.PortName, config.BaudRate, config.Parity, config.DataBit, config.StopBits);
                _port.Open();
                _port.DataReceived += new SerialDataReceivedEventHandler(_port_DataReceived);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int bytesToRead = _port.BytesToRead;
            ReceivedData = new byte[bytesToRead];
            _port.Read(ReceivedData, 0, bytesToRead);
            ReceivedStatus = true;
        }

        public async Task<byte[]> SendCommandAsync(byte SalavAddr, byte FCode, int StartAddr, int dataBit)
        {
            byte[] cmd = FillMessage(SalavAddr, FCode, StartAddr, dataBit);
            try
            {
                _port.Write(cmd, 0, cmd.Length);
                await Task.Run(delegate
                {
                    while (!ReceivedStatus)
                    {
                    }

                    ReceivedStatus = false;
                });
                if (verifyReciveData(FCode, ReceivedData))
                {
                    return ReceivedData;
                }

                return null;
            }
            catch (Exception ex2)
            {
                Exception ex = ex2;
                throw ex;
            }
        }

        public byte[] SendCommand(byte SalavAddr, byte FCode, int StartAddr, int dataBit)
        {
            byte[] array = FillMessage(SalavAddr, FCode, StartAddr, dataBit);
            try
            {
                _port.Write(array, 0, array.Length);
                while (!ReceivedStatus)
                {
                }

                ReceivedStatus = false;
                if (verifyReciveData(FCode, ReceivedData))
                {
                    return ReceivedData;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<byte[]> SendCommandAsync(byte SalavAddr, byte FCode, int CoilAddr, ColiStatus coliStatus)
        {
            byte[] cmd = FillMessage(SalavAddr, FCode, CoilAddr, (coliStatus == ColiStatus.ON) ? 65280 : 0);
            try
            {
                _port.Write(cmd, 0, cmd.Length);
                await Task.Run(delegate
                {
                    while (!ReceivedStatus)
                    {
                    }

                    ReceivedStatus = false;
                });
                if (verifyReciveData(FCode, ReceivedData))
                {
                    return ReceivedData;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public byte[] SendCommand(byte SalavAddr, byte FCode, int CoilAddr, ColiStatus coliStatus)
        {
            byte[] array = FillMessage(SalavAddr, FCode, CoilAddr, (coliStatus == ColiStatus.ON) ? 65280 : 0);
            try
            {
                _port.Write(array, 0, array.Length);
                while (!ReceivedStatus)
                {
                }

                ReceivedStatus = false;
                if (verifyReciveData(FCode, ReceivedData))
                {
                    return ReceivedData;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public byte[] ReadCils(byte SalavAddr, int StartAddr, int num)
        {
            return SendCommand(SalavAddr, 1, StartAddr, num);
        }

        public Task<byte[]> ReadCilsAsync(byte SalavAddr, int StartAddr, int num)
        {
            return SendCommandAsync(SalavAddr, 1, StartAddr, num);
        }

        public byte[] ReadDiscreteInputs(byte SalavAddr, int StartAddr, int num)
        {
            return SendCommand(SalavAddr, 2, StartAddr, num);
        }

        public Task<byte[]> ReadDiscreteInputsAsync(byte SalavAddr, int StartAddr, int num)
        {
            return SendCommandAsync(SalavAddr, 2, StartAddr, num);
        }

        public byte[] ReadHoldingRegisters(byte SalavAddr, int StartAddr, int num)
        {
            return SendCommand(SalavAddr, 3, StartAddr, num);
        }

        public Task<byte[]> ReadHoldingRegistersAsync(byte SalavAddr, int StartAddr, int num)
        {
            return SendCommandAsync(SalavAddr, 3, StartAddr, num);
        }

        public byte[] ReadInputRegisters(byte SalavAddr, int StartAddr, int num)
        {
            return SendCommand(SalavAddr, 4, StartAddr, num);
        }

        public Task<byte[]> ReadInputRegistersAsync(byte SalavAddr, int StartAddr, int num)
        {
            return SendCommandAsync(SalavAddr, 4, StartAddr, num);
        }

        public byte[] WriteSingleCoil(byte SalavAddr, int CoilAddr, ColiStatus status)
        {
            return SendCommand(SalavAddr, 5, CoilAddr, status);
        }

        public Task<byte[]> WriteSingleCoilAsync(byte SalavAddr, int CoilAddr, ColiStatus status)
        {
            return SendCommandAsync(SalavAddr, 5, CoilAddr, status);
        }

        public byte[] WriteSingleRegister(byte SalavAddr, int RegisterAddr, int Value)
        {
            return SendCommand(SalavAddr, 6, RegisterAddr, Value);
        }

        public Task<byte[]> WriteSingleCoilAsync(byte SalavAddr, int RegisterAddr, int Value)
        {
            return SendCommandAsync(SalavAddr, 6, RegisterAddr, Value);
        }

        public byte[] WriteMultipleCoils(byte SalavAddr, int StartAddr, int num, params byte[] CoilStatus)
        {
            try
            {
                byte b = (byte)CoilStatus.Length;
                byte[] array = FillMessage(SalavAddr, 15, StartAddr, num);
                byte[] array2 = new byte[CoilStatus.Length + 1 + array.Length];
                int num2 = 0;
                for (int i = 0; i < array2.Length - 2; i++)
                {
                    if (i < 6)
                    {
                        array2[i] = array[i];
                    }
                    else if (i == 6)
                    {
                        array2[i] = b;
                    }
                    else
                    {
                        array2[i] = CoilStatus[num2++];
                    }
                }

                byte[] array3 = CRC16(array2);
                array2[^2] = array3[0];
                array2[^1] = array3[1];
                _port.Write(array2, 0, array2.Length);
                while (!ReceivedStatus)
                {
                }

                ReceivedStatus = false;
                if (verifyReciveData(15, ReceivedData))
                {
                    return ReceivedData;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<byte[]> WriteSingleCoilAsync(byte SalavAddr, int StartAddr, int num, params byte[] CoilStatus)
        {
            try
            {
                byte byetLenth = (byte)CoilStatus.Length;
                byte[] bytes = FillMessage(SalavAddr, 15, StartAddr, num);
                byte[] cmd = new byte[CoilStatus.Length + 1 + bytes.Length];
                int j = 0;
                for (int i = 0; i < cmd.Length - 2; i++)
                {
                    if (i < 6)
                    {
                        cmd[i] = bytes[i];
                    }
                    else if (i == 6)
                    {
                        cmd[i] = byetLenth;
                    }
                    else
                    {
                        cmd[i] = CoilStatus[j++];
                    }
                }

                byte[] crc16 = CRC16(cmd);
                cmd[^2] = crc16[0];
                cmd[^1] = crc16[1];
                _port.Write(cmd, 0, cmd.Length);
                await Task.Run(delegate
                {
                    while (!ReceivedStatus)
                    {
                    }

                    ReceivedStatus = false;
                });
                if (verifyReciveData(15, ReceivedData))
                {
                    return ReceivedData;
                }

                return null;
            }
            catch (Exception ex2)
            {
                Exception ex = ex2;
                throw ex;
            }
        }

        public byte[] WriteMultipleRegisters(byte SalavAddr, int StartAddr, int num, params int[] values)
        {
            try
            {
                byte[] array = FillMessage(SalavAddr, 16, StartAddr, num);
                byte[] array2 = new byte[values.Length * 2];
                int num2 = 0;
                for (int i = 0; i < values.Length; i++)
                {
                    array2[num2++] = (byte)((values[i] - values[i] % 256) / 256);
                    array2[num2++] = (byte)(values[i] % 256);
                }

                byte[] array3 = new byte[array.Length + array2.Length + 1];
                int num3 = 0;
                for (int j = 0; j < array3.Length - 2; j++)
                {
                    if (j < 6)
                    {
                        array3[j] = array[j];
                    }
                    else if (j == 6)
                    {
                        array3[j] = (byte)array2.Length;
                    }
                    else
                    {
                        array3[j] = array2[num3++];
                    }
                }

                byte[] array4 = CRC16(array3);
                array3[^2] = array4[0];
                array3[^1] = array4[1];
                _port.Write(array3, 0, array3.Length);
                while (!ReceivedStatus)
                {
                }

                ReceivedStatus = false;
                if (verifyReciveData(16, ReceivedData))
                {
                    return ReceivedData;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<byte[]> WriteMultipleRegistersAsync(byte SalavAddr, int StartAddr, int num, params int[] values)
        {
            try
            {
                byte[] bytes = FillMessage(SalavAddr, 16, StartAddr, num);
                byte[] value = new byte[values.Length * 2];
                int l = 0;
                for (int i = 0; i < values.Length; i++)
                {
                    value[l++] = (byte)((values[i] - values[i] % 256) / 256);
                    value[l++] = (byte)(values[i] % 256);
                }

                byte[] cmd = new byte[bytes.Length + value.Length + 1];
                int k = 0;
                for (int j = 0; j < cmd.Length - 2; j++)
                {
                    if (j < 6)
                    {
                        cmd[j] = bytes[j];
                    }
                    else if (j == 6)
                    {
                        cmd[j] = (byte)value.Length;
                    }
                    else
                    {
                        cmd[j] = value[k++];
                    }
                }

                byte[] crc16 = CRC16(cmd);
                cmd[^2] = crc16[0];
                cmd[^1] = crc16[1];
                _port.Write(cmd, 0, cmd.Length);
                await Task.Run(delegate
                {
                    while (!ReceivedStatus)
                    {
                    }

                    ReceivedStatus = false;
                });
                if (verifyReciveData(16, ReceivedData))
                {
                    return ReceivedData;
                }

                return null;
            }
            catch (Exception ex2)
            {
                Exception ex = ex2;
                throw ex;
            }
        }

        private bool verifyReciveData(byte Fcode, byte[] ReciveData)
        {
            if (ReceivedData != null && ReceivedData.Length > 3 && ReceivedData[1] == Fcode)
            {
                byte[] array = CRC16(ReciveData);
                if (array[0] == ReciveData[^2] && array[1] == ReciveData[^1])
                {
                    return true;
                }
            }

            return false;
        }

        private byte[] FillMessage(byte SalavAddr, byte FCode, int StartAddr, int dataBit)
        {
            byte[] array = new byte[8]
            {
                SalavAddr,
                FCode,
                (byte)((StartAddr - StartAddr % 256) / 256),
                (byte)(StartAddr % 256),
                (byte)((dataBit - dataBit % 256) / 256),
                (byte)(dataBit % 256),
                0,
                0
            };
            byte[] array2 = CRC16(array);
            array[6] = array2[0];
            array[7] = array2[1];
            return array;
        }

        [DebuggerHidden]
        private byte[] CRC16(byte[] byteData)
        {
            byte[] array = new byte[2];
            ushort num = ushort.MaxValue;
            for (int i = 0; i < byteData.Length - 2; i++)
            {
                num = (ushort)(num ^ Convert.ToUInt16(byteData[i]));
                for (int j = 0; j < 8; j++)
                {
                    if ((num & 1) == 1)
                    {
                        num = (ushort)(num >> 1);
                        num = (ushort)(num ^ 0xA001u);
                    }
                    else
                    {
                        num = (ushort)(num >> 1);
                    }
                }
            }

            array[1] = (byte)((num & 0xFF00) >> 8);
            array[0] = (byte)(num & 0xFFu);
            return array;
        }

        public void Dispose()
        {
            try
            {
                _port.Close();
                _port.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        ~ModbusRtu()
        {
            try
            {
                _port.Close();
                _port.Dispose();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
    public enum ColiStatus
    {
        ON = 0xFF,
        OFF = 0
    }

    public class ModbusRtuConfig
    {
        public string PortName { get; set; }

        public int BaudRate { get; set; }

        public Parity Parity { get; set; }

        public int DataBit { get; set; }

        public StopBits StopBits { get; set; }

        public ModbusRtuConfig()
        {
        }

        public ModbusRtuConfig(string protName = "COM1", int baudRate = 9600, Parity parity = Parity.None, int DataBit = 8, StopBits stopBits = StopBits.One)
        {
            PortName = protName;
            BaudRate = baudRate;
            Parity = parity;
            this.DataBit = DataBit;
            StopBits = stopBits;
        }
    }
}
