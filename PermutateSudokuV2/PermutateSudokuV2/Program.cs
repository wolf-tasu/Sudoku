using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace PermutateSudokuV2
{
    class Program
    {
        #region Globals
        public static List<string> OneThroughNinePermutations { get; set; }
        public static Dictionary<string, string> InvalidatorDictionary { get; set; }
        public static Dictionary<int, string> CurrentpuzzleRows { get; set; }
        public static Dictionary<int, string> PreviousPuzzleRows { get; set; }
        public static List<string> LastPuzzleSolved { get; set; }
        public static List<string> LoadedPuzzlesFromDB { get; set; }
        public static string ValidXValues { get; set; }
        public static string ValidYValues { get; set; }
        public static string ValidBoxValues { get; set; }
        public static string ValueProbability { get; set; }
        public static string invYVal { get; set; }
        public static string invBVal { get; set; }
        public static int puzzleCount { get; set; }
        public static string constr { get; set; }
        public static SqlConnection con { get; set; }
        public static SqlCommand command { get; set; }
        public static int PKID { get; set; }
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);
        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;
        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }
        private static PerformanceCounter PC = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
        #endregion

        private static bool Handler(CtrlType sig)
        {
            switch (sig)
            {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                default:
                    UpdateWorkingStatusForNewSeedRow(command, 0);
                    break;
            }
            return true;
        }

        static void Main(string[] args)
        {
            try
            {
                
                ManualStartup();
                _handler += new EventHandler(Handler);
                SetConsoleCtrlHandler(_handler, true);
                InitializeGlobals();
                PermutateSudokuPuzzleBlocks();
            }
            catch (Exception ex)
            {
                Handler(CtrlType.CTRL_CLOSE_EVENT);
                MessageBox.Show(ex.Message);
                
            }
            finally
            {
                if (command != null && command.Connection.State == ConnectionState.Open)
                {
                    command.Connection.Close();
                    command.Dispose();
                }
            }
        }

        private static void ManualStartup()
        {
            string start = "";
            bool open = false;
            while (!open)
            {
                Console.Clear();
                Console.WriteLine("Type \"start\"");
                start = Console.ReadLine();
                if (start.ToUpper().Contains("START"))
                {
                    open = true;
                }
            }
            Console.Clear();
            Console.WriteLine("Starting...");
        }

        private static void CheckCPUSpeed()
        {
            Console.WriteLine("Checking CPU status before continuing...");
            float proc = PC.NextValue();
            Thread.Sleep(2000);
            proc = PC.NextValue();
            if (proc > 70)
            {
                throw new Exception("Your CPU is currently running at " + proc + "%. Opening another session may cause undesired results from your computer. \n The application session will now close.");
            }
            Console.WriteLine("CPU status is good.");
        }

        private static void InitializeGlobals()
        {
            constr = ConfigurationManager.ConnectionStrings["Local"].ToString();
            con = new SqlConnection(constr);
            command = new SqlCommand();
            command.Connection = con;
            command.CommandType = CommandType.StoredProcedure;
            OneThroughNinePermutations = new List<string>();
            GetOneThroughNinePermutations();
            PreviousPuzzleRows = new Dictionary<int, string>();
            CurrentpuzzleRows = new Dictionary<int, string>();
            LoadedPuzzlesFromDB = new List<string>();
            InitInvalidatorDictionary();
            ValidXValues = "";
            ValidYValues = "";
            ValidBoxValues = "";
            ValueProbability = "";
            LastPuzzleSolved = new List<string>();
            DeleteLoggingFilesOnNewStartup();
            invYVal = "";
            invBVal = "";
        }

        private static void DeleteLoggingFilesOnNewStartup()
        {
            if (File.Exists("SpinItLogger.txt"))
            {
                File.Delete("SpinItLogger.txt");
            }
        }

        private static void GetOneThroughNinePermutations()
        {
            command.Connection.Open();
            command.CommandText = "GetOneToNnePermutations";
            SqlDataReader rdr = command.ExecuteReader(CommandBehavior.CloseConnection);
            while (rdr.Read())
            {
                OneThroughNinePermutations.Add(rdr["Number"].ToString());
            }
            rdr.Close();
        }

        private static void PermutateSudokuPuzzleBlocks()
        {
            bool FS = true;
            string SeedRow = "";
            command.Connection.Open();
            foreach (string row in OneThroughNinePermutations)
            {
                FS = true;
                ReleaseLocks();
                Console.Clear();
                SeedRow = GetSeedFromDB(SeedRow);
                CheckCPUSpeed();
                FreshStart(ref FS);
                puzzleCount++;

                if (SeedRow != "")
                {
                    Console.WriteLine("-- NEXT --");
                    Console.WriteLine("Puzzle #: " + puzzleCount.ToString());
                    AddRow(SeedRow, 1);

                    foreach (string row2 in OneThroughNinePermutations)
                    {
                        if (!spinIt(ref FS, row2, 2) && !GetIsInYColumn(row2, 2) && !GetIsInBlock(2, row2))
                        {
                            AddRow(row2, 2);

                            foreach (string row3 in OneThroughNinePermutations)
                            {
                                if (CheckVsPreviousRowYValues(row2, row3) && !spinIt(ref FS, row3, 3) && !GetIsInYColumn(row3, 3) && !GetIsInBlock(3, row3))
                                {
                                    AddRow(row3, 3);

                                    foreach (string row4 in OneThroughNinePermutations)
                                    {
                                        if (CheckVsPreviousRowYValues(row3, row4) && !spinIt(ref FS, row4, 4) && !GetIsInYColumn(row4, 4) && !GetIsInBlock(4, row4))
                                        {
                                            AddRow(row4, 4);

                                            foreach (string row5 in OneThroughNinePermutations)
                                            {
                                                if (CheckVsPreviousRowYValues(row4, row5) && !spinIt(ref FS, row5, 5) && !GetIsInYColumn(row5, 5) && !GetIsInBlock(5, row5))
                                                {
                                                    AddRow(row5, 5);

                                                    foreach (string row6 in OneThroughNinePermutations)
                                                    {
                                                        if (CheckVsPreviousRowYValues(row5, row6) && !spinIt(ref FS, row6, 6) && !GetIsInYColumn(row6, 6) && !GetIsInBlock(6, row6))
                                                        {
                                                            AddRow(row6, 6);

                                                            foreach (string row7 in OneThroughNinePermutations)
                                                            {
                                                                if (CheckVsPreviousRowYValues(row6, row7) && !spinIt(ref FS, row7, 7) && !GetIsInYColumn(row7, 7) && !GetIsInBlock(7, row7))
                                                                {
                                                                    AddRow(row7, 7);

                                                                    foreach (string row8 in OneThroughNinePermutations)
                                                                    {
                                                                        if (CheckVsPreviousRowYValues(row7, row8) && !spinIt(ref FS, row8, 8) && !GetIsInYColumn(row8, 8) && !GetIsInBlock(8, row8))
                                                                        {
                                                                            AddRow(row8, 8);

                                                                            foreach (string row9 in OneThroughNinePermutations)
                                                                            {
                                                                                if (CheckVsPreviousRowYValues(row8, row9) && !spinIt(ref FS, row9, 9) && !GetIsInYColumn(row9, 9) && !GetIsInBlock(9, row9))
                                                                                {
                                                                                    CommitPuzzle(row9, SeedRow);
                                                                                }
                                                                            }
                                                                            CurrentpuzzleRows.Remove(8);
                                                                            InitInvalidatorDictionary(8);
                                                                        }
                                                                    }
                                                                    CurrentpuzzleRows.Remove(7);
                                                                    InitInvalidatorDictionary(7);
                                                                }
                                                            }
                                                            CurrentpuzzleRows.Remove(6);
                                                            InitInvalidatorDictionary(6);
                                                        }
                                                    }
                                                    CurrentpuzzleRows.Remove(5);
                                                    InitInvalidatorDictionary(5);
                                                }
                                            }
                                            CurrentpuzzleRows.Remove(4);
                                            InitInvalidatorDictionary(4);
                                        }
                                    }
                                    CurrentpuzzleRows.Remove(3);
                                    InitInvalidatorDictionary(3);
                                }
                            }
                            CurrentpuzzleRows.Remove(2);
                            InitInvalidatorDictionary(2);
                        }
                    }
                    CurrentpuzzleRows.Remove(1);
                    InitInvalidatorDictionary();
                    UpdateWorkingStatusForNewSeedRow(command, 0);
                }
            }
            command.Connection.Close();
        }

        private static void ReleaseLocks()
        {
            command.CommandText = "ReleaseWorkingLocks";
            ClearParms(command);
            command.ExecuteNonQuery();
        }

        private static string GetSeedFromDB(string seedrow)
        {
            if (LoadedPuzzlesFromDB != null && LoadedPuzzlesFromDB.Count > 0)
            {
                LoadedPuzzlesFromDB.Clear();
            }
            string Progress = "";
            string result = "";
            PKID = 0;
            UpdateForCompletedSeed(seedrow, command);
            SqlDataReader rdr = GetNewSeedRow(ref Progress, ref result, command);
            UpdateWorkingStatusForNewSeedRow(command, 1);
            rdr = LoadCompletedPuzzlesToListForSeedRowsAlreadyInProgress(Progress, result, command, rdr);
            Console.WriteLine("Seed " + result + " Status is " + Progress);
            return result;
        }

        private static SqlDataReader LoadCompletedPuzzlesToListForSeedRowsAlreadyInProgress(string Progress, string result, SqlCommand cmd, SqlDataReader rdr)
        {
            if (Progress == "In Prog")
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetPuzzlePermutationsForSeedRow";
                ClearParms(cmd);
                string temp = string.Concat(result, "%");
                cmd.Parameters.AddWithValue("@Seeder", temp);
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    LoadedPuzzlesFromDB.Add(rdr["Puzzle"].ToString());
                }
                rdr.Close();
                cmd.Parameters.Clear();
            }
            return rdr;
        }

        private static void ClearParms(SqlCommand cmd)
        {
            if (cmd.Parameters.Count > 0)
            {
                cmd.Parameters.Clear();
            }
        }

        private static void UpdateWorkingStatusForNewSeedRow(SqlCommand cmd, int workingStatus)
        {
            if (PKID != 0)
            {
                cmd.CommandText = "UpdateWorkingStatusForNewSeedRow";
                ClearParms(cmd);
                cmd.Parameters.AddWithValue("@workingStatus", workingStatus);
                cmd.Parameters.AddWithValue("@PKID", PKID);
                cmd.ExecuteNonQuery();
            }
        }

        private static SqlDataReader GetNewSeedRow(ref string Progress, ref string result, SqlCommand cmd)
        {
            cmd.CommandText = "GetNewSeedRow";
            ClearParms(cmd);
            SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.SingleRow);
            while (rdr.Read())
            {
                result = rdr["Seed"].ToString();
                Progress = rdr["Progress"].ToString();
                PKID = Int32.Parse(rdr["PKID"].ToString());
            }
            rdr.Close();
            return rdr;
        }

        private static void UpdateForCompletedSeed(string seedrow, SqlCommand cmd)
        {
            if (seedrow != "")
            {
                cmd.CommandText = "UpdateForCompletedSeed";
                ClearParms(cmd);
                cmd.ExecuteNonQuery();
            }
        }

        private static bool CheckVsPreviousRowYValues(string PreviousRow, string CurrentRow)
        {
            bool result = false;
            if (CurrentRow != "" && PreviousRow != "")
            {
                if (PreviousRow.ToArray()[0] != CurrentRow.ToArray()[0])
                {
                    result = true;
                }
            }
            return result;
        }

        private static void CommitPuzzle(string row9, string SeedRow)
        {
            CurrentpuzzleRows.Add(9, row9);
            PopulateValidators();
            OutputConsole(SeedRow);
            if (PreviousPuzzleRows.Count > 8)
            {
                PreviousPuzzleRows.Clear();
            }
            foreach (KeyValuePair<int, string> keyVP in CurrentpuzzleRows)
            {
                PreviousPuzzleRows.Add(keyVP.Key, keyVP.Value);
            }
            CurrentpuzzleRows.Remove(9);
            InitInvalidatorDictionary(9);
        }

        private static void OutputConsole(string SeedRow)
        {
            Console.WriteLine(" ");
            string puzzle = "";
            int outerCount = 0;
            int innerCount = 0;
            OutPutNewSudokuPuzzleToConsole(ref puzzle, ref outerCount, ref innerCount);
            AddPuzzleToDB(puzzle);
            string text = string.Concat("For SeedRow -- Puzzle #: ", puzzleCount.ToString(), " generated at: ", DateTime.Now.ToString());
            WriteToDBLoggingTables(text, SeedRow, "PermutationCompletionLog");
            puzzleCount++;
            Console.WriteLine(" ");
            Console.WriteLine("-- NEXT --");
            Console.WriteLine("Puzzle #: " + puzzleCount.ToString());
        }

        private static void AddPuzzleToDB(string Puzzle)
        {
            bool working = false;
            command.CommandText = "checkWorkStatus";
            ClearParms(command);
            command.Parameters.AddWithValue("@PKID", PKID);
            SqlDataReader rdr = command.ExecuteReader(CommandBehavior.SingleRow);
            while(rdr.Read())
            {
                working = bool.Parse(rdr["Working"].ToString());
            }
            rdr.Close();
            if (working)
            {
                command.CommandText = "AddPuzzle";
                ClearParms(command);
                command.Parameters.AddWithValue("@puzzle", Puzzle);
                command.ExecuteNonQuery();
                command.CommandText = "UpdateLastUpdate";
                ClearParms(command);
                command.Parameters.AddWithValue("@time", DateTime.Now);
                command.Parameters.AddWithValue("@id", PKID);
                command.ExecuteNonQuery();
            }
            else
            {
                throw new Exception("A DBA has terminated this session, probably due to low proccessing rates. \n Please Check that your CPU Usage is not overburdened and try again.");
            }
        }

        private static void OutPutNewSudokuPuzzleToConsole(ref string puzzle, ref int outerCount, ref int innerCount)
        {
            foreach (KeyValuePair<int, string> currentPuzzle in CurrentpuzzleRows)
            {
                puzzle += currentPuzzle.Value;
                if (PreviousPuzzleRows != null && PreviousPuzzleRows.Count > 8)
                {
                    ShowNewLinesForNewSudokuPuzzle(ref outerCount, ref innerCount, currentPuzzle.Value);
                }
                else
                {
                    Console.WriteLine(currentPuzzle.Value);
                }
            }
        }

        private static void ShowNewLinesForNewSudokuPuzzle(ref int outerCount, ref int innerCount, string currentPuzzle)
        {
            foreach (KeyValuePair<int, string> previousPuzzle in PreviousPuzzleRows)
            {
                if (innerCount == outerCount)
                {
                    if (currentPuzzle == previousPuzzle.Value)
                    {
                        Console.WriteLine(currentPuzzle);
                        break;
                    }
                    else
                    {
                        Console.WriteLine(currentPuzzle + " -- New Row");
                        break;
                    }
                }
                innerCount++;
            }
            innerCount = 0;
            outerCount++;
        }

        private static bool spinIt(ref bool FS, string row, int rowNumber)
        {
            bool result = false;
            if (FS)
            {
                if (rowNumber != 9 && Int32.Parse(row) != Int32.Parse(LastPuzzleSolved[rowNumber - 1]))
                {
                    result = true;
                }
                else if (rowNumber == 9 && Int32.Parse(row) <= Int32.Parse(LastPuzzleSolved[rowNumber - 1]))
                {
                    result = true;
                }

                if (rowNumber == 9 && !result)
                {
                    FS = false;
                }
            }
            return result;
        }

        private static void LoadValueForFreshStart(ref bool FS, int rowNumber, ref string row)
        {
            if (FS)
            {
                row = LastPuzzleSolved[rowNumber - 1];
                if (rowNumber == 9)
                {
                    FS = false;
                }
            }
        }

        private static void FreshStart(ref bool FS)
        {
            if (FS)
            {
                string LastPermutationCompleted;
                if (LoadedPuzzlesFromDB != null && LoadedPuzzlesFromDB.Count > 0)
                {
                    puzzleCount = LoadedPuzzlesFromDB.Count;
                    LastPermutationCompleted = LoadedPuzzlesFromDB[puzzleCount - 1];
                    Console.WriteLine(puzzleCount.ToString() + " Puzzles Found!");
                    if (LastPermutationCompleted.Length == 81)
                    {
                        LastPuzzleSolved.Add(LastPermutationCompleted.Substring(0, 9));
                        LastPuzzleSolved.Add(LastPermutationCompleted.Substring(9, 9));
                        LastPuzzleSolved.Add(LastPermutationCompleted.Substring(18, 9));
                        LastPuzzleSolved.Add(LastPermutationCompleted.Substring(27, 9));
                        LastPuzzleSolved.Add(LastPermutationCompleted.Substring(36, 9));
                        LastPuzzleSolved.Add(LastPermutationCompleted.Substring(45, 9));
                        LastPuzzleSolved.Add(LastPermutationCompleted.Substring(54, 9));
                        LastPuzzleSolved.Add(LastPermutationCompleted.Substring(63, 9));
                        LastPuzzleSolved.Add(LastPermutationCompleted.Substring(72, 9));
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    FS = false;
                }
            }
        }

        private static void AddRow(string row, int RowNumber)
        {
            CurrentpuzzleRows.Add(RowNumber, row);
            PopulateValidators();
        }

        private static void WriteToDBLoggingTables(string text, string SeedRow, string TableName)
        {
            command.CommandText = "WriteToPermutationCompletionLog";
            ClearParms(command);
            command.Parameters.AddWithValue("@seed", SeedRow);
            command.Parameters.AddWithValue("@text", text);
            command.ExecuteNonQuery();
        }

        private static void PopulateValidators()
        {
            InitInvalidatorDictionary();
            foreach (KeyValuePair<int, string> Row in CurrentpuzzleRows)
            {
                char[] c = Row.Value.ToCharArray();
                if (c.Length != 0)
                {
                    switch (Row.Key)
                    {
                        case 1:
                        case 2:
                        case 3:
                            switch (c.Length)
                            {
                                case 1:
                                    InvalidatorDictionary["box1"] += string.Concat(c[0].ToString());
                                    break;
                                case 2:
                                    InvalidatorDictionary["box1"] += string.Concat(c[0].ToString(), c[1].ToString());
                                    break;
                                case 3:
                                    InvalidatorDictionary["box1"] += string.Concat(c[0].ToString(), c[1].ToString(), c[2].ToString());
                                    break;
                                case 4:
                                    InvalidatorDictionary["box1"] += string.Concat(c[0].ToString(), c[1].ToString(), c[2].ToString());
                                    InvalidatorDictionary["box2"] += string.Concat(c[3].ToString());
                                    break;
                                case 5:
                                    InvalidatorDictionary["box1"] += string.Concat(c[0].ToString(), c[1].ToString(), c[2].ToString());
                                    InvalidatorDictionary["box2"] += string.Concat(c[3].ToString(), c[4].ToString());
                                    break;
                                case 6:
                                    InvalidatorDictionary["box1"] += string.Concat(c[0].ToString(), c[1].ToString(), c[2].ToString());
                                    InvalidatorDictionary["box2"] += string.Concat(c[3].ToString(), c[4].ToString(), c[5].ToString());
                                    break;
                                case 7:
                                    InvalidatorDictionary["box1"] += string.Concat(c[0].ToString(), c[1].ToString(), c[2].ToString());
                                    InvalidatorDictionary["box2"] += string.Concat(c[3].ToString(), c[4].ToString(), c[5].ToString());
                                    InvalidatorDictionary["box3"] += string.Concat(c[6].ToString());
                                    break;
                                case 8:
                                    InvalidatorDictionary["box1"] += string.Concat(c[0].ToString(), c[1].ToString(), c[2].ToString());
                                    InvalidatorDictionary["box2"] += string.Concat(c[3].ToString(), c[4].ToString(), c[5].ToString());
                                    InvalidatorDictionary["box3"] += string.Concat(c[6].ToString(), c[7].ToString());
                                    break;
                                case 9:
                                    InvalidatorDictionary["box1"] += string.Concat(c[0].ToString(), c[1].ToString(), c[2].ToString());
                                    InvalidatorDictionary["box2"] += string.Concat(c[3].ToString(), c[4].ToString(), c[5].ToString());
                                    InvalidatorDictionary["box3"] += string.Concat(c[6].ToString(), c[7].ToString(), c[8].ToString());
                                    break;
                            }
                            break;
                        case 4:
                        case 5:
                        case 6:
                            switch (c.Length)
                            {
                                case 1:
                                    InvalidatorDictionary["box4"] += string.Concat(c[0].ToString());
                                    break;
                                case 2:
                                    InvalidatorDictionary["box4"] += string.Concat(c[0].ToString(), c[1].ToString());
                                    break;
                                case 3:
                                    InvalidatorDictionary["box4"] += string.Concat(c[0].ToString(), c[1].ToString(), c[2].ToString());
                                    break;
                                case 4:
                                    InvalidatorDictionary["box4"] += string.Concat(c[0].ToString(), c[1].ToString(), c[2].ToString());
                                    InvalidatorDictionary["box5"] += string.Concat(c[3].ToString());
                                    break;
                                case 5:
                                    InvalidatorDictionary["box4"] += string.Concat(c[0].ToString(), c[1].ToString(), c[2].ToString());
                                    InvalidatorDictionary["box5"] += string.Concat(c[3].ToString(), c[4].ToString());
                                    break;
                                case 6:
                                    InvalidatorDictionary["box4"] += string.Concat(c[0].ToString(), c[1].ToString(), c[2].ToString());
                                    InvalidatorDictionary["box5"] += string.Concat(c[3].ToString(), c[4].ToString(), c[5].ToString());
                                    break;
                                case 7:
                                    InvalidatorDictionary["box4"] += string.Concat(c[0].ToString(), c[1].ToString(), c[2].ToString());
                                    InvalidatorDictionary["box5"] += string.Concat(c[3].ToString(), c[4].ToString(), c[5].ToString());
                                    InvalidatorDictionary["box6"] += string.Concat(c[6].ToString());
                                    break;
                                case 8:
                                    InvalidatorDictionary["box4"] += string.Concat(c[0].ToString(), c[1].ToString(), c[2].ToString());
                                    InvalidatorDictionary["box5"] += string.Concat(c[3].ToString(), c[4].ToString(), c[5].ToString());
                                    InvalidatorDictionary["box6"] += string.Concat(c[6].ToString(), c[7].ToString());
                                    break;
                                case 9:
                                    InvalidatorDictionary["box4"] += string.Concat(c[0].ToString(), c[1].ToString(), c[2].ToString());
                                    InvalidatorDictionary["box5"] += string.Concat(c[3].ToString(), c[4].ToString(), c[5].ToString());
                                    InvalidatorDictionary["box6"] += string.Concat(c[6].ToString(), c[7].ToString(), c[8].ToString());
                                    break;
                            }
                            break;
                        case 7:
                        case 8:
                        case 9:
                            switch (c.Length)
                            {
                                case 1:
                                    InvalidatorDictionary["box7"] += string.Concat(c[0].ToString());
                                    break;
                                case 2:
                                    InvalidatorDictionary["box7"] += string.Concat(c[0].ToString(), c[1].ToString());
                                    break;
                                case 3:
                                    InvalidatorDictionary["box7"] += string.Concat(c[0].ToString(), c[1].ToString(), c[2].ToString());
                                    break;
                                case 4:
                                    InvalidatorDictionary["box7"] += string.Concat(c[0].ToString(), c[1].ToString(), c[2].ToString());
                                    InvalidatorDictionary["box8"] += string.Concat(c[3].ToString());
                                    break;
                                case 5:
                                    InvalidatorDictionary["box7"] += string.Concat(c[0].ToString(), c[1].ToString(), c[2].ToString());
                                    InvalidatorDictionary["box8"] += string.Concat(c[3].ToString(), c[4].ToString());
                                    break;
                                case 6:
                                    InvalidatorDictionary["box7"] += string.Concat(c[0].ToString(), c[1].ToString(), c[2].ToString());
                                    InvalidatorDictionary["box8"] += string.Concat(c[3].ToString(), c[4].ToString(), c[5].ToString());
                                    break;
                                case 7:
                                    InvalidatorDictionary["box7"] += string.Concat(c[0].ToString(), c[1].ToString(), c[2].ToString());
                                    InvalidatorDictionary["box8"] += string.Concat(c[3].ToString(), c[4].ToString(), c[5].ToString());
                                    InvalidatorDictionary["box9"] += string.Concat(c[6].ToString());
                                    break;
                                case 8:
                                    InvalidatorDictionary["box7"] += string.Concat(c[0].ToString(), c[1].ToString(), c[2].ToString());
                                    InvalidatorDictionary["box8"] += string.Concat(c[3].ToString(), c[4].ToString(), c[5].ToString());
                                    InvalidatorDictionary["box9"] += string.Concat(c[6].ToString(), c[7].ToString());
                                    break;
                                case 9:
                                    InvalidatorDictionary["box7"] += string.Concat(c[0].ToString(), c[1].ToString(), c[2].ToString());
                                    InvalidatorDictionary["box8"] += string.Concat(c[3].ToString(), c[4].ToString(), c[5].ToString());
                                    InvalidatorDictionary["box9"] += string.Concat(c[6].ToString(), c[7].ToString(), c[8].ToString());
                                    break;
                            }
                            break;
                    }
                    if (c.Length > 8)
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            InvalidatorDictionary[string.Concat("column", (i + 1).ToString())] += c[i].ToString();
                        }
                    }
                }
            }

        }

        private static void InitInvalidatorDictionary(int RowNumber)
        {
            string boxKey = "box" + RowNumber.ToString();
            string columnKey = "column" + RowNumber.ToString();
            InvalidatorDictionary.Remove(boxKey);
            InvalidatorDictionary.Remove(columnKey);
            InvalidatorDictionary.Add(boxKey, "");
            InvalidatorDictionary.Add(columnKey, "");
            PopulateValidators();
        }

        private static void InitInvalidatorDictionary()
        {
            InvalidatorDictionary = new Dictionary<string, string>();
            InvalidatorDictionary.Add("box1", "");
            InvalidatorDictionary.Add("box2", "");
            InvalidatorDictionary.Add("box3", "");
            InvalidatorDictionary.Add("box4", "");
            InvalidatorDictionary.Add("box5", "");
            InvalidatorDictionary.Add("box6", "");
            InvalidatorDictionary.Add("box7", "");
            InvalidatorDictionary.Add("box8", "");
            InvalidatorDictionary.Add("box9", "");
            InvalidatorDictionary.Add("column1", "");
            InvalidatorDictionary.Add("column2", "");
            InvalidatorDictionary.Add("column3", "");
            InvalidatorDictionary.Add("column4", "");
            InvalidatorDictionary.Add("column5", "");
            InvalidatorDictionary.Add("column6", "");
            InvalidatorDictionary.Add("column7", "");
            InvalidatorDictionary.Add("column8", "");
            InvalidatorDictionary.Add("column9", "");
        }

        private static bool GetIsInYColumn(string ColumnCurrentValue, int rowNumber)
        {
            int count = 0;
            bool isInYColumn = false;
            invYVal = "";
            if (ColumnCurrentValue != "")
            {

                foreach (char S in ColumnCurrentValue.ToArray())
                {
                    count++;
                    string key = "column" + count.ToString();
                    if (InvalidatorDictionary[key].Contains(S.ToString()))
                    {
                        invYVal = string.Concat("Row is Invalid Because ", S.ToString(), " already exists in ", key, ".");
                        isInYColumn = true;
                        break;
                    }
                }
            }
            else
            {
                isInYColumn = true;
            }
            return isInYColumn;
        }

        private static bool GetIsInBlock(int RowNumber, string CurrentRow)
        {
            int currentBox = 0;
            bool isInBox = false;
            invBVal = "";
            int count = 0;
            foreach (char c in CurrentRow)
            {
                count++;
                switch (count)
                {
                    case 1:
                    case 2:
                    case 3:
                        currentBox = 1;
                        break;
                    case 4:
                    case 5:
                    case 6:
                        currentBox = 2;
                        break;
                    case 7:
                    case 8:
                    case 9:
                        currentBox = 3;
                        break;
                }

                isInBox = false;
                switch (RowNumber)
                {
                    case 1:
                    case 2:
                    case 3:
                        switch (currentBox)
                        {
                            case 1:
                                if (InvalidatorDictionary[string.Concat("box1")].Contains(c.ToString()))
                                {
                                    isInBox = true;

                                }
                                break;
                            case 2:
                                if (InvalidatorDictionary[string.Concat("box2")].Contains(c.ToString()))
                                {
                                    isInBox = true;
                                }
                                break;
                            case 3:
                                if (InvalidatorDictionary[string.Concat("box3")].Contains(c.ToString()))
                                {
                                    isInBox = true;
                                }
                                break;

                        }
                        break;
                    case 4:
                    case 5:
                    case 6:
                        switch (currentBox)
                        {
                            case 1:
                                if (InvalidatorDictionary[string.Concat("box4")].Contains(c.ToString()))
                                {
                                    isInBox = true;
                                }
                                break;
                            case 2:
                                if (InvalidatorDictionary[string.Concat("box5")].Contains(c.ToString()))
                                {
                                    isInBox = true;
                                }
                                break;
                            case 3:
                                if (InvalidatorDictionary[string.Concat("box6")].Contains(c.ToString()))
                                {
                                    isInBox = true;
                                }
                                break;

                        }
                        break;
                    case 7:
                    case 8:
                    case 9:
                        switch (currentBox)
                        {
                            case 1:
                                if (InvalidatorDictionary[string.Concat("box7")].Contains(c.ToString()))
                                {
                                    isInBox = true;
                                }
                                break;
                            case 2:
                                if (InvalidatorDictionary[string.Concat("box8")].Contains(c.ToString()))
                                {
                                    isInBox = true;
                                }
                                break;
                            case 3:
                                if (InvalidatorDictionary[string.Concat("box9")].Contains(c.ToString()))
                                {
                                    isInBox = true;
                                }
                                break;
                        }
                        break;
                }
                if (isInBox)
                {
                    invBVal = string.Concat("Row is invalid because ", c.ToString(), " is already in box", currentBox.ToString(), " for row", RowNumber.ToString());
                    break;
                }
            }
            return isInBox;
        }
    }
}
