using System.Threading.Tasks;

public interface ICommand
{
    public Task Do();
    public Task Undo();
}