namespace Equinox {
    namespace Graph {
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
}
