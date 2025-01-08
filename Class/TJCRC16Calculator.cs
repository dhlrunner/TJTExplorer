using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJTExplorer.Class
{
    internal class TJCRC16Calculator
    {
        //Thanks to Shikyu for providing code
        public short CRC { get; private set; }

        private readonly short[] crcTable = new short[256];
        private readonly short cnCrc16 = -32763;

        public TJCRC16Calculator() 
        {
            //Init CRC Table
            int m17m0;
            for (short i = 0; GetUnder16Bit(i) < 256; i = (short)(i + 1))
            {
                short data = (short)(GetUnder16Bit(i) << 8);
                short accum = 0;
                for (short j = 0; GetUnder16Bit(j) < 8; j = (short)(j + 1))
                {
                    if (((GetUnder16Bit(data) ^ GetUnder16Bit(accum)) & 32768) != 0)
                    {
                        m17m0 = (GetUnder16Bit(accum) << 1) ^ GetUnder16Bit(cnCrc16);
                    }
                    else
                    {
                        m17m0 = GetUnder16Bit(accum) << 1;
                    }
                    accum = (short)m17m0;
                    data = (short)(GetUnder16Bit(data) << 1);
                }
                crcTable[GetUnder16Bit(i)] = accum;
            }
        }

        public void Append(byte[] data)
        {
            short size = (short)data.Length;

            int dataIndex = 0;
            short accum = 0;
            short i = 0;
            while (GetUnder16Bit(i) < GetUnder16Bit(size))
            {
                accum = (short)(GetUnder16Bit(crcTable[Mask8(data[dataIndex]) ^ (GetUnder16Bit(accum) >> 8)]) ^ (GetUnder16Bit(accum) << 8));
                i = (short)(i + 1);
                dataIndex++;
            }
            this.CRC = accum;
        }

        //Private
        private int GetUnder16Bit(short s)
        {
            return s & ushort.MaxValue;
        }

        private int Mask8(byte b)
        {
            return b & byte.MaxValue;
        }
    }
}
