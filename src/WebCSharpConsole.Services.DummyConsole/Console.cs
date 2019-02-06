using System;
using System.IO;
using System.Security;
using System.Threading;

namespace WebCSharpConsole.Services.DummyConsole
{
    public static class Console
    {
        private static Lazy<TextWriter> _out;
        private static Stream _outputStream;

        static Console()
        {
            InitOut();
        }

        public static TextWriter Out => _out.Value;

        public static TextReader In
        {
            get
            {
                throw new SecurityException("Request for the permission of type 'System.Security.Permissions.SecurityPermission, failed.");
            }
        }

        public static string ReadOutputStreamToEnd()
        {
            _outputStream.Seek(0, SeekOrigin.Begin);
            var streamReader = new StreamReader(_outputStream);
            var result = streamReader.ReadToEnd();

            return result;
        }

        public static void Clear()
        {
            _outputStream.Dispose();
            InitOut();
        }

        //
        // Summary:
        //     Writes the specified string value to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        public static void Write(string value)
        {
            Out.Write(value);
        }

        //
        // Summary:
        //     Writes the specified array of Unicode characters to the standard output stream.
        //
        // Parameters:
        //   buffer:
        //     A Unicode character array.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        public static void Write(char[] buffer)
        {
            Out.Write(buffer);
        }

        //
        // Summary:
        //     Writes the specified subarray of Unicode characters to the standard output stream.
        //
        // Parameters:
        //   buffer:
        //     An array of Unicode characters.
        //
        //   index:
        //     The starting position in buffer.
        //
        //   count:
        //     The number of characters to write.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     buffer is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     index or count is less than zero.
        //
        //   T:System.ArgumentException:
        //     index plus count specify a position that is not within buffer.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        public static void Write(char[] buffer, int index, int count)
        {
            Out.Write(buffer, index, count);
        }

        //
        // Summary:
        //     Writes the text representation of the specified object to the standard output
        //     stream.
        //
        // Parameters:
        //   value:
        //     The value to write, or null.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        public static void Write(object value)
        {
            Out.Write(value);
        }

        //
        // Summary:
        //     Writes the text representation of the specified 64-bit unsigned integer value
        //     to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.

        public static void Write(ulong value)
        {
            Out.Write(value);
        }

        //
        // Summary:
        //     Writes the text representation of the specified 64-bit signed integer value to
        //     the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        public static void Write(long value)
        {
            Out.Write(value);
        }

        //
        // Summary:
        //     Writes the text representation of the specified single-precision floating-point
        //     value to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        public static void Write(float value)
        {
            Out.Write(value);
        }

        //
        // Summary:
        //     Writes the text representation of the specified double-precision floating-point
        //     value to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        public static void Write(double value)
        {
            Out.Write(value);
        }

        //
        // Summary:
        //     Writes the text representation of the specified System.Decimal value to the standard
        //     output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        public static void Write(decimal value)
        {
            Out.Write(value);
        }

        //
        // Summary:
        //     Writes the text representation of the specified 32-bit signed integer value to
        //     the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        public static void Write(int value)
        {
            Out.Write(value);
        }

        //
        // Summary:
        //     Writes the text representation of the specified 32-bit unsigned integer value
        //     to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        public static void Write(uint value)
        {
            Out.Write(value);
        }

        //
        // Summary:
        //     Writes the text representation of the specified Boolean value to the standard
        //     output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        public static void Write(bool value)
        {
            Out.Write(value);
        }

        //
        // Summary:
        //     Writes the specified Unicode character value to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        public static void Write(char value)
        {
            Out.Write(value);
        }

        //
        // Summary:
        //     Writes the text representation of the specified object to the standard output
        //     stream using the specified format information.
        //
        // Parameters:
        //   format:
        //     A composite format string (see Remarks).
        //
        //   arg0:
        //     An object to write using format.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //
        //   T:System.ArgumentNullException:
        //     format is null.
        //
        //   T:System.FormatException:
        //     The format specification in format is invalid.
        public static void Write(string format, object arg0)
        {
            Out.Write(format, arg0);
        }

        //
        // Summary:
        //     Writes the text representation of the specified objects to the standard output
        //     stream using the specified format information.
        //
        // Parameters:
        //   format:
        //     A composite format string (see Remarks).
        //
        //   arg0:
        //     The first object to write using format.
        //
        //   arg1:
        //     The second object to write using format.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //
        //   T:System.ArgumentNullException:
        //     format is null.
        //
        //   T:System.FormatException:
        //     The format specification in format is invalid.
        public static void Write(string format, object arg0, object arg1)
        {
            Out.Write(format, arg0, arg1);
        }

