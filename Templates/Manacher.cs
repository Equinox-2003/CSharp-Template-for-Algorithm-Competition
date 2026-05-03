namespace Equinox {
    namespace String {
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
}
