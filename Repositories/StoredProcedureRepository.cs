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
    }
}