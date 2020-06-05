using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_sqs_lambda.DelimitedReader
{
    /// <summary>
    /// Processing delimited file contract
    /// </summary>
    public interface IProcessDelimitedFile: IDisposable
    {
        /// <summary>
        /// Processing a delimited file stream
        /// </summary>
        /// <param name="inputStream">the content stream</param>
        /// <param name="bufferSize">the read buffer size</param>
        /// <param name="headerLineNo">the line where the header starts the file</param>
        /// <returns>void task</returns>
        Task ProcessDelimted(Stream inputStream, int bufferSize, int headerLineNo = 0);

        /// <summary>
        /// Event raised when the file has completed processing
        /// </summary>
        event EventHandler DelimitedFileProcessed;
    }
}
