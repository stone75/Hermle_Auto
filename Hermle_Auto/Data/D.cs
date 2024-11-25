using Hermle_Auto.Core;
using HermleCS.Comm;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
//using System.Windows.Forms;

namespace HermleCS.Data
{
    public class D
    {
        private static readonly D instance = new D();

        public Locations[,,] DrillLocations = new Locations[C.DRILL_LOCATION_SHELF_COUNT, C.DRILL_LOCATION_COLUMN_COUNT, C.DRILL_LOCATION_POCKET_COUNT];
        public Locations[,,] HSKLocations = new Locations[C.HSK_LOCATION_SHELF_COUNT, C.HSK_LOCATION_COLUMN_COUNT, C.HSK_LOCATION_POCKET_COUNT];
        public Locations[,,] RoundLocations = new Locations[C.ROUND_LOCATION_SHELF_COUNT, C.ROUND_LOCATION_COLUMN_COUNT, C.ROUND_LOCATION_POCKET_COUNT];

        public GeneralLocations[] DrillGeneralLocations = new GeneralLocations[C.DRILL_GENLOCATION_COUNT];
        public GeneralLocations[] HSKGeneralLocations = new GeneralLocations[C.HSK_GENLOCATION_COUNT];
        public GeneralLocations[] RoundGeneralLocations = new GeneralLocations[C.ROUND_GENLOCATION_COUNT];

        public Status[,,] DrillStatus = new Status[C.DRILL_STATUS_SHELF_COUNT, C.DRILL_STATUS_COLUMN_COUNT, C.DRILL_STATUS_POCKET_COUNT];
        public Status[,,] HSKStatus = new Status[C.HSK_STATUS_SHELF_COUNT, C.HSK_STATUS_COLUMN_COUNT, C.HSK_STATUS_POCKET_COUNT];
        public Status[,,] RoundStatus = new Status[C.ROUND_STATUS_SHELF_COUNT, C.ROUND_STATUS_COLUMN_COUNT, C.ROUND_STATUS_POCKET_COUNT];

        public WorkPiece[] WorkPiecesList = new WorkPiece[C.WORKPIECE_COUNT];
        public int CurrentWorkPieceIndex = 0;

        public IniFile iniFile = new IniFile();

        public string CURRENT_JOBNAME;

        private D() { }

        public static D Instance
        {
            get
            {
                return instance;
            }
        }


        public String getLocationValues(Locations[,,] locations)
        {
            String rval = "";

            for (int i = 0; i < locations.GetLength(0); i++)
            {
                for (int j = 0; j < locations.GetLength(1); j++)
                {
                    for (int k = 0; k < locations.GetLength(2); k++)
                    {
                        rval += locations[i, j, k].name + ",";
                        rval += locations[i, j, k].x + ",";
                        rval += locations[i, j, k].y + ",";
                        rval += locations[i, j, k].z + ",";
                        rval += locations[i, j, k].rx + ",";
                        rval += locations[i, j, k].ry + ",";
                        rval += locations[i, j, k].rz + ",";
                        rval += locations[i, j, k].dist + ",";
                        rval += locations[i, j, k].alfa + "\r\n";
                    }
                }
            }

            return rval;
        }

        public String getGeneralLocationValues(GeneralLocations[] locations)
        {
            String rval = "";

            for (int i = 0; i < locations.GetLength(0); i++)
            {
                rval += locations[i].name + ",";
                rval += locations[i].x + ",";
                rval += locations[i].y + ",";
                rval += locations[i].z + ",";
                rval += locations[i].rx + ",";
                rval += locations[i].ry + ",";
                rval += locations[i].rz + "\r\n";
            }

            return rval;
        }

        public String getStatusValues(Status[,,] status)
        {
            String rval = "";

            for (int i = 0; i < status.GetLength(0); i++)
            {
                for (int j = 0; j < status.GetLength(1); j++)
                {
                    for (int k = 0; k < status.GetLength(2); k++)
                    {
                        rval += status[i, j, k].name + ",";
                        rval += status[i, j, k].shelf + ",";
                        rval += status[i, j, k].column + ",";
                        rval += status[i, j, k].pocket + ",";
                        rval += status[i, j, k].diameter + ",";
                        rval += status[i, j, k].currenttool + ",";
                        rval += status[i, j, k].status + ",";
                        rval += status[i, j, k].workpiece + ",";
                        rval += status[i, j, k].programnumber + "\r\n";
                    }
                }
            }

            return rval;
        }

