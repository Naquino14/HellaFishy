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

        static List<int> stack = new();
        static List<List<int>> coStacks = new();
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
                            int p = PopGet();
                            if (p != 0)
                                continue;
                            pointer.x = pointer.d switch { '<' => pointer.x - 1, '>' => pointer.x + 1, _ => pointer.x };
                            pointer.y = pointer.d switch { '^' => pointer.y - 1, 'v' => pointer.y + 1, _ => pointer.y };
                            break;
                        case '.': // woogie lmao
                            pointer.x = PopGet();
                            pointer.y = PopGet();
                            break;

                        #endregion

                        #region Literals and operators

                        // todo

                        #endregion

                        #region Stack manipulation

                        case ':':
                            try
                            { Push(stack.Last()); }
                            catch (InvalidOperationException)
                            { throw new FishyException("Could not dupe the top value, the stack was empty."); }
                            break;
                        case '~':
                            Pop();
                            break;
                        case '$':
                            if (stack.Count < 2)
                                throw new FishyException("Stack does not contain enough elements to execute this instruction.");
                            var tmp = stack.Last();
                            stack[^1] = stack[^2]; // oh my god LMAOOO
                            stack[^2] = tmp;
                            break;
                        case '@':
                            if (stack.Count < 3)
                                throw new FishyException("Stack does not contain enough elements to execute this instruction.");
                            var tmp1 = stack.Last();
                            stack[^1] = stack[^3];
                            stack[^2] = stack[^3];
                            stack[^3] = tmp1;
                            break;
                        case '}':
                            if (stack.Count == 0)
                                throw new FishyException("Could not shift stack, the stack was empty.");
                            stack = stack.Skip(stack.Count - 1).Concat(stack.Take(stack.Count - 1)).ToList();
                            break;
                        case '{':
                            if (stack.Count == 0)
                                throw new FishyException("Could not shift stack, the stack was empty.");
                            stack = stack.Skip(1).Concat(stack.Take(1)).ToList();
                            break;
                        case 'r': // TODO: test
                            if (stack.Count == 0)
                                throw new FishyException("Could not reverse stack, the stack was empty.");
                            if (coStacks.Count == 0)
                                stack.Reverse();
                            else
                                coStacks.Last().Reverse();
                            break;
                        case 'l':
                            Push(stack.Count);
                            break;
                        case '[': // TODO: test
                            int c = PopGet();
                            coStacks.Add(new());
                            for (int i = 0; i < c; i++)
                                coStacks.Last().Add(stack[^i]);
                            break;
                        case ']': // TODO: test
                            if (coStacks.Count == 0)
                                throw new FishyException("Could not push costack, the costacks list was empty.");
                            foreach (var i in coStacks.Last())
                                stack.Add(i);
                            break;

                        #endregion

                        #region Reflection and misc
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

        static int PopGet()
        {
            try
            {
                int l = stack.Last();
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

        static void Push(int b) => stack.Insert(0, b);

        public class FishyException : Exception
        {
            public FishyException() : base() { }
            public FishyException(string msg) : base(msg) { }
        }
    }
}