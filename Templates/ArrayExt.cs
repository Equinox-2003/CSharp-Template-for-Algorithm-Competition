namespace Equinox {
    namespace IO {
        #region ArrayExt
        public static class ArrayExt {
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
            public static int LowerBound<T>(this IList<T> a, int lo, int hi, T M) where T : IComparable<T> {
                while (lo < hi) {
                    int x = (lo + hi) / 2;
                    if (a[x].CompareTo(M) >= 0) {
                        hi = x;
                    } else {
                        lo = x + 1;
                    }
                }
                return hi;
            }
            public static int LowerBound<T, K>(this IList<T> a, int lo, int hi, K M, Func<T, K, bool> cmp) {
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
            public static int UpperBound<T>(this IList<T> a, T M) where T : IComparable<T>
            => UpperBound<T>(a, 0, a.Count, M);
            public static int UpperBound<T, K>(this IList<T> a, K M, Func<T, K, bool> cmp)
=> UpperBound<T, K>(a, 0, a.Count, M, cmp);
            public static int UpperBound<T>(this IList<T> a, int lo, int hi, T M) where T : IComparable<T> {
                while (lo < hi) {
                    int x = (lo + hi) / 2;
                    if (a[x].CompareTo(M) > 0) {
                        hi = x;
                    } else {
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
                    if (i == -1 || a[i].CompareTo(a[j]) != 0) {
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
}
