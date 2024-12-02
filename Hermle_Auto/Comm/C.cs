using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HermleCS.Comm
{
    internal class C
    {
        public const int ERRNO_SUCCESS = 0;
        public const int ERRNO_FAILED = -1;

        public const int ERRNO_CONNECT = 200;

        public const int ERRNO_EXCEPTION = 500;

        // PLC Information
        public const string PLC_IP = "192.168.50.100";
        public const int PLC_PORT = 5100;

        // ROBOT Information
        public const string ROBOT_SERVER = "http://192.168.0.10/KAREL";

        // DRILL CONSTANT
        public const int DRILL_LOCATION_SHELF_COUNT = 3;
        public const int DRILL_LOCATION_COLUMN_COUNT = 12;
        public const int DRILL_LOCATION_POCKET_COUNT = 7;
        public const int DRILL_GENLOCATION_COUNT = 127;
        public const int DRILL_STATUS_SHELF_COUNT = 3;
        public const int DRILL_STATUS_COLUMN_COUNT = 1;
        public const int DRILL_STATUS_POCKET_COUNT = 12;

        // HSK CONSTANT
        public const int HSK_LOCATION_SHELF_COUNT = 3;
        public const int HSK_LOCATION_COLUMN_COUNT = 2;
        public const int HSK_LOCATION_POCKET_COUNT = 5;
        public const int HSK_GENLOCATION_COUNT = 137;
        public const int HSK_STATUS_SHELF_COUNT = 3;
        public const int HSK_STATUS_COLUMN_COUNT = 1;
        public const int HSK_STATUS_POCKET_COUNT = 10;

        // ROUND CONSTANT
        public const int ROUND_LOCATION_SHELF_COUNT = 3;
        public const int ROUND_LOCATION_COLUMN_COUNT = 12;
        public const int ROUND_LOCATION_POCKET_COUNT = 8;
        public const int ROUND_GENLOCATION_COUNT = 40;
        public const int ROUND_STATUS_SHELF_COUNT = 3;
        public const int ROUND_STATUS_COLUMN_COUNT = 1;
        public const int ROUND_STATUS_POCKET_COUNT = 12;


        public const int WORKPIECE_COUNT = 50;

        public static string ApplicationPath = "C:\\Users\\박병석\\source\\repos\\HermleCS";

        public static void log(string msg)
        {
            Console.WriteLine(DateTime.Now + " : " + msg);
        }
    }
}
