// but what if I like the boilerplate stuff :(
#pragma bruhhhhh

using System;
using c = System.Console;

namespace HellaFishy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            char[][] codeBox;
            // ok ig stage 1 is to actually read the file...

            var exit = HandleArgs(args, out var stepSpeed, out var enableStep, out var stdin, out var breakInst);
            if (exit)
                return;
            
            c.Clear();
            c.CursorVisible = true;
            if (args is not null && args.Length != 0)
            {
                if (new FileInfo(args[^1]).Name.EndsWith(".fish"))
                {
                    // setup rows and columns
                    var rows = File.ReadAllLines(args[^1]);
                    codeBox = new char[rows.Length][];
                    for (int i = 0; i < rows.Length; i++)
                        codeBox[i] = rows[i].ToCharArray();
                    if (stdin is null)
                    {
                        c.WriteLine("Enter stdin string (optional): ");
                        stdin = c.ReadLine();
                    }
                    c.CursorVisible = false;
                    c.Write($"{(enableStep || breakInst is not null ? "\n\n" : "")}" +
                        $"Output: {FishInterpreter.Interpret(codeBox, stdin, enableStep ? 1 : stepSpeed, enableStep, breakInst)}\n");
                    return;
                }
                else
                    throw new InvalidDataException("File is not a fish file.");
            }
            else
                throw new ArgumentException("No file specified.");
        }

        private static bool HandleArgs(in string[] args, out int stepSpeed, out bool enableStep, out string? stdin, out char? breakChar)
        {
            stepSpeed = 0;
            enableStep = false;
            stdin = null;
            breakChar = null;

            bool argSkipFlag = false;

            for (int i = 0; i < args.Length; i++)
            {
                try
                {
                    if (argSkipFlag)
                    { argSkipFlag = !argSkipFlag; continue; }
                    if (args is null || args[0] == "" || args[i] == "-help")
                    {
                        c.WriteLine("usage: HellaFishy [-d visual_enable_set_speed_ms] [-b (break_mode)] [-c break_mode_char] [-s stdin] file");
                        return true;
                    }
                    else if (args[i] == "-d")
                    {
                        argSkipFlag = true;
                        stepSpeed = Math.Abs(int.Parse(args[i + 1]));
                    }
                    else if (args[i] == "-s")
                    {
                        argSkipFlag = true;
                        stdin = args[i + 1];
                    }
                    else if (args[i] == "-b")
                        enableStep = true;
                    else if (args[i] == "-c")
                    {
                        argSkipFlag = true;
                        breakChar = args[i + 1][0];
                    }
                    else
                        argSkipFlag = true;
                }
                catch (Exception)
                { c.WriteLine("usage: HellaFishy [-d visual_enable_set_speed_ms] [-b (break_mode)] [-c break_mode_char] [-s stdin] file"); }
            }
            return false;
        }
    }
}