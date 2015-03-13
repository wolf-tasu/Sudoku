using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace PermutateSudokuV3
{
    class Program
    {
        public static List<string> row2And3Permutations { get; set; }
        public static List<string> row4To9Permutations { get; set; }
        public static List<string> PreviousRowsColumns { get; set; }
        public static List<string> MajorRowBlocks { get; set; }
        public static List<string> RowsForPuzzle { get; set; }
        public static List<string> PuzzleCiphers { get; set; }

        static void Main(string[] args)
        {
            Init();
            PopulateSmartLists();
            PermutateCiphers();
        }

        private static void PermutateCiphers()
        {
            Console.WriteLine("started at: " + DateTime.Now.ToString());

            foreach (string row1 in row2And3Permutations)
            {
                if (ValidateRow(1, row1))
                {
                    PopulateValidators(row1);
                    foreach (string row2 in row2And3Permutations)
                    {
                        if (ValidateRow(2, row2))
                        {
                            PopulateValidators(row2);
                            foreach (string row3 in row2And3Permutations)
                            {
                                if (ValidateRow(3, row3))
                                {
                                    RowsForPuzzle.Add(row3);
                                    string cipher = "";
                                    foreach (string row in RowsForPuzzle)
                                    {
                                        cipher += row;
                                    }
                                    PuzzleCiphers.Add(cipher);
                                    if (PuzzleCiphers.Count == 10000)
                                    {
                                        Console.WriteLine("puzzle count is at 10,000");
                                        Console.WriteLine("Current Time: " + DateTime.Now.ToString());
                                        //Console.WriteLine("Commiting to Database....");
                                        Console.ReadLine();
                                        //CommitToDb();
                                    }
                                    RowsForPuzzle.RemoveAt(2);
                                    PopulateValidators();
                                }
                            }
                            RowsForPuzzle.RemoveAt(1);
                            PopulateValidators();
                        }
                    }
                    RowsForPuzzle.RemoveAt(0);
                    PopulateValidators();
                }
            }
            Console.ReadLine();
        }

        private static void CommitToDb()
        {
            //using (SqlBulkCopy SBC = new SqlBulkCopy("Data Source=TASU-DEVPC-PC\\SQLEXPRESS;Initial Catalog=Sudoku;Persist Security Info=True;User ID=sa;Password=default01"))
            //{

            //    foreach (string cipher in PuzzleCiphers)
            //    {

            //    }
            //}
        }

        private static void PopulateValidators()
        {
            InitValidators();
            int rowNumber = 0;
            foreach (string row in RowsForPuzzle)
            {
                rowNumber++;
                int column = 1;
                foreach (char c in row)
                {
                    PopulateColumnValidators(column, c);
                    int boxIndex = 0;
                    boxIndex = GetBoxIndex(rowNumber, column, boxIndex);
                    MajorRowBlocks[boxIndex] += c.ToString();
                    column++;
                }
            }
        }

        private static void PopulateValidators(string currentRow)
        {
            RowsForPuzzle.Add(currentRow);
            InitValidators();
            int rowNumber = 0;
            foreach (string row in RowsForPuzzle)
            {
                rowNumber++;
                int column = 1;
                foreach (char c in row)
                {
                    PopulateColumnValidators(column, c);
                    int boxIndex = 0;
                    boxIndex = GetBoxIndex(rowNumber, column, boxIndex);
                    MajorRowBlocks[boxIndex] += c.ToString();
                    column++;
                }
            }
        }

        private static int GetBoxIndex(int rowNumber, int column, int boxIndex)
        {
            switch (rowNumber)
            {
                case 1:
                case 2:
                case 3:
                    switch (column)
                    {
                        case 1:
                        case 2:
                        case 3:
                            boxIndex = 0;
                            break;
                        case 4:
                        case 5:
                        case 6:
                            boxIndex = 1;
                            break;
                        case 7:
                        case 8:
                        case 9:
                            boxIndex = 2;
                            break;
                    }
                    break;
                case 4:
                case 5:
                case 6:
                    switch (column)
                    {
                        case 1:
                        case 2:
                        case 3:
                            boxIndex = 3;
                            break;
                        case 4:
                        case 5:
                        case 6:
                            boxIndex = 4;
                            break;
                        case 7:
                        case 8:
                        case 9:
                            boxIndex = 5;
                            break;
                    }
                    break;
                case 7:
                case 8:
                case 9:
                    switch (column)
                    {
                        case 1:
                        case 2:
                        case 3:
                            boxIndex = 6;
                            break;
                        case 4:
                        case 5:
                        case 6:
                            boxIndex = 7;
                            break;
                        case 7:
                        case 8:
                        case 9:
                            boxIndex = 8;
                            break;
                    }
                    break;
            }
            return boxIndex;
        }

        private static void PopulateColumnValidators(int column, char c)
        {
            switch (column)
            {
                case 1:
                    PreviousRowsColumns[column - 1] += c.ToString();
                    break;
                case 2:
                    PreviousRowsColumns[column - 1] += c.ToString();
                    break;
                case 3:
                    PreviousRowsColumns[column - 1] += c.ToString();
                    break;
                case 4:
                    PreviousRowsColumns[column - 1] += c.ToString();
                    break;
                case 5:
                    PreviousRowsColumns[column - 1] += c.ToString();
                    break;
                case 6:
                    PreviousRowsColumns[column - 1] += c.ToString();
                    break;
                case 7:
                    PreviousRowsColumns[column - 1] += c.ToString();
                    break;
                case 8:
                    PreviousRowsColumns[column - 1] += c.ToString();
                    break;
                case 9:
                    PreviousRowsColumns[column - 1] += c.ToString();
                    break;
            }
        }

        private static void Init()
        {
            row2And3Permutations = new List<string>();
            row4To9Permutations = new List<string>();
            RowsForPuzzle = new List<string>();
            PuzzleCiphers = new List<string>();
            //RowsForPuzzle.Add("ABCDEFGHI");
            PopulateValidators();
        }

        private static void InitValidators()
        {
            PreviousRowsColumns = new List<string>();
            PreviousRowsColumns.Add("");
            PreviousRowsColumns.Add("");
            PreviousRowsColumns.Add("");
            PreviousRowsColumns.Add("");
            PreviousRowsColumns.Add("");
            PreviousRowsColumns.Add("");
            PreviousRowsColumns.Add("");
            PreviousRowsColumns.Add("");
            PreviousRowsColumns.Add("");
            MajorRowBlocks = new List<string>();
            MajorRowBlocks.Add("");
            MajorRowBlocks.Add("");
            MajorRowBlocks.Add("");
            MajorRowBlocks.Add("");
            MajorRowBlocks.Add("");
            MajorRowBlocks.Add("");
            MajorRowBlocks.Add("");
            MajorRowBlocks.Add("");
            MajorRowBlocks.Add("");
        }

        private static void PopulateSmartLists()
        {
            List<string> AlphaPermutations = new List<string>();
            GetAlphaPermutations(AlphaPermutations);
            //for (int row = 2; row < 10; row++)
            //{
                foreach (string AlphaString in AlphaPermutations)
                {
                    row2And3Permutations.Add(AlphaString);
                    //if (ValidateRow(row, AlphaString))
                    //{
                    //    if (row == 2)
                    //    {
                    //        row2And3Permutations.Add(AlphaString);
                    //    }
                    //    else if (row == 4)
                    //    {
                    //        row4To9Permutations.Add(AlphaString);
                    //    }
                    //}
                }
            //}
        }

        private static bool ValidateRow(int row, string AlphaString)
        {
            int column = 1;
            bool isValid = true;
            foreach (char letter in AlphaString)
            {
                if (PreviousRowsColumns[column - 1].Contains(letter))
                {
                    isValid = false;
                    break;
                }
                int boxIndex = 0;
                boxIndex = GetBoxIndex(row, column, boxIndex);
                if (MajorRowBlocks[boxIndex].Contains(letter))
                {
                    isValid = false;
                    break;
                }
                column++;
            }
            return isValid;
        }

        private static void GetAlphaPermutations(List<string> AlphaPermutations)
        {
            List<string> OneThroughNinePermutations = new List<string>();
            GetOneThroughNineList(OneThroughNinePermutations);
            string[] CharacterValue = { "1","2","3","4","5","6","7","8","9" };
            //string[] CharacterValue = { "A","B","C","D","E","F","G","H","I" };
            string ABCLine = "";
            foreach (string permutation in OneThroughNinePermutations)
            {
                ABCLine = "";
                foreach (char c in permutation)
                {
                    switch (c)
                    {
                        case '1':
                            ABCLine += CharacterValue[0];//"A"
                            break;         //
                        case '2':          //
                            ABCLine += CharacterValue[1];//"B"
                            break;         //
                        case '3':          //
                            ABCLine += CharacterValue[2];//"C"
                            break;         //
                        case '4':          //
                            ABCLine += CharacterValue[3];//"D"
                            break;         //
                        case '5':          //
                            ABCLine += CharacterValue[4];//"E"
                            break;         //
                        case '6':          //
                            ABCLine += CharacterValue[5];//"F"
                            break;         //
                        case '7':          //
                            ABCLine += CharacterValue[6];//"G"
                            break;         //
                        case '8':          //
                            ABCLine += CharacterValue[7];//"H"
                            break;         //
                        case '9':          //
                            ABCLine += CharacterValue[8];//"I"
                            break;
                    }
                }
                AlphaPermutations.Add(ABCLine);
            }
        }

        private static void GetOneThroughNineList(List<string> OneThroughNinePermutations)
        {
            SqlCommand cmd = GetCommand("GetOneToNnePermutations");
            SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            while (rdr.Read())
            {
                OneThroughNinePermutations.Add(rdr["Number"].ToString());
            }
            rdr.Close();
        }

        private static SqlCommand GetCommand(string cmdText)
        {
            SqlConnection connection = new SqlConnection("Data Source=TASU-DEVPC-PC\\SQLEXPRESS;Initial Catalog=Sudoku;Persist Security Info=True;User ID=sa;Password=default01");
            SqlCommand cmd = new SqlCommand(cmdText, connection);
            cmd.Connection.Open();
            return cmd;
        }
    }
}
