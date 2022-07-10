#pragma success!!!!

using System;
using c = System.Console;

namespace HellaFishy
{
    public class FishInterpreter
    {
        #region exception messages
        static readonly string su2Ex = "Stack does not contain enough elements to execute this instruction.",
            sueEx = "The stack was empty.";
        #endregion
        
        static List<float> stack = new();
        static List<List<float>> coStacks = new();

        static float? register;
        static List<float?> coRegistries = new();

        static List<char> stdin = new();

        static bool contiinue = false;

        private static void Draw(in char[][] codeBox, Pointer p, ref bool enableStep, char inst, ref char? breakInst)
        {
            c.SetCursorPosition(0, 0);
            for (int i = 0; i < codeBox.Length; i++)
            {
                for (int j = 0; j < codeBox[i].Length; j++)
                {
                    c.BackgroundColor = i == p.y && j == p.x ? ConsoleColor.Green : ConsoleColor.Black;
                    c.ForegroundColor = i == p.y && j == p.x ? ConsoleColor.Black : ConsoleColor.White;
                    c.Write(codeBox[i][j]);
                }
                c.BackgroundColor = ConsoleColor.Black;
                c.ForegroundColor = ConsoleColor.White;
                c.WriteLine();
            }
            c.WriteLine();

            ClearLine();
            c.Write("Stack: ");
            foreach (var item in stack)
                c.Write(item + " ");
            c.WriteLine();

            c.WriteLine("\nCoStacks: ");
            for (int i = 0; i < coStacks.Count; i++)
            {
                ClearLine();
                c.Write("CoStack " + i + ": ");
                foreach (var item in coStacks[i])
                    c.Write($"{item} ");
                c.WriteLine();
            }

            c.WriteLine($"\nRegistry: {register}");

            c.WriteLine("\nCoRegistries: ");
            for (int i = 0; i < coRegistries.Count; i++)
            {
                ClearLine();
                c.Write("CoRegistry " + i + ": ");
                c.Write(coRegistries[i] + " ");
                c.WriteLine();
            }
            c.WriteLine("\n============================================================\n");
            if (enableStep && !contiinue)
            {
                ClearLine();
                c.Write("Press any key to step... Press c to continue.");
                if (c.ReadKey().KeyChar == 'c')
                    contiinue = true;
            }
            else if (enableStep && contiinue && breakInst is null)
            {
                ClearLine();
                c.Write("Press any key to step... Press c to continue.");
            }
            else if (breakInst is not null && inst == breakInst)
            {
                ClearLine();
                c.Write("Breakpoint hit. Press any key to step... Press c to continue. Press ` to edit breakpoint instruction.");
                var read = c.ReadKey();
                if (read.KeyChar == '`')
                {
                    enableStep = true;
                    contiinue = false;
                    ClearLine();
                    c.Write("Enter a new breakpoint instruction: ");
                    breakInst = c.ReadKey().KeyChar;
                    ClearLine();
                } else if (read.KeyChar != 'c')
                {
                    enableStep = true;
                    contiinue = false;
                }
            }
            else if (enableStep)
            {
                ClearLine();
                c.Write("Press any key to step... Press c to continue.");
            }
        }

        public static void ClearLine()
        {
            int prev = c.CursorTop;
            c.SetCursorPosition(0, c.CursorTop);
            c.Write(new string(' ', c.WindowWidth));
            c.SetCursorPosition(0, prev);
        }

