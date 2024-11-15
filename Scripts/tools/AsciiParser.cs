using System.Text;
using System;


namespace Visus.Robotics.RosBridge.Tools {
    public class AsciiParser
    {
        protected int nextPos;
        protected char[] data;
	
        public char decimalPoint = '.';
        
        public AsciiParser() {
        }
        
        /// <summary> Load the given String into the ASCII Parser. It is faster to load a char[] directly </summary>
        /// <param> text - String to load</param>
        public void loadText (string text) {
            data = text.ToCharArray();
            nextPos = 0;
            
        }
        
        /// <summary> Load the given char[] into the ASCII Parser. This is the fastest way to load data. </summary>
        /// <param> data - the char[] to load </param>
        public void loadData (char[] data) {
            this.data = data;
            nextPos = 0;
            
        }
        
        /// <summary> Load the given byte[] into the ASCII Parser. It is faster to load a char[] directly </summary>
        /// <param> data - the byte[] to load </param>
        public void loadData (byte[] data) {
            this.data = new char[data.Length];
            for (int i = 0; i < data.Length; i++) {
                this.data[i] = (char)(data[i] &0xff);
            }
            nextPos = 0;
            
        }
        
        /// <returns>an int[] with {lineNo, CharNo, nextPos}</returns>
        public int[] getLinePosition() {
            return getLinePosition(nextPos);
        }

        /// <param>nextPos - Position in text to get character for</param>
        /// <returns>an int[] with {lineNo, CharNo, nextPos}</returns>
        public int[] getLinePosition(int nextPos) {
            int line = 1;
            int chars = 1;
            
            for (int i = 0; i < nextPos; i++) {
                if (data[i] == '\n') {
                    line++;
                    chars = 1;
                } else {
                    chars++;
                }
            }
            
            return new int[] {line, chars, nextPos};
        }
        
        /// <returns>true if there is any data left to parse</returns>
        public bool available () {
            return nextPos < data.Length;
        }
        
        /// <summary>fetches the next char from data, without counting up the position</summary>
        /// <returns>the next char in data.</returns>
        /// <exception>AsciiParserException - if no more data is available</exception>
        public char peek () {
            if (!available()) {
                throw new AsciiParserException (getLinePosition(), "Unexpected EOF");
            }
            return data[nextPos];
        }
        
        /// <summary>Fetches the next char from data, counting the position up by one.</summary>
        /// <returns>the next char in data</returns>
        /// <exception>AsciiParserException  - if no more data is available</exception>
        public char read () {
            if (!available()) {
                throw new AsciiParserException (getLinePosition(), "Unexpected EOF");
            }
            return data[nextPos++];
        }
        
        /// <summary>consume the current character (goto the next character)</summary>
        /// <exception>AsciiParserException  - if no more data is available</exception>
        public void consume () {
            if (!available()) {
                throw new AsciiParserException (getLinePosition(), "Unexpected EOF");
            }
            nextPos++;
        }
        
        /// <summary>Checks if the next char equals c</summary>
        /// <param>c - the expected char</param>
        /// <exception>AsciiParserException - if the next char != c</exception>
        public void check (char c) {
            char t = peek ();
            if (t != c) {
                throw new AsciiParserException (getLinePosition(), "Char '" + c + "' Expected. Found '" + t + "'.");
            }
        }


        /// <summary>Checks if the next len(str) chars equal str</summary>
        /// <param>str - the expected string</param>
        /// <exception>AsciiParserException - if an unexpected char is found</exception>
        public void validate (string str) {
            foreach (char c in str.ToCharArray()) {
                if (peek() != c) {
                    throw new AsciiParserException (getLinePosition(), "Char '" + c + "' Expected. Found '" + peek() + "'.");
                }
                consume();
            }
        }
        
        ///<summary>Skips all characters identified as Whitespace by Char.IsWhiteSpace()</summary>
        ///<returns>the number of whitespaces skipped</returns>
        ///<exception>AsciiParserException</exception>
        public int skipWhitespace () {
            int whitespaces = 0;
            while (available()) {
                char c = peek ();
                if (Char.IsWhiteSpace(c)) {
                    consume();
                    whitespaces ++;
                } else {
                    return whitespaces;
                }
            }
            
            return whitespaces;
        }

