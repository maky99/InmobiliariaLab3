using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaLab3.Models;

public class Inmueblee
{
    public int Id_Propietario { get; set; }
    // [Required(ErrorMessage = "La dirección es obligatoria.")]
    public string? Direccion { get; set; }
    // [Required(ErrorMessage = "El uso es obligatorio.")]
    public string? Uso { get; set; }
    public int Ambientes { get; set; }
    // [Required(ErrorMessage = "El tamaño es obligatorio.")]
    public double Tamano { get; set; }
    public int Id_Tipo_Inmueble { get; set; }
    // [Required(ErrorMessage = "Los servicios son obligatorios.")]
    public string? Servicios { get; set; }
    // [Required(ErrorMessage = "La cantidad de baños es obligatoria.")]
    public int Bano { get; set; }
    // [Required(ErrorMessage = "La cochera es obligatoria.")]
    public int Cochera { get; set; }
    // [Required(ErrorMessage = "El patio es obligatorio.")]
    public int Patio { get; set; }
    public double Precio { get; set; }
    // [Required(ErrorMessage = "La condición es obligatoria.")]
    public string? Condicion { get; set; }
    public string? foto { get; set; }
    public int Estado_Inmueble { get; set; }


}
