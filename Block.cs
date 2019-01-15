using System.Collections.Generic;

namespace SudokuPlayer
{
    public class Block
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int? Value { get; set; }
        public List<int> Notes { get; set; } = new List<int>();
        public override string ToString() => $"({Row}, {Column}): {(Value is null ? "." : Value.ToString())} ({Notes.Count})";
    }
}