        public string readToChar(char seperator)
        {
            StringBuilder sb = new StringBuilder();
            char c = peek();
            while (available())
            {
                c = peek();
                if (c == seperator)
                {
                    break;
                }
                sb.Append(c);
                consume();
            }

            return sb.ToString();
        }

        ///<summary>Reads a String Literal started and ended by '"'. Support for the following Escape Sequences is implemented: '\"', '\\', '\n', '\t'</summary>
        ///<returns>the content of the StringLiteral</returns>
        ///<exception>AsciiParserException</exception>
        public string readStringLiteral () {
            skipWhitespace ();
            check ('"');
            consume ();
            StringBuilder sb = new StringBuilder ();
            while (true) {
                char c=peek ();
                switch (c) {
                case '"':
                    consume ();
                    return sb.ToString();
                case '\\':
                    consume ();
                    c=read();
                    switch (c) {
                    case '"':
                        sb.Append('"');
                        break;
                    case '\\':
                        sb.Append('\\');
                        break;
                    case '/':
                        sb.Append('/');
                        break;
                    case 'b':
                        sb.Append('\b');
                        break;
                    case 'f':
                        sb.Append('\f');
                        break;
                    case 'n':
                        sb.Append('\n');
                        break;
                    case 'r':
                        sb.Append('\r');
                        break;
                    case 't':
                        sb.Append('\t');
                        break;
                    case 'u':
                        int codepoint = readHexInteger(4);
                        sb.Append(Char.ConvertFromUtf32(codepoint));
                        break;
                    default:
                        throw new AsciiParserException (getLinePosition(), "Invalid Escape Sequence: '\\" + c + "'");
                    }
                    break;
                default:
                    sb.Append(c);
                    consume();
                    break;
                }
            }
        }
        
        /// <summary>read an Integer from data[]</summary>
        /// <returns>the Integer read</returns>
        /// <exception>AsciiParserException</exception>
        public int readHexInteger (uint digits) {
            int value = 0;
            skipWhitespace ();
            
            char c = peek ();
            if (c == '-') {
                consume ();
                skipWhitespace ();
                c = read ();
                value = -getHexNumberFromChar(c);
            } else {
                value = getHexNumberFromChar (c);
                consume ();
            }
            
            for (int i = 1; i < digits; i++) {
                c = peek ();
                value *= 16;
                value += getHexNumberFromChar(c);
                consume ();
            }

            return value;
        }

        ///<summary>read a Number from data[]</summary>
        ///<returns>the Number read (long or double)</returns>
        ///<exception>AsciiParserException</exception>
        public object readNumber () {
            long value = 0;
            long divisor = 1;
            int mul = 1;
            bool countDivisor = false;
            
            skipWhitespace ();
            
            char c = peek ();
            if (c == '-') {
                consume ();
                skipWhitespace ();
                c = read ();
                value = getNumberFromChar(c);
                mul = -1;
            } else {
                value = getNumberFromChar (c);
                consume ();
            }
            
            while (true) {
                if (!available()) {
                    break;
                }
                c = peek ();
                if (Char.IsNumber(c)) {
                    value *= 10;
                    value += getNumberFromChar(c);
                    consume ();
                    
                    if (countDivisor) {
                        divisor *= 10;
                    }
                    
                } else if ((c == decimalPoint)) {
                    countDivisor = true; 
                    consume ();
                } else if (c == 'e' || c == 'E') {
                    break;
                } else {
                    if (countDivisor == false) { // Value is Integer-Like, since no decimal point is present
                        return value;
                    }
                    break;
                }  
            }

            c = peek();
            if (c == 'e' || c == 'E') {
                consume ();
                int exp = readInteger();
                mul *= (int)Math.Pow(10, exp);
            }
            
            return ((float)value/divisor)*mul;
        }

        ///<summary>read an Integer from data[]</summary>
        ///<returns>the Integer read</returns>
        ///<exception>AsciiParserException</exception>
        public int readInteger () {
            int value = 0;
            skipWhitespace ();
            
            char c = peek ();
            if (c == '-') {
                consume ();
                skipWhitespace ();
                c = read ();
                value = -getNumberFromChar(c);
            } else {
                value = getNumberFromChar (c);
                consume ();
            }
            
            while (true) {
                if (!available()) {
                    return value;
                }
                c = peek ();
                if (!Char.IsNumber(c)) {
                    return value;
                }
                value *= 10;
                value += getNumberFromChar(c);
                consume ();
            }
        }
        
