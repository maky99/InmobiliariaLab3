using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaLab3.Models;
public class Pago
{
    [Key]
    public int Id_Pago { get; set; }
    public int Id_Contrato { get; set; }
    public double Importe { get; set; }
    public int CuotaPaga { get; set; }
    public int MesesPagos { get; set; }
    public DateTime Fecha { get; set; }
    public double Multa { get; set; }
    public int Id_Creado_Por { get; set; }
    public int Id_Terminado_Por { get; set; }
    public int Estado_Pago { get; set; }
    [ForeignKey("Id_Inquilino")]
    public Inquilino? inquilino { get; set; }
    [ForeignKey("Id_Contrato")]
    public Contrato? contrato { get; set; }

}