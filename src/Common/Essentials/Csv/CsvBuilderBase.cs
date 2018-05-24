using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.Essentials.Csv
{
    public abstract class CsvBuilderBase
    {
        readonly StringBuilder stringBuilder = new StringBuilder();

        public string FieldDelimiter { get; private set; }

        public string NewLineSeparator { get; private set; }

        public bool AllElementsInQuotes { get; private set; }

        public bool CreateHeaderRow { get; private set; }

        public abstract string BuildCsv();

        protected CsvBuilderBase()
        {
            // defaults
            this.FieldDelimiter = ";";
            this.NewLineSeparator = "\r\n";
            this.AllElementsInQuotes = true;
            this.CreateHeaderRow = true;
        }

        protected virtual void AppendElement(string rawValue)
        {
            stringBuilder.Append(CreateCsvElement(rawValue));
            stringBuilder.Append(FieldDelimiter);
        }

        protected virtual void AppendNewLine()
        {
            stringBuilder.Append(NewLineSeparator);
        }

        protected virtual string GetResult()
        {
            return stringBuilder.ToString();
        }

        protected virtual string CreateCsvElement(string rawText)
        {
            // null should be treated as empty value
            if (rawText == null)
            {
                rawText = string.Empty;
            }

            //csv requires all single quote be replaced with double quotes
            string result = rawText.Replace("\"", "\"\"");

            //csv requires wrapping element in quotes, if it contains certain chars
            if (AllElementsInQuotes ||
                rawText.Contains("\"") ||
                rawText.Contains(FieldDelimiter) ||
                rawText.Contains("\r") ||
                rawText.Contains("\n"))
            {
                result = "\"" + result + "\"";
            }

            return result;
        }
    }
}
