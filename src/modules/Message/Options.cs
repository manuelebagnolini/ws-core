﻿using Ws.Core.Extensions.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Ws.Core.Extensions.Message
{
    public interface IMessageConfiguration
    {
        IEnumerable<Options.Endpoint> Senders { get; set; }
        IEnumerable<Options.Endpoint> Receivers { get; set; }
    }
    public class Options : IOptions, IMessageConfiguration
    {
        public IEnumerable<Endpoint> Senders { get; set; }
        public IEnumerable<Endpoint> Receivers { get; set; }
        public class Endpoint
        {
            public string Address { get; set; }
            public int Port { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            [DefaultValue(false)]
            public bool EnableSsl { get; set; } = false;
            [Description("Only available in DEBUG mode")]
            [DefaultValue(false)]
            public bool SkipCertificateValidation { get; set; } = false;
        }
    }
}