        public static string Interpret(in char[][] codeBox, in string? inp, int delayMs, bool enableStep, char? breakIns) // TODO: GUI?
        {
            // https://esolangs.org/wiki/Fish

            // x coord, y coord, direction (can be ^,v,<,> ). direction defaults to the right
            Pointer pointer = new(0, 0, '>');

            bool stopFlag = false;
            string res = "";
            bool sPush = false;

            Random rand = new();
            char ins = codeBox[pointer.y][pointer.x]; // TODO!!!! FIX THIS CRAP. x and y are reversed.

            if (inp is not null && inp!.Length != 0)
            { stdin = inp.ToCharArray().ToList(); stdin.Reverse(); }

            c.Clear();
            if (delayMs > 0)
                Draw(codeBox, pointer, ref enableStep, ins, ref breakIns);

            try
            {
                while (!stopFlag)
                {
                    ins = codeBox[pointer.y][pointer.x];
                    // execute instruction
                    if (!sPush)
                    {
                        switch (ins)
                        {
                            #region movement

                            case ' ':
                                break;
                            case '^':
                                pointer.d = ins;
                                break;
                            case 'v':
                                pointer.d = ins;
                                break;
                            case '<':
                                pointer.d = ins;
                                break;
                            case '>':
                                pointer.d = ins;
                                break;
                            // mirro
                            case '/':
                                pointer.d = pointer.d == '^'
                                    ? pointer.d = '>'
                                    : pointer.d == 'v'
                                        ? pointer.d = '<'
                                        : pointer.d == '<'
                                            ? pointer.d = 'v'
                                            : '^'; // tehehee
                                break;
                            case '\\':
                                pointer.d = pointer.d == '^'
                                    ? pointer.d = '<'
                                    : pointer.d == 'v'
                                        ? pointer.d = '>'
                                        : pointer.d == '<'
                                            ? pointer.d = '^'
                                            : 'v';
                                break;
                            case '_':
                                pointer.d = pointer.d == '^'
                                    ? pointer.d = 'v'
                                    : pointer.d == 'v'
                                        ? pointer.d = '^'
                                        : pointer.d;
                                break;
                            case '|':
                                pointer.d = pointer.d == '<'
                                    ? pointer.d = '>'
                                    : pointer.d == '>'
                                        ? pointer.d = '<'
                                        : pointer.d;
                                break;
                            case '#': // what
                                pointer.d = rand.Next(3) switch { 0 => '^', 1 => 'v', 2 => '<', _ => '>' };
                                break;
                            case 'x':
                                pointer.d = rand.Next(3) switch { 0 => '^', 1 => 'v', 2 => '<', _ => '>' };
                                break;
                            case '!': // its too late at night for me to explain how this works just trust it
                                pointer.x = pointer.d switch { '<' => pointer.x - 1, '>' => pointer.x + 1, _ => pointer.x };
                                pointer.y = pointer.d switch { '^' => pointer.y - 1, 'v' => pointer.y + 1, _ => pointer.y };
                                break;
                            case '?':
                                var p = PopGet();
                                if (p != 0)
                                    break;
                                pointer.x = pointer.d switch { '<' => pointer.x - 1, '>' => pointer.x + 1, _ => pointer.x };
                                pointer.y = pointer.d switch { '^' => pointer.y - 1, 'v' => pointer.y + 1, _ => pointer.y };
                                break;
                            case '.': // woogie lmao
                                pointer.x = (int)PopGet();
                                pointer.y = (int)PopGet();
                                break;

                            #endregion

                            #region Literals and operators

                            case '+':
                                if (stack.Count < 2)
                                    throw new FishyException(su2Ex);
                                Push(PopGet() + PopGet());
                                break;
                            case '-':
                                if (stack.Count < 2)
                                    throw new FishyException(su2Ex);
                                Push(PopGet() - PopGet());
                                break;
                            case '*':
                                if (stack.Count < 2)
                                    throw new FishyException(su2Ex);
                                Push(PopGet() * PopGet());
                                break;
                            case ',':
                                if (stack.Count < 2)
                                    throw new FishyException(su2Ex);
                                Push(PopGet() / PopGet());
                                break;
                            case '%':
                                if (stack.Count < 2)
                                    throw new FishyException(su2Ex);
                                Push(PopGet() % PopGet());
                                break;
                            case '=':
                                if (stack.Count < 2)
                                    throw new FishyException(su2Ex);
                                Push(PopGet() == PopGet() ? 1 : 0);
                                break;
                            case ')':
                                if (stack.Count < 2)
                                    throw new FishyException(su2Ex);
                                Push(PopGet() > PopGet() ? 1 : 0);
                                break;
                            case '(':
                                if (stack.Count < 2)
                                    throw new FishyException(su2Ex);
                                Push(PopGet() < PopGet() ? 1 : 0);
                                break;
                            case '"':
                                sPush = true;
                                break;
                            case '\'':
                                sPush = true;
                                break;

                            #endregion

                            #region Stack manipulation

                            case ':':
                                try
                                { Push(stack.Last()); }
                                catch (InvalidOperationException)
                                { throw new FishyException(sueEx); }
                                break;
                            case '~':
                                Pop();
                                break;
                            case '$':
                                if (stack.Count < 2)
                                    throw new FishyException(su2Ex);
                                var tmp = stack.Last();
                                stack[^1] = stack[^2]; // oh my god LMAOOO
                                stack[^2] = tmp;
                                break;
                            case '@':
                                if (stack.Count < 3)
                                    throw new FishyException(su2Ex);
                                var tmp1 = stack.Last();
                                stack[^1] = stack[^3];
                                stack[^2] = stack[^3];
                                stack[^3] = tmp1;
                                break;
                            case '}':
                                if (stack.Count == 0)
                                    throw new FishyException(sueEx);
                                stack = stack.Skip(stack.Count - 1).Concat(stack.Take(stack.Count - 1)).ToList();
                                break;
                            case '{':
                                if (stack.Count == 0)
                                    throw new FishyException(sueEx);
                                stack = stack.Skip(1).Concat(stack.Take(1)).ToList();
                                break;
                            case 'r': // TODO: test
                                if (stack.Count == 0)
                                    throw new FishyException(sueEx);
                                if (coStacks.Count == 0)
                                    stack.Reverse();
                                else
                                    coStacks.Last().Reverse();
                                break;
                            case 'l':
                                Push(stack.Count);
                                break;
                            case '[': // TODO: test
                                int c = (int)PopGet();
                                if (stack.Count < c)
                                    throw new FishyException("Stack did not contain enough elements to perform this instruction.");
                                coStacks.Add(new());
                                for (int i = 0; i < c; i++)
                                    coStacks.Last().Add(stack[^i]);
                                break;
                            case ']': // TODO: test
                                if (coStacks.Count == 0)
                                    throw new FishyException("Could not push costack, the costacks list was empty."); // todo: change this to clear stack and register
                                foreach (var i in coStacks.Last())
                                    stack.Add(i);
                                break;

                            #endregion

                            #region Input/output

                            case 'o':
                                res += (char)PopGet();
                                break;
                            case 'n': // todo test if whole numbers return decimals
                                res += PopGet();
                                break;
                            case 'i':
                                if (stdin.Count == 0)
                                { Push(-1); break; }
                                Push(stdin.First());
                                stdin.RemoveAt(0);
                                break;

                            #endregion

                            #region Reflection and misc

                            case '&':
                                if (register is null && stack.Count == 0)
                                    throw new FishyException(sueEx);
                                else if (register is not null)
                                {
                                    Push((float)register);
                                    register = null;
                                }
                                else
                                    register = PopGet();
                                break;
                            case 'g':
                                if (stack.Count < 2)
                                    throw new FishyException(su2Ex);
                                {
                                    int x = (int)PopGet(),
                                        y = (int)PopGet();
                                    var g = codeBox[y][x];
                                    Push(g == ' ' ? 0 : g);
                                }
                                break;
                            case 'p':
                                if (stack.Count < 3)
                                    throw new FishyException(su2Ex);
                                {
                                    int x = (int)PopGet(),
                                        y = (int)PopGet();
                                    Push(codeBox[y][x] = (char)PopGet());
                                }
                                break;
                            case ';':
                                return res;
                            default:
                                if (byte.TryParse($"{ins}", out var b))
                                { Push(b); break; }
                                else
                                    throw new FishyException();

                                #endregion
                        }
                    }
                    else if (ins == '"' || ins == '\'')// pushing every char as a string
                        sPush = false;
                    else
                        Push(ins);

                    // move the pointer
                    pointer.x = pointer.d switch { '<' => pointer.x - 1, '>' => pointer.x + 1, _ => pointer.x };
                    pointer.y = pointer.d switch { '^' => pointer.y - 1, 'v' => pointer.y + 1, _ => pointer.y };

                    // translate the pointer in-bounds
                    pointer.y = TranslateInBounds(pointer.y, codeBox.Length);
                    pointer.x = TranslateInBounds(pointer.x, codeBox[0].Length);

                    if (delayMs > 0)
                    {
                        char nIns = codeBox
                            [TranslateInBounds(pointer.d switch { '^' => pointer.y - 1, 'v' => pointer.y + 1, _ => pointer.y }, codeBox.Length)]
                            [TranslateInBounds(pointer.d switch { '<' => pointer.x - 1, '>' => pointer.x + 1, _ => pointer.x }, codeBox[0].Length)];
                        Draw(codeBox, pointer, ref enableStep, nIns, ref breakIns);
                        Thread.Sleep(delayMs);
                    }
                }
            }
            catch (IndexOutOfRangeException)
            { throw new FishyException("Coordinates were outside of the codebox."); }
            catch (FishyException f)
            {
                return $"Something smells fishy!\nError parsing instruction at " +
                                $"({pointer.x}, {pointer.y})=>" +
                                $"{pointer.d switch { '^' => "up", 'v' => "down", '<' => "left", '>' => "right", _ => "right" }}" +
                                $" | instruction: \"{ins}\".\n{f}";
            }

            // TODO:
            throw new NotImplementedException();
        }

        static int TranslateInBounds(int component, int limit) => component < 0 ? limit + component : component % limit;


        static float PopGet()
        {
            try
            {
                float l = stack.Last();
                Pop();
                return l;
            }
            catch (InvalidOperationException)
            { throw new FishyException(); }
        }

        static void Pop()
        {
            try
            { stack.RemoveAt(stack.Count - 1); }
            catch (ArgumentOutOfRangeException)
            { throw new FishyException(); }
        }

        static void Push(float e) => stack.Add(e); // keeps falling out to here????

        public class FishyException : Exception
        {
            public FishyException() : base() { }
            public FishyException(string msg) : base(msg) { }
        }
    }
}
