using DemoProject.Models;

namespace DemoProject.Services.Implimentation
{
    public interface IRegistrationServices
    {
        Task<Tuple<bool, string>> InsertRegistration(RegistrationModel registrationModel);
        Task<List<RegistrationModel>> GetAllRegistration(int pageNumber, int pageSize);
        Task<RegistrationModel> GetRegistrationById(int id);
        Task<Tuple<bool, string>> UpdateRegistration(RegistrationModel registrationModel);
        Task<Tuple<bool, string>> DeleteRegistration(int id);
        Task<int> GetTotalRegistrationCount();
    }
}
