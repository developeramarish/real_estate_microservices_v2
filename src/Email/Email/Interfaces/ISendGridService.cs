
using System.Threading.Tasks;

namespace Email.Interfaces
{
    public interface ISendGridService
    {
        Task SendMessageUserCreated(string destination);
        Task SendMessagePropertyCreated(string destination);
    }
}
