
namespace Entities;
/// Proposed change,  waiting for the Main Admin
/// News, Media, Country and Reference changes all use this one class. Pleasseeee

public class ChangeRequest
{
    public int Id { get; set; }

    ///  website the change is about
    public ChangeTarget TargetType { get; set; }

    /// Create, update or delete
    public ChangeType Type { get; set; }

    /// Id of the existing item. Empty for Create,
    public int? TargetId { get; set; }

    
    /// Proposed data as JSON. Shape depends on Target :)
    public string? PayloadJson { get; set; }

    public ChangeStatus Status { get; set; }

    /// Id of the staff member who submitted it
    public int RequestedBy { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}