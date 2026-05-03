namespace Equinox {
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

    }
}
