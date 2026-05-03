using System.Diagnostics;

namespace Equinox {
    namespace DataStructure {
        #region Treap
        public struct Treap<Key>
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
}
