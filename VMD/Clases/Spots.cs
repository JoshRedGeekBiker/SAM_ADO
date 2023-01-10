using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Powered ByRED 16ABR2021
/// </summary>
public class Spots
{


    #region Variables
    ComInterop.CComInterop SyncSpots;

  
    private bool finSync = false; //Powered ByRED 21ABR2021
    private DateTime InicioSync;

    #endregion

    #region Eventos
    /// <summary>
    /// Se encarga de enviar un mensaje al SYNC
    /// Powered ByRED 16ABR2021
    /// </summary>
    /// <param name="mensaje"></param>
    /// <param name="final"></param>
    public delegate void ActualizaSync(string mensaje);
    public event ActualizaSync EventoSync;
    #endregion

    #region Constructores
    /// <summary>
    /// Constructor Principal
    /// Powered ByRED 16ABR2021
    /// </summary>
    public Spots()
    {
        
        SyncSpots = new ComInterop.CComInterop();
        SyncSpots.MensajeEvent += () =>
        {
            //RecibeLogSpots(SyncSpots.Log);
            RecibeLogSpots();
        };

        //SyncSpots.MensajeEvent += this.RecibeLogSpots;

    }
    #endregion

    #region Métodos
    /// <summary>
    /// Sincroniza DLL SPOTS
    /// Powered ByRED 16ABR2021
    /// Powered ByRED 21ABR2021
    /// </summary>
    public void SincronizarSpots()
    {
        try
        {
            EventoSync("Sincronizando SPOTS...");

            SyncSpots.Iniciar();
            //Flageamos para esperar a que termine
            this.InicioSync = DateTime.Now;

            //Encerramos en éste ciclo hasta que termine la sync
            while(this.finSync == false)
            {
                RecibeLogSpots();
                //Pero Aseguramos que si pasa más de 30 segundos del ultimo mensaje recibido, la sincronización de los
                //demás sistemas continue
                if ((DateTime.Now - InicioSync).TotalSeconds > 30)
                {
                    this.finSync = true;
                    EventoSync("Se agoto el tiempo de espera para SPOTS");
                }
            }
            
            //Espero un momento para poder ver el resultado
            Thread.Sleep(2000);

        }
        catch
        {
            EventoSync("Error interno de DLL SPOTS");
        }

    }

    /// <summary>
    /// Se encarga de redirigir el log hacia la sync
    /// Powered ByRED 16ABR2021
    /// Powered ByRED 21ABR2021
    /// </summary>
    /// <param name="mensaje"></param>
    private void RecibeLogSpots(String mensaje)
    {
        if (!this.finSync)
        {
            EventoSync(mensaje);
            //Se valida si es el mensaje de fin de sincronización
            if (mensaje.Equals("Sincronización de SPOTS terminada"))
            {
                this.finSync = true;
            }
            else
            {
                this.InicioSync = DateTime.Now;
            }
        }
    }

    private void RecibeLogSpots()
    {
        if (!this.finSync)
        {
            var mensaje = SyncSpots.Log;
            EventoSync(mensaje);
            //Se valida si es el mensaje de fin de sincronización
            if (mensaje.Equals("Sincronización de SPOTS terminada"))
            {
                this.finSync = true;
            }
            else
            {
                this.InicioSync = DateTime.Now;
            }
        }
    }
    #endregion
}
