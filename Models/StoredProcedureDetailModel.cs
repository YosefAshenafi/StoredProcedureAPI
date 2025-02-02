using System.Collections.Generic;

namespace StoredProcedureAPI.Models
{
    public class StoredProcedureDetailModel
    {
        public string Name { get; set; }
        public List<ParameterDetailInfo> Parameters { get; set; } = new();
        public object SampleResponse { get; set; }
    }

    public class ParameterDetailInfo
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public string Mode { get; set; }
    }
}