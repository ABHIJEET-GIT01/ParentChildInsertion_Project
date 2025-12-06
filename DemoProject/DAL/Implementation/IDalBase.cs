using System.Data;
using System.Data.SqlClient;

namespace DemoProject.DAL
{
    public interface IDalBase
    {
        Task<DataSet> ExecuteProcedureAsync(string SPName, string[] pName, object[] pValue);
        Task<DataSet> ExecuteProcedureWithoutParametersAsync(string SPName);
        Task<int> ExecuteProcedureInsertAsync(string SPName, string[] pName, object[] pValue);
    }
}
