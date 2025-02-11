namespace ZooERP.Services.Abstractions;

public interface IHealthValidator<T>
{
    bool CheckHealth(T individual);
}
