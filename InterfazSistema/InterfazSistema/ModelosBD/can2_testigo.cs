//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InterfazSistema.ModelosBD
{
    using System;
    using System.Collections.Generic;
    
    public partial class can2_testigo
    {
        public int id_testigo { get; set; }
        public string numeroEconomico { get; set; }
        public int geocercaId { get; set; }
        public int parametroid { get; set; }
        public double ValorParametro { get; set; }
        public System.DateTime fechaEvento { get; set; }
        public double latitud { get; set; }
        public double longitud { get; set; }
        public bool enviado { get; set; }
    }
}
