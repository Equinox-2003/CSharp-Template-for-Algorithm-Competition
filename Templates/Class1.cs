using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace Equinox
{
    namespace IO
    {
        #region IO
        public class BufferedReader : IDisposable
        {
            private StreamReader sr;
            private char[] buffer;
            private int S = 0, T = 0;

            private int Peek()
            {
                if (buffer == null)
                {
                    return sr.Peek();
                }
                if (S == T)
                {
                    T = sr.ReadBlock(buffer.AsSpan());
                    if (T == 0) return -1;
                    S = 0;
                }
                return buffer[S];
            }

            private int Read()
            {
                if (buffer == null)
                {
                    return sr.Read();
                }
                int p = Peek();
                if (p >= 0) ++S;
                return p;
            }

            private string readLine(bool skipSpace)
            {
                int c;
                StringBuilder sb = new();
                while ((c = Read()) != -1)
                {
                    if (c == '\r') continue; // omit '\r'
                    if (c == ' ' && skipSpace)
                    {
                        if (sb.Length == 0)
                        {
                            continue;
                        }
                        break;
                    }
                    if (c == '\n')
                    {
                        if (sb.Length > 0) break;
                        else continue; // omit empty lines
                    }
                    sb.Append((char)c);
                }
                return sb.ToString();
            }

            public string ReadString() => readLine(true);
            public string ReadLine() => readLine(false);

            public BufferedReader(Stream stream, int capacity)
            {
                sr = new(stream);
                if (capacity > 0)
                {
                    buffer = new char[capacity];
                }
                else
                {
                    buffer = null;
                }
            }

            public bool EndOfStream { get => buffer == null ? sr.EndOfStream : Peek() == -1; }

            public T ReadInt<T>() where T : INumber<T>
            {
                char c;
                T res = default;
                T sign = T.CreateChecked(1);
                while (!EndOfStream && char.IsWhiteSpace((char)Peek())) Read();
                if (!EndOfStream && (char)Peek() == '-')
                {
                    Read();
                    sign = T.CreateChecked(-1);
                }
                while (!EndOfStream && char.IsDigit((char)Peek()))
                {
                    c = (char)Read();
                    res = res * T.CreateChecked(10) + T.CreateChecked(c - '0');
                }
                return res * sign;
            }

            public T[] ReadArray<T>(int cnt) where T : INumber<T> => ReadArray<T>(cnt, 0);
            public T[] ReadArray<T>(int cnt, int startIndex) where T : INumber<T>
            {
                T[] arr = new T[cnt + startIndex];
                for (int i = 0; i < cnt; ++i) arr[i + startIndex] = ReadInt<T>();
                return arr;
            }

            public int ReadInt32() => ReadInt<int>();
            public long ReadInt64() => ReadInt<long>();
            public int[] ReadInt32(int cnt) => ReadArray<int>(cnt);
            public long[] ReadInt64(int cnt) => ReadArray<long>(cnt);

            public double ReadDouble()
            {
                double res = ReadInt64();
                if ((char)Peek() == '.')
                {
                    Read();
                    double tail = 0.1;
                    while (!EndOfStream && char.IsDigit((char)Peek()))
                    {
                        char c = (char)Read();
                        res += (c - '0') * tail;
                        tail *= 0.1;
                    }
                }
                return res;
            }
            public void Dispose()
            {
                sr.Close();
            }
        }

        public class BufferedWriter : IDisposable
        {
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
            public void Output()
            {
                sw.Write(sb.ToString());
                sw.Flush();
                sb.Clear();
            }
            public void Dispose()
            {
                sw.Close();
            }
        }
        #endregion

        #region ArrayExt
        public static class ArrayExt
        {
            public static Span<T> AsSpan<T>(this T[,] array) => asSpan<T>(array);
            public static Span<T> AsSpan<T>(this T[,,] array) => asSpan<T>(array);
            public static Span<T> AsSpan<T>(this T[,,,] array) => asSpan<T>(array);
            public static Span<T> AsSpan<T>(this T[,,,,] array) => asSpan<T>(array);
            public static Span<T> AsSpan<T>(this T[,,,,,] array) => asSpan<T>(array);
            public static Span<T> AsSpan<T>(this T[,,,,,,] array) => asSpan<T>(array);

            static Span<T> asSpan<T>(Array array)
                => System.Runtime.InteropServices.MemoryMarshal.CreateSpan(
                    ref System.Runtime.CompilerServices.Unsafe.As<byte, T>(
                        ref System.Runtime.InteropServices.MemoryMarshal.GetArrayDataReference(array)
                    ), array.Length);
            public static int LowerBound<T>(this IList<T> a, T M) where T : IComparable<T>
            => LowerBound<T>(a, 0, a.Count, M);
            public static int LowerBound<T, K>(this IList<T> a, K M, Func<T, K, bool> cmp)
            => LowerBound<T, K>(a, 0, a.Count, M, cmp);
            public static int LowerBound<T>(this IList<T> a, int lo, int hi, T M) where T : IComparable<T>
            {
                while (lo < hi)
                {
                    int x = (lo + hi) / 2;
                    if (a[x].CompareTo(M) >= 0)
                    {
                        hi = x;
                    }
                    else
                    {
                        lo = x + 1;
                    }
                }
                return hi;
            }
            public static int LowerBound<T, K>(this IList<T> a, int lo, int hi, K M, Func<T, K, bool> cmp)
            {
                while (lo < hi)
                {
                    int x = (lo + hi) / 2;
                    if (cmp(a[x], M))
                    {
                        hi = x;
                    }
                    else
                    {
                        lo = x + 1;
                    }
                }
                return hi;
            }
            public static int UpperBound<T>(this IList<T> a, T M) where T : IComparable<T>
            => UpperBound<T>(a, 0, a.Count, M);
            public static int UpperBound<T, K>(this IList<T> a, K M, Func<T, K, bool> cmp)
=> UpperBound<T, K>(a, 0, a.Count, M, cmp);
            public static int UpperBound<T>(this IList<T> a, int lo, int hi, T M) where T : IComparable<T>
            {
                while (lo < hi)
                {
                    int x = (lo + hi) / 2;
                    if (a[x].CompareTo(M) > 0)
                    {
                        hi = x;
                    }
                    else
                    {
                        lo = x + 1;
                    }
                }
                return hi;
            }
            public static int UpperBound<T, K>(this IList<T> a, int lo, int hi, K M, Func<T, K, bool> cmp) { 
                while (lo < hi) {
                    int x = (lo + hi) / 2;
                    if (cmp(a[x], M)) { 
                        hi = x;
                    } else {
                        lo = x + 1;
                    }
                }
                return hi;
            }
            public static int Unique<T>(this IList<T> a) where T : IComparable<T> {
                int i = -1, j = 0;
                while (j < a.Count) { 
                    if (i == -1 || a[i].CompareTo(a[j]) != 0){
                        a[++i] = a[j];
                    }
                    ++j;
                }
                return i + 1;
            }

            static Random rng = new(998244353);
            private static (int, int) partition<T>(IList<T> a, int lo, int hi) where T : IComparable<T> {
                int x = lo + (rng.Next() % (hi - lo + 1));
                (a[lo], a[x]) = (a[x], a[lo]);
                T key = a[lo];
                int i = lo, j = lo, k = hi;
                while (j <= k) {
                    if (a[j].CompareTo(key) < 0) {
                        (a[j], a[i]) = (a[i], a[j]);
                        ++i;
                        ++j;
                    } else if (a[j].CompareTo(key) > 0) {
                        (a[j], a[k]) = (a[k], a[j]);
                        --k;
                    } else {
                        ++j;
                    }
                }
                return (i, k);  // [i...k] all equal to key
            }
            public static void KthElement<T>(IList<T> nums, int k) where T : IComparable<T> {
                int n = nums.Count;
                int l = 0, r = n - 1;
                while (l <= r) {
                    var (L, R) = partition(nums, l, r);
                    if (k < L) {
                        r = L - 1;
                    } else if (L <= k && k <= R) {
                        return;
                    } else {
                        l = R + 1;
                    }
                }
            }
        }
        #endregion
    }
    namespace Graph {
        #region EBCC
        /// <summary>
        /// EBCC (Edge Biconnected Component)
        /// </summary>
        public struct EBccGraph {
            private int n;
            private Stack<int> stk;
            private int[] dfn, low, bel;
            private int cur, cnt;
            public int Cnt => cnt;

            private HashSet<(int u, int v)> E;

            private int[] head;
            private (int v, int nxt)[] e;
            private int tot;

            public EBccGraph(int n, int m) {
                Init(n, m);
            }

            public void Init(int n, int m) {
                this.n = n;
                head = new int[n];
                Array.Fill(head, -1);
                e = new (int v, int nxt)[2 * m];
                dfn = new int[n];
                Array.Fill(dfn, -1);
                low = new int[n];
                bel = new int[n];
                Array.Fill(bel, -1);
                stk = new();
                cur = cnt = tot = 0;
                E = new();
            }

            public void AddEdge(int u, int v) {
                e[tot] = (v, head[u]);
                head[u] = tot++;
                e[tot] = (u, head[v]);
                head[v] = tot++;
            }

            public void Dfs(int x, int pre) {
                dfn[x] = low[x] = cur++;
                stk.Push(x);

                for (int i = head[x]; i >= 0; i = e[i].nxt) {
                    if (i == (pre ^ 1)) {
                        continue;
                    }

                    int y = e[i].v;
                    if (dfn[y] == -1) {
                        Dfs(y, i);
                        low[x] = Math.Min(low[x], low[y]);
                        if (low[y] > dfn[x]) {
                            E.Add((x, y));
                        }
                    } else {
                        low[x] = Math.Min(low[x], dfn[y]);
                    }
                }

                if (dfn[x] == low[x]) {
                    int y;
                    do {
                        y = stk.Pop();
                        bel[y] = cnt;
                    } while (y != x);
                    ++cnt;
                }
            }

            public void Work() {
                for (int i = 0; i < n; ++i) {
                    if (dfn[i] == -1) {
                        Dfs(i, -1);
                    }
                }
            }

            public IEnumerable<int> this[int u] {
                get {
                    for (int i = head[u]; i >= 0; i = e[i].nxt) {
                        yield return e[i].v;
                    }
                }
            }

            public bool IsBridge(int u, int v) => E.Contains((u, v)) || E.Contains((v, u));
            public int VerCount => n;
            public int Bel(int i) => bel[i];
        }
        #endregion

        #region HLD

        /// <summary>
        /// HLD (Heavy-Light Decomposition)
        /// </summary>
        public class HLD {
            public int n;
            public List<int> siz, top, dep, parent, In, Out;
            public List<int> seq;
            public List<List<int>> adj;
            public int cur;

            public HLD() { }

            public HLD(int _n) {
                Init(_n);
            }

            public void Init(int _n) {
                n = _n;
                siz = new List<int>(new int[n]);
                top = new List<int>(new int[n]);
                dep = new List<int>(new int[n]);
                parent = new List<int>(new int[n]);
                In = new List<int>(new int[n]);
                Out = new List<int>(new int[n]);
                seq = new List<int>(new int[n]);
                cur = 0;
                adj = new List<List<int>>(n);
                for (int i = 0; i < n; i++) {
                    adj.Add(new List<int>());
                }
            }

            public void addEdge(int u, int v) {
                adj[u].Add(v);
                adj[v].Add(u);
            }

            public void work(int root = 0) {
                top[root] = root;
                dep[root] = 0;
                parent[root] = -1;
                dfs1(root);
                dfs2(root);
            }

            public void dfs1(int u) {
                if (parent[u] != -1) {
                    adj[u].Remove(parent[u]);
                }

                siz[u] = 1;
                for (int i = 0; i < adj[u].Count; i++) {
                    int v = adj[u][i];
                    parent[v] = u;
                    dep[v] = dep[u] + 1;
                    dfs1(v);
                    siz[u] += siz[v];
                    if (i == 0 || siz[v] > siz[adj[u][0]]) {
                        int temp = adj[u][0];
                        adj[u][0] = v;
                        adj[u][i] = temp;
                    }
                }
            }

            public void dfs2(int u) {
                In[u] = cur;
                seq[cur] = u;
                cur++;

                foreach (int v in adj[u]) {
                    if (v == adj[u][0])
                        top[v] = top[u];
                    else
                        top[v] = v;
                    dfs2(v);
                }
                Out[u] = cur;
            }

            public int lca(int u, int v) {
                while (top[u] != top[v]) {
                    if (dep[top[u]] > dep[top[v]])
                        u = parent[top[u]];
                    else
                        v = parent[top[v]];
                }
                return dep[u] < dep[v] ? u : v;
            }

            public int dist(int u, int v) {
                int l = lca(u, v);
                return dep[u] + dep[v] - 2 * dep[l];
            }

            public int jump(int u, int k) {
                if (dep[u] < k)
                    return -1;

                int d = dep[u] - k;
                while (dep[top[u]] > d)
                    u = parent[top[u]];

                return seq[In[u] - (dep[u] - d)];
            }

            public bool isAncester(int u, int v) {
                return In[u] <= In[v] && In[v] < Out[u];
            }

            public int rootedParent(int u, int v) {
                (u, v) = (v, u);

                if (u == v)
                    return u;
                if (!isAncester(u, v))
                    return parent[u];

                int index = LowerBound(adj[u], v);
                return adj[u][index - 1];
            }

            private int LowerBound(List<int> list, int v) {
                int target = In[v];
                int lo = 0, hi = list.Count;
                while (lo < hi) {
                    int x = (lo + hi) / 2;
                    if (In[list[x]] < target)
                        hi = x;
                    else
                        lo = x + 1;
                }
                return lo;
            }

            public int rootedSize(int u, int v) {
                if (u == v)
                    return n;
                if (!isAncester(v, u))
                    return siz[v];
                return n - siz[rootedParent(u, v)];
            }

            public int rootedLca(int a, int b, int c) {
                return lca(a, b) ^ lca(b, c) ^ lca(c, a);
            }
        }
        #endregion

    }
    namespace String {
        #region StringHash
        /// <summary>
        /// StringHash (字符串哈希)
        /// </summary>
        public struct StringHash {
            static readonly long timestamp = Stopwatch.GetTimestamp();
            static readonly int seed = (int)(timestamp ^ (timestamp >> 32)); // 混合高低位
            static readonly Random rng = new Random(seed);
            static readonly int Base = rng.Next((int)8E8, (int)9E8);
            static readonly int P = (int)1E9 + 7;
            static int[] pw = [1];
            int n;
            int[] h;
            public StringHash(string s) {
                n = s.Length;
                h = new int[n + 1];
                Init(n);
                for (int i = 0; i < n; ++i) {
                    h[i + 1] = (int)(1L * h[i] * Base % P + s[i]) % P;
                }
            }
            /// <summary>
            /// Get hashVal for s[l..r]
            /// </summary>
            /// <param name="l"></param>
            /// <param name="r"></param>
            /// <returns></returns>
            public int this[int l, int r] {
                get => (int)((h[r] + 1L * (P - h[l]) * pw[r - l]) % P);
            }
            static void Init(int n) {
                if (n < pw.Length) return;
                int m = pw.Length;
                Array.Resize(ref pw, n + 1);
                for (int i = m; i <= n; i++) pw[i] = (int)(1L * pw[i - 1] * Base % P);
            }
        }
        #endregion

        #region Manacher
        public struct Manacher { 
            public static int[] RBox(string s) => manacher(s.AsSpan());
            public static int[] RBox(char[] s) => manacher(s.AsSpan());
            private static int[] manacher(ReadOnlySpan<char> s) {
                List<char> t = ['#'];
                foreach (char c in s) {
                    t.Add(c);
                    t.Add('#');
                }
                int n = t.Count;
                int[] r = new int[n];
                for (int i = 0, j = 0; i < n; ++i) {
                    if (j + r[j] > i)
                        r[i] = Math.Min(r[2 * j - i], j + r[j] - i);
                    while (i - r[i] >= 0 && i + r[i] < n && t[i - r[i]] == t[i + r[i]])
                        ++r[i];
                    if (i + r[i] > j + r[j])
                        j = i;
                }
                return r;
            }
        }

        #endregion
    }
    namespace DataStructure {
        #region SparseTable
        public struct SparseTable<T> {
            private T[,] st;
            private int[] log2;
            private Func<T, T, T> func;
            public SparseTable(IList<T> a, Func<T, T, T> _func) {
                func = _func;
                int n = a.Count;
                log2 = new int[n + 1];
                for (int i = 2; i <= n; ++i) log2[i] = log2[i >> 1] + 1;
                int B = log2[n];
                st = new T[n, B + 1];
                for (int i = 0; i < n; ++i) st[i, 0] = a[i];
                for (int b = 1; b <= B; ++b) {
                    for (int i = 0; i + (1 << b) - 1 < n; ++i) {
                        st[i, b] = func(st[i, b - 1], st[i + (1 << (b - 1)), b - 1]);
                    }
                }
            }
            public T Query(int l, int r) {
                int lg = log2[r - l + 1];
                return func(st[l, lg], st[r - (1 << lg) + 1, lg]);
            }
        }
        #endregion
        #region Sieve
        public static class Sieve {
            static private IList<int> primes_;
            static private int[] minp_;
            static private int n = 100_000;
            static Sieve() {
                primes_ = new List<int>();
                minp_ = new int[n + 1];
                for (int i = 2; i <= n; ++i) {
                    if (minp_[i] == 0) {
                        minp_[i] = i;
                        primes_.Add(i);
                    }
                    foreach (int p in primes_) {
                        if (i * p > n) break;
                        minp_[i * p] = p;
                        if (minp_[i] == p) break;
                    }
                }
            }
            static public int minp(int x) => minp_[x];
            static public IList<int> primes() => primes_;
            static public int prime(int i) => primes_[i];
            static public bool isPrime(int x) => minp_[x] == x;
            static public int Count() => primes_.Count;
        }
        #endregion
        #region Treap
        struct Treap<Key>
        where Key : IComparable<Key> {
            static readonly long timestamp = Stopwatch.GetTimestamp();
            static readonly int seed = (int)(timestamp ^ (timestamp >> 32)); // 混合高低位
            static readonly Random rng = new Random(seed);
            public class Node {
                public int w = rng.Next();
                public Key key;
                public int siz = 1;
                public Node l;
                public Node r;
                public Node(Key k, Node l_ = null, Node r_ = null) {
                    key = k;
                    l = l_;
                    r = r_;
                }
            }
            private Node root = null;
            public Treap() { }
            private int size(Node t) {
                return t is null ? 0 : t.siz;
            }
            void Pull(Node t) {
                t.siz = 1 + size(t.l) + size(t.r);
            }
            // <前p小, xxx>
            (Node, Node) SplitAt(Node t, int p) {
                if (t is null) {
                    return (t, t);
                }
                if (p <= size(t.l)) {
                    var (l, r) = SplitAt(t.l, p);
                    t.l = r;
                    Pull(t);
                    return (l, t);
                } else {
                    var (l, r) = SplitAt(t.r, p - 1 - size(t.l));
                    t.r = l;
                    Pull(t);
                    return (t, r);
                }
            }

            // < <=p, >p >
            (Node, Node) SplitAt(Node t, Key p) {
                if (t is null) {
                    return (t, t);
                }

                if (t.key.CompareTo(p) > 0) {
                    var (l, r) = SplitAt(t.l, p);
                    t.l = r;
                    Pull(t);
                    return (l, t);
                } else {
                    var (l, r) = SplitAt(t.r, p);
                    t.r = l;
                    Pull(t);
                    return (t, r);
                }
            }

            // a <= b
            Node Merge(Node a, Node b) {
                if (a is null || b is null) {
                    return a ?? b;
                }

                if (a.w < b.w) {
                    a.r = Merge(a.r, b);
                    Pull(a);
                    return a;
                } else {
                    b.l = Merge(a, b.l);
                    Pull(b);
                    return b;
                }
            }

            public void Insert(Key k) {
                Node x = new(k);
                var (l, r) = SplitAt(root, x.key);
                root = Merge(Merge(l, x), r);
            }

            public void Extract(Key p) {
                var (l, r) = SplitAt(root, p);
                var (ll, lr) = SplitAt(l, size(l) - 1);
                root = Merge(ll, r);
            }

            public int Size() {
                return size(root);
            }

            public bool Empty() {
                return size(root) == 0;
            }

            Node Find_Kth(Node t, int p) {
                if (size(t.l) + 1 == p) {
                    return t;
                }
                if (p <= size(t.l)) {
                    return Find_Kth(t.l, p);
                }
                return Find_Kth(t.r, p - size(t.l) - 1);
            }

            public Node Kth(int p) {
                Debug.Assert(0 < p && p <= Size());
                return Find_Kth(root, p);
            }

            public Node Pre(Key key) {
                Node res = null;
                for (Node t = root; t is not null;) {
                    if (t.key.CompareTo(key) < 0) {
                        res = t;
                        t = t.r;
                    } else {
                        t = t.l;
                    }
                }
                return res;
            }

            public Node Suf(Key key) {
                Node res = null;
                for (Node t = root; t is not null;) {
                    if (key.CompareTo(t.key) < 0) {
                        res = t;
                        t = t.l;
                    } else {
                        t = t.r;
                    }
                }
                return res;
            }

            public Node Lower_bound(Key key) {
                Node res = null;
                for (Node t = root; t is not null;) {
                    if (key.CompareTo(t.key) <= 0) {
                        res = t;
                        t = t.l;
                    } else {
                        t = t.r;
                    }
                }
                return res;
            }

            public bool Contains(Key key) {
                Node t = Lower_bound(key);
                return t is not null && t.key.CompareTo(key) == 0;
            }

            public int Rank(Key key) {
                Node p = Pre(key);
                if (p is null) {
                    return 1;
                }

                var (l, r) = SplitAt(root, p.key);
                int res = size(l) + 1;
                root = Merge(l, r);
                return res;
            }

            public Node Front() {
                Debug.Assert(!Empty());
                return Kth(1);
            }

            public Node Back() {
                Debug.Assert(!Empty());
                return Kth(Size());
            }
        }
        #endregion
    }
    namespace MathLib {
        #region Comb
        public class Comb {
            int n;
            readonly int P;
            List<int> _fac;
            List<int> _invfac;
            List<int> _inv;

            public Comb(int P_) {
                _fac = new List<int> { 1 };
                _invfac = new List<int> { 1 };
                _inv = new List<int> { 0 };
                P = P_;
            }
            public Comb(int n, int P_) : this(P_) {
                init(n);
            }
            private int power(int a, int b) {
                int res = 1;
                while (b > 0) {
                    if (b % 2 == 1) {
                        res = (int)(1L * res * a % P);
                    }
                    a = (int)(1L * a * a % P);
                    b /= 2;
                }
                return res;
            }
            void init(int m) {
                if (m <= n) return;
                while (_fac.Count < m + 1) {
                    _fac.Add(default(int));
                    _invfac.Add(default(int));
                    _inv.Add(default(int));
                }
                for (int i = n + 1; i <= m; i++) {
                    _fac[i] = (int)(1L * _fac[i - 1] * i % P);
                }
                _invfac[m] = power(_fac[m], P - 2);
                for (int i = m; i > n; i--) {
                    _invfac[i - 1] = (int)(1L * _invfac[i] * i % P);
                    _inv[i] = (int)(1L * _invfac[i] * _fac[i - 1] % P);
                }
                n = m;
            }
            public int fac(int m) {
                if (m > n) init(2 * m);
                return _fac[m];
            }
            public int invfac(int m) {
                if (m > n) init(2 * m);
                return _invfac[m];
            }
            public int inv(int m) {
                if (m > n) init(2 * m);
                return _inv[m];
            }
            public int binom(int n, int m) {
                if (n < m || m < 0) return 0;
                return (int)(1L * fac(n) * invfac(m) % P * invfac(n - m) % P);
            }
        };
        #endregion
    }
}
