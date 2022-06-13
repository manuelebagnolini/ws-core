﻿using Ws.Core.Extensions.Base;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;

namespace Ws.Core.Extensions.StaticFiles
{
    public class Options : IOptions
    {
        public FolderOption[] Paths { get; set; }

        public class FolderOption : Ws.Core.Shared.StaticFilesFolder.Options
        {
            [Description("List of default static files, i.e. index.html")]
            public String[] DefaultFiles { get; set; }
            [DefaultValue(false)]
            public bool EnableDirectoryBrowser { get; set; } = false;
        }
    }
}