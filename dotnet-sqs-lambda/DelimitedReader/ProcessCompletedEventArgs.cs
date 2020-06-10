using System;
using System.Collections.Generic;
using System.Text;

namespace dotnet_sqs_lambda.DelimitedReader
{
    public class ProcessCompletedEventArgs: EventArgs
    {
        public string Message { get; set; }

        public ProcessCompletedEventArgs()
        {

        }

        public ProcessCompletedEventArgs(string message)
        {
            Message = message;
        }

        public override string ToString()
        {
            return Message;
        }
    }
}
