namespace SmartAccountant.Models;

public interface IModel
{
    Guid Id { get; set; }
}

public abstract record class BaseModel : IModel
{
    public Guid Id { get; set; }
}
