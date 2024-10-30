using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Mvc;

namespace InmobiliariaSarchioniAlfonzo.Controllers.Services
{
    public static class ControllerBaseExtensions
    {
        public static string GenerarUrlCompleta(this ControllerBase controllerBase, string actionName, string controllerName, IWebHostEnvironment environment)
        {
            // se obtiene el esquema (http o https)
            string scheme = controllerBase.Request.Scheme;

            // se obtiene la autoridad (dominio y puerto)
            string dominio = environment.IsDevelopment() ? GetLocalIpAddress() : controllerBase.Request.Host.Host;

            // se obtiene el puerto
            var port = controllerBase.Request.Host.Port;

            // se genera la ruta relativa a la accion especifica del controlador
            var relativeUrl = controllerBase.Url.Action(actionName, controllerName);

            // Construye la URL completa
            string urlCompleta = $"{scheme}://{dominio}:{port}{relativeUrl}";

            return urlCompleta;
        }

        private static string GetLocalIpAddress()
        {
            string localIp = "";

            // se obtiene todas las direcciones IP del host local
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                // se selecciona la direccion IPv4 no loopback
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