        //
        // Summary:
        //     Writes the text representation of the specified objects to the standard output
        //     stream using the specified format information.
        //
        // Parameters:
        //   format:
        //     A composite format string (see Remarks).
        //
        //   arg0:
        //     The first object to write using format.
        //
        //   arg1:
        //     The second object to write using format.
        //
        //   arg2:
        //     The third object to write using format.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //
        //   T:System.ArgumentNullException:
        //     format is null.
        //
        //   T:System.FormatException:
        //     The format specification in format is invalid.
        public static void Write(string format, object arg0, object arg1, object arg2)
        {
            Out.Write(format, arg0, arg1, arg2);
        }

        public static void Write(string format, object arg0, object arg1, object arg2, object arg3)
        {
            Out.Write(format, arg0, arg1, arg2, arg3);
        }

        //
        // Summary:
        //     Writes the text representation of the specified array of objects to the standard
        //     output stream using the specified format information.
        //
        // Parameters:
        //   format:
        //     A composite format string (see Remarks).
        //
        //   arg:
        //     An array of objects to write using format.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //
        //   T:System.ArgumentNullException:
        //     format or arg is null.
        //
        //   T:System.FormatException:
        //     The format specification in format is invalid.
        public static void Write(string format, params object[] arg)
        {
            if (arg == null)
            {
                // avoid ArgumentNullException from String.Format
                // faster than Out.Write(format, (Object)arg);
                Out.Write(format, null, null);
            }
            else
            {
                Out.Write(format, arg);
            }
        }

        //
        // Summary:
        //     Writes the current line terminator to the standard output stream.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        public static void WriteLine()
        {
            Out.WriteLine();
        }

        //
        // Summary:
        //     Writes the text representation of the specified single-precision floating-point
        //     value, followed by the current line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        public static void WriteLine(float value)
        {
            Out.WriteLine(value);
        }

        //
        // Summary:
        //     Writes the text representation of the specified 32-bit signed integer value,
        //     followed by the current line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        public static void WriteLine(int value)
        {
            Out.WriteLine(value);
        }

        //
        // Summary:
        //     Writes the text representation of the specified 32-bit unsigned integer value,
        //     followed by the current line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        public static void WriteLine(uint value)
        {
            Out.WriteLine(value);
        }

        //
        // Summary:
        //     Writes the text representation of the specified 64-bit signed integer value,
        //     followed by the current line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        public static void WriteLine(long value)
        {
            Out.WriteLine(value);
        }

        //
        // Summary:
        //     Writes the text representation of the specified 64-bit unsigned integer value,
        //     followed by the current line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        public static void WriteLine(ulong value)
        {
            Out.WriteLine(value);
        }

        //
        // Summary:
        //     Writes the text representation of the specified object, followed by the current
        //     line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        public static void WriteLine(object value)
        {
            Out.WriteLine(value);
        }

        //
        // Summary:
        //     Writes the specified string value, followed by the current line terminator, to
        //     the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        public static void WriteLine(string value)
        {
            Out.WriteLine(value);
        }

        //
        // Summary:
        //     Writes the text representation of the specified object, followed by the current
        //     line terminator, to the standard output stream using the specified format information.
        //
        // Parameters:
        //   format:
        //     A composite format string (see Remarks).
        //
        //   arg0:
        //     An object to write using format.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //
        //   T:System.ArgumentNullException:
        //     format is null.
        //
        //   T:System.FormatException:
        //     The format specification in format is invalid.
        public static void WriteLine(string format, object arg0)
        {
            Out.WriteLine(format, arg0);
        }

        //
        // Summary:
        //     Writes the text representation of the specified objects, followed by the current
        //     line terminator, to the standard output stream using the specified format information.
        //
        // Parameters:
        //   format:
        //     A composite format string (see Remarks).
        //
        //   arg0:
        //     The first object to write using format.
        //
        //   arg1:
        //     The second object to write using format.
        //
        //   arg2:
        //     The third object to write using format.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //
        //   T:System.ArgumentNullException:
        //     format is null.
        //
        //   T:System.FormatException:
        //     The format specification in format is invalid.
        public static void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            Out.WriteLine(format, arg0, arg1, arg2);
        }

