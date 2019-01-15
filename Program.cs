using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace SudokuPlayer
{
    internal static class Program
    {
        private static Block[,] Map { get; set; }
        public static List<Block> EmptyBlocks { get; set; }

        private static void Main(string[] args)
        {
            GetMap();
            EmptyBlocks = GetEmptyBlocks();
            foreach (Block emptyBlock in EmptyBlocks)
            {
                SetNotes(emptyBlock);
            }
            do
            {
                EliminateByRow();
                EliminateByColumn();
                EliminateByGroup();
                ShowMap();
            }
            while (EmptyBlocks.Count != 0);
            ShowMap();
        }

        private static void GetMap()
        {
            Map = new Block[9, 9];
            for (int r = 0; r < 9; r++)
            {
                string chars = ReadLine();
                for (int c = 0; c < 9; c++)
                {
                    Map[r, c] = new Block { Row = r, Column = c };
                    if (chars[c] == '.')
                    {
                        Map[r, c].Value = null;
                    }
                    else
                    {
                        Map[r, c].Value = int.Parse(chars[c].ToString());
                    }
                }
            }
        }

        private static List<Block> GetEmptyBlocks()
        {
            List<Block> emptyBlocks = new List<Block>();
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (Map[r, c].Value is null)
                    {
                        emptyBlocks.Add(Map[r, c]);
                    }
                }
            }
            return emptyBlocks;
        }

        private static void SetNotes(Block emptyBlock)
        {
            List<Block> relatedBlocks = GetRelatedBlocks(emptyBlock);
            for (int n = 1; n < 10; n++)
            {
                if (!relatedBlocks.ToList().ConvertAll(b => b.Value).Contains(n))
                {
                    emptyBlock.Notes.Add(n);
                }
            }
        }

        private static List<Block> GetRelatedBlocks(Block block) // Note: This will also return the block itself. 
        {
            List<Block> allRelatedBlocks = new List<Block>();
            for (int c = 0; c < 9; c++)
            {
                allRelatedBlocks.Add(Map[block.Row, c]); // Get blocks in the same row, including the argument itself. 
            }
            for (int r = 0; r < 9; r++)
            {
                allRelatedBlocks.Add(Map[r, block.Column]); // Get blocks in the same column, including the argument itself. 
            }
            allRelatedBlocks.AddRange(GetGroup(block.Row / 3, block.Column / 3));  // Get blocks in the same group, including the argument itself.  

            List<Block> distinctRelatedBlocks = new List<Block>();
            foreach (Block relatedBlock in allRelatedBlocks)
            {
                if (!distinctRelatedBlocks.Contains(relatedBlock))
                {
                    distinctRelatedBlocks.Add(relatedBlock);
                }
            }
            return distinctRelatedBlocks;
        }

        private static List<Block> GetGroup(int rGroup, int cGroup)
        {
            List<Block> group = new List<Block>();
            for (int rOffset = 0; rOffset < 3; rOffset++)
            {
                for (int cOffset = 0; cOffset < 3; cOffset++)
                {
                    group.Add(Map[rGroup * 3 + rOffset, cGroup * 3 + cOffset]);
                }
            }
            return group;
        }

        private static bool EliminateByRow()
        {
            bool changed = false;
            for (int r = 0; r < 9; r++) // For each row. 
            {
                List<int> rowNotesCollection = new List<int>();
                for (int c = 0; c < 9; c++)
                {
                    rowNotesCollection.AddRange(Map[r, c].Notes); // Get all the notes on the blocks in the row. 
                }
                for (int n = 1; n < 10; n++)
                {
                    if (rowNotesCollection.Where(v => v == n).Count() == 1) // Check for unique value of notes. 
                    {
                        for (int c = 0; c < 9; c++)
                        {
                            if (Map[r, c].Notes.Contains(n)) // See where the note is written in. 
                            {
                                Map[r, c].Value = n; // Then the value of the block must be the unique note. 
                                changed = true;
                                UpdateNotes(Map[r, c]);
                            }
                        }
                    }
                }
            }
            return changed; // Returns whether a change has been made. 
        }

        private static bool EliminateByColumn()
        {
            bool changed = false;
            for (int c = 0; c < 9; c++)
            {
                List<int> columnNotesCollection = new List<int>();
                for (int r = 0; r < 9; r++)
                {
                    columnNotesCollection.AddRange(Map[r, c].Notes);
                }
                for (int n = 1; n < 10; n++)
                {
                    if (columnNotesCollection.Where(v => v == n).Count() == 1)
                    {
                        for (int r = 0; r < 9; r++)
                        {
                            if (Map[r, c].Notes.Contains(n))
                            {
                                Map[r, c].Value = n;
                                changed = true;
                                UpdateNotes(Map[r, c]);
                            }
                        }
                    }
                }
            }
            return changed;
        }

        private static bool EliminateByGroup()
        {
            bool changed = false;
            for (int rGroup = 0; rGroup < 3; rGroup++)
            {
                for (int cGroup = 0; cGroup < 3; cGroup++)
                {
                    List<Block> group = new List<Block>(GetGroup(rGroup, cGroup));
                    List<int> groupNotesCollection = new List<int>();
                    foreach (Block block in group)
                    {
                        groupNotesCollection.AddRange(block.Notes);
                    }
                    for (int n = 1; n < 10; n++)
                    {
                        if (groupNotesCollection.Where(v => v == n).Count() == 1)
                        {
                            foreach (Block block in group)
                            {
                                if (block.Notes.Contains(n))
                                {
                                    Map[block.Row, block.Column].Value = n;
                                    changed = true;
                                    UpdateNotes(block);
                                }
                            }
                        }
                    }
                }
            }
            return changed;
        }

        private static void UpdateNotes(Block block)
        {
            foreach (Block relatedBlock in GetRelatedBlocks(block))
            {
                if (relatedBlock.Notes.Contains((int)block.Value))
                {
                    relatedBlock.Notes.Remove((int)block.Value);
                }
            }
            EmptyBlocks.Remove(block);
        }

        private static void ShowMap()
        {
            WriteLine();
            for (int r = 0; r < 9; r++)
            {
                string line = "";
                for (int c = 0; c < 9; c++)
                {
                    if (Map[r, c].Value is null)
                    {
                        line += ".";
                    }
                    else
                    {
                        line += Map[r, c].Value;
                    }
                }
                WriteLine(line);
            }
            ReadLine();
        }
    }
}
