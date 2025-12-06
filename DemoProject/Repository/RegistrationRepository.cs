using DemoProject.DAL;
using DemoProject.Logics;
using DemoProject.Models;
using DemoProject.Repository.Implementation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace DemoProject.Repository
{
    public class RegistrationRepository : IRegistrationRepository
    {
        private IDalBase _dalBase;
        public RegistrationRepository(IDalBase dalBase)
        {
            _dalBase = dalBase;
        }

        public async Task<bool> InsertRegistration(RegistrationModel model, DataTable dt)
        {
            try
            {
                string[] pName = { "@Name", "@Email", "@Address", "@Qualification", "@Technologies", "@Experiences" };
                object[] pValue = { model.Name, model.Email, model.Address, model.Qualification, model.Technologies, dt };
                int count = await _dalBase.ExecuteProcedureInsertAsync(StoreProcedures.InsertRegistration.GetDescription(), pName, pValue);
                return count > 0 ? true : false;
            }
            catch (Exception e)
            {
                return false;
                throw;
            }
        }

        public async Task<DataTable> GetAllRegistration(int pageNumber, int pageSize)
        {
            DataTable? dt = null;

            try
            {
                string[] pName = { "@pageNumber", "@pageSize" };
                object[] pValue = { pageNumber, pageSize };
                DataSet res = await _dalBase.ExecuteProcedureAsync(StoreProcedures.GetAllRegistration.GetDescription(), pName, pValue);
                
                if (res != null && res.Tables.Count > 0)
                {
                    dt = res.Tables[0];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return dt;
        }

        public async Task<DataTable> GetRegistrationById(int id)
        {
            DataTable? dt = null;

            try
            {
                string[] pName = { "@Id" };
                object[] pValue = { id };
                DataSet res = await _dalBase.ExecuteProcedureAsync(StoreProcedures.GetRegistrationById.GetDescription(), pName, pValue);
                
                if (res != null && res.Tables.Count > 0)
                {
                    dt = res.Tables[0];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return dt;
        }

        public async Task<bool> UpdateRegistration(RegistrationModel model, DataTable dt)
        {
            try
            {
                string[] pName = { "@Id", "@Name", "@Email", "@Address", "@Qualification", "@Technologies", "@Experiences" };
                object[] pValue = { model.Id, model.Name, model.Email, model.Address, model.Qualification, model.Technologies, dt };
                int count = await _dalBase.ExecuteProcedureInsertAsync(StoreProcedures.UpdateRegistration.GetDescription(), pName, pValue);
                return count > 0 ? true : false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public async Task<bool> DeleteRegistration(int id)
        {
            try
            {
                string[] pName = { "@Id" };
                object[] pValue = { id };
                int count = await _dalBase.ExecuteProcedureInsertAsync(StoreProcedures.DeleteRegistration.GetDescription(), pName, pValue);
                return count > 0 ? true : false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public async Task<int> GetTotalRegistrationCount()
        {
            try
            {
                string[] pName = { };
                object[] pValue = { };
                DataSet res = await _dalBase.ExecuteProcedureAsync(StoreProcedures.GetTotalRegistrationCount.GetDescription(), pName, pValue);
                
                if (res != null && res.Tables.Count > 0 && res.Tables[0].Rows.Count > 0)
                {
                    return Convert.ToInt32(res.Tables[0].Rows[0][0]);
                }
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }
    }
}
