using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hermle_Auto.Data
{
    public enum ROBOT_STATE_ENUM
    {
        AUTO_MODE           = 0x01,
        MANUAL_MODE         = 0x02,
        PGM_RUNNING         = 0x04,
        PGM_PAUSED          = 0x08,
        MOTION_HELD         = 0x10,
        FAULT               = 0x20,
        PEACH               = 0x40,
        TP_ENABLED          = 0x80,
        BUSY                = 0x0100,
        E_STOP              = 0x0200,
        E_STOP_REVERSE      = 0x0400,
    }
    public class Robot
    {
    }
}
