using System.Numerics;
using System.Runtime.CompilerServices;

namespace Equinox {
    namespace DataStructure {
        /// <summary>
        /// 懒删除堆, 实时维护元素和
        /// </summary>
        /// <typeparam name="T">元素类型，如 int, long</typeparam>
        public struct LazyHeap<T> where T : INumber<T> {
            // 数据堆，存放所有插入的元素
            private PriorityQueue<T, T> dataHeap;
            // 删除堆，存放被标记删除的元素
            private PriorityQueue<T, T> deletedHeap;

            /// <summary>
            /// 当前堆内有效元素的和
            /// </summary>
            public T Sum { get; private set; }

            /// <summary>
            /// 当前堆内有效元素的个数
            /// </summary>
            public int Count { get; private set; }

            /// <summary>
            /// 堆是否为空
            /// </summary>
            public bool IsEmpty => Count == 0;

            /// <summary>
            /// 初始化懒删除堆
            /// </summary>
            /// <param name="initialCapacity">初始容量</param>
            /// <param name="comparer">自定义比较器 (默认 null 为小根堆，传大根堆比较器则变为大根堆)</param>
            public LazyHeap(int initialCapacity = 1024, IComparer<T>? comparer = null) {
                dataHeap = new PriorityQueue<T, T>(initialCapacity, comparer);
                // 删除操作通常少于插入操作，预分配少一点的空间
                deletedHeap = new PriorityQueue<T, T>(initialCapacity / 4 + 1, comparer);
                Sum = T.Zero;
                Count = 0;
            }

            /// <summary>
            /// 插入一个新元素
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Push(T value) {
                dataHeap.Enqueue(value, value);
                Sum += value;
                ++Count;
            }

            /// <summary>
            /// 懒删除一个元素。
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Remove(T value) {
                deletedHeap.Enqueue(value, value);
                Sum -= value;
                --Count;
            }

            /// <summary>
            /// 将堆顶已经被标记删除的元素清理掉
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void Prune() {
                // 当删除堆不为空，且两个堆的堆顶元素严格相等时，说明堆顶元素是被删除的，一起弹出
                while (deletedHeap.Count > 0
                    && dataHeap.TryPeek(out T dataTop, out _)
                    && deletedHeap.TryPeek(out T delTop, out _)) {
                    // INumber<T> 使得我们可以直接使用 == 进行值比较，无视装箱和比较器逻辑
                    if (dataTop == delTop) {
                        dataHeap.Dequeue();
                        deletedHeap.Dequeue();
                    } else {
                        break;
                    }
                }
            }

            /// <summary>
            /// 弹出并返回堆顶元素
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T Pop() {
                Prune();
                T val = dataHeap.Dequeue();
                Sum -= val;
                Count--;
                return val;
            }

            /// <summary>
            /// 获取堆顶元素，但不弹出
            /// </summary>
            public T Top {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get {
                    Prune();
                    return dataHeap.Peek();
                }
            }
        }

        /// <summary>
        /// 懒删除对顶堆 (动态维护前 K 小及它们的和)
        /// 依赖于前文提供的 LazyHeap<T>
        /// </summary>
        /// <typeparam name="T">元素类型 (int, long等)</typeparam>
        public struct LazyTwoHeaps<T> where T : INumber<T> {
            /// <summary>
            /// 存放前 K 小的元素（大根堆，堆顶为第 K 小）
            /// </summary>
            private LazyHeap<T> LowerMaxHeap;

            /// <summary>
            /// 存放剩余的大元素（小根堆）
            /// </summary>
            private LazyHeap<T> UpperMinHeap;

            /// <summary>
            /// 我们需要维护的前 K 小的数量
            /// </summary>
            public int K { get; private set; }

            /// <summary>
            /// O(1) 获取当前前 K 小的元素之和（极其适合 "滑动窗口 Top K 和" 等题目）
            /// </summary>
            public T SumK => LowerMaxHeap.Sum;
            public T AllSum => LowerMaxHeap.Sum + UpperMinHeap.Sum;

            /// <summary>
            /// O(1) 获取当前堆内有效元素的总个数
            /// </summary>
            public int TotalCount => LowerMaxHeap.Count + UpperMinHeap.Count;

            /// <summary>
            /// 无额外分配的大根堆比较器
            /// </summary>
            private struct DescendingComparer<T> : IComparer<T> where T : INumber<T> {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public int Compare(T x, T y) => y.CompareTo(x);
            }

            /// <summary>
            /// 初始化带懒删除的对顶堆
            /// </summary>
            public LazyTwoHeaps(int k = 1, int initialCapacity = 1024) {
                K = k;
                LowerMaxHeap = new LazyHeap<T>(initialCapacity, new DescendingComparer<T>());
                UpperMinHeap = new LazyHeap<T>(initialCapacity);
            }

            /// <summary>
            /// 插入一个新元素
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Add(T value) {
                // 如果下半部为空，或者新元素比前 K 小中的最大值还要小/相等，则归属下半部
                if (LowerMaxHeap.IsEmpty || value <= LowerMaxHeap.Top) {
                    LowerMaxHeap.Push(value);
                } else {
                    UpperMinHeap.Push(value);
                }

                // 插入后可能导致数量失衡，需要重新平衡
                Balance();
            }

            /// <summary>
            /// 懒删除一个元素 (调用者须保证元素必然存在于堆中)
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Remove(T value) {
                // 【核心巧思】：因为我们严格维护了 LowerMaxHeap 里的元素都 <= UpperMinHeap 里的元素
                // 所以只要 value <= LowerMaxHeap.Top，它当初必定被塞进了 LowerMaxHeap！
                if (!LowerMaxHeap.IsEmpty && value <= LowerMaxHeap.Top) {
                    LowerMaxHeap.Remove(value);
                } else {
                    UpperMinHeap.Remove(value);
                }

                // 删除后可能导致 LowerMaxHeap 缺人，需要重新平衡
                Balance();
            }

            /// <summary>
            /// 动态修改维护的 K 的大小 (支持变长)
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetK(int newK) {
                K = newK;
                Balance();
            }

            /// <summary>
            /// 内部逻辑：维持 LowerMaxHeap 恰好有 min(K, TotalCount) 个元素
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private void Balance() {
                // 如果 LowerMaxHeap 膨胀了，把超出的最大元素踢给上半部
                while (LowerMaxHeap.Count > K) {
                    UpperMinHeap.Push(LowerMaxHeap.Pop());
                }

                // 如果 LowerMaxHeap 缺人了，且上半部还有人，从上半部拿最小的来补充
                while (LowerMaxHeap.Count < K && !UpperMinHeap.IsEmpty) {
                    LowerMaxHeap.Push(UpperMinHeap.Pop());
                }
            }

            /// <summary>
            /// O(log) 获取当前第 K 小的元素。
            /// </summary>
            public T TopLower {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => LowerMaxHeap.Top;
            }

            /// <summary>
            /// O(log) 获取当前第 K + 1 小的元素。
            /// </summary>
            public T TopUpper {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => UpperMinHeap.Top;
            }
        }
    }
}
