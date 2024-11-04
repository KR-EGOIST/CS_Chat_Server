using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    struct JobTimerElem : IComparable<JobTimerElem>
    {
        public int execTick; // 실행 시간
        public Action action;

        // 실행 시간 비교
        public int CompareTo(JobTimerElem other)
        {
            return other.execTick - this.execTick;
        }
    }

    class JobTimer
    {
        PriorityQueue<JobTimerElem> _pq = new PriorityQueue<JobTimerElem>();
        object _lock = new object();

        public  static JobTimer Instance { get; } = new JobTimer();

        public void Push(Action action, int tickAfter = 0)
        {
            JobTimerElem job;
            job.execTick = System.Environment.TickCount + tickAfter; // 언제 실행해야 되냐?
            job.action = action; // 무엇을 실행해야 하냐?

            lock (_lock)
            {
                _pq.Push(job);
            }
        }

        public void Flush()
        {
            while (true)
            {
                int now = System.Environment.TickCount;

                JobTimerElem job;

                lock (_lock)
                {
                    if (_pq.Count == 0) // 비어있으면
                        break;

                    job = _pq.Peek();
                    if (job.execTick > now) // 아직 실행시간이 안되었으면
                        break;

                    _pq.Pop();
                }

                job.action.Invoke(); // 일감 실행
            }
        }
    }
}
