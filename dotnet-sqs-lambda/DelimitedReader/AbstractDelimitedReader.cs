using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace dotnet_sqs_lambda.DelimitedReader
{
    /// <summary>
    /// Abstract of class to read delimited files
    /// </summary>
    public abstract class AbstractDelimitedReader: IProcessDelimitedFile
    {
        #region Fields
        private ConcurrentBag<string> _errorBag;
        private AsyncBuffer<DelimitedRow> _asyncBuffer;
        private readonly string[] Delimiters;
        private string[] _columnNames;
        #endregion

        #region Properties
        /// <summary>
        /// The column names from the first line of the file
        /// </summary>
        public string[] ColumnNames
        {
            get { return _columnNames; }
            private set
            {
                _columnNames = new string[value.Length];
                for(var i = 0;i < value.Length; i++)
                {
                    _columnNames[i] = value[i].Trim();
                }
            }
        }

        /// <summary>
        /// The current row number
        /// </summary>
        public int RowNumber { get; set; }

        /// <summary>
        /// The total input number expected
        /// </summary>
        public int InputNumber { get; set; }

        private ConcurrentBag<string>  ErrorBag
        {
            get
            {
                _errorBag = _errorBag ?? new ConcurrentBag<string>();
                return _errorBag;
            }
        }

        private AsyncBuffer<DelimitedRow> RowBuffer
        {
            get
            {
                if(_asyncBuffer == null)
                {
                    _asyncBuffer = new AsyncBuffer<DelimitedRow>();
                    _asyncBuffer.ItemSpooled += LineBuffer_Item;
                    _asyncBuffer.ExceptionEncountered += LineBuffer_ExceptionEncountered;
                }

                return _asyncBuffer;
            }
        }

        private void LineBuffer_Empty()
        {
            ReportMessage("Buffer is empty");
        }

        private void LineBuffer_ExceptionEncountered(object sender, Exception ex)
        {
            ReportMessage($"Sender: {sender}", ex);
        }

        private void LineBuffer_Item(DelimitedRow item)
        {
            var success = PreProcessLine(item).Result;
            if(!success)
            {
                ErrorBag.Add(item.Line);
            }
        }
        #endregion

        #region Events and Delegates
        public event EventHandler DelimitedFileProcessed;
        #endregion

        #region Ctors
        /// <summary>
        /// Default Ctor
        /// </summary>
        /// <param name="delimiters">array of delimiters (default is ",")</param>
        public AbstractDelimitedReader(params string[] delimiters)
        {
            if (delimiters.Length == 0)
                delimiters = new string[] { "," };

            Delimiters = delimiters;
        }

        #endregion

        #region Publics
        /// <summary>
        /// Process a delimited stream
        /// </summary>
        /// <param name="inputStream">the content stream</param>
        /// <param name="bufferSize">the size of the processing buffer</param>
        /// <param name="headerLineNo">which line has column headers</param>
        /// <returns></returns>
        public async Task ProcessDelimted(Stream inputStream, int bufferSize, int headerLineNo = 0)
        {
            int lineNo = 0;
            int bytesRead = 0;

            bufferSize = bufferSize > 0 ? bufferSize : 1024;
            headerLineNo = headerLineNo >= 0 ? headerLineNo : 0;

            byte[] buffer = new byte[bufferSize];
            List<char> collected = new List<char>();
            string line = string.Empty;
            do
            {
                bytesRead = await inputStream.ReadAsync(buffer, 0, bufferSize);

                for(var i = 0; i < bytesRead; i++)
                {
                    char c = (char)buffer[i];
                    switch(c)
                    {
                        case '\n':
                            if(lineNo == headerLineNo)
                            {
                                var names = new string(collected.ToArray());
                                ColumnNames = names.Split(Delimiters, StringSplitOptions.None);
                            }
                            else
                            {
                                var rowLine = new string(collected.ToArray());
                                RowBuffer.AddItem(new DelimitedRow(InputNumber, rowLine, ColumnNames, Delimiters));
                                InputNumber++;
                            }
                            lineNo++;
                            collected.Clear();
                            break;
                        default:
                            collected.Add(c);
                            break;
                    }
                }

            } while (bytesRead != 0);

            if(collected.Count > 0)
            {
                var rowLine = new string(collected.ToArray());
                RowBuffer.AddItem(new DelimitedRow(InputNumber, rowLine, ColumnNames, Delimiters));
                InputNumber++;
            }

            while(RowNumber == InputNumber)
            {

            }
        }
        #endregion

        #region Protected
        protected void RaiseProcessCompleted(string message)
        {
            var handler = DelimitedFileProcessed;
            if(handler != null)
            {
                ProcessCompletedEventArgs pcea = new ProcessCompletedEventArgs(message);
                handler(this, pcea);
            }
        }
        #endregion

        #region Abstracts
        protected abstract Task<bool> ProcessLine(DelimitedRow row);

        protected abstract Task ReportMessage(string message, Exception ex = null);
        #endregion

        #region Privates
        private async Task<bool> PreProcessLine(DelimitedRow row)
        {
            RowNumber++;
            var success = await ProcessLine(row);

            if(RowNumber == InputNumber)
            {
                RaiseProcessCompleted("Completed");
            }

            return success;
        }
        #endregion

        #region Virtuals
        /// <summary>
        /// Dispose managed objects (called by Dispose())
        /// </summary>
        protected virtual void DisposeManaged()
        {

        }

        /// <summary>
        /// Dispose unmanaged objects (called by Dispose())
        /// </summary>
        protected virtual void DisposeUnManaged()
        {

        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if(_asyncBuffer != null)
                    {
                        _asyncBuffer.ItemSpooled -= LineBuffer_Item;
                        _asyncBuffer.ExceptionEncountered -= LineBuffer_ExceptionEncountered;

                        _asyncBuffer.Dispose();
                    }
                    DisposeManaged();
                }

                DisposeUnManaged();

                disposedValue = true;
            }
        }

        ~AbstractDelimitedReader()
        {
            Dispose(false);
        }

        /// <summary>
        /// Release all resources used by this object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
