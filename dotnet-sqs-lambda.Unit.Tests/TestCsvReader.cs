using dotnet_sqs_lambda.DelimitedReader;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;

namespace dotnet_sqs_lambda.Unit.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void SimpleReadCsvTest()
        {
            var sampleDir = Path.Combine(TestContext.CurrentContext.WorkDirectory, "SampleFiles");
            var sampleFile = Path.Combine(sampleDir, "MOCK_DATA.csv");

            bool wait = true;

            var strList = new System.Collections.Generic.List<string>();

            if(File.Exists(sampleFile))
            {
                FileStream fs = new FileStream(sampleFile, FileMode.Open, FileAccess.Read);
                CsvReader reader = new CsvReader((row) =>
                {
                    var readStr = $"{row["first_name"]} {row["last_name"]} {row["vin"]}";
                    strList.Add(readStr);
                    return true;
                });

                reader.DelimitedFileProcessed += (sender, args) =>
                {
                    wait = false;
                };

                reader.ProcessDelimted(fs, 1000).Wait();
            }
            Thread.Sleep(500);

            Assert.AreEqual(1000, strList.Count);
            Assert.IsTrue(!wait);
        }


        [Test]
        public void ConvertReadCsvTest()
        {
            var sampleDir = Path.Combine(TestContext.CurrentContext.WorkDirectory, "SampleFiles");
            var sampleFile = Path.Combine(sampleDir, "MOCK_DATA.csv");

            bool wait = true;

            var strList = new System.Collections.Generic.List<string>();

            if (File.Exists(sampleFile))
            {
                FileStream fs = new FileStream(sampleFile, FileMode.Open, FileAccess.Read);
                CsvReader reader = new CsvReader((row) =>
                {
                    var id = row.ValueTo<int>("id");
                    var isActive = row.ValueTo<bool>("active");

                    var readStr = $"{id}, {row["first_name"]} {row["last_name"]} {row["vin"]} Active is {isActive}";
                    strList.Add(readStr);
                    return true;
                });

                reader.DelimitedFileProcessed += (sender, args) =>
                {
                    wait = false;
                };

                reader.ProcessDelimted(fs, 1000).Wait();

                Thread.Sleep(500);
            }

            Assert.AreEqual(1000, strList.Count);
            Assert.IsTrue(!wait);
        }

    }
}