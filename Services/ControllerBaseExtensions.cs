using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Mvc;

namespace InmobiliariaSarchioniAlfonzo.Controllers.Services
{
    public static class ControllerBaseExtensions
    {
        public static string GenerarUrlCompleta(this ControllerBase controllerBase, string actionName, string controllerName, IWebHostEnvironment environment)
        {
            // Obtiene el esquema (http o https)
            string scheme = controllerBase.Request.Scheme;

            // Obtiene la autoridad (dominio y puerto)
            string dominio = environment.IsDevelopment() ? GetLocalIpAddress() : controllerBase.Request.Host.Host;

            // Obtiene el puerto
            var port = controllerBase.Request.Host.Port;

            // Genera la ruta relativa a la acción específica del controlador
            var relativeUrl = controllerBase.Url.Action(actionName, controllerName);

            // Construye la URL completa
            string urlCompleta = $"{scheme}://{dominio}:{port}{relativeUrl}";

            return urlCompleta;
        }

        private static string GetLocalIpAddress()
        {
            string localIp = "";

            // Obtiene todas las direcciones IP del host local
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                // Selecciona la dirección IPv4 no loopback
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIp = ip.ToString();
                    break;
                }
            }

            return localIp;
        }
    }
}