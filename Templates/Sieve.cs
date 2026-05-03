namespace Equinox {
    namespace DataStructure {
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

    }
}
