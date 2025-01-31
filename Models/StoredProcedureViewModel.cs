using System.ComponentModel.DataAnnotations;

namespace StoredProcedureAPI.Models
{
    public class StoredProcedureViewModel
    {
        public string Name { get; set; }
        public bool IsVisible { get; set; }
        public string Description { get; set; }
        public List<ParameterInfo> Parameters { get; set; } = new();
    }

    public class ParameterInfo
    {
        public string Name { get; set; }
        public string DataType { get; set; }
    }
}