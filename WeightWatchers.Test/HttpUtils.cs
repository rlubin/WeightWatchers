using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeightWatchers.Test;

internal class HttpUtils
{
    public static StringContent GetJsonHttpContent(object items)
    {
        return new StringContent(JsonConvert.SerializeObject(items), Encoding.UTF8, "application/json");
    }
}