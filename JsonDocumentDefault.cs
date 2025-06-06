using Swashbuckle.AspNetCore.Filters;
using System.Text.Json;

namespace Compellio.DCAP.Web
{
    public class JsonDocumentDefault : IExamplesProvider<JsonDocument>
    {
        public JsonDocument GetExamples() => JsonDocument.Parse("{ \"name\": \"John Doe\", \"age\": 28 }");
    }
}
