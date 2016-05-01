using System;
using System.IO;
using System.Text;
using AldursLab.WurmApi.Extensions.DotNet;

namespace AldursLab.WurmApi.Modules.Wurm.LogReading
{
    abstract class LogFileStreamReader : IDisposable
    {
        // Mono implementation differs from .NET, file byte positions are not as easy to obtain
        // Do not use trackFileBytePositions outside .NET, rely on line indexes, this is much slower but at least it works.

        static Func<StreamReader, int> _getCharPosAccessor;

        static Func<StreamReader, int> GetCharPosAccessor
        {
            get
            {
                if (_getCharPosAccessor == null)
                {
                    _getCharPosAccessor =
                        ReflectionHelper.GetFieldAccessor<StreamReader, int>("charPos");
                }
                return _getCharPosAccessor;
            }
        }

        static Func<StreamReader, int> _getCharLenAccessor;

        static Func<StreamReader, int> GetCharLenAccessor
        {
            get
            {
                if (_getCharLenAccessor == null)
                {
                    _getCharLenAccessor =
                        ReflectionHelper.GetFieldAccessor<StreamReader, int>("charLen");
                }
                return _getCharLenAccessor;
            }
        }

        private readonly long startPosition;
        private readonly bool trackFileBytePositions;
        protected readonly StreamReader StreamReader;

        protected int CurrentResult;
        protected int NextResult;
        protected char CurrentChar;
        protected char NextChar;

        protected readonly StringBuilder StringBuilder = new StringBuilder();

        protected LogFileStreamReader(string fileFullPath, long startPosition = 0, bool trackFileBytePositions = false)
        {
            if (fileFullPath == null)
                throw new ArgumentNullException(nameof(fileFullPath));
            if (startPosition < 0)
                throw new ArgumentException("startPosition must be non-negative");
            this.startPosition = startPosition;
            this.trackFileBytePositions = trackFileBytePositions;

            var stream = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader = new StreamReader(stream, Encoding.UTF8);
            if (startPosition != 0)
            {
                StreamReader.BaseStream.Seek(startPosition, SeekOrigin.Begin);
            }
            LastReadLineIndex = -1;
        }

        /// <summary>
        /// After reaching end of file, this points to the last character.
        /// </summary>
        public long LastReadLineStartPosition { get; private set; }

        public void Seek(long byteOffsetFromBeginning)
        {
            StreamReader.BaseStream.Seek(byteOffsetFromBeginning, SeekOrigin.Begin);
        }

        public void SeekToLineIndex(int index)
        {
            for (int i = 0; i < index; i++)
            {
                // simply read all lines until index is reached
                // this is not efficient, but works
                TryReadNextLine();
            }
        }

        private void UpdateCurrentLineStartPosition()
        {
            long charlen = GetCharLenAccessor(StreamReader);
            long charpos = GetCharPosAccessor(StreamReader);
            LastReadLineStartPosition = StreamReader.BaseStream.Position - charlen + charpos;
        }

        /// <summary>
        /// -1 if no lines read yet.
        /// After reaching end of file, this still indicates last line index.
        /// </summary>
        public int LastReadLineIndex { get; private set; }

        public long StreamPosition => StreamReader.BaseStream.Position;

        bool endOfStreamPositionUpdated;

        public string TryReadNextLine()
        {
            var pk = StreamReader.Peek();
            if (pk == -1)
            {
                if (trackFileBytePositions && !endOfStreamPositionUpdated)
                {
                    UpdateCurrentLineStartPosition();
                    endOfStreamPositionUpdated = true;
                }
                return null;
            }
            StringBuilder.Clear();
            
            if (trackFileBytePositions)
            {
                UpdateCurrentLineStartPosition();
            }

            var chars = ReadCharsForNextLine();
            LastReadLineIndex++;
            return chars;
        }

        protected abstract string ReadCharsForNextLine();

        public void Dispose()
        {
            StreamReader.Dispose();
        }

        ~LogFileStreamReader()
        {
            var sr = StreamReader;
            if (sr != null)
            {
                try
                {
                    sr.Dispose();
                }
                catch (Exception)
                {
                    // nothing more can be done
                }
            }
        }

        public abstract void FastForwardLinesCount(int lineCountToSkip);
    }
}