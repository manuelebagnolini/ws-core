﻿using System;
using System.Collections.Generic;
using System.Linq;
using Ws.Core.Extensions.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Ws.Core.Extensions.Data.EF.SqlServer
{
    public class Options : IOptions
    {
        public IEnumerable<Data.DbConnection> Connections { get; set; }
        public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Scoped;
        public StoredProcedureConfig StoredProcedure { get; set; }
        public class StoredProcedureConfig {
            /// <summary>
            /// Stored procedure schema; default: dbo
            /// </summary>
            public string Schema { get; set; } = "dbo";
            public IEnumerable<MappingConfig> Mappings { get; set; }
            public class MappingConfig
            {
                public string NameSpace { get; set; }
                /// <summary>
                /// Entity type name
                /// </summary>
                public string Name { get; set; }
                /// <summary>
                /// default: StoredProcedureConfig.Schema
                /// </summary>
                public string Schema { get; set; }
                /// <summary>
                /// Stored procedure name; default: Name
                /// </summary>
                public string StoredProcedure { get; set; }
                /// <summary>
                /// Call sp for this methods only; default: all methods
                /// </summary>
                public IEnumerable<MethodType> Methods { get; set; }
                public enum MethodType
                {                    
                    List,
                    Find,
                    Add,
                    AddMany,
                    Update,
                    UpdateMany,
                    Merge,
                    Delete,
                    DeleteMany
                }

                public CommandTimeOutConfig CommandTimeOut { get; set; }
                /// <summary>
                /// Max execution time in seconds
                /// </summary>
                public class CommandTimeOutConfig {
                    public int Read { get; set; } 
                    public int Write { get; set; }
                    public int Sync { get; set; } 
                }
            }
        }
        public MergeConfig Merge { get; set; }
        public class MergeConfig: EFCore.BulkExtensions.BulkConfig
        {
            public new bool? UseTempDB { get; set; } = true;
            public new List<string> UpdateByProperties = new[] { "Id" }.ToList();
            public new int? BulkCopyTimeout { get; set; } = 180;
        }
    }

}

namespace Ws.Core.Extensions.Data.EF.SqlServer.Extensions
{
    public static class OptionExtensions
    {
        public static bool? HasMethod(this Options.StoredProcedureConfig.MappingConfig mapping, Options.StoredProcedureConfig.MappingConfig.MethodType method) 
            =>
            mapping == null || mapping.Methods == null || !mapping.Methods.Any() || mapping.Methods.Contains(method);
    }
}