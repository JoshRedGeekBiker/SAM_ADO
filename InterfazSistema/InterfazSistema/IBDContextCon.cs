using InterfazSistema;

/// <summary>
/// Contexto de la base de datos de condusat
/// </summary>
public interface IBDContextCon
{
    condusatEntities CONDUSAT_BD { get; }
}