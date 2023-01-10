using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfazSistema;

public class Secuencia : IBDContext
{
    public vmdEntities VMD_BD { get; }


    private ProtocoloCAN IProtocoloCAN;
    private Globales IGlobales;
    private can_parametrosinicio IParametrosInicio;

    public delegate void Conductor(string clvConductor);
    public event Conductor ValidaConductor;
   
    public Secuencia(ref Globales _Globales, ref ProtocoloCAN _ProtocoloCAN, can_parametrosinicio _Parametrosinicio)
    {
        VMD_BD = new vmdEntities();
        IProtocoloCAN = _ProtocoloCAN;

        IParametrosInicio = _Parametrosinicio;

        _Globales.Corrida.Region = _Parametrosinicio.Region.ToString();
        _Globales.Corrida.Marca = _Parametrosinicio.Marca.ToString();
        _Globales.Corrida.Zona = _Parametrosinicio.Zona;
        _Globales.Corrida.Servicio = Int32.Parse(_Parametrosinicio.Servicio);
        _Globales.Corrida.Autobus = _Parametrosinicio.Autobus;

        IGlobales = _Globales;
    }

    public Secuencia()
    {
        VMD_BD = new vmdEntities();
    }


    /// <summary>
    /// Secuencias
    /// </summary>
    public void AsignaSecuenciaCAN()
   {
        try
        {
            DateTime CualFechaHora;

            var CANSecuencia = (from x in VMD_BD.can_secuencia
                                where x.autobus == IParametrosInicio.Autobus
                                orderby x.idsecuencia
                                select x).FirstOrDefault();

            if (CANSecuencia != null)
            {
                if (CANSecuencia.confirmado == true)
                {
                    //*PantallaCan = "SecuenciaFinalizada"
                    //*MuestraModo
                    IGlobales.Corrida.SecuenciaAbierta = false;
                }
                else //Obtengo los datos para la venta de Origen y Destino Final, de la corrida
                {
                    IGlobales.Corrida.IDSecuencia = CANSecuencia.idsecuencia;
                    IGlobales.Corrida.Region = CANSecuencia.idregion;
                    IGlobales.Corrida.Marca = CANSecuencia.idmarca;
                    IGlobales.Corrida.Zona = CANSecuencia.idzona;
                    IGlobales.Corrida.Servicio = CANSecuencia.servicio;
                    IGlobales.Corrida.Autobus = CANSecuencia.autobus;
                    IGlobales.Corrida.VersionSecuencia = CANSecuencia.version;
                    IGlobales.ClearViajes();

                    var CANDetSecuencia = (from x in VMD_BD.can_detsecuencia
                                           join poborigen in VMD_BD.can_poblaciones on x.origen equals poborigen.idpob
                                           join pobdestino in VMD_BD.can_poblaciones on x.destino equals pobdestino.idpob
                                           join pobvia in VMD_BD.can_poblaciones on (long)x.via equals pobvia.idpob
                                           where x.idsecuencia == IGlobales.Corrida.IDSecuencia
                                           select new { x.fecha, x.numerotramo, x.origen, x.destino, x.via, x.oper1, x.oper2, descripcionor = poborigen.despob, descripciondes = pobdestino.despob, descripcionvia = pobvia.despob, x.orden, x.confirmado }).ToList();

                    foreach (var detsec in CANDetSecuencia)
                    {

                        IGlobales.AddViaje();
                        var viajeNuevo = new clsCorridaDet();

                        viajeNuevo.FechaHora = detsec.fecha;
                        viajeNuevo.NumTramo = detsec.numerotramo;
                        viajeNuevo.OrigenID = detsec.origen;
                        viajeNuevo.DestinoID = detsec.destino;
                        viajeNuevo.ViaID = (int)detsec.via;
                        viajeNuevo.Conductor1 = detsec.oper1;
                        viajeNuevo.Conductor2 = (int)detsec.oper2;
                        viajeNuevo.OrigenDes = detsec.descripcionor;
                        viajeNuevo.DestinoDes = detsec.descripciondes;
                        viajeNuevo.ViaDes = detsec.descripcionvia;
                        viajeNuevo.IDDetSecuencia = detsec.orden;
                        viajeNuevo.Confirmado = (bool)detsec.confirmado;

                        if (!viajeNuevo.Confirmado == false && IGlobales.Corrida.ViajeActual == 0)
                        {
                            IGlobales.Corrida.ViajeActual = viajeNuevo.IDDetSecuencia;
                        }
                        IGlobales.Viajes.Add(viajeNuevo);

                    }

                    CANDetSecuencia.Clear();

                    if (IGlobales.Corrida.NumViajes > 0)
                    {// Checo si ya había un viaje abierto
                        var regSecuencia = (from x in VMD_BD.can_regsecuencia
                                            join operadores in VMD_BD.can_operadores on x.conductor equals operadores.cveemp
                                            select new { x.idsecuencia, x.orden, x.conductor, operadores.nombre }).ToList();

                        foreach (var regsec in regSecuencia)
                        {
                            if (regsec.idsecuencia > 0 && regsec.orden <= IGlobales.Corrida.NumViajes)
                            {
                                IGlobales.Corrida.IDSecuencia = regsec.idsecuencia;
                                IGlobales.Corrida.ViajeActual = regsec.orden;
                                IGlobales.Corrida.SecuenciaAbierta = true;
                                IGlobales.ViajeAbierto = true;
                                IGlobales.Corrida.ConductorActualID = (int)regsec.conductor;
                                IGlobales.Corrida.ConductorActualNom = regsec.nombre;
                                //Pantalla CAN = Viaje abierto: conductor:
                                //MuestraModo

                            }

                        }

                        if (IGlobales.Corrida.ViajeActual > 0)
                        {
                            //Pantalla CAN VIAJE SELECCIONADO
                            //muestraModo
                            IGlobales.Corrida.SecuenciaAbierta = true;
                        }
                        else
                        {
                            //Pantalla CAN no hay viaje abierto
                            //muestramodo
                            IGlobales.Corrida.SecuenciaAbierta = false;
                        }

                    }
                    else
                    {
                        //PantallaCan = "CAN: no hay viajes en la secuencia";
                        //muestraModo()
                        IGlobales.Corrida.SecuenciaAbierta = false;
                    }
                }
            }
            else //No hay secuencia asignada, chexo si no existia un viaje abierto sin validación de conductor
            {
                var movTosCAN = (from x in VMD_BD.can_movtoscan
                                 where (x.accion == "VA" || x.accion == "VC" || x.accion == "CM") && x.autobus == IParametrosInicio.Autobus
                                 orderby x.NumVuelta descending, x.NumReg descending, x.fechahora descending
                                 select new { x.fechaviaje, x.fechahora, x.operador, x.accion, x.TotalDistance, x.LiterPerHour, x.NumVuelta }).FirstOrDefault();

                IGlobales.DatosCANIniciados = true;

                if (movTosCAN != null)
                {
                    IProtocoloCAN.KmsIniDg = movTosCAN.TotalDistance; //Sólo en el cáso de DG serviría éste campo y está variable
                    IProtocoloCAN.LtsIniDG = movTosCAN.LiterPerHour; //Sólo en el cáso de DG serviría éste campo y está variable

                    //13/05/2014
                    if ((!movTosCAN.accion.Equals("VC") && IProtocoloCAN.KmsIniDg == 0) || (!movTosCAN.accion.Equals("VC") && IProtocoloCAN.LtsIniDG == 0))
                    {
                        //Tiene que ser por evento
                        IProtocoloCAN.CargaKmsLts("VA", movTosCAN.fechaviaje);
                    }
                    else
                    {
                        //Tiene que ser por evento
                        IProtocoloCAN.CargaKmsLts("T", movTosCAN.fechaviaje);
                    }

                    if (movTosCAN.accion.Equals("VA"))
                    {
                        if ((movTosCAN.fechahora - DateTime.Now).Hours <= 3000)
                        {
                            IGlobales.EsperaIniciaViaje = true;
                            IGlobales.EsperaCambioManos = false;
                            IGlobales.EsperaFinViaje = false;

                            IGlobales.CargandoViajeAnterior = true;
                            IGlobales.ViajeAbierto = false;
                            //ValidaComandoCAN(movTosCAN.operador);*********************
                            ValidaConductor(movTosCAN.operador.ToString());
                            IGlobales.CargandoViajeAnterior = false;
                        }
                        else
                        {
                            //PantallaCAN NO HAY SECUENCIA ASIGNADA
                            //MuestraModo();
                            IGlobales.Corrida.SecuenciaAbierta = false;
                        }

                    }
                    else if (movTosCAN.accion.Equals("CM"))
                    {//Si es cambio de manos, localizo el siguiente registro, por que ahi esta la clave del nuevo conductor
                        if ((movTosCAN.fechahora - DateTime.Now).TotalHours <= 3000)
                        {
                            CualFechaHora = DateTime.Now;

                            var movtos_operador = (from x in VMD_BD.can_movtoscan
                                                   where x.fechahora <= CualFechaHora
                                                   orderby x.fechahora
                                                   select x).FirstOrDefault();

                            if (movtos_operador != null)
                            {
                                IGlobales.EsperaIniciaViaje = true;
                                IGlobales.EsperaCambioManos = false;
                                IGlobales.EsperaFinViaje = false;
                                IGlobales.CargandoViajeAnterior = true;
                                IGlobales.ViajeAbierto = false;
                                //ValidaComandoCAN(operador.operador);****************************
                                ValidaConductor(movTosCAN.operador.ToString());
                                IGlobales.CargandoViajeAnterior = false;
                            }
                            else
                            {
                                //Pantalla CAN NO HAY SECUENCIA ASIGNADA
                                //MuestraModo();
                                IGlobales.Corrida.SecuenciaAbierta = false;
                            }
                        }
                        else
                        {
                            //PantallaCAN No hay secuencia asignada
                            //MuestraModo();
                            IGlobales.Corrida.SecuenciaAbierta = false;
                        }

                    }
                    else
                    {
                        //PantallaCAN No hay secuencia asignada
                        //MuestraModo();
                        IGlobales.Corrida.SecuenciaAbierta = false;
                    }
                }
                else //en caso de que no exista un resitro con alguna acción recuperamos el último valor almacenado en la tabla can_movtos
                {
                    var movtoscan2 = (from x in VMD_BD.can_movtoscan
                                      where x.accion == "T" && x.autobus == IParametrosInicio.Autobus
                                      orderby x.NumVuelta descending, x.NumReg ascending, x.fechahora ascending
                                      select new { x.fechahora, x.operador, x.accion, x.TotalDistance, x.LiterPerHour }).FirstOrDefault();

                    if (movtoscan2 != null)
                    {
                        IProtocoloCAN.KmsIniDg = movtoscan2.TotalDistance; //Sólo en el cáso de DG serviría este campo y ésta variable
                        IProtocoloCAN.LtsIniDG = movtoscan2.LiterPerHour; //Sólo en el cáso de DG serviría este campo y ésta variable

                        //13/05/2014
                        if (IProtocoloCAN.KmsIniDg == 0 || IProtocoloCAN.LtsIniDG == 0)
                        {
                            IProtocoloCAN.CargaKmsLts("VA", movtoscan2.fechahora);
                        }
                    }
                    else
                    {
                        //Si no hay registros en CAN_movtosCAN se dejan como iniciales los valores enviados por el ADORelay(ADOCAN)
                        IProtocoloCAN.KmsIniDg = IProtocoloCAN.TripDistanceEstimationVS;
                        IProtocoloCAN.LtsIniDG = IProtocoloCAN.FuelEconomyEstimationFE;

                    }

                    //PantallaCAN NO HAY SECUENCIA
                    //MuestraModo();
                    IGlobales.Corrida.SecuenciaAbierta = false;

                }
            }

            MuestraSecuencia();
        }
        catch
        {
            //MuestraError("AsignaSecuencia");
        }
    }


