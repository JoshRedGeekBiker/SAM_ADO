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
    
    public partial class sia_scripts
    {
        public long IDScript { get; set; }
        public string Script { get; set; }
        public bool Ejecutado { get; set; }
        public System.DateTime FechaEjecucion { get; set; }
        public string Nombre { get; set; }
        public System.DateTime FechaDescarga { get; set; }
        public string BD { get; set; }
    }
}