        public static void WriteLine(string format, object arg0, object arg1, object arg2, object arg3)
        {
            Out.WriteLine(format, arg0, arg1, arg2, arg3);
        }

        //
        // Summary:
        //     Writes the text representation of the specified array of objects, followed by
        //     the current line terminator, to the standard output stream using the specified
        //     format information.
        //
        // Parameters:
        //   format:
        //     A composite format string (see Remarks).
        //
        //   arg:
        //     An array of objects to write using format.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //
        //   T:System.ArgumentNullException:
        //     format or arg is null.
        //
        //   T:System.FormatException:
        //     The format specification in format is invalid.
        public static void WriteLine(string format, params object[] arg)
        {
            if (arg == null)
            {   // avoid ArgumentNullException from String.Format
                // faster than Out.WriteLine(format, (Object)arg);
                Out.WriteLine(format, null, null);
            }
            else
            {
                Out.WriteLine(format, arg);
            }
        }

        //
        // Summary:
        //     Writes the specified subarray of Unicode characters, followed by the current
        //     line terminator, to the standard output stream.
        //
        // Parameters:
        //   buffer:
        //     An array of Unicode characters.
        //
        //   index:
        //     The starting position in buffer.
        //
        //   count:
        //     The number of characters to write.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     buffer is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     index or count is less than zero.
        //
        //   T:System.ArgumentException:
        //     index plus count specify a position that is not within buffer.
        //
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        public static void WriteLine(char[] buffer, int index, int count)
        {
            Out.WriteLine(buffer, index, count);
        }

        //
        // Summary:
        //     Writes the text representation of the specified System.Decimal value, followed
        //     by the current line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.

        public static void WriteLine(decimal value)
        {
            Out.WriteLine(value);
        }

        //
        // Summary:
        //     Writes the specified array of Unicode characters, followed by the current line
        //     terminator, to the standard output stream.
        //
        // Parameters:
        //   buffer:
        //     A Unicode character array.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        public static void WriteLine(char[] buffer)
        {
            Out.WriteLine(buffer);
        }

        //
        // Summary:
        //     Writes the specified Unicode character, followed by the current line terminator,
        //     value to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        public static void WriteLine(char value)
        {
            Out.WriteLine(value);
        }

        //
        // Summary:
        //     Writes the text representation of the specified Boolean value, followed by the
        //     current line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        public static void WriteLine(bool value)
        {
            Out.WriteLine(value);
        }

        //
        // Summary:
        //     Writes the text representation of the specified objects, followed by the current
        //     line terminator, to the standard output stream using the specified format information.
        //
        // Parameters:
        //   format:
        //     A composite format string (see Remarks).
        //
        //   arg0:
        //     The first object to write using format.
        //
        //   arg1:
        //     The second object to write using format.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        //
        //   T:System.ArgumentNullException:
        //     format is null.
        //
        //   T:System.FormatException:
        //     The format specification in format is invalid.
        public static void WriteLine(string format, object arg0, object arg1)
        {
            Out.WriteLine(format, arg0, arg1);
        }

        //
        // Summary:
        //     Writes the text representation of the specified double-precision floating-point
        //     value, followed by the current line terminator, to the standard output stream.
        //
        // Parameters:
        //   value:
        //     The value to write.
        //
        // Exceptions:
        //   T:System.IO.IOException:
        //     An I/O error occurred.
        public static void WriteLine(double value)
        {
            Out.Write(value);
        }

        public static int Read()
        {
            Thread.Sleep(int.MaxValue);
            throw new InvalidOperationException();
        }

        public static ConsoleKeyInfo ReadKey()
        {
            Thread.Sleep(int.MaxValue);
            throw new InvalidOperationException();
        }
        
        public static ConsoleKeyInfo ReadKey(bool intercept)
        {
            throw new SecurityException("Request for the permission of type 'System.Security.Permissions.SecurityPermission, failed.");
        }

        public static string ReadLine()
        {
            Thread.Sleep(int.MaxValue);
            throw new InvalidOperationException();
        }
        
        private static void InitOut()
        {
            _outputStream = new MemoryStream();
            _out = new Lazy<TextWriter>(() =>
            {
                var streamWriter = new StreamWriter(_outputStream)
                {
                    AutoFlush = true
                };
                return streamWriter;
            });
        }
    }
}
