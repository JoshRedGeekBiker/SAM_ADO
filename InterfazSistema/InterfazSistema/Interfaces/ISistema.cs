
public interface ISistema
    {
        int OrdenDescarga { get; set; }

        int OrdenLoad { get; set; }

        Sistema Sistema { get; }

        string GetVersionSistema { get; }

        void Inicializar();

        void Finalizar();

        bool Sincronizar();

        void Actualizar();

        bool ModoPrueba { get; set; }

        bool ModoNocturno { get; set; }
    }

