using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebSockets.Operations
{
    public delegate OperationResult Operation(JObject playload);
}
