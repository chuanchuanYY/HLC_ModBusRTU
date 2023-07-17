using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MdbusRTU.Framework4._7._2
{
    public static class ModbusExtension
    {
        /// <summary>
        /// 提取读线圈返回的报文数据
        /// </summary>
        /// <param name="modbus"></param>
        /// <param name="bytes"></param>
        /// <returns><see langword="List<bool>"/> 下标0为起始地址线圈 </returns>
        public static List<bool> BytesOfReadColisToBoolList(this ModbusRtu modbus, byte[] bytes)
        {
            List<bool> coilsStatas = new List<bool>();
            if (bytes != null && bytes.Length >= 5)
            {
                if (bytes[1] == (byte)0x01)
                {
                    for (int i = 0; i < Convert.ToInt32(bytes[2]); i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            coilsStatas.Add((byte)((byte)((byte)(bytes[i + 3] >> j) << 7) >> 7) == (byte)0x01 ? true : false);
                        }
                    }
                }
            }
            return coilsStatas;
        }
        /// <summary>
        /// 提取读寄存器返回的报文数据
        /// </summary>
        /// <param name="modbus"></param>
        /// <param name="bytes"></param>
        /// <returns><see langword="int[]"/>从0开始，每一项为一个寄存器的值
        /// error return null</returns>
        public static int[] BytesOfReadKeepRegisterToIntArr(this ModbusRtu modbus, byte[] bytes)
        {
            if (bytes is null || bytes.Length < 3) return null;
            int dataLenth = (int)bytes[2];
            int[] result = new int[dataLenth / 2];
            int databitStart = 3;
            int RegisterLen = 2;
            int j = 0;
            try
            {
                for (int i = databitStart; i < dataLenth * 2; i += RegisterLen)
                {
                    if (i >= bytes.Length) break;
                    result[j] = bytes[i];
                    result[j] <<= 8;
                    result[j] |= bytes[i + 1];
                    j++;
                }
            }
            catch (Exception)
            {

            }

            return result;
        }
    }
}
