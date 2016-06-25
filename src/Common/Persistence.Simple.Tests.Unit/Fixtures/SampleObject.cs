using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace AldursLab.Persistence.Simple.Tests.Fixtures
{
    public class SampleObject : INotifyPropertyChanged
    {
        string data;
        SampleNestedObject sampleNestedObject;
        public event PropertyChangedEventHandler PropertyChanged;

        public string Data
        {
            get { return data; }
            set
            {
                if (value == data) return;
                data = value;
                OnPropertyChanged();
            }
        }

        public SampleNestedObject SampleNestedObject
        {
            get { return sampleNestedObject; }
            set
            {
                if (Equals(value, sampleNestedObject)) return;
                sampleNestedObject = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}