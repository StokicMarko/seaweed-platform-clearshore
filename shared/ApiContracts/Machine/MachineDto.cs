namespace ApiContracts.Machine;

public class MachineDto
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
}
