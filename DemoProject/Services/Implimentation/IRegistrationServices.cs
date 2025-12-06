using DemoProject.Models;

namespace DemoProject.Services.Implimentation
{
    public interface IRegistrationServices
    {
        Task<Tuple<bool, string>> InsertRegistration(RegistrationModel registrationModel);
        Task<List<RegistrationModel>> GetAllRegistration(int pageNumber, int pageSize);
    }
}
