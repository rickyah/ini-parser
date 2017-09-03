using System.IO;
using System.Collections.Generic;
using IniParser.Model.Configuration;
using System;

namespace IniParser.Parser
{
    public class Scanner
    {
        readonly static List<char> WHITESPACE_CHARS = new List<char> { ' ', '\t', '\n' };

        List<Token> mTokens = new List<Token>();
        List<char> mBuffer = new List<char>(256);
        bool mIsSectionOpened = false;

        public List<Token> Tokenize(StringReader dataSource, IniScheme scheme)
        {
            uint lineCount = 1;
            int currentCharIdx = 0;
            mTokens.Clear();
            mBuffer.Clear();
            mIsSectionOpened = false;

            string currentLine = ReadLine(dataSource);

            while (currentLine != null)
            {
                currentCharIdx = ProcessWhitespace(currentLine, currentCharIdx, lineCount);
                currentCharIdx = ProcessComment(currentLine, currentCharIdx, lineCount, scheme);
                currentCharIdx = ProcessSectionStart(currentLine, currentCharIdx, lineCount,  scheme);
                currentCharIdx = ProcessSectionEnd(currentLine, currentCharIdx, lineCount,  scheme);
                currentCharIdx = ProcessProperty(currentLine, currentCharIdx, lineCount,  scheme);
                currentCharIdx = ProcessValue(currentLine, currentCharIdx, lineCount, scheme);
                currentCharIdx = ProcessNewLine(currentLine, currentCharIdx, lineCount);

                if (IsEndOfLine(currentLine, currentCharIdx))
                {
                    currentLine = ReadLine(dataSource);

                    lineCount++;
                    currentCharIdx = 0;
                }
            }

            return mTokens;
        }

        private string ReadLine(StringReader dataSource)
        {
            mBuffer.Clear();
            int c = dataSource.Read();

            while(c != '\n' && c != -1)
            {
                if (c != '\r')
                {
                    mBuffer.Add((char)c);
                }

                c = dataSource.Read();
            }

            if (c != -1)
            {
                mBuffer.Add('\n');
            }

            if (mBuffer.Count == 0) return null;

            return new string(mBuffer.ToArray());
        }

        private int ProcessNewLine(string currentLine, int currentCharIdx, uint lineCount)
        {
            if (currentCharIdx < currentLine.Length
            && currentLine[currentCharIdx] == '\n')
            {
                var token = new Token(Token.Type.NEWLINE, null, lineCount);
                mTokens.Add(token);
                currentCharIdx++;
            }

            return currentCharIdx;
        }

        private int ProcessProperty(string currentLine, int currentCharIdx, uint lineCount, IniScheme scheme)
        {
            if (IsPropertyDelimiter(currentLine, currentCharIdx, scheme))
            {
                var token = new Token(Token.Type.PROPERTY_DELIMITER, null, lineCount);
                mTokens.Add(token);
                currentCharIdx += scheme.PropertyDelimiterString.Length;
            }

            return currentCharIdx;
        }

        private int ProcessSectionEnd(string currentLine, int currentCharIdx, uint lineCount, IniScheme scheme)
        {
            if (IsSectionEnd(currentLine, currentCharIdx, scheme))
            {
                var token = new Token(Token.Type.SECTION_CLOSE, null, lineCount);
                mTokens.Add(token);
                currentCharIdx += scheme.SectionEndString.Length;
                mIsSectionOpened = false;
            }

            return currentCharIdx;
        }

        private int ProcessSectionStart(string currentLine, int currentCharIdx, uint lineCount, IniScheme scheme)
        {
            if (IsSectionStart(currentLine, currentCharIdx, scheme))
            {
                var token = new Token(Token.Type.SECTION_OPEN, null, lineCount);
                mTokens.Add(token);
                currentCharIdx += scheme.SectionStartString.Length;
                mIsSectionOpened = true;
            }

            return currentCharIdx;
        }

