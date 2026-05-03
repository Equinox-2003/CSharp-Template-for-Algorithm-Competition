using AtCoder;
using AtCoder.Internal;
using Equinox.IO;
using Info = (int minVal, int maxIdx);
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;
using System.Text;
namespace TemplateA {
    public class SolutionA
    {
        public void Solve() {
            int n = br.ReadInt32(), q = br.ReadInt32();
            int[] a = br.ReadInt32(n);
            Segtree<Info, Op> seg = new(Enumerable.Range(0, n).Select(i => (a[i], i)).ToArray().AsSpan());
            while (q-- > 0) {
                if (br.ReadInt32() == 1) {
                    int i = br.ReadInt32() - 1;
                    seg[i] = (br.ReadInt32(), i);
                } else {
                    int l = br.ReadInt32() - 1, r = br.ReadInt32();
                    int i = seg.MaxRight(l, info => info.minVal > info.maxIdx - l);
                    if (i < r) {
                        var info = seg.Prod(l, i + 1);
                        bw.AppendLine(info.minVal == info.maxIdx - l ? 1 : 0);
                    } else {
                        bw.AppendLine(0);
                    }
                }
            }
        }
        public struct Op : ISegtreeOperator<Info> {
            public Info Identity => (int.MaxValue, -1);
            public Info Operate(Info left, Info right) => (Math.Min(left.minVal, right.minVal), Math.Max(left.maxIdx, right.maxIdx));
        }
        public void Main()
        {
            int T = 1;
             T = br.ReadInt32();
            while (T-- > 0)
            {
                Solve();
            }
            bw.Output();
            SourceExpander.Expander.Expand();
        }
        public SolutionA() {}

