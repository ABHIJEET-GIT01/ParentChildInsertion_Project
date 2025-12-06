using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DemoProject.DAL
{
    public class DalBase : IDalBase
    {
        private readonly string _connectionString;
        private readonly ILogger<DalBase> _logger;

        public DalBase(IConfiguration configuration, ILogger<DalBase> logger)
        {
            _connectionString = configuration.GetSection("ConnectionStrings")["MyConnection"];
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Connection string 'MyConnection' is not configured.");
            }
        }

        private async Task<T> ExecuteAsync<T>(Func<SqlConnection, Task<T>> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            using SqlConnection connection = new SqlConnection(_connectionString);
            try
            {
                await connection.OpenAsync();
                return await action.Invoke(connection);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Database operation failed: {Message}", sqlEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during database operation: {Message}", ex.Message);
                throw;
            }
        }

        private void AddParameters(SqlCommand command, string[] parameterNames, object[] parameterValues)
        {
            if (parameterNames == null || parameterValues == null)
                return;

            if (parameterNames.Length != parameterValues.Length)
                throw new ArgumentException("Parameter names and values arrays must have the same length.");

            for (int i = 0; i < parameterNames.Length; i++)
            {
                command.Parameters.AddWithValue(parameterNames[i], parameterValues[i] ?? DBNull.Value);
            }
        }

        public async Task<DataSet> ExecuteProcedureAsync(string SPName, string[] pName, object[] pValue)
        {
            if (string.IsNullOrEmpty(SPName))
                throw new ArgumentException("Stored procedure name cannot be null or empty.", nameof(SPName));

            return await ExecuteAsync(async connection =>
            {
                using SqlCommand sqlCommand = new SqlCommand(SPName, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                AddParameters(sqlCommand, pName, pValue);

                using SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                DataSet dsResultSet = new DataSet();
                await Task.Run(() => sqlDataAdapter.Fill(dsResultSet));
                return dsResultSet;
            });
        }

        public async Task<DataSet> ExecuteProcedureWithoutParametersAsync(string SPName)
        {
            if (string.IsNullOrEmpty(SPName))
                throw new ArgumentException("Stored procedure name cannot be null or empty.", nameof(SPName));

            return await ExecuteAsync(async connection =>
            {
                using SqlCommand sqlCommand = new SqlCommand(SPName, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                using SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                DataSet dsResultSet = new DataSet();
                await Task.Run(() => sqlDataAdapter.Fill(dsResultSet));
                return dsResultSet;
            });
        }

        public async Task<int> ExecuteProcedureInsertAsync(string SPName, string[] pName, object[] pValue)
        {
            if (string.IsNullOrEmpty(SPName))
                throw new ArgumentException("Stored procedure name cannot be null or empty.", nameof(SPName));

            return await ExecuteAsync(async connection =>
            {
                using SqlCommand sqlCommand = new SqlCommand(SPName, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                AddParameters(sqlCommand, pName, pValue);

                var retParam = sqlCommand.Parameters.Add("@returnStatus", SqlDbType.Int);
                retParam.Direction = ParameterDirection.ReturnValue;

                try
                {
                    await sqlCommand.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error executing insert procedure: {ProcedureName}", SPName);
                    throw;
                }

                if (sqlCommand.Parameters["@returnStatus"].Value == DBNull.Value)
                {
                    _logger.LogWarning("Return status parameter was not set by stored procedure: {ProcedureName}", SPName);
                    return 0;
                }

                return Convert.ToInt32(sqlCommand.Parameters["@returnStatus"].Value);
            });
        }
    }
}



