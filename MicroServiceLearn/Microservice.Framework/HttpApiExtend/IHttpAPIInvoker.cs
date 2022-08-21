using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.Framework.HttpApiExtend
{
    public interface IHttpAPIInvoker
    {
        string InvokeApi(string url);
    }
}
