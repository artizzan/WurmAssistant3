using System;

namespace AldursLab.Deprec.Core
{
    public struct Percent
    {
        private byte _value;

        public byte Value
        {
            get { return _value; }
            private set { _value = value; }
        }

        public Percent(int percentValueZeroToHundred)
        {
            if (percentValueZeroToHundred < 0)
                percentValueZeroToHundred = 0;
            if (percentValueZeroToHundred > 100)
                percentValueZeroToHundred = 100;
            _value = (byte)percentValueZeroToHundred;
        }

        public Percent Add(int percentValueZeroToHundred)
        {
            return new Percent(_value + percentValueZeroToHundred);
        }

        public static implicit operator Int32(Percent p)
        {
            return p.Value;
        }

        public static implicit operator Percent(Int32 p)
        {
            return new Percent(p);
        }

        public static implicit operator String(Percent p)
        {
            return p.ToString();
        }

        public override string ToString()
        {
            return Value + "%";
        }
    }
}