        private int ProcessValue(string currentLine, int currentCharIdx, uint lineCount, IniScheme scheme)
        {
            int startIdx = currentCharIdx;

            while (currentCharIdx < currentLine.Length
            && !IsPropertyDelimiter(currentLine, currentCharIdx, scheme)
            && !IsComment(currentLine, currentCharIdx, scheme)
            && !IsWhiteSpace(currentLine, currentCharIdx))
            {
                if (mIsSectionOpened && IsSectionEnd(currentLine, currentCharIdx, scheme))
                {
                    break;
                }

                currentCharIdx++;
            }
            if (currentCharIdx > startIdx)
            {
                var literal = currentLine.Substring(startIdx, currentCharIdx - startIdx);
                var token = new Token(Token.Type.VALUE, literal, lineCount);
                mTokens.Add(token);
            }

            return currentCharIdx;
        }

        private static bool IsEndOfLine(string currentLine, int currentCharIdx)
        {
            return currentCharIdx >= currentLine.Length;
        }

        private static bool IsSectionEnd(string currentLine, int currentCharIdx, IniScheme scheme)
        {
            return MatchesSubstring(currentLine, currentCharIdx, scheme.SectionEndString);
        }

        private static bool IsSectionStart(string currentLine, int currentCharIdx, IniScheme scheme)
        {
            return MatchesSubstring(currentLine, currentCharIdx, scheme.SectionStartString);
        }

        private static bool IsPropertyDelimiter(string currentLine, int currentCharIdx, IniScheme scheme)
        {
            return MatchesSubstring(currentLine, currentCharIdx, scheme.PropertyDelimiterString);
        }

        private static bool IsComment(string currentLine, int currentCharIdx, IniScheme scheme)
        {
            return MatchesSubstring(currentLine, currentCharIdx, scheme.CommentString);
        }

        private int ProcessComment(string currentLine, int currentCharIdx, uint lineCount, IniScheme scheme)
        {
            if (MatchesSubstring(currentLine, currentCharIdx, scheme.CommentString))
            {
                int startIdx = currentCharIdx + scheme.CommentString.Length;
                int endIdx = currentLine.Length - startIdx;

                if (currentLine[currentLine.Length - 1] == '\n')
                {
                    endIdx--;
                }

                var literal = currentLine.Substring(startIdx, endIdx);

                var token = new Token(Token.Type.COMMENT, literal, lineCount);

                mTokens.Add(token);

                currentCharIdx = currentLine.Length-1;
            }

            return currentCharIdx;
        }

        private bool IsWhiteSpace(string currentLine, int currentCharIdx)
        {
            return WHITESPACE_CHARS.Contains(currentLine[currentCharIdx]);
        }

        private int ProcessWhitespace(string line, int startIndex, uint lineCount)
        {
            int nextIndex = GetIndexToNextNonWhitespace(line, startIndex);

            if (nextIndex > startIndex)
            {
                var wsLiteral = line.Substring(startIndex, nextIndex - startIndex);

                var token = new Token(Token.Type.WHITESPACE, wsLiteral, lineCount);
                mTokens.Add(token);

                startIndex = nextIndex;
            }

            return startIndex;
        }

        private static int GetIndexToNextWhitespace(string line, int startIndex)
        {
            while(startIndex < line.Length
            && !WHITESPACE_CHARS.Contains(line[startIndex]))
            {
                startIndex++;
            }

            return startIndex;
        }

        private static int GetIndexToNextNonWhitespace(string line, int startIndex)
        {
            while(startIndex < line.Length
            && WHITESPACE_CHARS.Contains(line[startIndex])
            && line[startIndex] != '\n')
            {
                startIndex++;
            }

            return startIndex;
        }

        private static bool MatchesSubstring(string line, int startIndex, string stringToMatch)
        {
            for (int i = 0; i < stringToMatch.Length; ++i)
            {
                if (startIndex + i >= line.Length
                || line[startIndex + i] != stringToMatch[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
