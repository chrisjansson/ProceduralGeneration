using System;
using System.Collections.Concurrent;

namespace CjClutter.OpenGl.Gui
{
    public class JobDispatcher
    {
        private readonly BlockingCollection<Action> _jobs;

        private JobDispatcher()
        {
            _jobs = new BlockingCollection<Action>(10);
        }

        public void Enqueue(Action action)
        {
            _jobs.Add(action);
        }

        public bool TryEnqueue(Action action)
        {
            return _jobs.TryAdd(action, 0);
        }

        public Action Dequeue()
        {
            return _jobs.Take();
        }

        private static JobDispatcher _instance;
        public static JobDispatcher Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new JobDispatcher();
                return _instance;
            }
        }
    }
}