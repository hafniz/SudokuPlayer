using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace SudokuPlayer
{
    internal static class Program
    {
        private static Block[,] Map { get; set; } = new Block[9, 9];

        private static void Main(string[] args)
        {
            GetMap();
            List<Block> emptyBlocks = GetEmptyBlocks();
            foreach (Block emptyBlock in emptyBlocks)
            {
                SetNotes(emptyBlock);
            }
            bool changed;
            do
            {
                changed = false;
                changed |= EliminateByRow();
                changed |= EliminateByColumn();
                changed |= EliminateByGroup();
            }
            while (changed);
            //EliminateByRow();
            //EliminateByColumn();
            //EliminateByGroup();
            ShowMap();
        }

        private static void ShowMap()
        {
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
                        line += Map[r, c].Value.ToString();
                    }
                }
                WriteLine(line);
            }
            ReadLine();
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

        private static IEnumerable<Block> GetGroup(int rGroup, int cGroup)
        {
            for (int rOffset = 0; rOffset < 3; rOffset++)
            {
                for (int cOffset = 0; cOffset < 3; cOffset++)
                {
                    yield return Map[rGroup * 3 + rOffset, cGroup * 3 + cOffset];
                }
            }
        }

        private static bool EliminateByRow()
        {
            bool changed = false;
            for (int r = 0; r < 9; r++)
            {
                List<int> rowNotesCollection = new List<int>();
                for (int c = 0; c < 9; c++)
                {
                    rowNotesCollection.AddRange(Map[r, c].Notes);
                }
                for (int n = 1; n < 10; n++)
                {
                    if (rowNotesCollection.Where(v => v == n).Count() == 1)
                    {
                        for (int c = 0; c < 9; c++)
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

        private static void UpdateNotes(Block block)
        {
            foreach (Block relatedBlock in GetRelatedBlocks(block))
            {
                if (relatedBlock.Notes.Contains((int)block.Value))
                {
                    relatedBlock.Notes.Remove((int)block.Value);
                }
            }
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

        private static void SetNotes(Block emptyBlock)
        {
            List<Block> relatedBlocks = GetRelatedBlocks(emptyBlock);
            for (int v = 1; v < 10; v++)
            {
                if (!relatedBlocks.ConvertAll(b => b.Value).Contains(v))
                {
                    emptyBlock.Notes.Add(v);
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
                        emptyBlocks.Add(new Block { Row = r, Column = c, Value = null });
                    }
                }
            }
            return emptyBlocks;
        }

        private static void GetMap()
        {
            Map = new Block[9, 9];
            for (int r = 0; r < 9; r++)
            {
                string chars = ReadLine();
                for (int c = 0; c < chars.Length; c++)
                {
                    Map[r, c] = new Block();
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

        private static List<Block> GetRelatedBlocks(Block block)
        {
            List<Block> relatedBlocks = new List<Block>();
            for (int c = 0; c < 9; c++)
            {
                relatedBlocks.Add(Map[block.Row, c]);
            }
            for (int r = 0; r < 9; r++)
            {
                relatedBlocks.Add(Map[r, block.Column]);
            }
            relatedBlocks.AddRange(GetGroup(block.Row / 3, block.Column / 3));
            return relatedBlocks;
        }
    }
}