        public String getWorkPiecesList(WorkPiece[] workpiece)
        {
            String rval = "";

            for (int i = 0; i < WorkPiecesList.GetLength(0); i++)
            {
                rval += (i+1);
                rval += workpiece[i].wpnumber + ",";
                rval += workpiece[i].ncprogram + ",";
                rval += workpiece[i].toolamount + ",";
                rval += workpiece[i].toolamountleft + ",";
                rval += workpiece[i].tooldiameter + ",";
                rval += workpiece[i].wptooltype + "\r\n";
            }

            return rval;
        }


        public int ReadLocations(String toolname)
        {
            Locations[,,] target;
            String targetfile;
            int shelf_count, column_count, pocket_count;

            if (toolname.Equals("DRILL") || toolname.Equals("drill"))
            {
                target = DrillLocations;
                targetfile = "Drill";
                shelf_count = C.DRILL_LOCATION_SHELF_COUNT;
                column_count = C.DRILL_LOCATION_COLUMN_COUNT;
                pocket_count = C.DRILL_LOCATION_POCKET_COUNT;
            }
            else if (toolname.Equals("HSK") || toolname.Equals("hsk"))
            {
                target = HSKLocations;
                targetfile = "HSK";
                shelf_count = C.HSK_LOCATION_SHELF_COUNT;
                column_count = C.HSK_LOCATION_COLUMN_COUNT;
                pocket_count = C.HSK_LOCATION_POCKET_COUNT;
            }
            else if (toolname.Equals("ROUND") || toolname.Equals("round"))
            {
                target = RoundLocations;
                targetfile = "Round";
                shelf_count = C.ROUND_LOCATION_SHELF_COUNT;
                column_count = C.ROUND_LOCATION_COLUMN_COUNT;
                pocket_count = C.ROUND_LOCATION_POCKET_COUNT;
            }
            else
            {
                return C.ERRNO_FAILED;
            }



            String dummy;
            int shelf, column, pocket;
            String filePath;
            int lines = 0;

            try
            {
                //                filePath = Path.Combine(C.ApplicationPath, "WorkDirectory", "Data", arrayname, ".csv");
                filePath = Path.Combine(C.ApplicationPath, "CSV", targetfile + "Locations.csv");

                using (StreamReader reader = new StreamReader(filePath))
                {
                    // 헤더 읽기
                    dummy = reader.ReadLine();

                    for (shelf = 0; shelf < shelf_count; shelf++)
                    {
                        for (column = 0; column < column_count; column++)
                        {
                            for (pocket = 0; pocket < pocket_count; pocket++)
                            {
                                dummy = reader.ReadLine();
                                string[] values = dummy.Split(',');
                                target[shelf, column, pocket].name = values[0].Trim();
                                target[shelf, column, pocket].x = double.Parse((values[1].Length > 0) ? values[1] : "0");
                                target[shelf, column, pocket].y = double.Parse((values[2].Length > 0) ? values[2] : "0");
                                target[shelf, column, pocket].z = double.Parse((values[3].Length > 0) ? values[3] : "0");
                                target[shelf, column, pocket].rx = double.Parse((values[4].Length > 0) ? values[4] : "0");
                                target[shelf, column, pocket].ry = double.Parse((values[5].Length > 0) ? values[5] : "0");
                                target[shelf, column, pocket].rz = double.Parse((values[6].Length > 0) ? values[6] : "0");
                                target[shelf, column, pocket].dist = double.Parse((values[7].Length > 0) ? values[7] : "0");
                                target[shelf, column, pocket].alfa = double.Parse((values[8].Length > 0) ? values[8] : "0");

                                lines++;
                            }
                        }
                    }

                    C.log($"Reading : {lines}");
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                C.log("Read Locations Exception ; " + ex.Message);
                return C.ERRNO_FAILED;
            }

            return lines;
        }


        public int WriteLocations(String toolname)
        {
            Locations[,,] target;
            String targetfile;
            int shelf_count, column_count, pocket_count;

            if (toolname.Equals("DRILL") || toolname.Equals("drill"))
            {
                target = DrillLocations;
                targetfile = "Drill";
                shelf_count = C.DRILL_LOCATION_SHELF_COUNT;
                column_count = C.DRILL_LOCATION_COLUMN_COUNT;
                pocket_count = C.DRILL_LOCATION_POCKET_COUNT;
            }
            else if (toolname.Equals("HSK") || toolname.Equals("hsk"))
            {
                target = HSKLocations;
                targetfile = "HSK";
                shelf_count = C.HSK_LOCATION_SHELF_COUNT;
                column_count = C.HSK_LOCATION_COLUMN_COUNT;
                pocket_count = C.HSK_LOCATION_POCKET_COUNT;
            }
            else if (toolname.Equals("ROUND") || toolname.Equals("round"))
            {
                target = RoundLocations;
                targetfile = "Round";
                shelf_count = C.ROUND_LOCATION_SHELF_COUNT;
                column_count = C.ROUND_LOCATION_COLUMN_COUNT;
                pocket_count = C.ROUND_LOCATION_POCKET_COUNT;
            }
            else
            {
                return C.ERRNO_FAILED;
            }

            int shelf, column, pocket;
            String filePath;
            int lines = 0;

            try
            {
                //                filePath = Path.Combine(C.ApplicationPath, "WorkDirectory", "Data", arrayname, ".csv");
                filePath = Path.Combine(C.ApplicationPath, "CSV", targetfile + "Locations.csv");

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("Pocket Name , x , y , z , Rx , Ry , Rz , Distance , Alfa");

                    for (shelf = 0; shelf < shelf_count; shelf++)
                    {
                        for (column = 0; column < column_count; column++)
                        {
                            for (pocket = 0; pocket < pocket_count; pocket++)
                            {
                                var name = target[shelf, column, pocket].name;
                                var x = target[shelf, column, pocket].x;
                                var y = target[shelf, column, pocket].y;
                                var z = target[shelf, column, pocket].z;
                                var rx = target[shelf, column, pocket].rx;
                                var ry = target[shelf, column, pocket].ry;
                                var rz = target[shelf, column, pocket].rz;
                                var dist = target[shelf, column, pocket].dist;
                                var alfa = target[shelf, column, pocket].alfa;
                                writer.WriteLine($"{name} , {x} , {y} , {z} , {rx} , {ry} , {rz} , {dist}, {alfa}");

                                lines++;
                            }
                        }
                    }
                    C.log($"Writer Locations : {lines}");
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                C.log("Write Locations Exception ; " + ex.Message);
                return C.ERRNO_FAILED;
            }

            return lines;
        }

        public int ReadGeneralLocations(String toolname)
        {
            GeneralLocations[] target;
            String targetfile;
            int count;

            if ( toolname.Equals("DRILL") || toolname.Equals("drill") )
            {
                target = DrillGeneralLocations;
                targetfile = "Drill";
                count = C.DRILL_GENLOCATION_COUNT;
            }
            else if (toolname.Equals("HSK") || toolname.Equals("hsk"))
            {
                target = HSKGeneralLocations;
                targetfile = "HSK";
                count = C.HSK_GENLOCATION_COUNT;
            }
            else if (toolname.Equals("ROUND") || toolname.Equals("round"))
            {
                target = RoundGeneralLocations;
                targetfile = "Round";
                count = C.ROUND_GENLOCATION_COUNT;
            }
            else
            {
                return C.ERRNO_FAILED;
            }

            String dummy;
            String filePath;
            int lines = 0;

            try
            {
                //                filePath = Path.Combine(C.ApplicationPath, "WorkDirectory", "Data", arrayname, ".csv");
                filePath = Path.Combine(C.ApplicationPath, "CSV", targetfile + "GeneralLocations.csv");

                using (StreamReader reader = new StreamReader(filePath))
                {
                    // 헤더 읽기
                    dummy = reader.ReadLine();

                    for (int i = 0; i < count; i++)
                    {
                        dummy = reader.ReadLine();
                        string[] values = dummy.Split(',');
                        target[i].name = values[0].Trim();
                        target[i].x = double.Parse((values[1].Length > 0) ? values[1] : "0");
                        target[i].y = double.Parse((values[2].Length > 0) ? values[2] : "0");
                        target[i].z = double.Parse((values[3].Length > 0) ? values[3] : "0");
                        target[i].rx = double.Parse((values[4].Length > 0) ? values[4] : "0");
                        target[i].ry = double.Parse((values[5].Length > 0) ? values[5] : "0");
                        target[i].rz = double.Parse((values[6].Length > 0) ? values[6] : "0");

                        lines++;
                    }

                    C.log($"Reading General Locations : {lines}");
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                C.log("Read General Locations Exception ; " + ex.Message);
                return C.ERRNO_FAILED;
            }

            return lines;
        }


        public int WriteGeneralLocations(String toolname)
        {
            GeneralLocations[] target;
            String targetfile;

            if (toolname.Equals("DRILL") || toolname.Equals("drill"))
            {
                target = DrillGeneralLocations;
                targetfile = "Drill";
            }
            else if (toolname.Equals("HSK") || toolname.Equals("hsk"))
            {
                target = HSKGeneralLocations;
                targetfile = "HSK";
            }
            else if (toolname.Equals("ROUND") || toolname.Equals("round"))
            {
                target = RoundGeneralLocations;
                targetfile = "Round";
            }
            else
            {
                return C.ERRNO_FAILED;
            }

            String filePath;
            int lines = 0;
            int count = target.Length;

            try
            {
                //                filePath = Path.Combine(C.ApplicationPath, "WorkDirectory", "Data", arrayname, ".csv");
                filePath = Path.Combine(C.ApplicationPath, "CSV", targetfile + "GeneralLocations.csv");

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("General Location , x , y , z , Rx , Ry , Rz");

                    for (int i = 0; i < count; i++)
                    {
                        var name = target[i].name;
                        var x = target[i].x;
                        var y = target[i].y;
                        var z = target[i].z;
                        var rx = target[i].rx;
                        var ry = target[i].ry;
                        var rz = target[i].rz;
                        writer.WriteLine($"{name} , {x} , {y} , {z} , {rx} , {ry} , {rz}");

                        lines++;
                    }
                    C.log($"Writer General Locations : {lines}");
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                C.log("Write General Locations Exception ; " + ex.Message);
                return C.ERRNO_FAILED;
            }

            return lines;
        }


        public int ReadStatus(String toolname)
        {
            Status[,,] target;
            String targetfile;
            int shelf_count, column_count, pocket_count;

            if (toolname.Equals("DRILL") || toolname.Equals("drill"))
            {
                target = DrillStatus;
                targetfile = "Drill";
                shelf_count = C.DRILL_STATUS_SHELF_COUNT;
                column_count = C.DRILL_STATUS_COLUMN_COUNT;
                pocket_count = C.DRILL_STATUS_POCKET_COUNT;
            }
            else if (toolname.Equals("HSK") || toolname.Equals("hsk"))
            {
                target = HSKStatus;
                targetfile = "HSK";
                shelf_count = C.HSK_STATUS_SHELF_COUNT;
                column_count = C.HSK_STATUS_COLUMN_COUNT;
                pocket_count = C.HSK_STATUS_POCKET_COUNT;
            }
            else if (toolname.Equals("ROUND") || toolname.Equals("round"))
            {
                target = RoundStatus;
                targetfile = "Round";
                shelf_count = C.ROUND_STATUS_SHELF_COUNT;
                column_count = C.ROUND_STATUS_COLUMN_COUNT;
                pocket_count = C.ROUND_STATUS_POCKET_COUNT;
            }
            else
            {
                return C.ERRNO_FAILED;
            }

            String dummy;
            int shelf, column, pocket;
            String filePath;
            int lines = 0;

            try
            {
                //                filePath = Path.Combine(C.ApplicationPath, "WorkDirectory", "Data", arrayname, ".csv");
                filePath = Path.Combine(C.ApplicationPath, "CSV", targetfile + "Status.csv");

                using (StreamReader reader = new StreamReader(filePath))
                {
                    // 헤더 읽기
                    dummy = reader.ReadLine();

                    for (shelf = 0; shelf < shelf_count; shelf++)
                    {
                        for (column = 0; column < column_count; column++)
                        {
                            for (pocket = 0; pocket < pocket_count; pocket++)
                            {
                                dummy = reader.ReadLine();


                                string[] values = dummy.Split(',');
                                target[shelf, column, pocket].name = values[0].Trim();
                                target[shelf, column, pocket].shelf = int.Parse((values[1].Length > 0) ? values[1] : "0");
                                target[shelf, column, pocket].column = int.Parse((values[2].Length > 0) ? values[2] : "0");
                                target[shelf, column, pocket].pocket = int.Parse((values[3].Length > 0) ? values[3] : "0");
                                target[shelf, column, pocket].diameter = int.Parse((values[4].Length > 0) ? values[4] : "0");
                                target[shelf, column, pocket].currenttool = int.Parse((values[5].Length > 0) ? values[5] : "0");
                                target[shelf, column, pocket].status = int.Parse((values[6].Length > 0) ? values[6] : "0");
                                target[shelf, column, pocket].workpiece = int.Parse((values[7].Length > 0) ? values[7] : "0");
                                target[shelf, column, pocket].programnumber = int.Parse((values[8].Length > 0) ? values[8] : "0");


                                lines++;
                            }
                        }
                    }

                    C.log($"Reading Status : {lines}");
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                C.log("Read Status Exception ; " + ex.Message);
                return C.ERRNO_FAILED;
            }

            return lines;
        }


        public int WriteStatus(String toolname)
        {
            Status[,,] target;
            String targetfile;
            int shelf_count, column_count, pocket_count;

            if (toolname.Equals("DRILL") || toolname.Equals("drill"))
            {
                target = DrillStatus;
                targetfile = "Drill";
                shelf_count = C.DRILL_STATUS_SHELF_COUNT;
                column_count = C.DRILL_STATUS_COLUMN_COUNT;
                pocket_count = C.DRILL_STATUS_POCKET_COUNT;
            }
            else if (toolname.Equals("HSK") || toolname.Equals("hsk"))
            {
                target = HSKStatus;
                targetfile = "HSK";
                shelf_count = C.HSK_STATUS_SHELF_COUNT;
                column_count = C.HSK_STATUS_COLUMN_COUNT;
                pocket_count = C.HSK_STATUS_POCKET_COUNT;
            }
            else if (toolname.Equals("ROUND") || toolname.Equals("round"))
            {
                target = RoundStatus;
                targetfile = "Round";
                shelf_count = C.ROUND_STATUS_SHELF_COUNT;
                column_count = C.ROUND_STATUS_COLUMN_COUNT;
                pocket_count = C.ROUND_STATUS_POCKET_COUNT;
            }
            else
            {
                return C.ERRNO_FAILED;
            }

            int shelf, column, pocket;
            String filePath;
            int lines = 0;

            try
            {
                //                filePath = Path.Combine(C.ApplicationPath, "WorkDirectory", "Data", arrayname, ".csv");
                filePath = Path.Combine(C.ApplicationPath, "CSV", targetfile + "Status.csv");

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("Pocket Name , shelf , column , pocket , diameter , CurrentTool , Status , WorkPiece , programNumber");

                    for (shelf = 0; shelf < shelf_count; shelf++)
                    {
                        for (column = 0; column < column_count; column++)
                        {
                            for (pocket = 0; pocket < pocket_count; pocket++)
                            {
                                var name = target[shelf, column, pocket].name;
                                var x = target[shelf, column, pocket].shelf;
                                var y = target[shelf, column, pocket].column;
                                var z = target[shelf, column, pocket].pocket;
                                var rx = target[shelf, column, pocket].diameter;
                                var ry = target[shelf, column, pocket].currenttool;
                                var rz = target[shelf, column, pocket].status;
                                var dist = target[shelf, column, pocket].workpiece;
                                var alfa = target[shelf, column, pocket].programnumber;
                                writer.WriteLine($"{name} , {x} , {y} , {z} , {rx} , {ry} , {rz} , {dist}, {alfa}");

                                lines++;
                            }
                        }
                    }
                    C.log($"Writer Status : {lines}");
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                C.log("Write Status Exception ; " + ex.Message);
                return C.ERRNO_FAILED;
            }

            return lines;
        }

        public int ReadWorkPieceList()
        {
            var target = WorkPiecesList;
            String targetfile;
            int count = C.WORKPIECE_COUNT;

            String dummy;
            String filePath;
            int lines = 0;

            try
            {
                //                filePath = Path.Combine(C.ApplicationPath, "WorkDirectory", "Data", arrayname, ".csv");
                filePath = Path.Combine(C.ApplicationPath, "CSV", "ALLWorkPiece.csv");

                using (StreamReader reader = new StreamReader(filePath))
                {
                    // 헤더 읽기
                    dummy = reader.ReadLine();

                    for (int i = 0; i < count; i++)
                    {
                        dummy = reader.ReadLine();
                        string[] values = dummy.Split(',');
                        int linenumber = (values[0].Length > 0) ? int.Parse(values[0]) : i;
                        linenumber++;
                        target[linenumber].wpnumber = int.Parse((values[1].Length > 0) ? values[1] : "0");
                        target[linenumber].ncprogram = int.Parse((values[2].Length > 0) ? values[2] : "0");
                        target[linenumber].toolamount = int.Parse((values[3].Length > 0) ? values[3] : "0");
                        target[linenumber].toolamountleft = int.Parse((values[4].Length > 0) ? values[4] : "0");
                        target[linenumber].tooldiameter = int.Parse((values[5].Length > 0) ? values[5] : "0");
                        target[linenumber].wptooltype = values[6];

                        lines++;
                    }

                    C.log($"Reading Work Piece : {lines}");
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                C.log("Read Work Piece Exception ; " + ex.Message);
                return C.ERRNO_FAILED;
            }


            return lines;
        }


        public int WriteWorkPieceList()
        {
            WorkPiece[] target = WorkPiecesList;

            String filePath;
            int lines = 0;
            int count = target.Length;

            try
            {
                //                filePath = Path.Combine(C.ApplicationPath, "WorkDirectory", "Data", arrayname, ".csv");
                filePath = Path.Combine(C.ApplicationPath, "CSV", "ALLWorkPiece.csv");

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("Line Number , WorkPieceNumber , NCProgram , Tool Amount , ToolAmountLeft , ToolDiameter , WP ToolTypex");

                    for (int i = 0; i < count; i++)
                    {
                        var linenumber = i;
                        var x = target[i].wpnumber;
                        var y = target[i].ncprogram;
                        var z = target[i].toolamount;
                        var rx = target[i].toolamountleft;
                        var ry = target[i].tooldiameter;
                        var rz = target[i].wptooltype;
                        writer.WriteLine($"{linenumber} , {x} , {y} , {z} , {rx} , {ry} , {rz}");

                        lines++;
                    }
                    C.log($"Writer Work Piece : {lines}");
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                C.log("Write Work Piece Exception ; " + ex.Message);
                return C.ERRNO_FAILED;
            }

          

            return lines;
        }

        public Locations getCurrentLocation()
        {
            Locations rVal = new Locations();

            Random random = new Random();

            // -100에서 100까지의 랜덤 값 생성
            rVal.x = Math.Round((random.NextDouble() * 200) - 100, 3);
            rVal.y = Math.Round((random.NextDouble() * 200) - 100, 3);
            rVal.z = Math.Round((random.NextDouble() * 200) - 100, 3);
            rVal.rx = Math.Round((random.NextDouble() * 200) - 100, 3);
            rVal.ry = Math.Round((random.NextDouble() * 200) - 100, 3);
            rVal.rz = Math.Round((random.NextDouble() * 200) - 100, 3);

            return rVal;
        }


        public void ReadIniFile()
        {
           

            string filePath = Path.Combine(C.ApplicationPath, "IS2904.ini");
            //string filePath = Path.Combine(C.ApplicationPath, "CSV", "IS2904.ini");

            try
            {
                var iniReader = new IniFileReader(filePath);

                iniFile.gripper.style = int.Parse(iniReader.TryGetValue("gripper", "style"));

                iniFile.application.AppToolType = int.Parse(iniReader.TryGetValue("application", "AppToolType"));
                iniFile.application.ToolSensor = int.Parse(iniReader.TryGetValue("application", "ToolSensor"));
                iniFile.application.simulator = int.Parse(iniReader.TryGetValue("application", "simulator"));
                iniFile.application.saw = int.Parse(iniReader.TryGetValue("application", "saw"));


                iniFile.shelvs.first = int.Parse(iniReader.TryGetValue("shelvs", "first"));
                iniFile.shelvs.second = int.Parse(iniReader.TryGetValue("shelvs", "second"));
                iniFile.shelvs.third = int.Parse(iniReader.TryGetValue("shelvs", "third"));

                iniFile.shelvsOffset.First = int.Parse(iniReader.TryGetValue("ShelvsOffset", "First"));
                iniFile.shelvsOffset.Second = int.Parse(iniReader.TryGetValue("ShelvsOffset", "Second"));
                iniFile.shelvsOffset.Thierd = int.Parse(iniReader.TryGetValue("ShelvsOffset", "Thierd"));


                iniFile.offsets.AbovePocket = int.Parse(iniReader.TryGetValue("offsets", "AbovePocket"));
                iniFile.offsets.AboveChuck = int.Parse(iniReader.TryGetValue("offsets", "AboveChuck"));
                iniFile.offsets.ChuckStopper = int.Parse(iniReader.TryGetValue("offsets", "ChuckStopper"));
                iniFile.offsets.ChuckDepth = int.Parse(iniReader.TryGetValue("offsets", "ChuckDepth"));
                iniFile.offsets.PocketStopper = int.Parse(iniReader.TryGetValue("offsets", "PocketStopper"));
                iniFile.offsets.KioskStopper = int.Parse(iniReader.TryGetValue("offsets", "KioskStopper"));

                iniFile.documentation.UseExternalFile = int.Parse(iniReader.TryGetValue("Documentation", "UseExternalFile"));


                /* if (iniReader.TryGetValue(section, key, out value))
                 {
                     iniFile.application.AppToolType = int.Parse(value);
                     Console.WriteLine($"Found: [{section}] {key} = {value}");
                 }*/




            }
            catch (Exception ex)
            {
                C.log("ReadIniFile Exception ; " + ex.Message);
            }
        }

        public void WriteIniFile()
        {


            string filePath = Path.Combine(C.ApplicationPath, "IS2904.ini");
            //string filePath = Path.Combine(C.ApplicationPath, "CSV", "IS2904.ini");
       
            try
            {
                var iniReader = new IniFileReader(filePath);


                iniReader.SetValue("gripper", "style",                  iniFile.gripper.style.ToString());
                iniReader.SetValue("application", "AppToolType"       , iniFile.application.AppToolType      .ToString()          );
                iniReader.SetValue("application", "ToolSensor"        , iniFile.application.ToolSensor       .ToString()          );
                iniReader.SetValue("application", "simulator"         , iniFile.application.simulator        .ToString()          );
                iniReader.SetValue("application", "saw"               , iniFile.application.saw              .ToString()          );
                iniReader.SetValue("shelvs", "first"                  , iniFile.shelvs.first                 .ToString()          );
                iniReader.SetValue("shelvs", "second"                 , iniFile.shelvs.second                .ToString()          );
                iniReader.SetValue("shelvs", "third"                  , iniFile.shelvs.third                 .ToString()          );
                iniReader.SetValue("ShelvsOffset", "First"            , iniFile.shelvsOffset.First           .ToString()          );
                iniReader.SetValue("ShelvsOffset", "Second"           , iniFile.shelvsOffset.Second          .ToString()          );
                iniReader.SetValue("ShelvsOffset", "Thierd"           , iniFile.shelvsOffset.Thierd          .ToString()          );
                iniReader.SetValue("offsets", "AbovePocket"           , iniFile.offsets.AbovePocket          .ToString()          );
                iniReader.SetValue("offsets", "AboveChuck"            , iniFile.offsets.AboveChuck           .ToString()          );
                iniReader.SetValue("offsets", "ChuckStopper"          , iniFile.offsets.ChuckStopper         .ToString()          );
                iniReader.SetValue("offsets", "ChuckDepth"            , iniFile.offsets.ChuckDepth           .ToString()         );
                iniReader.SetValue("offsets", "PocketStopper"         , iniFile.offsets.PocketStopper        .ToString()       );
                iniReader.SetValue("offsets", "KioskStopper"          , iniFile.offsets.KioskStopper         .ToString()       );
                iniReader.SetValue("Documentation", "UseExternalFile" , iniFile.documentation.UseExternalFile.ToString()      );

                // 파일 저장
                iniReader.Save();

            }
            catch (Exception ex)
            {
                C.log("Read General Locations Exception ; " + ex.Message);
            }
        }
        public string GetToolType()
        {
            if(iniFile.application.AppToolType == 0)
            {
                return "Other";
            }
            else if (iniFile.application.AppToolType == 1)
            {
                return "HSK";
            }
            else if (iniFile.application.AppToolType == 2)
            {
                return "DRILL";
            }
            else if (iniFile.application.AppToolType == 3)
            {
                return "ROUND";
            }

            return "";
        }

        public Status[,,] GetStatus(String toolname)
        {

            if (toolname.Equals("DRILL") || toolname.Equals("drill"))
            {
                return DrillStatus;
            }
            else if (toolname.Equals("HSK") || toolname.Equals("hsk"))
            {
                return HSKStatus;
            }
            else if (toolname.Equals("ROUND") || toolname.Equals("round"))
            {
                return RoundStatus;
            }

            return null;
        }

        public PocketStatus GetPocketStatus(int shelf, int pocketNumber)
        {
            //PocketStatus myStatus;

            // Uncomment the following lines if needed
            // if (!AppShelves[shelf].ShelfEnable)
            // {
            //     return PocketStatus.Occupied;
            // }

            Status[,,] statusArr = GetStatus(GetToolType());


            return (PocketStatus)statusArr[shelf, 0, pocketNumber].status;


           /* if (GetToolType() == "HSK")
            {
                myStatus =  AutomationStatus(shelf, pocketNumber).Status;
            }
            else if (GetToolType() == "Drill")
            {
                myStatus = AutomationStatus(shelf, pocketNumber).Status;
            }
            else if (GetToolType() == "Round")
            {
                myStatus = AutomationStatus(shelf, pocketNumber).Status;
            }
            else
            {
                throw new InvalidOperationException("Invalid tool type.");
            }*/
            //return myStatus;
        }

        public void SetPocketStatus(PocketStatus singlePocketStatus, int pocketName)
        {
            Status[,,] statusArr = GetStatus(GetToolType());

            int shelf = pocketName / 100; // 100의 자리          // First character
            int column = pocketName % 100; // 나머지 두 자리      // Next two characters


            statusArr[shelf, 0, column].status = (int)singlePocketStatus;


            WriteStatus(GetToolType());

            //SaveAutomation();
        }

        public void SaveAutomation(PocketStatus singlePocketStatus, int pocketName)
        {

        }

        public string PocketStatusConberter(int status)
        {
            string str = "";

            if (status == 1) return "Empty";
            if (status == 2) return "Unmachined";
            if (status == 3) return "Machined";
            if (status == 4) return "Reserved";
            if (status == 5) return "Mask";
            if (status == 6) return "Occupied";
            if (status == 7) return "Broken";
            if (status == 8) return "Disable";

            return str;
        }

        public GeneralLocations[] getGeneralLocations(string toolName)
        {
            toolName = (toolName + "").ToUpper();
            if (toolName == "DRILL")
            {
                return DrillGeneralLocations;
            }
            else if (toolName == "HSK")
            {
                return HSKGeneralLocations;
            }
            else if (toolName == "ROUND")
            {
                return RoundGeneralLocations;
            }

            return null;
        }
    }
}
