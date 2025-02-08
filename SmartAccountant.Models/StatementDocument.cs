namespace SmartAccountant.Models;

public class StatementDocument : BaseModel
{
    public Guid DocumentId { get; init; }

    public Guid StatementId { get; init; }

    public Statement? Statement { get; set; }

    public required string FilePath { get; set; }
}
