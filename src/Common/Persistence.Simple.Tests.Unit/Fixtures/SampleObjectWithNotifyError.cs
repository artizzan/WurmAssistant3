using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace AldursLab.Persistence.Simple.Tests.Fixtures
{
    public class SampleObjectWithNotifyError : INotifyPropertyChanged
    {
        string data;

        public string Data
        {
            get { return data; }
            set
            {
                data = value;
                // woops, I forgot to notify! 
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}