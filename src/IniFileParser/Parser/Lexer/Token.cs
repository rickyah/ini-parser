using System;

namespace IniParser.Parser
{
    public struct Token
    {
        public enum Type {
            SECTION_OPEN,
            SECTION_CLOSE,
            PROPERTY_DELIMITER,
            VALUE,
            COMMENT,
            WHITESPACE,
            NEWLINE
        }

        public Type type;
        public uint line;
        public string literal;

        public Token(Type type, string literal, uint line)
        {
            this.type = type;
            this.literal = literal;
            this.line = line;
        }

        public override string ToString()
        {
            if (literal != null)
            {
                return $"{type.ToString().ToUpper()} \"{literal}\" line:{line}";
            }
            return $"{type.ToString().ToUpper()} line:{line}";
        }
    }
}
