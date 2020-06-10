using System;

namespace dotnet_sqs_lambda.DelimitedReader
{
    /// <summary>
    /// Asynchornous FIFO buffer to handle objects asynchonously in a queue (buffer)
    /// </summary>
    /// <typeparam name="T">the object type for the buffer</typeparam>
    public interface IAsyncBuffer<T>: IDisposable
    {
        /// <summary>
        /// Does the buffer still have items in it?
        /// </summary>
        bool HasMore { get; }

        /// <summary>
        /// Event raised when the buffer has an exception
        /// </summary>
        event AsyncBuffer<T>.ExceptionEncounteredDelegate ExceptionEncountered;

        /// <summary>
        /// Event raised for each item as it is spooled from the buffer (queue)
        /// </summary>
        event AsyncBuffer<T>.ItemSpooledDelegate ItemSpooled;

        /// <summary>
        /// Event raised when the spool for the buffer is empty
        /// </summary>
        event AsyncBuffer<T>.SpoolerEmptyDelegate SpoolerEmpty;

        /// <summary>
        /// add an object of type T to the buffer
        /// </summary>
        /// <param name="item">the object to add</param>
        /// <param name="itemCausesStop">when spooling this item stop on the object</param>
        void AddItem(T item, bool itemCausesStop = false);

        /// <summary>
        /// Virtual dispose method called in the object dispose (callng Dispose())
        /// </summary>
        void DeterministicDispose();

        /// <summary>
        /// Virtual dispose method called in the object dispose (finalizer calls dispose)
        /// </summary>
        void FinalizeDispose();

        /// <summary>
        /// Virtual dispose method called in the object dispose (always called)
        /// </summary>
        void GeneralDispose();

        /// <summary>
        /// Clears the buffer
        /// </summary>
        void Reset();

        /// <summary>
        /// Resume spooling the buffer after a stop
        /// </summary>
        void Resume();

        /// <summary>
        /// Stop, sort the objects and then resume
        /// </summary>
        void Sort();

        /// <summary>
        /// Stop spooling the buffer
        /// </summary>
        void Stop();
    }
}