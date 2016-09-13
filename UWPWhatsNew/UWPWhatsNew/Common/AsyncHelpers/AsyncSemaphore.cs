using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace UWPWhatsNew.Common.AsyncHelpers
{
    /// <summary>
    /// Limits the number of awaiters that can access a resource or pool of resources concurrently.
    /// </summary>
    public class AsyncSemaphore
    {
        private static readonly Task _completedTask = Task.FromResult(true);
        private readonly Queue<TaskCompletionSource<bool>> _waiters = new Queue<TaskCompletionSource<bool>>();
        private int _currentCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncSemaphore" /> class, reserving some concurrent entries.
        /// </summary>
        /// <param name="initialCount">The initial count.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">initialCount</exception>
        public AsyncSemaphore(int initialCount)
        {
            if (initialCount < 0)
            {
                throw new ArgumentOutOfRangeException("initialCount");
            }

            _currentCount = initialCount;
        }

        /// <summary>
        /// Blocks the current awaiter until the semaphore receives a signal.
        /// </summary>
        [DebuggerHidden]
        public Task WaitAsync()
        {
            lock (_waiters)
            {
                if (_currentCount > 0)
                {
                    Interlocked.Decrement(ref _currentCount);

                    return _completedTask;
                }

                var waiter = new TaskCompletionSource<bool>();
                _waiters.Enqueue(waiter);

                return waiter.Task;
            }
        }

        /// <summary>
        /// Exits the semaphore and returns the previous count.
        /// </summary>
        public void Release()
        {
            TaskCompletionSource<bool> toRelease = null;

            lock (_waiters)
            {
                if (_waiters.Count > 0)
                {
                    toRelease = _waiters.Dequeue();
                }
                else
                {
                    Interlocked.Increment(ref _currentCount);
                }
            }

            if (toRelease != null)
            {
                toRelease.SetResult(true);
            }
        }
    }
}