        ///<summary>read a Double from data[]</summary>
        ///<returns>the Double read</returns>
        ///<exception>AsciiParserException</exception>
        public double readDouble () {
            long value = 0;
            long divisor = 1;
            int mul = 1;
            bool countDivisor = false;
            
            skipWhitespace ();
            
            char c = peek ();
            if (c == '-') {
                consume ();
                skipWhitespace ();
                c = read ();
                value = getNumberFromChar(c);
                mul = -1;
            } else {
                value = getNumberFromChar (c);
                consume ();
            }
            
            while (true) {
                if (!available()) {
                    break;
                }
                c = peek ();
                if (Char.IsNumber(c)) {
                    value *= 10;
                    value += getNumberFromChar(c);
                    consume ();
                    
                    if (countDivisor) {
                        divisor *= 10;
                    }
                    
                } else if ((c == decimalPoint)) {
                    countDivisor = true; 
                    consume ();
                } else {
                    break;
                }

            }

            c = peek();
            if (c == 'e' || c == 'E') {
                consume ();
                int exp = readInteger();
                mul *= (int)Math.Pow(10, exp);
            }
            
            return ((double)value/divisor)*mul;
        }
        
        ///<summary>read a Float from data[]</summary>
        ///<returns>the Float read</returns>
        ///<exception>AsciiParserException</exception>
        public float readFloat () {
            long value = 0;
            long divisor = 1;
            int mul = 1;
            bool countDivisor = false;
            
            skipWhitespace ();
            
            char c = peek ();
            if (c == '-') {
                consume ();
                skipWhitespace ();
                c = read ();
                value = getNumberFromChar(c);
                mul = -1;
            } else {
                value = getNumberFromChar (c);
                consume ();
            }
            
            while (true) {
                if (!available()) {
                    break;
                }
                c = peek ();
                if (Char.IsNumber(c)) {
                    value *= 10;
                    value += getNumberFromChar(c);
                    consume ();
                    
                    if (countDivisor) {
                        divisor *= 10;
                    }
                    
                } else if ((c == decimalPoint)) {
                    countDivisor = true; 
                    consume ();
                } else {
                    break;
                }
            }
            
            c = peek();
            if (c == 'e' || c == 'E') {
                consume ();
                int exp = readInteger();
                mul *= (int)Math.Pow(10, exp);
            }
            
            return ((float)value/divisor)*mul;
        }
        
        ///<summary>Resolves Characters to digits</summary>
        ///<param>c - c the character to solve</param>
        ///<returns>the digit corresponding to c</returns>
        ///<exception>AsciiParserException - if c is not representing a digit</exception>
        protected int getNumberFromChar (char c) {
            if (!Char.IsNumber(c)) {
                throw new AsciiParserException (getLinePosition(), "Internal Error: Digit expected, got " + c);
            }
            return c - '0';
        }

        ///<param>c - the char to check</param>
        ///<returns>true if c is a hex digit, false otherwise</returns>
        protected bool isHexDigit(char c) {
            if (c >= '0' && c <= '9') {
                return true;
            }

            if (c >= 'a' && c <= 'f') {
                return true;
            }

            if (c >= 'A' && c <= 'F') {
                return true;
            }

            return false;
        }

        ///<summary>Converts the given hex digit to its numeric value</summary>
        ///<param>c - char to convert</param>
        ///<returns>the value of c</returns>
        ///<exception>AsciiParserException - if c is not a hex digit</exception>
        protected int getHexNumberFromChar(char c) {
            if (c >= '0' && c <= '9') {
                return c - '0';
            }

            if (c >= 'a' && c <= 'f') {
                return 10 + (c - 'a');
            }

            if (c >= 'A' && c <= 'F') {
                return 10 + (c - 'A');
            }

            throw new AsciiParserException (getLinePosition(), "Internal Error: Hex Digit expected, got " + c);
        }
    }
}