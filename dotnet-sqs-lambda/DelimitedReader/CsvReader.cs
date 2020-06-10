using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_sqs_lambda.DelimitedReader
{
    public class CsvReader : AbstractDelimitedReader
    {
        private Func<DelimitedRow, bool> LineProcessDelegate { get; set; }


        public CsvReader(Func<DelimitedRow, bool> lineProcessDelegate)
        {
            LineProcessDelegate = lineProcessDelegate;
        }

        protected async override Task<bool> ProcessLine(DelimitedRow row)
        {
           return await Task.Run(() =>
           {
               return LineProcessDelegate(row);
           });
            
        }

        protected async override Task ReportMessage(string message, Exception ex = null)
        {
            await Task.Run(() =>
            {
                Console.WriteLine($"{message}");
            });
            
        }
    }
}
