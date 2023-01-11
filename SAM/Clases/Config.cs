using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using InterfazSistema.ModelosBD;
using Microsoft.Win32;

public class Config
{

    //Para almacenar las posibles metas por region que tuviera
    //Powered ByRED 10/SEP/2020
    private List<string> ListaMetas = new List<string>();


    /// <summary>
    /// Constructor principal
    /// </summary>
    public Config()
    {


    }

    /// <summary>
    /// Verifica la existencia del número de región
    /// de ser así guarda los parametros en la tabla de
    /// parametros inicio del movil
    /// </summary>
    /// <param name="VMD_BD"></param>
    /// <param name="reg"></param>
    /// <returns></returns>
    public string VerificarRegion(vmdEntities VMD_BD, long reg)
    {
        can_referenciaregion region = (from x in VMD_BD.can_referenciaregion
                                       where x.IdRegion == reg
                                       select x).FirstOrDefault();

        if (region != null)
        {//Si encontró región, entonces guardamos en parámetros inicio

            var parametros = (from x in VMD_BD.can_parametrosinicio
                              select x).ToList();


            foreach (can_parametrosinicio par in parametros)
            {
                par.Region = region.IdRegion;
                par.Marca = Convert.ToInt64(region.Marca);
                par.Zona = region.Zona;
                par.IDGrupo = Convert.ToInt64(region.IdGrupo);
                par.IDDireccion = Convert.ToInt64(region.IdDireccion);
                par.IDRegionOperativa = Convert.ToInt64(region.IdRegionOperativa);
                par.NombreTipoMetaCAN = region.NombreTipoMetaCAN;
                par.NombreRegionOperCAN = region.NombreRegionOperCAN;
                par.CnStrServer = region.CnStrServer;

            }

            VMD_BD.SaveChanges();


            var regionstr = reg.ToString();
            //Validamos de una vez si tiene Metas por region para CAN
            //Powered ByRED 10/SEP/2020
            ListaMetas = (from x in VMD_BD.can_cattipometasregion
                          where x.id_region == regionstr
                          select x.nombre_tipo_meta).ToList();

            return region.Region;
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// Se encarga de Renombrar el nombre del equipo
    /// para que pueda identificarse en la Red
    /// </summary>
    /// <param name="newName"></param>
    public void CambiarNombreEquipo(string newName)
    {
        try
        {
            RegistryKey key = Registry.LocalMachine;

            string activeComputerName = @"SYSTEM\CurrentControlSet\Control\ComputerName\ActiveComputerName";
            RegistryKey activeCmpName = key.CreateSubKey(activeComputerName);
            activeCmpName.SetValue("ComputerName", newName);
            activeCmpName.Close();
            string computerName = @"SYSTEM\CurrentControlSet\Control\ComputerName\ComputerName";
            RegistryKey cmpName = key.CreateSubKey(computerName);
            cmpName.SetValue("ComputerName", newName);
            cmpName.Close();
            string _hostName = @"SYSTEM\CurrentControlSet\services\Tcpip\Parameters\";
            RegistryKey hostName = key.CreateSubKey(_hostName);
            hostName.SetValue("Hostname", newName);
            hostName.SetValue("NV Hostname", newName);
            hostName.Close();
        }
        catch
        {
            
        }
    }

    /// <summary>
    /// Se encarga de obtener la MACAddress del adaptador de RED
    /// </summary>
    public string ObtenerMacAddress()
    {
        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

        foreach(NetworkInterface adapter in nics)
        {
            string tipo = adapter.NetworkInterfaceType.ToString();

            if(tipo.Contains("Wireless"))
            {
                return adapter.GetPhysicalAddress().ToString();
            }
        }
        return "";
    }

    /// <summary>
    /// Se encarga de planchar la MacAddress del equipo
    /// </summary>
    /// <param name="VMD_BD"></param>
    /// <param name="mac"></param>
    public void InsertarMacAddress(vmdEntities VMD_BD, string mac)
    {
        can_parametrosinicio Parametros = (from x in VMD_BD.can_parametrosinicio
                                           select x).FirstOrDefault();

        if (Parametros != null)
        {
            Parametros.MacAddress = mac;
            VMD_BD.SaveChanges();
        }
    }


    /// <summary>
    /// Se encarga de regresar la existencia de metas CAN por region
    /// Powered ByRED 10/SEP/2020
    /// </summary>
    /// <returns></returns>
    public bool ValidarMetasCAN()
    {
        if(ListaMetas.Count > 0) { return true; } else { return false; }
    }

    /// <summary>
    /// Se encarga de retornar la lista de metas obtenidas
    /// Powered ByRED 10/SEP/2020
    /// </summary>
    /// <returns></returns>
    public List<string> RecuperarListaMetas()
    {
        return ListaMetas;
    }
}