using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace File_Mgr
{
    class ConsoleUtils
    {
        //[DllImport("kernel32.dll", SetLastError = true)]
        //static extern IntPtr GetStdHandle(int nStdHandle);

        //[DllImport("kernel32.dll", SetLastError = true)]
        //static extern bool ReadConsoleOutputCharacter(
        //    IntPtr hConsoleOutput,
        //    //[Out] StringBuilder lpCharacter,
        //    //[Out] char[] lpCharacter,
        //    [Out] byte[] lpCharacter,
        //    uint length,
        //    COORD bufferCoord,
        //    out uint lpNumberOfCharactersRead);

        //[StructLayout(LayoutKind.Sequential)]
        //public struct COORD
        //{
        //    public short X;
        //    public short Y;
        //}

        //public static char ReadCharacterAt(int x, int y)
        //{
        //    IntPtr consoleHandle = GetStdHandle(-11);
        //    if (consoleHandle == IntPtr.Zero)
        //    {
        //        return '\0';
        //    }
        //    COORD position = new COORD
        //    {
        //        X = (short)x,
        //        Y = (short)y
        //    };
        //    //StringBuilder result = new StringBuilder(1);
        //    //char[] result = new char[20];
        //    byte[] result = new byte[20];
        //    uint read = 0;
        //    if (ReadConsoleOutputCharacter(consoleHandle, result, 1, position, out read))
        //    {
        //        return (char)result[0];
        //    }
        //    else
        //    {
        //        return '\0';
        //    }
        //}
    }
}
