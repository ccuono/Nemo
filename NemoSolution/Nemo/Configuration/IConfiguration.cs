﻿using Nemo.Logging;
using Nemo.Serialization;
using Nemo.UnitOfWork;

namespace Nemo.Configuration
{
    public interface IConfiguration
    {
        L1CacheRepresentation DefaultL1CacheRepresentation { get; }
        bool Logging { get; }
        FetchMode DefaultFetchMode { get; }
        MaterializationMode DefaultMaterializationMode { get; }
        string DefaultConnectionName { get; }
        string OperationPrefix { get; }
        ChangeTrackingMode DefaultChangeTrackingMode { get; }
        OperationNamingConvention OperationNamingConvention { get; }
        SerializationMode DefaultSerializationMode { get; }
        bool GenerateDeleteSql { get; }
        bool GenerateInsertSql { get; }
        bool GenerateUpdateSql { get; }
        IAuditLogProvider AuditLogProvider { get; }
        ILogProvider LogProvider { get; }
        IExecutionContext ExecutionContext { get; }
        string HiLoTableName { get; }

        IConfiguration SetDefaultL1CacheRepresentation(L1CacheRepresentation value);
        IConfiguration SetLogging(bool value);
        IConfiguration SetDefaultFetchMode(FetchMode value);
        IConfiguration SetDefaultMaterializationMode(MaterializationMode value);
        IConfiguration SetOperationPrefix(string value);
        IConfiguration SetDefaultConnectionName(string value);
        IConfiguration SetDefaultChangeTrackingMode(ChangeTrackingMode value);
        IConfiguration SetOperationNamingConvention(OperationNamingConvention value);
        IConfiguration SetDefaultSerializationMode(SerializationMode value);
        IConfiguration SetGenerateDeleteSql(bool value);
        IConfiguration SetGenerateInsertSql(bool value);
        IConfiguration SetGenerateUpdateSql(bool value);
        IConfiguration SetAuditLogProvider(IAuditLogProvider value);
        IConfiguration SetExecutionContext(IExecutionContext value);
        IConfiguration SetHiLoTableName(string value);
        IConfiguration SetLogProvider(ILogProvider value);

        IConfiguration Merge(IConfiguration configuration);
    }
}
