using System;
using System.Collections.Generic;

namespace AldursLab.Essentials.Csv
{
    public class EnumerableToCsvBuilder
    {
        public static EnumerableToCsvBuilder<T> Create<T>(IEnumerable<T> sourceData)
        {
            return new EnumerableToCsvBuilder<T>(sourceData);
        }
    }

    public class EnumerableToCsvBuilder<T> : CsvBuilderBase
    {
        readonly List<ConverterColumnDefinition> converterList = new List<ConverterColumnDefinition>();

        public EnumerableToCsvBuilder(IEnumerable<T> sourceData)
        {
            Input = sourceData;
        }

        private IEnumerable<T> Input { get; set; }

        public EnumerableToCsvBuilder<T> AddMapping(string header, Func<T, string> converterFunc)
        {
            if (header == null)
            {
                throw new ArgumentNullException("header");
            }
            if (converterFunc == null)
            {
                throw new ArgumentNullException("converterFunc");
            }

            this.converterList.Add(new ConverterColumnDefinition()
            {
                ColumnHeader = header,
                FieldConverter = converterFunc
            });
            return this;
        }

        public override string BuildCsv()
        {
            if (this.Input == null)
            {
                throw new InvalidOperationException("Input is null");
            }

            if (this.CreateHeaderRow)
            {
                foreach (ConverterColumnDefinition converterColumnDefinition in this.converterList)
                {
                    this.AppendElement(converterColumnDefinition.ColumnHeader);
                }
                this.AppendNewLine();
            }

            foreach (T item in this.Input)
            {
                foreach (var converterColumnDefinition in this.converterList)
                {
                    string text = converterColumnDefinition.FieldConverter(item);
                    this.AppendElement(text);
                }
                this.AppendNewLine();
            }

            return this.GetResult();
        }

        class ConverterColumnDefinition
        {
            public Func<T, string> FieldConverter { get; set; }
            public string ColumnHeader { get; set; }
        }
    }
}