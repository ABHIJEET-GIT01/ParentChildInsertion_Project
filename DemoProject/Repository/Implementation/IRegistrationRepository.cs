using DemoProject.Models;
using System.Data;

namespace DemoProject.Repository.Implementation
{
    public interface IRegistrationRepository
    {
        Task<bool> InsertRegistration(RegistrationModel model,DataTable dt);
        Task<DataTable> GetAllRegistration(int pageNumber, int pageSize);
    }
}
