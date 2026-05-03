namespace Equinox {
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
    }
}
