using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class Utilidades
{
    public static float ConvertirDecimalesACANLatitud(float fltCordenada)
    {
        float res = 0F;
        try
        {
            float dblCordenada = 0;
            float dblGrados = 0;
            float dblMinutos = 0;
            String strCordenada = String.Empty;
            Boolean bolEsNegativo = false;
            if (fltCordenada < 0)
            {
                bolEsNegativo = true;
                fltCordenada *= -1;
            }
            dblGrados = (float)Convert.ToDouble(fltCordenada.ToString().Substring(0, 2));
            dblMinutos = (float)Convert.ToDouble(fltCordenada.ToString().Substring(2)) * 60;
            strCordenada = dblGrados.ToString() + dblMinutos.ToString();
            dblCordenada = (float)Convert.ToDouble(strCordenada);
            if (bolEsNegativo)
            {
                dblCordenada *= -1;
            }

            res = dblCordenada;
        }
        catch (Exception ex)
        {
        }
        return res;
    }

    public static float ConvertirCordenadasCANaDecimalLongitud(float fltCordenada)
    {
        float res = 0F;
        try
        {
            float dblCordenada = 0;
            float dblGrados = 0;
            float dblMinutos = 0;
            String strCordenada = String.Empty;
            Boolean bolEsNegativo = false;
            int PstnCdn = 0;
            if (fltCordenada < 0)
            {
                bolEsNegativo = true;
                fltCordenada *= -1;
            }
            PstnCdn = fltCordenada.ToString().Substring(0, 1).Equals("1") ? 3 : 2;
            //PstnCdn = fltCordenada.ToString().IndexOf(".");

            dblGrados = (float)Convert.ToDouble(fltCordenada.ToString().Substring(0, PstnCdn));
            dblMinutos = (float)(Convert.ToDouble(fltCordenada.ToString().Substring(PstnCdn)) / 60);
            //strCordenada = dblGrados.ToString() + dblMinutos.ToString();
            dblCordenada = dblGrados + dblMinutos;
            if (bolEsNegativo)
            {
                dblCordenada *= -1;
            }

            res = dblCordenada;
        }
        catch (Exception ex)
        {
        }
        return res;
    }

    public static float ConvertirCordenadasCANaDecimalLatitud(float fltCordenada)
    {
        float res = 0F;
        try
        {
            float dblCordenada = 0;
            float dblGrados = 0;
            float dblMinutos = 0;
            String strCordenada = String.Empty;
            Boolean bolEsNegativo = false;
            int PstnCdn = 2;
            if (fltCordenada < 0)
            {
                bolEsNegativo = true;
                fltCordenada *= -1;
            }
            //PstnCdn = fltCordenada.ToString().Substring(0, 1).Equals("1") ? 3 : 2;
            //PstnCdn = fltCordenada.ToString().IndexOf(".");

            dblGrados = (float)Convert.ToDouble(fltCordenada.ToString().Substring(0, PstnCdn));
            dblMinutos = (float)(Convert.ToDouble(fltCordenada.ToString().Substring(PstnCdn)) / 60);
            //strCordenada = dblGrados.ToString() + dblMinutos.ToString();
            dblCordenada = dblGrados + dblMinutos;
            if (bolEsNegativo)
            {
                dblCordenada *= -1;
            }

            res = dblCordenada;
        }
        catch (Exception ex)
        {
        }
        return res;
    }
    public Boolean in_angle(float comienzo, float punto, float final)
    {
        Boolean res = false;
        double p = Math.Sin(30 * Math.PI / 360);

        if (comienzo >= 180 && comienzo > final)
        {

            if (punto >= 180 && punto <= 360 + final)
            {
                if (punto > final && punto < 360 && punto < comienzo)
                {

                    res = false;

                }
                else { res = true; }



            }


            else

            {
                if (punto <= 180 && punto < final)
                {
                    res = true;
                }
                else
                {
                    res = false;


                }

            }


        }
        else if (comienzo >= 90 && comienzo > final)
        {

            if (punto >= 90 && punto <= 360 + final)
            {
                if (punto > final && punto < 360 && punto < comienzo)
                {

                    res = false;

                }
                else { res = true; }

            }


            else

            {
                if (punto <= 90 && punto < final)
                {
                    res = true;
                }
                else
                {
                    res = false;

                }

            }


        }
        else if (comienzo >= 0 && comienzo > final)
        {

            if (punto >= 0 && punto <= 360)
            {
                if (punto > final && punto < 360 && punto < comienzo)
                {

                    res = false;

                }
                else { res = true; }

            }


            else

            {
                if (punto <= 0 && punto < final)
                {
                    res = true;
                }
                else
                {
                    res = false;

                }

            }


        }



        else
        {

            double A = (Math.Cos(comienzo * Math.PI / 360)) * (Math.Sin(punto * Math.PI / 360)) - (Math.Sin(comienzo * Math.PI / 360)) * (Math.Cos(punto * Math.PI / 360));
            double Ab = (Math.Cos(comienzo * Math.PI / 360)) * (Math.Sin(final * Math.PI / 360)) - (Math.Sin(comienzo * Math.PI / 360)) * (Math.Cos(final * Math.PI / 360));
            double B = (Math.Cos(final * Math.PI / 360)) * (Math.Sin(punto * Math.PI / 360)) - (Math.Sin(final * Math.PI / 360)) * (Math.Cos(punto * Math.PI / 360));
            double Bb = (Math.Cos(final * Math.PI / 360)) * (Math.Sin(comienzo * Math.PI / 360)) - (Math.Sin(final * Math.PI / 360)) * (Math.Cos(comienzo * Math.PI / 360));

            if (A * Ab >= 0 && B * Bb >= 0)
            {

                res = true;
            }
        }



        return res;
    }

    public void convertChon()
    {
        var s = CrontabSchedule.Parse("59 12-21   5,10,20,30 5   5  ");
        var start = new DateTime(2000, 1, 1);
        var end = start.AddYears(1);
        var occurrences = s.GetNextOccurrences(start, end);
        //Console.WriteLine(string.Join(Environment.NewLine,from t in occurrences select $"{t:ddd, dd MMM yyyy HH:mm}"));
        //DateTime d = s.GetNextOccurrence();
    }
}
