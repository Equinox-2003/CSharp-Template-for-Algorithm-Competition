using System.Diagnostics;

namespace Equinox {
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
    }
}
