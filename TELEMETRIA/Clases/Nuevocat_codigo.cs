using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfazSistema;

/// <summary>
/// Se crea ésta clase para poder serializar el catalogo que descargamos
/// en formato JSON cuando se Sincroniza catalogos
/// </summary>

public class Nuevocat_codigo
{
    public List<cat_codigo> CatalogoCodigos { get; set; }
}
