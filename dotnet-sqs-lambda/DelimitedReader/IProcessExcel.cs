using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_sqs_lambda.DelimitedReader
{
    public interface IProcessExcel
    {
        Task ProcessExcel(Stream inputStream);

        event EventHandler ExcelFileProcessed;
    }
}