        public SolutionA(Stream inputStream = null, Stream outputStream = null)
        {
            br = new BufferedReader(inputStream ?? Console.OpenStandardInput(), 0);
            bw = new BufferedWriter(outputStream ?? Console.OpenStandardOutput());
        }
        public static void Main(string[] args) => new SolutionA().Main();
        private readonly BufferedReader br;
        private readonly BufferedWriter bw; 
    }
}
#region Expanded by https://github.com/kzrnm/SourceExpander
namespace AtCoder{public interface ISegtreeOperator<T>{T Identity{get;}T Operate(T x,T y);}}
namespace AtCoder{public class Segtree<TValue,TOp>where TOp:struct,ISegtreeOperator<TValue>{private static readonly TOp op=default;public int Length{get;}internal readonly int log;internal readonly int size;public readonly TValue[]d;public Segtree(int n){Length=n;log=InternalBit.CeilPow2(n);size=1<<log;d=new TValue[2*size];Array.Fill(d,op.Identity);}public Segtree(TValue[]v):this((ReadOnlySpan<TValue>)v){}public Segtree(Span<TValue>v):this((ReadOnlySpan<TValue>)v){}public Segtree(ReadOnlySpan<TValue>v):this(v.Length){v.CopyTo(d.AsSpan(size));for(int i=size-1;i>=1;i--){Update(i);}}[MethodImpl(256)]public void Update(int k)=>d[k]=op.Operate(d[2*k],d[2*k+1]);public TValue this[int p]{[MethodImpl(256)]set{p+=size;d[p]=value;for(int i=1;i<=log;i++)Update(p>>i);}[MethodImpl(256)]get{return d[p+size];}}[MethodImpl(256)]public TValue Slice(int l,int len)=>Prod(l,l+len);[MethodImpl(256)]public TValue Prod(int l,int r){TValue sml=op.Identity,smr=op.Identity;l+=size;r+=size;while(l<r){if((l&1)!=0)sml=op.Operate(sml,d[l++]);if((r&1)!=0)smr=op.Operate(d[ --r],smr);l>>=1;r>>=1;}return op.Operate(sml,smr);}public TValue AllProd=>d[1];[MethodImpl(256)]public int MaxRight(int l,Predicate<TValue>f){if(l==Length)return Length;l+=size;var sm=op.Identity;do{while(l%2==0)l>>=1;if(!f(op.Operate(sm,d[l]))){while(l<size){l=(2*l);if(f(op.Operate(sm,d[l]))){sm=op.Operate(sm,d[l]);l++;}}return l-size;}sm=op.Operate(sm,d[l]);l++;}while((l&-l)!=l);return Length;}[MethodImpl(256)]public int MinLeft(int r,Predicate<TValue>f){if(r==0)return 0;r+=size;var sm=op.Identity;do{r--;while(r>1&&(r%2)!=0)r>>=1;if(!f(op.Operate(d[r],sm))){while(r<size){r=(2*r+1);if(f(op.Operate(d[r],sm))){sm=op.Operate(d[r],sm);r--;}}return r+1-size;}sm=op.Operate(d[r],sm);}while((r&-r)!=r);return 0;}}}
namespace AtCoder.Internal{public static class InternalBit{[MethodImpl(256)]public static uint ExtractLowestSetBit(int n){if(Bmi1.IsSupported){return Bmi1.ExtractLowestSetBit((uint)n);}return(uint)(n&-n);}[MethodImpl(256)]public static int Bsf(uint n){return BitOperations.TrailingZeroCount(n);}[MethodImpl(256)]public static int CeilPow2(int n){var un=(uint)n;if(un<=1)return 0;return BitOperations.Log2(un-1)+1;}}}
namespace Equinox { namespace IO { public class BufferedReader : IDisposable { private StreamReader sr; private char[] buffer; private int S = 0, T = 0; private int Peek() { if (buffer == null) { return sr.Peek(); }  if (S == T) { T = sr.ReadBlock(buffer.AsSpan()); if (T == 0) return -1; S = 0; }  return buffer[S]; }  private int Read() { if (buffer == null) { return sr.Read(); }  int p = Peek(); if (p >= 0) ++S; return p; }  private string readLine(bool skipSpace) { int c; StringBuilder sb = new(); while ((c = Read()) != -1) { if (c == '\r') continue; if (c == ' ' && skipSpace) { if (sb.Length == 0) { continue; }  break; }  if (c == '\n') { if (sb.Length > 0) break; else continue; }  sb.Append((char)c); }  return sb.ToString(); }  public string ReadString() => readLine(true); public string ReadLine() => readLine(false); public BufferedReader(Stream stream, int capacity) { sr = new(stream); if (capacity > 0) { buffer = new char[capacity]; } else { buffer = null; } }  public bool EndOfStream { get => buffer == null ? sr.EndOfStream : Peek() == -1; }  public T ReadInt<T>() where T : INumber<T> { char c; T res = default; T sign = T.CreateChecked(1); while (!EndOfStream && char.IsWhiteSpace((char)Peek())) Read(); if (!EndOfStream && (char)Peek() == '-') { Read(); sign = T.CreateChecked(-1); }  while (!EndOfStream && char.IsDigit((char)Peek())) { c = (char)Read(); res = res * T.CreateChecked(10) + T.CreateChecked(c - '0'); }  return res * sign; }  public T[] ReadArray<T>(int cnt) where T : INumber<T> => ReadArray<T>(cnt, 0); public T[] ReadArray<T>(int cnt, int startIndex) where T : INumber<T> { T[] arr = new T[cnt + startIndex]; for (int i = 0; i < cnt; ++i) arr[i + startIndex] = ReadInt<T>(); return arr; }  public int ReadInt32() => ReadInt<int>(); public long ReadInt64() => ReadInt<long>(); public int[] ReadInt32(int cnt) => ReadArray<int>(cnt); public long[] ReadInt64(int cnt) => ReadArray<long>(cnt); public double ReadDouble() { double res = ReadInt64(); if ((char)Peek() == '.') { Read(); double tail = 0.1; while (!EndOfStream && char.IsDigit((char)Peek())) { char c = (char)Read(); res += (c - '0') * tail; tail *= 0.1; } }  return res; }  public void Dispose() { sr.Close(); } }  public class BufferedWriter : IDisposable { private StringBuilder sb = new(); private StreamWriter sw; public BufferedWriter() : this(Console.OpenStandardOutput()) { }  public BufferedWriter(Stream stream) { sw = new(stream); }  public void Append(object val) => sb.AppendFormat("{0}", val); public void AppendLine(object val) => sb.AppendFormat("{0}\n", val); public void AppendYes(bool suc) => sb.AppendLine(suc ? "YES" : "NO"); public void AppendJoin<T>(char delim, IEnumerable<T> values) => sb.AppendJoin(delim, values).AppendLine(); public void AppendJoin<T>(string? delim, IEnumerable<T> values) => sb.AppendJoin(delim, values).AppendLine(); public void AppendFormat(string format, params object[] args) => sb.AppendFormat(format, args); public void Output() { sw.Write(sb.ToString()); sw.Flush(); sb.Clear(); }  public void Dispose() { sw.Close(); } }  public static class ArrayExt { public static Span<T> AsSpan<T>(this T[, ] array) => asSpan<T>(array); public static Span<T> AsSpan<T>(this T[,, ] array) => asSpan<T>(array); static Span<T> asSpan<T>(Array array) => System.Runtime.InteropServices.MemoryMarshal.CreateSpan(ref System.Runtime.CompilerServices.Unsafe.As<byte, T>(ref System.Runtime.InteropServices.MemoryMarshal.GetArrayDataReference(array)), array.Length); public static int lower_bound<T>(this IList<T> a, T M) where T : IComparable<T> => lower_bound<T>(a, 0, a.Count, M); public static int lower_bound<T>(this IList<T> a, int lo, int hi, T M) where T : IComparable<T> { while (lo < hi) { int x = (lo + hi) / 2; if (a[x].CompareTo(M) >= 0) { hi = x; } else { lo = x + 1; } }  return hi; }  public static int upper_bound<T>(this IList<T> a, T M) where T : IComparable<T> => upper_bound<T>(a, 0, a.Count, M); public static int upper_bound<T>(this IList<T> a, int lo, int hi, T M) where T : IComparable<T> { while (lo < hi) { int x = (lo + hi) / 2; if (a[x].CompareTo(M) > 0) { hi = x; } else { lo = x + 1; } }  return hi; } } } }
namespace SourceExpander{public class Expander{[Conditional("EXP")]public static void Expand(string inputFilePath=null,string outputFilePath=null,bool ignoreAnyError=true){}public static string ExpandString(string inputFilePath=null,bool ignoreAnyError=true){return "";}}}
#endregion Expanded by https://github.com/kzrnm/SourceExpander
