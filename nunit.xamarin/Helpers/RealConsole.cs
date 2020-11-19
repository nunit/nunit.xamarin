using System;
using System.IO;

public static class RealConsole
{
    private static TextWriter _realConsole;

    public static void Init(TextWriter writer)
    {
        _realConsole = writer;
    }

    public static void WriteLine(string value)
    {
        _realConsole.WriteLine(value);
    }
}
