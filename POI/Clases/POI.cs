using InterfazSistema.ModelosBD;

public class POI : IBDContext, IBDContextPOI, IGPS, ISistema
{
    #region "Propiedades"
    //Contexto de la base de datos
    public vmdEntities VMD_BD => throw new System.NotImplementedException();
    public poiEntities POI_BD => throw new System.NotImplementedException();
    public GPSData Datos_GPS { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public int OrdenDescarga { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public int OrdenLoad { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public Sistema Sistema => throw new System.NotImplementedException();

    public string GetVersionSistema => throw new System.NotImplementedException();

    public bool ModoPrueba { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public bool ModoNocturno { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    
    #endregion

    #region "Variables"

    #endregion

    #region "Eventos"

    #endregion

    #region "Constructores"
    /// <summary>
    /// Constructor principal
    /// </summary>
    public POI()
    {

    }


    #endregion

    #region "Métodos privados"

    #endregion

    #region "Métodos publicos"

    #endregion

    #region "Métodos heredados"
    public void Inicializar()
    {
        throw new System.NotImplementedException();
    }

    public void Finalizar()
    {
        throw new System.NotImplementedException();
    }

    public bool Sincronizar()
    {
        throw new System.NotImplementedException();
    }

    public void Actualizar()
    {
        throw new System.NotImplementedException();
    }
    #endregion

    #region "Timers"

    #endregion
}