using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MapTileGenerator.Core
{
    public class QueueTaskWorker<T>
    {
        private BlockingCollection<T> _queue;
        private CancellationTokenSource _tokenSource = null;
        private bool _isClosed = false;
        private Action<T> _action = null;
        private int _threadCount = 1;
        private bool _isBlocking = false;
        private Task[] _tasks = null;

        public QueueTaskWorker(int threadCount, Action<T> action, bool isBlock)
        {
            _threadCount = threadCount;
            _action = action;
            _isBlocking = isBlock;
            _tokenSource = new CancellationTokenSource();
            Init();
        }

        public void TryQueue(T item)
        {
            _queue.Add(item);
        }

        public void Init()
        {
            if (_isBlocking)
            {
                _queue = new BlockingCollection<T>(_threadCount);
            }
            else
            {
                _queue = new BlockingCollection<T>();
            }
        }

        public void Start()
        {
            var token = _tokenSource.Token;
            var taskFactory = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.LongRunning);
            _tasks = new Task[_threadCount];
            for (int i = 0; i < _threadCount; i++)
            {
                _tasks[i] = taskFactory.StartNew(() =>
                {
                    while (!_isClosed)
                    {
                        token.ThrowIfCancellationRequested();

                        T item;
                        //if ((item =_queue.Take())!=null)
                        if(_queue.TryTake(out item))
                        {
                            _action(item);
                        }
                    }
                });
            }
        }

        public void Close()
        {
            _isClosed = true;
            if (_tasks != null)
            {
                Task.WaitAll(_tasks, 100, _tokenSource.Token);
            }
            //if (_tokenSource != null)
            //{
            //    _tokenSource.Cancel();
            //}
            _queue.Dispose();
        }

        public void Dispose()
        {
            Close();
        }
    }
}
