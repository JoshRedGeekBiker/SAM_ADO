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
    
    public partial class spot_poi
    {
        public long spotListaId { get; set; }
        public string nombre { get; set; }
        public long orientacionInicial { get; set; }
        public Nullable<long> orientacionFinal { get; set; }
        public bool activo { get; set; }
        public System.DateTime fechaCreacion { get; set; }
        public System.DateTime fechaVigenciaInicio { get; set; }
        public System.DateTime fechaVigenciaFin { get; set; }
    }
}