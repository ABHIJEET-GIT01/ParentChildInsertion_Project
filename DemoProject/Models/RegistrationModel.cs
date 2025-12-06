using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace DemoProject.Models
{
    public class RegistrationModel 
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Email { get; set; }

      
        public string Address { get; set; }
     
        public string Qualification { get; set; }
        

        public string Technologies { get; set; }
        public List<Experience> Experiences { get; set; }

    }
    public class Experience
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public int Years { get; set; }
    }
}
