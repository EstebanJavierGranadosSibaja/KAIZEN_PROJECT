using System;
using System.Collections.Generic;

namespace ParadigmasLang
{
    internal class CharStream
    {
        private readonly string source;
        public int Position { get; private set; }
        public int Length => source.Length;

        public CharStream(string source)
        {
            this.source = source ?? string.Empty;
            Position = 0;
        }

        public char? Peek()
        {
            if (Position >= source.Length) return null;
            return source[Position];
        }

        public char? Peek(int ahead)
        {
            var pos = Position + ahead;
            if (pos >= source.Length) return null;
            return source[pos];
        }

        public char? Read()
        {
            if (Position >= source.Length) return null;
            return source[Position++];
        }

        public string ReadWhile(Func<char, bool> predicate)
        {
            int start = Position;
            while (Position < source.Length && predicate(source[Position])) Position++;
            return source.Substring(start, Position - start);
        }

        public bool EOF => Position >= source.Length;
    }
}
