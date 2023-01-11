using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfazSistema.ModelosBD;

/// <summary>
/// Powered ByRED 23FEB2021
/// </summary>
public class Mensajes: IBDContextSIA, IBDContext
{

    #region "Propiedades"

    #endregion

    #region Variables
    private List<catsms> CatMensajesPred;
    #endregion

    #region "Propiedades Heredadas"

    public SIAEntities SIA_BD { get; }
    public vmdEntities VMD_BD { get; }//Powered ByRED 13ABR2021
    #endregion

    #region "Variables"
    private smstouch MensajeConductor;
    private List<smstouch> Cintillos;
    private can_parametrosinicio ParametrosInicio;//Powered ByRED 13ABR2021
    private bool Flip_Flop = true; //Powered ByRED 20MAY2021

    private bool MostrarCintillo = false;//Powered ByRED 13ABR2021
    private bool PrimerInicioCintillo = true;//Powered ByRED 13ABR2021

    //Timers
    private System.Windows.Forms.Timer timerCintillo = new System.Windows.Forms.Timer();//Powered ByRED 13ABR2021
    #endregion

    #region "Variables de Eventos"
    /// <summary>
    /// Se encarga de enviarl el mensaje para ser mostrado en el front ENGINE
    /// </summary>
    /// <param name="id"></param>
    /// <param name="tipo"></param>
    /// <param name="texto"></param>
    public delegate bool EnviarMensajeSIA(int tipo, string texto);
    public event EnviarMensajeSIA MandarMensaje;
    #endregion

    #region "Constructores"
    /// <summary>
    /// Constructor Principal
    /// </summary>
    public Mensajes()
    {
        SIA_BD = new SIAEntities();
        VMD_BD = new vmdEntities();//Powered ByRED 13ABR2021
        ParametrosInicio = (from x in VMD_BD.can_parametrosinicio
                            select x).FirstOrDefault();

        //Obtenemos los mensajes predefinidos
        //Powered ByRED 18MAR2021
        CatMensajesPred = (from x in SIA_BD.catsms
                           select x).ToList();

        //RAB && ROJO
        EliminarRegistros();

        if ((bool)ParametrosInicio.HabilitarCintillo && (bool)ParametrosInicio.VMD)
        {
            PreparaTimers();//Powered ByRED 13ABR2021
        }
        

    }

    #endregion

    #region "Métodos Públicos"
    public void ProcesarMensajes()
    {
        try
        {
            //Verificamos los registros
            VerificarRegistros();

            if ((bool)ParametrosInicio.MensajesConductor)
            {//Enviamos sms a conductor
                MostrarSMSConductor();
            }

            //Si tenemos que mostrar cintillo lo mostramos
            if (this.MostrarCintillo)
            {
                this.MostrarCintillo = false;
                MostrarMensajeAbordo();
            }
            
        }
        catch(Exception ex)
        {
            var holaerror = ex.ToString();
        }
        
    }


    /// <summary>
    /// Se encarga de recuperar los mensajes predefinidos para ser mostrados en front
    /// Powered ByRED 18MAR2021
    /// </summary>
    /// <returns></returns>
    public List<string> ObtenerMensajesPred()
    {
        var listaRetorno = new List<string>();

        try
        {
            foreach(catsms sms in CatMensajesPred)
            {
                listaRetorno.Add(sms.TextoSMS);
            }
        }
        catch
        {

        }

        return listaRetorno;
    }

    /// <summary>
    /// se encarga de recuperar los mensajes recibidos
    /// Powered ByRED 18MAR2021
    /// </summary>
    /// <returns></returns>
    public List<string> ObtenerMensajesRecibidos()
    {
        var listaRetorno = new List<string>();

        try
        {
            var received = (from x in SIA_BD.smstouch
                            where x.IdDestinatario == 2
                            select x).ToList();

            foreach(smstouch sms in received)
            {
                listaRetorno.Add(sms.FechaSMS.ToString("dd/MM/yyyy") + " | " + sms.TextoSMS);
            }
        }
        catch
        {

        }
        return listaRetorno;
    }

