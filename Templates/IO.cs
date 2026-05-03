using System.Numerics;
using System.Text;

namespace Equinox {
    namespace IO {
        #region IO
        public class BufferedReader : IDisposable {
            private StreamReader sr;
            private char[] buffer;
            private int S = 0, T = 0;

            private int Peek() {
                if (buffer == null) {
                    return sr.Peek();
                }
                if (S == T) {
                    T = sr.ReadBlock(buffer.AsSpan());
                    if (T == 0) return -1;
                    S = 0;
                }
                return buffer[S];
            }

            private int Read() {
                if (buffer == null) {
                    return sr.Read();
                }
                int p = Peek();
                if (p >= 0) ++S;
                return p;
            }

            private string readLine(bool skipSpace) {
                int c;
                StringBuilder sb = new();
                while ((c = Read()) != -1) {
                    if (c == '\r') continue; // omit '\r'
                    if (c == ' ' && skipSpace) {
                        if (sb.Length == 0) {
                            continue;
                        }
                        break;
                    }
                    if (c == '\n') {
                        if (sb.Length > 0) break;
                        else continue; // omit empty lines
                    }
                    sb.Append((char)c);
                }
                return sb.ToString();
            }

            public string ReadString() => readLine(true);
            public string ReadLine() => readLine(false);

            public BufferedReader(Stream stream, int capacity) {
                sr = new(stream);
                if (capacity > 0) {
                    buffer = new char[capacity];
                } else {
                    buffer = null;
                }
            }

            public bool EndOfStream { get => buffer == null ? sr.EndOfStream : Peek() == -1; }

            public T ReadInt<T>() where T : INumber<T> {
                char c;
                T res = default;
                T sign = T.CreateChecked(1);
                while (!EndOfStream && char.IsWhiteSpace((char)Peek())) Read();
                if (!EndOfStream && (char)Peek() == '-') {
                    Read();
                    sign = T.CreateChecked(-1);
                }
                while (!EndOfStream && char.IsDigit((char)Peek())) {
                    c = (char)Read();
                    res = res * T.CreateChecked(10) + T.CreateChecked(c - '0');
                }
                return res * sign;
            }

            public T[] ReadArray<T>(int cnt) where T : INumber<T> => ReadArray<T>(cnt, 0);
            public T[] ReadArray<T>(int cnt, int startIndex) where T : INumber<T> {
                T[] arr = new T[cnt + startIndex];
                for (int i = 0; i < cnt; ++i) arr[i + startIndex] = ReadInt<T>();
                return arr;
            }

            public int ReadInt32() => ReadInt<int>();
            public long ReadInt64() => ReadInt<long>();
            public int[] ReadInt32(int cnt) => ReadArray<int>(cnt);
            public long[] ReadInt64(int cnt) => ReadArray<long>(cnt);

            public double ReadDouble() {
                double res = ReadInt64();
                if ((char)Peek() == '.') {
                    Read();
                    double tail = 0.1;
                    while (!EndOfStream && char.IsDigit((char)Peek())) {
                        char c = (char)Read();
                        res += (c - '0') * tail;
                        tail *= 0.1;
                    }
                }
                return res;
            }
            public void Dispose() {
                sr.Close();
            }
        }

        public class BufferedWriter : IDisposable {
            private StringBuilder sb = new();
            private StreamWriter sw;
            public BufferedWriter() : this(Console.OpenStandardOutput()) { }
            public BufferedWriter(Stream stream) { sw = new(stream); }

            public void Append(object val) => sb.AppendFormat("{0}", val);

            public void AppendLine(object val) => sb.AppendFormat("{0}\n", val);

            public void AppendYes(bool suc) => sb.AppendLine(suc ? "YES" : "NO");

            public void AppendJoin<T>(char delim, IEnumerable<T> values) => sb.AppendJoin(delim, values).AppendLine();
            public void AppendJoin<T>(string? delim, IEnumerable<T> values) => sb.AppendJoin(delim, values).AppendLine();
            public void AppendFormat(string format, params object[] args) => sb.AppendFormat(format, args);
            public void Output() {
                sw.Write(sb.ToString());
                sw.Flush();
                sb.Clear();
            }
            public void Dispose() {
                sw.Close();
            }
        }
        #endregion
    }
}
