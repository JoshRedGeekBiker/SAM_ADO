using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfazSistema;
using System.IO;

/// <summary>
/// Powered ByRED 23MAR2021
/// </summary>
public class PuntosInteres: IBBContextSIA, IBDContext
{
    #region "Propiedades"

    #endregion

    #region Variables
    #endregion

    #region "Propiedades Heredadas"

    public SIAEntities SIA_BD { get; }
    public vmdEntities VMD_BD { get; }//Powered ByRED 13ABR2021
    #endregion

    #region "Variables"
    private smstouch _puntoInteres;
    private can_parametrosinicio ParametrosInicio;//Powered ByRED 13ABR2021
    #endregion

    #region "Variables de Eventos"

    /// <summary>
    /// Se encarga de mandar el POI hacia el front
    /// Powered ByRED 23MAR2021
    /// </summary>
    /// <param name="Multimedia"></param>
    public delegate bool MandarPOI(List<string> Multimedia);
    public event MandarPOI POI;

    /// <summary>
    /// Se encarga de avisar a MINISIA el IDPUNTO
    /// CLAUS && ROJO
    /// </summary>
    /// <param name="id_punto"></param>
    public delegate void AvisarMiniSIA(int id_punto);
    public event AvisarMiniSIA MiniSIA;
    #endregion

    #region "Constructores"
    /// <summary>
    /// Constructor Principal
    /// </summary>
    public PuntosInteres()
    {
        SIA_BD = new SIAEntities();
        VMD_BD = new vmdEntities();

        ParametrosInicio = (from x in VMD_BD.can_parametrosinicio
                            select x).FirstOrDefault();
        
    }
    #endregion

    #region "Métodos Públicos"

    /// <summary>
    /// Se encarga de Procesar los POI´s
    /// Powered ByRED 23MAR2021
    /// </summary>
    public void ProcesarPOI()
    {
        try
        {
            if ((bool)ParametrosInicio.VMD && (bool)ParametrosInicio.HabilitarPI)//Powered ByRED 13ABR2021
            {
                VerificarPuntosInteres();

                MostrarPuntosInteres();
            }
        }
        catch
        {

        }
    }

    #endregion

    #region  "Métodos Privados"
    /// <summary>
    /// Se encarga de consultar si hay nuevos puntos de interés por mostrar
    /// Powered ByRED 23MAR2021
    /// </summary>
    private void VerificarPuntosInteres()
    {

        if (_puntoInteres == null)
        {
            _puntoInteres = (from x in SIA_BD.smstouch
                             where x.IdDestinatario == 1 && x.IdEstatusAtendido == 0 && x.IdPunto != 0
                             orderby x.IdSmsTouch ascending
                             select x).FirstOrDefault();
        }
    }

    /// <summary>
    /// Se encarga de comunicar el punto de interes
    /// Powered ByRED 23MAR2021
    /// </summary>
    private void MostrarPuntosInteres()
    {
        try
        {
            if (_puntoInteres != null)
            {
                if (_puntoInteres.IdEstatusAtendido != 1)
                {
                    var multimedia = RecuperarMultimedia(Convert.ToInt32(_puntoInteres.IdPunto));

                    if(multimedia.Count == 3)
                    {
                        if (POI(multimedia))
                        {
                            PoiAtendido(Convert.ToInt32(_puntoInteres.IdSmsTouch));

                            //Si tenemos miniSIA
                            //CLAUS && ROJO
                            MiniSIA(Convert.ToInt32(_puntoInteres.IdPunto));
                        }
                    }
                    else
                    {//Tenemos que determinar que haremos en éste caso
                        PoiAtendido(Convert.ToInt32(_puntoInteres.IdSmsTouch));
                    }
                }
                else
                {//Powered ByRED 20ABR2021
                    PoiAtendido(Convert.ToInt32(_puntoInteres.IdSmsTouch));
                }
            }
        }
        catch(Exception ex)
        {
            var error = ex.ToString();
        }
    }

    /// <summary>
    /// Se encarga de recuperar la multimedia según el POI
    /// Powered ByRED 23MAR2021
    /// </summary>
    /// <returns></returns>
    private List<String> RecuperarMultimedia(int _idpunto)
    {
        List<string> Multimedia = new List<string>();
        try
        {
            var poi = (from x in SIA_BD.puntosinteres
                       where x.IdPunto == _idpunto
                       select x).FirstOrDefault();

            if (poi != null)
            {
                var audio = AppDomain.CurrentDomain.BaseDirectory + @"PuntoInteres\" + poi.Imagen + ".mp3";
                var imagen = AppDomain.CurrentDomain.BaseDirectory + @"PuntoInteres\" + poi.Imagen + "Pasajeros.gif";
                var tiempo = poi.tiempo_exposicion.ToString();


                //Validamos las existencias
                if (File.Exists(audio)) { Multimedia.Add(audio); }

                if (File.Exists(imagen)) { Multimedia.Add(imagen); }

                Multimedia.Add(tiempo);

            }
        }
        catch
        {

        }
        return Multimedia;
    }

    /// <summary>
    /// Se encargará de cerrar el registro en BD
    /// Powered ByRED 23MAR2021
    /// </summary>
    /// <param name="id"></param>
    private void PoiAtendido(int id)
    {
        try
        {
            var poixcerrar = (from x in SIA_BD.smstouch
                              where x.IdSmsTouch == id
                              select x).ToList();

            foreach (smstouch registro in poixcerrar)
            {
                registro.IdEstatusAtendido = 1;
            }

            SIA_BD.SaveChanges();

            _puntoInteres = null;
        }
        catch
        {

        }
        
    }
    #endregion

    #region "Eventos"

    #endregion

    #region "Timers"

    #endregion

}
