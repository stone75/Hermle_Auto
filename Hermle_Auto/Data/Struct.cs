using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

using HermleCS.Comm;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace HermleCS.Data
{
    public struct Locations
    {
        public String name;
        public double x, y, z, rx, ry, rz, dist, alfa;
    }

    public struct GeneralLocations
    {
        public String name;
        public double x, y, z, rx, ry, rz;
    }

    public struct Status
    {
        public String name;
        public int shelf, column, pocket, diameter, currenttool, status, workpiece, programnumber;
    }


    public enum PocketStatus
    {
        empty = 1,
        unmachined = 2,
        machined = 3,
        reserved = 4,
        Mask = 5,
        occupied = 6,
        Broken = 7,
        Disable = 8
    }

    public enum ToolType
    {
        other = 0,
        HSK = 1,
        Drill = 2,
        Round = 3
    }

    public struct PocketProperties
    {
        public string name;
        public int shelf, column, pocket, diameter;
        public ToolType currenttool;
        public PocketStatus status;
        public int workpiece;
        public double programnumber;
    }

    public struct WorkPiece
    {
        public int wpnumber;
        public int ncprogram;
        public int toolamount, toolamountleft, tooldiameter;
        public string wptooltype;
    }
}
