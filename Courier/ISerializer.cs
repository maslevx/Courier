using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Courier
{
    public interface ISerializer
    {
        string Serialize<T>(T message);
        T Deserialize<T>(string data);
    }
}
