using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Toolkit.Tasks
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Handles faulted tasks by logging an exception record for a failed task.
        /// </summary>
        public static void ObserveAndLogFailure(this Task task, string message)
        {
            task.ContinueWith(t =>
            {
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                if (t.Exception != null && t.Exception.InnerException != null)
                {
                    Debug.WriteLine($"{message}\r\n\tException:{t.Exception.InnerException.ToString()}");
                    t.Exception.Handle((ex) => true);
                }
            },
            TaskContinuationOptions.OnlyOnFaulted);
        }

        /// <summary>
        /// Handles faulted tasks by taking an action on the same thread as the caller.
        /// </summary>
        public static void ObserveFailureWith(this Task task, Action<Task> action)
        {
            task.ContinueWith(t =>
            {
                action.Invoke(t);
            },
            CancellationToken.None,
            TaskContinuationOptions.OnlyOnFaulted,
            TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Convenience method to enable running of task continuations on the thread that created the task.
        /// Note: Continuation is responsible for check if the task succeeded
        /// </summary>
        public static Task ContinueOnThisThreadWith(this Task task, Action<Task> continuationAction)
        {
            return task.ContinueWith(t =>
            {
                if (!t.IsCanceled &&
                    t.IsCompleted)
                {
                    continuationAction(t);
                }
            },
            TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Convenience method to enable running of task continuations on the thread that created the task.
        /// Note: Continuation is responsible for check if the task succeeded
        /// </summary>
        public static Task<T> ContinueOnThisThreadWith<T>(this Task task, Func<Task, T> continuationAction)
        {
            return task.ContinueWith(t =>
            {
                if (!t.IsCanceled &&
                    t.IsCompleted)
                {
                    return continuationAction(t);
                }

                return default(T);
            },
            TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Convenience method to enable running of task continuations on the thread that created the task.
        /// Note: Continuation is responsible for check if the task succeeded
        /// </summary>
        public static Task ContinueOnThisThreadWith<T>(this Task<T> task, Action<Task<T>> continuationAction)
        {
            return task.ContinueWith(t =>
            {
                if (!t.IsCanceled &&
                    t.IsCompleted)
                {
                    continuationAction(t);
                }
            },
            TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Convenience method to enable running of task continuations on the thread that created the task.
        /// Note: Continuation is responsible for check if the task succeeded
        /// </summary>
        public static Task<TNew> ContinueOnThisThreadWith<T, TNew>(this Task<T> task, Func<Task<T>, TNew> continuationAction)
        {
            return task.ContinueWith(t =>
            {
                if (!t.IsCanceled &&
                    t.IsCompleted)
                {
                    return continuationAction(t);
                }

                return default(TNew);
            },
            TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Convenience method to enable fail fast if a task continuation encountered an exception that was unexpected or unobserved.
        /// </summary>
        public static void ObserveAndFailFast(this Task task, string message)
        {
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    t.Exception.FailFastUnhandled("ObserveAndFailFast: " + message);
                }
            });
        }

        /// <summary>
        /// Convenience method to get the result of an async task and return a default value if an exception
        /// or cancellation occurs while executing the task.
        /// </summary>
        public static Task<T> GetSafeResult<T>(this Task<T> task, T defaultResult, string errorMessage)
        {
            return task.ContinueWith(t =>
            {
                T result = (t.IsFaulted || t.IsCanceled) ? defaultResult : t.Result;
                if (t.IsFaulted)
                {
                    t.Exception.Handle((ex) => true);
                }

                return result;
            });
        }

        /// <summary>
        /// Respond to an unhandled exception by fail fasting, optionally invoking a callback.  If no callback is
        /// specified, use default logging.
        /// </summary>
        /// <param name="e">The Exception that is unhandled.</param>
        /// <param name="message">A message to log for the exception.</param>
        public static void FailFastUnhandled(this Exception e, string message)
        {
            var baseException = e.GetBaseException();

            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }

            // RaiseFailFastException to terminate the application and generate a Watson dump
            Environment.FailFast(message, e);
        }
    }
}
