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
    
    public partial class spotlista_poi
    {
        public long spot_id { get; set; }
        public long spotListaId { get; set; }
        public long secuencia { get; set; }
        public long spotArchivoId { get; set; }
        public string nombre { get; set; }
        public string url { get; set; }
        public bool activo { get; set; }
        public System.DateTime fechaVigenciaInicio { get; set; }
        public System.DateTime fechaVigenciaFin { get; set; }
        public long spotArchivoTipoId { get; set; }
        public string descripcion { get; set; }
        public bool activo_t { get; set; }
    }
}