    /// <summary>
    /// se encarga de recuperar los mensajes enviados
    /// Powered ByRED 18MAR2021
    /// </summary>
    /// <returns></returns>
    public List<string> ObtenerMensajesEnviados()
    {
        var listaRetorno = new List<string>();
        try
        {
            var received = (from x in SIA_BD.smstouch
                            where x.IdDestinatario == 3
                            select x).ToList();

            foreach (smstouch sms in received)
            {
                if(sms.IdSms != null)//Powered ByRED 08JUN2021
                {
                    listaRetorno.Add(sms.FechaSMS.ToString() + " | " + (from x in CatMensajesPred
                                                                        where x.IdSMS == sms.IdSms
                                                                        select x.TextoSMS).FirstOrDefault());
                }
                
            }
        }
        catch
        {

        }

        return listaRetorno;
    }

    /// <summary>
    /// se encarga de poner el mensaje que se desea enviar en BD
    /// para que satelite de SIA lo envié en cuanto tenga disponibilad
    /// de interner
    /// Powered ByRED 18MAR2021
    /// </summary>
    /// <param name="msj"></param>
    /// <returns></returns>
    public bool EnviarMensajeaBD(string _msj)
    {
        try
        {
            var sms = (from x in CatMensajesPred
                       where x.TextoSMS.Equals(_msj)
                       select x).FirstOrDefault();

            if (sms != null)
            {
                smstouch nuevoSMS = new smstouch();
                nuevoSMS.IdSmsTouch = ultimoIDSMSTouch();
                nuevoSMS.IdSms = sms.IdSMS;
                nuevoSMS.IdMarca = null;
                nuevoSMS.FechaSMS = DateTime.Now;
                nuevoSMS.IdEstatusAtendido = 0;
                nuevoSMS.IdDestinatario = 3;
                nuevoSMS.TextoSMS = null;
                nuevoSMS.clvConductor = null;
                nuevoSMS.NombreConductor = null;
                nuevoSMS.IdPunto = null;

                SIA_BD.smstouch.Add(nuevoSMS);

                SIA_BD.SaveChanges();

                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }


    /// <summary>
    /// Se encarga de mostrar el cintillo de bienvenida
    /// </summary>
    public void MostrarCintilliInicial()
    {
        if ((bool)ParametrosInicio.HabilitarCintillo)
        {
            if (this.PrimerInicioCintillo)
            {
                this.PrimerInicioCintillo = false;

                //Mandamos el mensaje Inicial
                MandarMensaje(1, ParametrosInicio.MensajeInicial);
            }
        }
    }

    /// <summary>
    /// Se encarga de generar un registro de Alerta de Robo
    /// Powered ByRED 08JUN2021
    /// </summary>
    public bool GenerarAlertaRobo()
    {

        bool respuesta = false;
        try
        {
            smstouch nuevosms = new smstouch();

            nuevosms.IdSmsTouch = ultimoIDSMSTouch();
            nuevosms.IdSms = null;
            nuevosms.IdMarca = null;
            nuevosms.FechaSMS = DateTime.Now;
            nuevosms.IdTipoSms = null;
            nuevosms.IdEstatusAtendido = 0;
            nuevosms.IdDestinatario = 3;
            nuevosms.TextoSMS = "Robo";
            nuevosms.clvConductor = null;
            nuevosms.NombreConductor = null;

            SIA_BD.smstouch.Add(nuevosms);

            respuesta = true;
        }
        catch
        {

        }

        return respuesta;
    }
    #endregion

    #region  "Métodos Privados"

    /// <summary>
    /// Se encarga de recuperar los mensajes pendientes por procesar
    /// Powered ByRED 23FEB2021
    /// Powered byRED 13ABR2021 -> Se encarga de validar los registros validos
    /// se quitaron la recuperación de los registros de aquí
    /// //RAB && ROJO
    /// </summary>
    private void VerificarRegistros()
    {
        var fecha = DateTime.Now.ToString("yyyy-MM-dd 00:00:00");

        //Eliminar registros sin texto por aparecer y con fecha diferente al sistema
        var registrosinvalidos = (from x in SIA_BD.smstouch
                                  where (x.TextoSMS.Equals("") || x.TextoSMS == null) && x.IdPunto == 0 && x.IdSms == null
                                  select x).ToList();

        foreach (smstouch registroinvalido in registrosinvalidos)
        {
            var fechaBD = Convert.ToDateTime(registroinvalido.FechaSMS.ToString());

            if (fechaBD != DateTime.Today)
            {
                registroinvalido.IdEstatusAtendido = 1;
            }
            else
            {
                registroinvalido.IdEstatusAtendido = 0;
            }
        }

        SIA_BD.SaveChanges();

        //Recuperamos Mensajes para pasajero
        //var MensajesAbordoTemp = (from x in SIA_BD.smstouch

        //                          select x).ToList();

        //Validamos si son mensajes nuevos, para agregarlos a memoria.

        //foreach (smstouch nuevoMensaje in MensajesAbordoTemp)
        //{
        //    if (!EvitarCodigoReplicado(nuevoMensaje, MensajesAbordo))
        //    {
        //        MensajesAbordo.Add(nuevoMensaje);
        //    }
        //}

        //(from x in VMD_BD.can_parametrosinicio
        //select x).FirstOrDefault();

        // (from x in TELEMATICS_BD1.codigo
        //  where x.Procesado == 0
        //orderby x.FechaHora_Inicio ascending
        //select x).Take(250).ToList();


        //codigo falla = (from x in Fallas_Mem
        //              where x.Modulo == code.Modulo && x.Codigo1 == code.Codigo1
        //            select x).FirstOrDefault();
        //return falla == null ? false : true;
    }

    /// <summary>
    /// Se encarga de mostrarle al conductor los mensajes recibidos a través de la central de SIA
    /// </summary>
    private void MostrarSMSConductor()
    {
        try
        {
            //Recuperamos el mensaje según el orden de llegada sin atender para el conductor
            if (MensajeConductor == null)
            {
                MensajeConductor = (from x in SIA_BD.smstouch
                                    where x.IdDestinatario == 2 && x.IdEstatusAtendido == 0
                                    //&& x.FechaSMS == DateTime.Now
                                    orderby x.IdSmsTouch ascending
                                    select x).FirstOrDefault();
            }

            if (MensajeConductor != null)
            {
                if (MensajeConductor.IdEstatusAtendido != 1)
                {
                    if (MensajeConductor.IdSms != null)
                    {
                        //Acompletamos el mensaje
                        MensajeConductor.TextoSMS = (from x in CatMensajesPred
                                                     where x.IdSMS == MensajeConductor.IdSms
                                                     select x.TextoSMS).FirstOrDefault();
                    }

                    //Ejecutamos el evento para enviarle el registro
                    if(MandarMensaje(Convert.ToInt32(MensajeConductor.IdDestinatario), MensajeConductor.FechaSMS.ToString("dd/MM/yyyy") + "|" + MensajeConductor.TextoSMS))
                    {
                        SMSAtendido(Convert.ToInt32(MensajeConductor.IdSmsTouch));
                        MensajeConductor = null;
                    }
                    else
                    {

                    }   
                }
            }
        }
        catch
        {

        }
    }

    /// <summary>
    /// Se encarga de detonar el ecvento
    /// </summary>
    private void MostrarMensajeAbordo()
    {
        try
        {
            if (!this.PrimerInicioCintillo)
            {
                //Recuperamos el mensaje según el órden de llegada sin atender para el pasajero(cintillo)
                if (Cintillos == null)
                {
                    Cintillos = (from x in SIA_BD.smstouch
                                where x.IdDestinatario == 1 && x.IdEstatusAtendido == 0 && x.IdPunto == 0
                                //&& x.FechaSMS == DateTime.Now
                                orderby x.IdSmsTouch ascending
                                select x).ToList();

                    if (Cintillos.Count == 0)
                    {
                        Cintillos = null;
                    }
                }

                if (Cintillos != null)
                {

                    //Powered ByRED 25MAY2021
                    //Para poder determinar si mostramos en la siguiente secuencia el 1er cintillo
                    //o el 2do cintillo

                    smstouch cintilloxmostrar;
                    if (Flip_Flop)
                    {
                        Flip_Flop = false;
                        cintilloxmostrar = Cintillos.ElementAtOrDefault(0);//1er cintillo
                    }
                    else
                    {
                        Flip_Flop = true;
                        cintilloxmostrar = Cintillos.ElementAtOrDefault(1);//segundo cintillo (si hubiera)
                    }

                    if (cintilloxmostrar.IdEstatusAtendido != 1)
                    {
                        //Ejecutamos el evento que detona el envio de mensaje de pasajero
                        if (MandarMensaje(Convert.ToInt32(cintilloxmostrar.IdDestinatario), cintilloxmostrar.TextoSMS))
                        {
                            CintilloAtendido(Convert.ToInt32(cintilloxmostrar.IdSmsTouch), cintilloxmostrar.TextoSMS);
                            Cintillos = null;
                        }
                        else
                        {

                        }
                        //MensajeAbordo.IdEstatusAtendido = 1;
                    }
                }
            }
        }
        catch(Exception ex)
        {
            var error = ex.ToString();
        }
    }

    /// <summary>
    /// Se encargará de cerrar el registro en BD de los mensajes
    /// hacia el conductor (SMS)
    /// </summary>
    /// <param name="id"></param>
    private void SMSAtendido(int id)
    {
        try
        {
            var mensajesxcerrar = (from x in SIA_BD.smstouch
                                   where x.IdSmsTouch == id
                                   select x).ToList();

            foreach (smstouch registro in mensajesxcerrar)
            {
                registro.IdEstatusAtendido = 1;
            }

            SIA_BD.SaveChanges();
        }
        catch
        {

        }
    }

    /// <summary>
    /// Se encarga de cerrar el registro en BD del cintillo
    /// y de poner el registro en la historial de la publicidad
    /// Powered ByRED 13ABR2021
    /// </summary>
    /// <param name="id"></param>
    private void CintilloAtendido(int _id, string _textoSMS)
    {
        try
        {
            var mensajesxcerrar = (from x in SIA_BD.smstouch
                                   where x.IdSmsTouch == _id
                                   select x).ToList();

            foreach (smstouch registro in mensajesxcerrar)
            {
                registro.IdEstatusAtendido = 1;
            }

            SIA_BD.SaveChanges();
        }
        catch
        {

        }
        try 
        {
            var ultimoId = (from x in SIA_BD.historialpublicidad
                            orderby x.IdHistorial descending
                            select x.IdHistorial).FirstOrDefault();

            ultimoId++;

            historialpublicidad nuevoregistro = new historialpublicidad();

            nuevoregistro.IdHistorial = ultimoId;
            nuevoregistro.TextoSMS = _textoSMS;
            nuevoregistro.Fecha = DateTime.Now;

            SIA_BD.historialpublicidad.Add(nuevoregistro);

            SIA_BD.SaveChanges();
        }
        catch
        {

        }
    }

    /// <summary>
    /// Recupera el ultimo id de la BD para poder generar un nuevo registro
    /// Powered ByRED 18MAR2021
    /// </summary>
    /// <returns></returns>
    private int ultimoIDSMSTouch()
    {
        int retorno = 0;
        try
        {
            retorno = Convert.ToInt32((from x in SIA_BD.smstouch
                                       orderby x.IdSmsTouch descending
                                       select x.IdSmsTouch).FirstOrDefault());
            retorno++;
        }
        catch(Exception ex)
        {
            var error = ex.ToString();
            retorno = 0;
        }
        return retorno;

    }

    /// <summary>
    /// Se encarga de eliminar los registros cuando se inicialice el sistema
    /// RAB && ROJO 07MAR2021
    /// </summary>
    private void EliminarRegistros()
    {
        try
        {
            var objCtx = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)SIA_BD).ObjectContext;

            //Borramos la tabla de Orden de descarga
            objCtx.ExecuteStoreCommand("Truncate Table smstouch");

            //Borramos la tabla historialpublicidad
            objCtx.ExecuteStoreCommand("Truncate Table historialpublicidad");
        }
        catch
        {

        }
        
    }

    /// <summary>
    /// Se encarga de configurar los timers
    /// Powered ByRED 13ABR2021
    /// </summary>
    private void PreparaTimers()
    {
        try
        {
            var tiempo = (from x in SIA_BD.parametros
                          select x.Tiempo).FirstOrDefault();

            if (tiempo > 0)
            {
                timerCintillo.Interval = Convert.ToInt32(tiempo * 1000);
            }
        }
        catch
        {
            timerCintillo.Interval = 1800000;
        }

        timerCintillo.Enabled = true;
        timerCintillo.Tick += new EventHandler(timerCintillo_Tick);
    }
    #endregion

    #region "Eventos"

    #endregion

    #region "Timers"
    /// <summary>
    /// Powered ByRED 13ABR2021
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void timerCintillo_Tick(object sender, EventArgs e)
    {
        this.MostrarCintillo = true;
    }
    #endregion
}
