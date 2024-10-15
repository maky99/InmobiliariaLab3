using System.ComponentModel.DataAnnotations;

namespace InmobiliariaLab3.Models;

public class Login
{
    public string? Email { get; set; }
    public string? Clave { get; set; }
}