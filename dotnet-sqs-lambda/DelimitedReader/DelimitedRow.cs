using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_sqs_lambda.DelimitedReader
{
    /// <summary>
    /// Data class for values in a row and accessed by 0 based index
    /// </summary>
    public class DelimitedRow
    {
        #region Fields
        private List<string> _parseList;
        private List<string> _columnNames;
        #endregion

        #region Private properties
        private List<string> ColumnNames
        {
            get
            {
                if(_columnNames == null)
                {
                    _columnNames = new List<string>();
                }
                return _columnNames;
            }
            set
            {
                _columnNames = value;
            }
        }
        private List<string> ParseList
        {
            get
            {
                _parseList = _parseList ?? new List<string>();
                return _parseList;
            }
        }

        private string[] Delimiters { get; set; }
        private bool IsParsed { get; set; }
        #endregion

        #region Public properties
        public int RowIndex { get; set; }

        /// <summary>
        /// The orignal line content
        /// </summary>
        public string Line { get; set; }
        #endregion

        #region Ctors
        /// <summary>
        /// Default Ctor
        /// </summary>
        /// <param name="ndx">the index for this row from the file read</param>
        /// <param name="line">the content of the row</param>
        /// <param name="columnNames">the column names for the row</param>
        /// <param name="delimiters">the delimiters for parsing the row</param>
        public DelimitedRow(int ndx, string line, string[] columnNames, string[] delimiters)
        {
            RowIndex = ndx;
            Line = line;
            Delimiters = delimiters;
             ColumnNames.AddRange(columnNames);
        }
        #endregion

        #region Accessors and value converters
        /// <summary>
        /// Accessor by Index value
        /// </summary>
        /// <param name="ndx">the index for the row</param>
        /// <returns>the value at the row</returns>
        /// <exception cref="IndexOutOfRangeException">thrown if index is out of range for the row</exception>
        public string this[int ndx]
        {
            get
            {
                ParseLine();

                return ParseList[ndx];
            }
        }

        /// <summary>
        /// Accessor by Column name
        /// </summary>
        /// <param name="columnName">the column name to get the value from</param>
        /// <returns>the value for the column</returns>
        /// <exception cref="IndexOutOfRangeException">thrown if column name cannot be recognized</exception>
        public string this[string columnName]
        {
            get
            {
                ParseLine();
                int ndx = ColumnNames.IndexOf(columnName);

                return ParseList[ndx];
            }
        }

        /// <summary>
        /// Get the value at the index as a type (T)
        /// </summary>
        /// <typeparam name="T">the type to convert the cell value</typeparam>
        /// <param name="ndx">the column index for the value</param>
        /// <returns>the value as the type specified</returns>
        /// <exception cref="IndexOutOfRangeException">thrown if index is out of range for the row</exception>
        public T ValueTo<T>(int ndx) where T:struct
        {        
            ParseLine();
            return (T)Convert.ChangeType(ParseList[ndx].Trim(), typeof(T));
        }

        /// <summary>
        /// Get the value for the column name as a type (T)
        /// </summary>
        /// <typeparam name="T">the type to convert the cell value</typeparam>
        /// <param name="ndx">the column name for the value</param>
        /// <returns>the value as the type specified</returns>
        /// <exception cref="IndexOutOfRangeException">thrown if the column name for the row is not found</exception>
        public T ValueTo<T>(string columnName) where T : struct
        {
            ParseLine();
            int ndx = ColumnNames.IndexOf(columnName);
            return (T)Convert.ChangeType(ParseList[ndx].Trim(), typeof(T));
        }
        #endregion

        #region Privates
        private void ParseLine()
        {
            if(!IsParsed)
            {
                ParseList.AddRange(Line.Split(Delimiters, StringSplitOptions.None));
                IsParsed = true;
            }
        }
        #endregion
    }
}
