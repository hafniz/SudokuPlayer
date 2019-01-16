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
            GetEmptyBlocks();
            foreach (Block emptyBlock in EmptyBlocks)
            {
                SetNotes(emptyBlock);
            }
            while (EliminateByEight() || EliminateByRow() || EliminateByColumn() || EliminateByGroup())
            {
                ShowMap();
            }
            ReadLine();
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

        private static void GetEmptyBlocks()
        {
            EmptyBlocks = new List<Block>();
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (Map[r, c].Value is null)
                    {
                        EmptyBlocks.Add(Map[r, c]);
                    }
                }
            }
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

        private static List<Block> GetRelatedBlocks(Block block)
        {
            List<Block> allRelatedBlocks = new List<Block>();
            allRelatedBlocks.AddRange(GetRow(block.Row));
            allRelatedBlocks.AddRange(GetColumn(block.Column));
            allRelatedBlocks.AddRange(GetGroup(block.Row / 3, block.Column / 3));
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

        private static List<Block> GetRow(int r)
        {
            List<Block> group = new List<Block>();
            for (int c = 0; c < 9; c++)
            {
                group.Add(Map[r, c]);
            }
            return group;
        }

        private static List<Block> GetColumn(int c)
        {
            List<Block> group = new List<Block>();
            for (int r = 0; r < 9; r++)
            {
                group.Add(Map[r, c]);
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
                                changed = Assign(Map[r, c], n); // Then the value of the block must be the unique note. 
                            }
                        }
                    }
                }
            }
            return changed; // Returns whether a change has been made. 
        }

        private static bool Assign(Block block, int value)
        {
            block.Value = value;
            UpdateNotes(block);
            EliminateByOne();
            return true; // A change has been made. 
        }

        private static bool EliminateByOne() // If a block has only one note, then its value must be that of the note. 
        {
            bool changed = false;
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (Map[r, c].Notes.Count == 1)
                    {
                        changed = Assign(Map[r, c], Map[r, c].Notes.Single());
                    }
                }
            }
            return changed;
        }

        private static bool EliminateByEight() // If eight blocks in a row, column or group, then the remaining one is determined. 
        {
            bool changed = false;
            for (int r = 0; r < 9; r++)
            {
                List<Block> blocksInRow = GetRow(r);
                if (blocksInRow.Where(b => b.Value.HasValue).Count() == 8)
                {
                    for (int v = 1; v < 10; v++)
                    {
                        if (!blocksInRow.Any(b => b.Value == v))
                        {
                            changed = Assign(blocksInRow.Single(b => b.Value is null), v);
                        }
                    }
                }
            }
            for (int c = 0; c < 9; c++)
            {
                List<Block> blocksInColumn = GetColumn(c);
                if (blocksInColumn.Where(b => b.Value.HasValue).Count() == 8)
                {
                    for (int v = 1; v < 10; v++)
                    {
                        if (!blocksInColumn.Any(b => b.Value == v))
                        {
                            changed = Assign(blocksInColumn.Single(b => b.Value is null), v);
                        }
                    }
                }
            }
            for (int rGroup = 0; rGroup < 3; rGroup++)
            {
                for (int cGroup = 0; cGroup < 3; cGroup++)
                {
                    List<Block> blocksInGroup = GetGroup(rGroup, cGroup);
                    if (blocksInGroup.Where(b => b.Value.HasValue).Count() == 8)
                    {
                        for (int v = 1; v < 10; v++)
                        {
                            if (!blocksInGroup.Any(b => b.Value == v))
                            {
                                changed = Assign(blocksInGroup.Single(b => b.Value is null), v);
                            }
                        }
                    }
                }
            }
            return changed;
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
                                changed = Assign(Map[r, c], n);
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
                                    changed = Assign(Map[block.Row, block.Column], n);
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
            block.Notes = new List<int>();
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
        }
    }
}
