using System.Text.Json;
using System.Text.Json.Serialization;

namespace Services;

/// JSON format/ set of setting -> every change request payload 
/// submitter WRITES with it and the appliers READ with it,
// both sides always agree :)))) Reusable
public static class ChangeRequestJson
{
  
                            //+ JSON type + Shared setting object    //web standard setting camelCase
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }   //Enums  written as text (database readable)
    };
}