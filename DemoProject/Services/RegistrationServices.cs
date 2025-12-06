using DemoProject.Logics;
using DemoProject.Models;
using DemoProject.Repository.Implementation;
using DemoProject.Services.Implimentation;
using System.Data;

namespace DemoProject.Services
{
    public class RegistrationServices : IRegistrationServices
    {
        private IRegistrationRepository _registrationRepository;

        public RegistrationServices(IRegistrationRepository registrationRepository)
        {
            _registrationRepository = registrationRepository;
        }

        public async Task<Tuple<bool, string>> InsertRegistration(RegistrationModel registrationModel)
        {
            if (registrationModel == null)
                return new Tuple<bool, string>(false, Constants.InvalidData);

            if (!IsValidRegistration(registrationModel))
                return new Tuple<bool, string>(false, Constants.InvalidData);

            DataTable dt = CreateExperiencesDataTable(registrationModel.Experiences);

            bool res = await _registrationRepository.InsertRegistration(registrationModel, dt);

            return new Tuple<bool, string>(res, res ? Constants.DataSave : Constants.Error);
        }

        public async Task<List<RegistrationModel>> GetAllRegistration(int pageNumber, int pageSize)
        {
            DataTable dt = await _registrationRepository.GetAllRegistration(pageNumber, pageSize);

            if (dt == null || dt.Rows.Count == 0)
                return new List<RegistrationModel>();

            return MapDataTableToRegistrations(dt);
        }

        public async Task<RegistrationModel> GetRegistrationById(int id)
        {
            DataTable dt = await _registrationRepository.GetRegistrationById(id);

            if (dt == null || dt.Rows.Count == 0)
                return null;

            return MapDataTableToRegistrations(dt).FirstOrDefault();
        }

        public async Task<Tuple<bool, string>> UpdateRegistration(RegistrationModel registrationModel)
        {
            if (registrationModel == null)
                return new Tuple<bool, string>(false, Constants.InvalidData);

            if (!IsValidRegistration(registrationModel))
                return new Tuple<bool, string>(false, Constants.InvalidData);

            DataTable dt = CreateExperiencesDataTable(registrationModel.Experiences, registrationModel.Id);

            bool res = await _registrationRepository.UpdateRegistration(registrationModel, dt);

            return new Tuple<bool, string>(res, res ? "Registration updated successfully!" : Constants.Error);
        }

        public async Task<Tuple<bool, string>> DeleteRegistration(int id)
        {
            try
            {
                bool res = await _registrationRepository.DeleteRegistration(id);
                return new Tuple<bool, string>(res, res ? "Registration deleted successfully!" : Constants.Error);
            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, "Error deleting registration: " + ex.Message);
            }
        }

        public async Task<int> GetTotalRegistrationCount()
        {
            return await _registrationRepository.GetTotalRegistrationCount();
        }

        private DataTable CreateExperiencesDataTable(List<Experience> experiences, int id = 0)
        {
            DataTable dt = new DataTable();

            //if (id > 0)
            //{
            //    dt.Columns.Add("RegistrationId", typeof(int));
            //}

            dt.Columns.Add("CompanyName", typeof(string));
            dt.Columns.Add("Years", typeof(int));
            

            if (experiences != null)
            {
                foreach (var item in experiences)
                {
                    dt.Rows.Add(item.CompanyName, item.Years);

                    //if (id > 0)
                    //{
                    //    dt.Rows.Add(id, item.CompanyName, item.Years);
                    //}
                    //else
                    //{
                    //}
                }
            }
    
            return dt;
        }

        private bool IsValidRegistration(RegistrationModel model)
        {
            return !string.IsNullOrWhiteSpace(model.Name) &&
                   !string.IsNullOrWhiteSpace(model.Email) &&
                   !string.IsNullOrWhiteSpace(model.Address);
        }

        private List<RegistrationModel> MapDataTableToRegistrations(DataTable dt)
        {
            var registrations = new Dictionary<int, RegistrationModel>();

            foreach (DataRow row in dt.Rows)
            {
                int id = Convert.ToInt32(row["Id"]);

                if (!registrations.ContainsKey(id))
                {
                    registrations[id] = new RegistrationModel
                    {
                        Id = id,
                        Name = row["Name"]?.ToString() ?? string.Empty,
                        Email = row["Email"]?.ToString() ?? string.Empty,
                        Address = row["Address"]?.ToString() ?? string.Empty,
                        Qualification = row["Qualification"]?.ToString() ?? string.Empty,
                        Technologies = row["Technologies"]?.ToString() ?? string.Empty,
                        Experiences = new List<Experience>()
                    };
                }

                registrations[id].Experiences.Add(new Experience
                {
                    Id = id,
                    CompanyName = row["CompanyName"]?.ToString() ?? string.Empty,
                    Years = Convert.ToInt32(row["Years"])
                });
            }

            return registrations.Values.ToList();
        }
    }
}
