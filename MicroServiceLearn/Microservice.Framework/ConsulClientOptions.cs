﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microservice.Framework
{
    public class ConsulClientOptions
    {
        public string? IP { get; set; }
        public int Port { get; set; }
        public string? Datacenter { get; set; }
    }
}
