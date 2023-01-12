using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Response_spot
{
    public int spotId { get; set; }
    public int orientacionInicial { get; set; }
    public int orientacionFinal { get; set; }
    public String programacionHoraria { get; set; }
    public int duracion { get; set; }
    public int tiempoEspera { get; set; }
    public Boolean activo { get; set; }
    public DateTime fechaCreacion { get; set; }
    public Parada parada { get; set; }

    public Archivo archivo { get; set; }
}

