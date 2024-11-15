using System;

using UnityEngine;

namespace Visus.Robotics.RosBridge.Tools {
public class AsciiParserException : Exception
    {
        /// <summary>
        /// AsciiParser Exception
        /// </summary>
        /// <param name="linepos">Array containing at least 2 Elements. The first element is the line number of the
        /// parsed file the exception occured at, the second element is the character number within that line</param>
        /// <param name="message">Error message describing the error</param>
        public AsciiParserException(int[] linepos, string message) : base(message + " at line " + linepos[0] + ", column " + linepos[1]) { }
    }
}