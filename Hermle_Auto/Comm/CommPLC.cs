using HermleCS.Comm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using McProtocol;
using McProtocol.Mitsubishi;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows;
using System.Threading;


namespace Hermle_Auto.Comm
{
    public class CommPLC
    {
        public McProtocolTcp mcProtocolTcp;

        private static readonly CommPLC instance = new CommPLC();

        private CommPLC()
        {
        }

        public static CommPLC Instance
        {
            get
            {
                return instance;
            }
        }

        public async Task WritePLC(PlcDeviceType type, int addr, int value)
        {
            int[] addrs = new int[1];
            int[] values = new int[1];

            addrs[0] = addr;
            values[0] = value;

            try
            {
                // 2024/12/03 flagmoon
                if (mcProtocolTcp.Connected == false)
                {
                    return;
                }
                //---
                
                await mcProtocolTcp.SetBitDevice(type, addrs[0], 1, values);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("eee : PLC 연결 실패");
                Console.WriteLine(ex.Message);
                Console.WriteLine(mcProtocolTcp.ToString());
                return;
            }
        }

        
        public async Task WritePLCBlock(int addr, int value)
        {
            int[] addrs = new int[1];
            int[] values;

            addrs[0] = addr;
            values = ConvertIntToShort(value);

            try
            {
                // 2024/12/03 flagmoon
                if (mcProtocolTcp.Connected == false)
                {
                    return;
                }
                //---

                await mcProtocolTcp.WriteDeviceBlock(McProtocol.Mitsubishi.PlcDeviceType.D, addrs[0], 2, values);
            }
            catch (Exception ex)
            {
                //MessageBox.Show("eee : PLC 연결 실패");
                Console.WriteLine(ex.Message);
                Console.WriteLine(mcProtocolTcp.ToString());
                return;
            }
        }

        public async void Set (string addr)
        {
            try
            {
                int[]   data    = new int[1];
                data[0]         = 1;
                // 2024/12/03 flagmoon
                if (mcProtocolTcp.Connected == false)
                {
                    return;
                }

                await mcProtocolTcp.SetBitDevice (addr, data.Length, data);
                //---
                
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString ());
            }
        }

        public async void Clear (string addr)
        {
            try
            {
                int[]   data    = new int[1];
                data[0]         = 0;
                // 2024/12/03 flagmoon
                if (mcProtocolTcp.Connected == false)
                {
                    return;
                }

                await mcProtocolTcp.SetBitDevice (addr, data.Length, data);
                //---

            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.ToString ());
            }

        }

        public int[] ConvertIntToShort(int f)
        {
            int[] data = new int[2];
            byte[] cvt = new byte[4];
            byte[] ordData = new byte[2];

            try
            {
                cvt = BitConverter.GetBytes(f);

                ordData[0] = cvt[0];
                ordData[1] = cvt[1];

                data[0] = BitConverter.ToInt16(ordData, 0);

                ordData[0] = cvt[2];
                ordData[1] = cvt[3];

                data[1] = BitConverter.ToInt16(ordData, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return data;
        }

    }
}
