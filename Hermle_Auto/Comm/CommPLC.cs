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
                await mcProtocolTcp.SetBitDevice(type, addrs[0], 1, values);
            }
            catch
            {
                MessageBox.Show("eee : PLC 연결 실패");
                return;
            }
        }

    }


}
