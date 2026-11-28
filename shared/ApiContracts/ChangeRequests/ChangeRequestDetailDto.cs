using System.Text.Json;

namespace ApiContracts.ChangeRequests;

/// One change request with the data it proposes, +PAyloaf            for the admin preview
/// Payload proposed data JSON, shape depends on TargetType: (Media = MediaPayloadDto) 
public class ChangeRequestDetailDto : ChangeRequestDto //extends proposed change 
{
    public JsonElement? Payload { get; set; }
}