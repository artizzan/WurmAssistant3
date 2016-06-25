using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AldursLab.Persistence
{
    public interface ISerializer
    {
        string Serialize<T>(T obj);

        T Deserialize<T>(string data);
    }
}
