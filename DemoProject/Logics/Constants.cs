using System.ComponentModel;

namespace DemoProject.Logics
{
    public class Constants
    {
        public const string DataSave = "Your data has been saved.";
        public const string InvalidData = "Invalid data";
        public const string NotFound = "No record found.";
        public const string Error = "Something went wrong.";

    }
    public enum StoreProcedures
    {
        [Description("InsertRegistration")]
        InsertRegistration,
        [Description("GetAllRegistration")]
        GetAllRegistration
    }
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            if (fieldInfo != null)
            {
                var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute));

                if (attribute != null)
                {
                    return attribute.Description;
                }
            }

            // If no DescriptionAttribute found, return the enum value as a string
            return value.ToString();
        }
    }
}
