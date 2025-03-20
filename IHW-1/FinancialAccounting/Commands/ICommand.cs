using System.Threading.Tasks;

public interface ICommand
{
    Task ExecuteAsync();
}
