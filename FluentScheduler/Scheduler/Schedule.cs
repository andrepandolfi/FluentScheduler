﻿namespace FluentScheduler
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// A job schedule.
    /// </summary>
    public class Schedule
    {
        internal InternalSchedule Internal { get; private set; }

        /// <summary>
        /// Creates a new schedule for the given job.
        /// </summary>
        /// <param name="job">Job to be scheduled</param>
        /// <param name="specifier">Scheduling of this schedule</param>
        /// <returns>A schedule for the given job</returns>
        public Schedule(Action job, Action<RunSpecifier> specifier) => Internal = new InternalSchedule(job, specifier);

        /// <summary>
        /// True if the schedule is started, false otherwise.
        /// </summary>
        public bool Running
        {
            get
            {
                lock (Internal.RunningLock)
                {
                    return Internal.Running();
                }
            }
        }

        /// <summary>
        /// Date and time of the next job run.
        /// </summary>
        public DateTime? NextRun { get => Internal.NextRun; }

        /// <summary>
        /// Event raised when the job starts.
        /// </summary>
        public event EventHandler<JobStartedEventArgs> JobStarted
        {
            add => Internal.JobStarted += value;
            remove => Internal.JobStarted -= value;
        }

        /// <summary>
        /// Event raised when the job ends.
        /// </summary>
        public event EventHandler<JobEndedEventArgs> JobEnded
        {
            add => Internal.JobEnded += value;
            remove => Internal.JobEnded -= value;
        }

        /// <summary>
        /// Resets the scheduling of this schedule.
        /// You must not call this method if the schedule is running.
        /// </summary>
        public void ResetScheduling()
        {
            lock (Internal.RunningLock)
            {
                Internal.ResetScheduling();
            }
        }

        /// <summary>
        /// Changes the scheduling of this schedule.
        /// You must not call this method if the schedule is running.
        /// </summary>
        /// <param name="specifier">Scheduling of this schedule</param>
        public void SetScheduling(Action<RunSpecifier> specifier)
        {
            lock (Internal.RunningLock)
            {
                Internal.SetScheduling(specifier);
            }
        }

        /// <summary>
        /// Starts the schedule or does nothing if it's already running.
        /// </summary>
        public void Start()
        {
            lock (Internal.RunningLock)
            {
                Internal.Start();
            }
        }

        /// <summary>
        /// Stops the schedule or does nothing if it's not running.
        /// This calls does not block.
        /// </summary>
        public void Stop()
        {
            lock (Internal.RunningLock)
            {
                Internal.Stop(false, null);
            }
        }

        /// <summary>
        /// Stops the schedule or does nothing if it's not running.
        /// This calls blocks (it waits for the running job to end its execution).
        /// </summary>
        public void StopAndBlock()
        {
            lock (Internal.RunningLock)
            {
                Internal.Stop(false, null);
            }
        }

        /// <summary>
        /// Stops the schedule or does nothing if it's not running.
        /// This calls blocks (it waits for the running job to end its execution).
        /// </summary>
        /// <param name="timeout">Milliseconds to wait</param>
        public void StopAndBlock(int timeout)
        {
            lock (Internal.RunningLock)
            {
                Internal.Stop(false, timeout);
            }
        }

        /// <summary>
        /// Stops the schedule or does nothing if it's not running.
        /// This calls blocks (it waits for the running job to end its execution).
        /// </summary>
        /// <param name="timeout">Time to wait</param>
        public void StopAndBlock(TimeSpan timeout)
        {
            lock (Internal.RunningLock)
            {
                Internal.Stop(false, timeout.Milliseconds);
            }
        }
    }
}
