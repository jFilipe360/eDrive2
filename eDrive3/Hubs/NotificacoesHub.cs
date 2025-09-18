using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace eDrive3.Hubs
{
    public class NotificacoesHub : Hub
    {
        // Método chamado pela secretaria ao marcar aula
        public async Task NotificarNovaAula(string mensagem)
        {
            // Enviar para todos os clientes (todos os utilizadores conectados)
            await Clients.All.SendAsync("ReceberNotificacao", mensagem);
        }
    }
}
