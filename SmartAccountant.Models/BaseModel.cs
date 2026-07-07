namespace SmartAccountant.Models;

public abstract record class BaseModel : IModel
{
    public Guid Id { get; set; }
}

public interface IModel
{
    Guid Id { get; set; }
}
