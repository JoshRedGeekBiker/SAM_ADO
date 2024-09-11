using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class MATH
{
    const float RADIUS_PLANET = 6371000f;

    /// <summary>
    /// Convertimos grados a radianes
    /// </summary>
    /// <param name="degrees"></param>
    /// <returns></returns>
    public static double TO_RADIANS(Double degrees)
    {
        return (Convert.ToDouble(degrees) / (180 / Math.PI));
    }

    /// <summary>
    /// Convertimos de radianes a grados
    /// </summary>
    /// <param name="radians"></param>
    /// <returns></returns>
    public static double TO_DEGREES(float radians)
    {
        return (Convert.ToDouble(radians) * (180 / Math.PI));
    }

    /// <summary>
    /// Obtenemos la distancia cartgrafica entre dos puntos
    /// </summary>
    /// <param name="P1"></param>
    /// <param name="P2"></param>
    /// <returns></returns>
    public static float GET_DISTANCE(POINT P1, POINT P2)
    {
        double dblRadiusLatitud1 = TO_RADIANS(P1.Latitud);
        double dblRadiusLatitud2 = TO_RADIANS(P2.Latitud);
        double dblDeltaLatitud = TO_RADIANS(P2.Latitud - P1.Latitud);
        double dblDeltaLongitud = TO_RADIANS(P2.Longitud - P1.Longitud);

        double a = Math.Sin(dblDeltaLatitud / 2) * Math.Sin(dblDeltaLatitud / 2) +
            Math.Cos(dblRadiusLatitud1) * Math.Cos(dblRadiusLatitud2) *
            Math.Sin(dblDeltaLongitud / 2) * Math.Sin(dblDeltaLongitud / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double d = RADIUS_PLANET * c;

        return (float)d;
    }

    /// <summary>
    /// Calculamos el angulo entre un centro y dos puntos. El angulo es regresado en Grados
    /// </summary>
    /// <param name="CENTER"></param>
    /// <param name="P1"></param>
    /// <param name="P2"></param>
    /// <returns></returns>
    public static float GET_ANGLE(POINT CENTER, POINT P1, POINT P2)
    {
        float fltAngulo = 0f;
        float fltHip = 0f;
        float fltOpuesto = GET_DISTANCE(P1, P2);

        //Determinamos donde esta la hipotemusa
        if (GET_DISTANCE(CENTER, P2) >= GET_DISTANCE(CENTER, P1))
        {
            fltHip = GET_DISTANCE(CENTER, P2);
        }
        else
        {
            fltHip = GET_DISTANCE(CENTER, P1);
        }

        //Calculamos el Sen del Angulo
        fltAngulo = fltOpuesto / fltHip;

        //Si el angulo es Mayor a 1
        if (fltAngulo > 1)
        {
            //Hacemos el ajuste al complento
            //fltAngulo = 1 - (fltAngulo - 1);
            fltAngulo = fltAngulo - 1;
            //asdasda
            //Calculoamos el arco seno y lo regresamos como grados
            fltAngulo = (float)Math.Asin(fltAngulo);
            fltAngulo = (float)MATH.TO_DEGREES(fltAngulo) + 90f;
        }
        else
        {
            //Calculoamos el arco seno y lo regresamos como grados
            fltAngulo = (float)Math.Asin(fltAngulo);
            fltAngulo = (float)MATH.TO_DEGREES(fltAngulo);
        }



        //Regresamos el angulo
        return (float)Math.Round(fltAngulo, 4);
    }

    /// <summary>
    /// Convertimos una cordenada a decimal
    /// </summary>
    /// <param name="coordinate"></param>
    /// <returns></returns>
    public static float GET_DECIMAL_COORDINATE(float coordinate, string Orientacion)
    {
        double dblCordenadaDecimal = 0;
        double dblGrados = 0;
        double dblMInutos = 0;

        string stringcoordenada = Math.Abs(coordinate).ToString();
        try
        {
            dblGrados = Convert.ToDouble(stringcoordenada.Substring(0, 2));
            dblMInutos = Convert.ToDouble(stringcoordenada.Substring(2));

            // Convertimos a decimal
            dblMInutos = dblMInutos / 60;
            dblCordenadaDecimal = dblGrados + dblMInutos;
        }
        catch
        {
        }

        //Determinamos la orientacion
        if (Orientacion == "S" || Orientacion == "W")
        {
            dblCordenadaDecimal = dblCordenadaDecimal * -1;
        }

        return (float)dblCordenadaDecimal;
    }

}
