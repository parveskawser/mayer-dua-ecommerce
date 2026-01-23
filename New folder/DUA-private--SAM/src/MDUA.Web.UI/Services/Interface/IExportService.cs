using System.Collections.Generic;

namespace MDUA.Web.UI.Services.Interface
{
    public interface IExportService
    {
        byte[] GenerateFile(List<Dictionary<string, object>> data, string format, List<string> columns);
    }
}