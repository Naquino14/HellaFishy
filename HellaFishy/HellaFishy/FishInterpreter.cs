// but what if I like the boilerplate stuff :(
#pragma bruhhhhh

using System;
using c = System.Console;

namespace HellaFishy
{
    public class FishInterpreter
    {
        
        public static void Main(string[] args)
        {
            char[][] codeBox;
            // ok ig stage 1 is to actually read the file...
            if (args is not null && args.Length != 0)
            {
                if (new FileInfo(args[0]).Name.Contains(".fish"))
                {
                    // setup rows and columns
                    var rows = File.ReadAllLines(args[0]);
                    codeBox = new char[rows.Length][];
                    for (int i = 0; i < rows.Length; i++)
                        codeBox[i] = rows[i].ToCharArray();
                    c.Write(Interpret(codeBox));
                    return;
                }
                else
                    throw new InvalidDataException("File is not a fish file.");
            }
            else
                throw new ArgumentException("No file specified.");
        }

        static List<byte> stack = new();
        static byte? register;

        public static string Interpret(in char[][] codeBox) // TODO: GUI?
        {
            // https://esolangs.org/wiki/Fish

            // x coord, y coord, direction (can be ^,v,<,> ). direction defaults to the right
            (int x, int y, char d) pointer = new(0, 0, 'r');

            bool stopFlag = false;
            string res = "";

            Random rand = new();
            char ins = codeBox[pointer.x][pointer.y];

            try
            {
                while (!stopFlag)
                {
                    ins = codeBox[pointer.x][pointer.y];
                    // execute instruction
                    switch (ins)
                    {
                        #region movement
                        case ' ':
                            continue;
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
                            byte p = PopGet();
                            if (p != 0x0)
                                continue;
                            pointer.x = pointer.d switch { '<' => pointer.x - 1, '>' => pointer.x + 1, _ => pointer.x };
                            pointer.y = pointer.d switch { '^' => pointer.y - 1, 'v' => pointer.y + 1, _ => pointer.y };
                            break;
                        case '.': // woogie lmao
                            pointer.x = PopGet();
                            pointer.y = PopGet();
                            break;
                        #endregion

                        #region literals and operators

                        // TODO: this

                        #endregion

                        #region reflection and misc
                        case ';':
                            return res;
                        default:
                            throw new FishyException();
                            #endregion
                    }

                    // move the pointer


                    // translate the pointer in-bounds
                    pointer.x %= codeBox.Length;
                    pointer.y %= codeBox[0].Length;
                }
            } catch (FishyException f)
            {
                return $"Something smells fishy!\nError parsing instruction at " +
                                $"({pointer.x}, {pointer.y})=>" +
                                $"{pointer.d switch { '^' => "up", 'v' => "down", '<' => "left", '>' => "right", _ => "right" }}" +
                                $" | instruction: \"{ins}\".\n{f}";
            }
            
            // TODO:
            throw new NotImplementedException();
        }

        static byte PopGet()
        {
            try
            { return stack.First(); }
            catch (InvalidOperationException u)
            { throw new FishyException(); }
        }

        static void Pop()
        {
            try
            { stack.RemoveAt(0); }
            catch (ArgumentOutOfRangeException u)
            { throw new FishyException(); }
        }

        public class FishyException : Exception
        {
            public FishyException() : base() { }
            public FishyException(string msg) : base(msg) { }
        }
    }
}