    //Por tiempos se detiene el proceso de ésta lógica
    public bool CargaSecuencia()
    {
        try
        {

            return false;

        }
        catch
        {
            return false;
        }
    }


    //Por tiempos se detiene el proceso de ésta lógica
    private bool ChecaSecuenciaLocal()
    {
        try
        {
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Obtiene la secuencia temporal
    /// </summary>
    /// <returns></returns>
    public int GetIdSecuenciaTemp()
    {
        int id = 0;
        controlsecuencias secuencia = (from x in VMD_BD.controlsecuencias
                                       select x).FirstOrDefault();

        if (secuencia != null)
        {
            if (secuencia.idsecuencia > 100000)
            {
                id = -1;
                secuencia.idsecuencia = id;
                VMD_BD.SaveChanges();
            }
            else
            {
                id = secuencia.idsecuencia - 1;
                secuencia.idsecuencia = id;
                VMD_BD.SaveChanges();

            }
        }
        else //Es la primera secuencia
        {
            id = -1;
            controlsecuencias nuevaSecuencia = new controlsecuencias();
            nuevaSecuencia.idsecuencia = id;

            VMD_BD.controlsecuencias.Add(nuevaSecuencia);
            VMD_BD.SaveChanges();
        }


        return id;
    }

    /// <summary>
    /// Muestra la secuencia Actual
    /// </summary>
    private void MuestraSecuencia()
    {
        try
        {
            if (IGlobales.Corrida.SecuenciaAbierta)
            {
                //Pantalla CAN = "Sec. Asign.: " & Corrida.IDSecuencia & vbCrLf & "Ver.: " & Left(CStr(Corrida.VersionSecuencia), 2) & "  C: " & Left(CStr(Corrida.ConductorActualID), 7) & vbCrLf & "Viaje " & Corrida.ViajeActual & " de " & Corrida.NumViajes & vbCrLf & "O: " & Left(Viajes(Corrida.ViajeActual).OrigenDes, 6) & " D: " & Left(Viajes(Corrida.ViajeActual).DestinoDes, 7)
                //MuestraModo()

            }
            else if(IGlobales.Corrida.NumViajes > 0)
            {
                //PantallaCAN = "Sec. Asign.: " & Corrida.IDSecuencia & vbCrLf & "Ver.: " & Left(CStr(Corrida.VersionSecuencia), 2) & "  C: " & Left(CStr(Corrida.ConductorActualID), 7) & vbCrLf & "Viaje " & Corrida.ViajeActual & " de " & Corrida.NumViajes & vbCrLf & "O: " & Left(Viajes(Corrida.ViajeActual).OrigenDes, 6) & " D: " & Left(Viajes(Corrida.ViajeActual).DestinoDes, 7)
                //MuestraModo()
            }
            else
            {
                if (!(bool)IParametrosInicio.ValidarCveCondSec)
                {
                    //PantallaCAN NO HAY SECUENCIA
                    //MuestraMODO
                }
                else
                {
                    //PantallaCAN NO HAY SECUENCIA
                    //MuestraMODO
                }
            }


        }
        catch
        {

        }
    }

    /// <summary>
    /// Limpia las secuencias
    /// </summary>
    public void LimpiaSecuenciaTemp()
    {
        try
        {
            //Grabo los datos de la secuencia actual, por si apagan el equipo
            can_regsecuencia registroSecuencia = (from x in VMD_BD.can_regsecuencia
                                                  select x).FirstOrDefault();

            registroSecuencia.idsecuencia = 0;
            registroSecuencia.orden = 0;
            registroSecuencia.conductor = 0;

            VMD_BD.SaveChanges();

            //Actualizo el viaje (Detalle), para confirmarlo como cerrado

            can_detsecuencia detalleSecuencia = (from x in VMD_BD.can_detsecuencia
                                                 where x.idsecuencia == IGlobales.Corrida.IDSecuencia && x.orden == IGlobales.Viajes[IGlobales.Corrida.ViajeActual].IDDetSecuencia
                                                 select x).FirstOrDefault();

            detalleSecuencia.confirmado = true;

            VMD_BD.SaveChanges();

            //Marco la variable del Viaje Actual, como confirmado

            if (IGlobales.Corrida.ViajeActual != 0)
            {
                IGlobales.Viajes[IGlobales.Corrida.ViajeActual].Confirmado = true;

                //Checo si todos los viajes están cerrados, para marcar TODA la secuencia como confirmada

                for (int i = 1; i == IGlobales.Corrida.NumViajes; i++)
                {
                    if (!IGlobales.Viajes[i].Confirmado)
                    {
                        return;
                    }
                }
            }

            //Cnx.Execute("update secuencia set confirmado=1 where idsecuencia=" & Corrida.IDSecuencia)  ni siquiera existe la tabla de secuencia
            //pudo haber sido la de can_secuencia, pero en teoria en codigo no funcionaria, se pone para no perdelo de vista

        }
        catch
        {
            //MuestraError("LimpiaSecuenciaTemp")
        }
    }

    /// <summary>
    /// Graba una secuencia temporal
    /// </summary>
    public void GrabaSecuenciaTemp()
    {
        try
        {
            can_regsecuencia regSecuencia = (from x in VMD_BD.can_regsecuencia
                                             select x).FirstOrDefault();

            regSecuencia.idsecuencia = IGlobales.Corrida.IDSecuencia;
            regSecuencia.orden = IGlobales.Viajes[IGlobales.Corrida.ViajeActual].IDDetSecuencia;
            regSecuencia.conductor = IGlobales.Corrida.ConductorActualID;
        }
        catch
        {
            //MuestraError("GrabaSecuenciaTemp")
        }
    }

}