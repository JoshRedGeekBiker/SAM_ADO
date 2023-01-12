using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class Parada
{
    public int paradaId { get; set; }
    public String nombre { get; set; }
    public Boolean activo { get; set; }
    public String clave { get; set; }
    public DateTime fechaCreacion { get; set; }

    public IList<Coordenadas> coordenadas { get; set; }
}

