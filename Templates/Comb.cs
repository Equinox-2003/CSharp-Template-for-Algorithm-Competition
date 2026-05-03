namespace Equinox {
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
