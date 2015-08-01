using System;
using System.IO;
using System.Text;
using AldursLab.Essentials.Extensions.DotNet;

namespace AldurSoft.WurmApi.Utility
{
    public class LogFileStreamReaderFactory
    {
        public virtual LogFileStreamReader Create(
            string fileFullPath,
            long startPosition = 0,
            bool trackFileBytePositions = false)
        {
            return new LogFileStreamReader(fileFullPath, startPosition, trackFileBytePositions);
        }
    }

    public class LogFileStreamReader : IDisposable
    {
        private readonly long startPosition;
        private readonly bool trackFileBytePositions;
        private readonly StreamReader streamReader;

        int currentResult;
        int nextResult;
        char currentChar;
        char nextChar;

        readonly StringBuilder stringBuilder = new StringBuilder();

        public LogFileStreamReader(string fileFullPath, long startPosition = 0, bool trackFileBytePositions = false)
        {
            if (fileFullPath == null)
                throw new ArgumentNullException("fileFullPath");
            if (startPosition < 0)
                throw new ArgumentException("startPosition must be non-negative");
            this.startPosition = startPosition;
            this.trackFileBytePositions = trackFileBytePositions;

            var stream = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            streamReader = new StreamReader(stream, Encoding.Default);
            if (startPosition != 0)
            {
                streamReader.BaseStream.Seek(startPosition, SeekOrigin.Begin);
            }
            LastReadLineIndex = -1;
        }

        /// <summary>
        /// After reaching end of file, this points to the last character.
        /// </summary>
        public long LastReadLineStartPosition { get; private set; }

        public void Seek(long offsetFromBeginning)
        {
            streamReader.BaseStream.Seek(offsetFromBeginning, SeekOrigin.Begin);
        }

        private void UpdateCurrentLineStartPosition()
        {
            long charlen = GetCharLenAccessor(streamReader);
            long charpos = GetCharPosAccessor(streamReader);
            LastReadLineStartPosition = streamReader.BaseStream.Position - charlen + charpos;
        }

        private static readonly Func<StreamReader, int> GetCharPosAccessor = ReflectionHelper.GetFieldAccessor<StreamReader, int>("charPos");
        private static readonly Func<StreamReader, int> GetCharLenAccessor = ReflectionHelper.GetFieldAccessor<StreamReader, int>("charLen");

        /// <summary>
        /// -1 if no lines read yet.
        /// After reaching end of file, this still indicates last line index.
        /// </summary>
        public int LastReadLineIndex { get; private set; }

        public long StreamPosition
        {
            get { return streamReader.BaseStream.Position; }
        }

        private bool endOfStreamPositionUpdated = false;

        public virtual string TryReadNextLine()
        {
            var pk = streamReader.Peek();
            if (pk == -1)
            {
                if (trackFileBytePositions && !endOfStreamPositionUpdated)
                {
                    UpdateCurrentLineStartPosition();
                    endOfStreamPositionUpdated = true;
                }
                return null;
            }
            stringBuilder.Clear();
            LastReadLineIndex++;
            if (trackFileBytePositions)
            {
                UpdateCurrentLineStartPosition();
            }

            while (true)
            {
                currentResult = streamReader.Read();
                if (currentResult == -1)
                {
                    return stringBuilder.ToString();
                }
                currentChar = (char)currentResult;
                if (currentChar == '\r')
                {
                    nextResult = streamReader.Read();
                    if (nextResult != -1)
                    {
                        nextChar = (char)nextResult;
                        if (nextChar == '\n')
                        {
                            return stringBuilder.ToString();
                        }
                        else
                        {
                            stringBuilder.Append(currentChar);
                            stringBuilder.Append(nextChar);
                        }
                    }
                    else
                    {
                        stringBuilder.Append(currentChar);
                    }
                }
                else
                {
                    stringBuilder.Append(currentChar);
                }
            }
        }

        public void Dispose()
        {
            streamReader.Dispose();
        }
    }
}
