using Dapper;
using MySql.Data.MySqlClient;
using StoredProcedureAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoredProcedureAPI.Repositories
{
    public class StoredProcedureRepository : IStoredProcedureRepository
    {
        // Add a method to check SP status
        public async Task<bool> IsStoredProcedurePublic(string procedureName)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var query = @"SELECT is_public FROM stored_procedure_settings 
                             WHERE procedure_name = @ProcedureName";
                
                var result = await connection.QueryFirstOrDefaultAsync<bool?>(query, 
                    new { ProcedureName = procedureName });
                
                return result ?? false; // If no setting exists, consider it private
            }
        }
        private readonly string _connectionString;

        public StoredProcedureRepository(DatabaseSettings settings)
        {
            _connectionString = settings.ConnectionString;
        }

        public async Task<IEnumerable<StoredProcedureInfo>> GetAvailableStoredProceduresAsync()
        {
            const string sql = @"
                SELECT DISTINCT
                    p.SPECIFIC_NAME as Name,
                    COALESCE(s.is_public, true) as IsPublic,
                    COALESCE(s.created_date, NOW()) as CreatedDate
                FROM information_schema.ROUTINES p
                INNER JOIN stored_procedure_settings s ON p.SPECIFIC_NAME = s.procedure_name
                WHERE p.ROUTINE_SCHEMA = @databaseName
                AND p.ROUTINE_TYPE = 'PROCEDURE'";
    
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<StoredProcedureInfo>(sql, 
                    new { databaseName = connection.Database });
            }
        }
    
        public async Task<bool> UpdateStoredProcedureVisibilityAsync(string procedureName, bool isPublic)
        {
            const string sql = @"
                UPDATE stored_procedure_settings set is_public = @isPublic 
                WHERE procedure_name = @name";
    
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var result = await connection.ExecuteAsync(sql, new { name = procedureName, isPublic });
                return result > 0;
            }
        }
    
        public async Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(string procedureName, object parameters = null)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<T>(procedureName, parameters, 
                    commandType: System.Data.CommandType.StoredProcedure);
            }
        }
    
        public async Task<int> ExecuteNonQueryStoredProcedureAsync(string procedureName, object parameters = null)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.ExecuteAsync(procedureName, parameters, 
                    commandType: System.Data.CommandType.StoredProcedure);
            }
        }
        public async Task<List<ParameterDetailInfo>> GetStoredProcedureParametersAsync(string procedureName)
        {
            const string sql = @"
                SELECT 
                    PARAMETER_NAME as Name,
                    DATA_TYPE as DataType,
                    PARAMETER_MODE as Mode
                FROM INFORMATION_SCHEMA.PARAMETERS
                WHERE SPECIFIC_SCHEMA = @databaseName
                AND SPECIFIC_NAME = @procedureName
                ORDER BY ORDINAL_POSITION";
    
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var parameters = await connection.QueryAsync<ParameterDetailInfo>(sql,
                    new { databaseName = connection.Database, procedureName });
                return parameters.ToList();
            }
        }

        public async Task<object> ExecuteStoredProcedureWithDefaultsAsync(string procedureName)
        {
            var parameters = await GetStoredProcedureParametersAsync(procedureName);
            var dynamicParams = new DynamicParameters();

            // Set default values based on parameter types
            foreach (var param in parameters)
            {
                object defaultValue = GetDefaultValueForType(param.DataType);
                dynamicParams.Add(param.Name, defaultValue);
            }

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                try
                {
                    var result = await connection.QueryAsync(procedureName, dynamicParams,
                        commandType: System.Data.CommandType.StoredProcedure);
                    return result.ToList();
                }
                catch (Exception)
                {
                    return new List<object>();
                }
            }
        }

        private object GetDefaultValueForType(string dataType)
        {
            return dataType.ToLower() switch
            {
                "varchar" or "char" or "text" => "sample",
                "int" or "bigint" or "smallint" or "tinyint" => 1,
                "decimal" or "float" or "double" => 1.0,
                "datetime" or "timestamp" or "date" => DateTime.Now,
                "bit" or "boolean" => false,
                _ => DBNull.Value
            };
        }
    }
}