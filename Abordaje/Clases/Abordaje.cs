using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using InterfazSistema.ModelosBD;

public class Abordaje: IBDContext
{
    #region "Propiedades"
    private string PassTVE { get; set; } = "1357";
    public string VersionDLL { get; set; } = "";

    //Contexto de Base de datos
    public vmdEntities VMD_BD { get; }
    #endregion

    #region "Propiedades Heredadas"

    #endregion

    #region "Variables"

    private TVE.TVE MyTVE;

    #endregion

    #region "Variables de evento"

    #endregion

    #region "Constructores"
    /// <summary>
    /// Constructor Principal
    /// </summary>
    public Abordaje()
    {
        ObtenerPass();

        VMD_BD = new vmdEntities();

        //Para obtener la versión de la DLL Powered ByRED 15ENE2021
        VersionDLL = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        //Reportamos versión a BD
        ReportarVersion();
    }

    #endregion

    #region "Metodos Publicos"

    /// <summary>
    /// Se encarga de preparar al sistema para la transferencia
    /// </summary>
    /// <returns></returns>
    public Task<bool> Transferencia()
    {
        return Task<bool>.Run(
           async () =>
           {
               await Task.Delay(1);
               try
               {
                   Inicializar();

                   if (MyTVE.FuncEjecutarQRConexion().Equals("done_qrConexion"))
                   {
                       
                       MyTVE.FuncBorrarInformacion();

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
           });
    }

    /// <summary>
    /// Se encarga de validar la transferencia
    /// </summary>
    /// <returns></returns>
    public Task<bool> ValidarTransferencia()
    {
        return Task<bool>.Run(
           async () =>
           {
               await Task.Delay(1);
               try
               {
                  return MyTVE.FuncValidarTransferencia() ? true : false;
               }
               catch
               {
                   return false;
               }
           });
    }

    /// <summary>
    /// Se encarga de retornar el valor de exito
    /// de la transferencia
    /// </summary>
    /// <returns></returns>
    public bool EstadoTransferencia()
    {
        try
        {
            return MyTVE.EstadoDeTransferencia;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Obtiene el estado de la conexion de TVE
    /// </summary>
    /// <returns></returns>
    public Task<bool> EstadoConexionTVE()
    {
        return Task<bool>.Run(
          async () =>
          {
              await Task.Delay(1);
              try
              {
                  return MyTVE.FuncValidarConexion() ? true : false;
              }
              catch
              {
                  return false;
              }
          });
    }

    /// <summary>
    /// Se encarga de generar los QR para la consulta de los datos
    /// </summary>
    /// <returns></returns>
    public Task<bool> GenerarQr()
    {
        return Task<bool>.Run(
          async () =>
          {
              await Task.Delay(1);
              try
              {
                  var hola = MyTVE.FuncEjecutarQR();
                  return true;
              }
              catch
              {
                  return false;
              }
          });
    }

    /// <summary>
    /// Se encarga de regresar la información de la corrida
    /// </summary>
    /// <returns></returns>
    public string InformacionCorrida()
    {
        try
        {
            return MyTVE.FuncInfoCorrida();
        }
        catch
        {
            return "";
        }
    }

    /// <summary>
    /// Se encarga de finalizar la lógica de TVE
    /// </summary>
    public void Finalizar()
    {
        try
        {
            MyTVE = null;
        }
        catch
        {

        }
        
    }

    /// <summary>
    /// Se encarga de obtener la contraseña del sistema de Abordaje
    /// </summary>
    /// <returns></returns>
    public string PassAbordaje()
    {
        return this.PassTVE;
    }

    #endregion

    #region "Método Privados"
    /// <summary>
    /// Se encarga de inicializar la lógica de TVE
    /// </summary>
    private void Inicializar()
    {
        if (MyTVE == null)
        {
            MyTVE = new TVE.TVE();
        }
    }

    /// <summary>
    /// Se encarga de obtener la contraseña de la logica de TVE
    /// </summary>
    private void ObtenerPass()
    {
        try
        {
            Inicializar();
            this.PassTVE = MyTVE.FuncValidarUsuarioConsulta();
            Finalizar();
        }
        catch
        {

        }
    }

    /// <summary>
    /// Se encarga de reportar la versión a la tabla de Plat_versiones
    /// Powered ByRED 15ENE2021
    /// </summary>
    private void ReportarVersion()
    {
        try
        {
            //Mandamos a planchar la version de SAM en la tabla Versiones de PLAT

            var plat_versiones = (from x in VMD_BD.plat_versiones
                                  select x).ToList();

            if (plat_versiones != null)
            {
                var version_abordaje = plat_versiones.Where(x => x.Sistemas.Equals("SAM - Abordaje")).FirstOrDefault();

                //No existe el registro en la tabla, lo agregamos
                if (version_abordaje == null)
                {
                    plat_versiones nuevaVersion = new plat_versiones();

                    nuevaVersion.Sistemas = "SAM - Abordaje";
                    nuevaVersion.Versiones = this.VersionDLL;

                    VMD_BD.plat_versiones.Add(nuevaVersion);

                }
                else//sólo planchamos la version
                {
                    version_abordaje.Versiones = this.VersionDLL;
                }

                VMD_BD.SaveChanges();
            }
        }
        catch
        {

        }
    }
    #endregion

    #region "Metodos Heredados"

    #endregion

    #region "Eventos"

    #endregion

    #region  "Timers"

    #endregion
}