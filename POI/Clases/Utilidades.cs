﻿using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Utilidades
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

    /*public Boolean in_angle(float comienzo, float punto, float final)
    {
        Boolean res = false;
        try
        {
            Boolean is_vuelta =false;
            Boolean termino = false;
            if (comienzo > final) {
                float aux = final;
                do {
                    aux++;
                        if (aux > 360) {
                            is_vuelta = true;
                        aux = 0;
                        }

                    if (aux == final) {
                        termino = true;
                    }
                } while (!termino == true);
            }
            
            comienzo = gradeToDec(comienzo);
            punto = (is_vuelta ? gradeToDec(punto) + 100 : gradeToDec(punto));
            final = (is_vuelta ? gradeToDec(final) + 100 : gradeToDec(final));
            if ((comienzo <= punto) && (punto <= final)) {
                res = true;
            }
        }
        catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
        return res;
    }

    public float gradeToDec(float grade) {
        Double val_grade = 0.277777778;
        return (float) val_grade*grade;
    }*/

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

    private double CoordenadaToDecimal(double coordenada)

    {

        bool negativo = false;
        if (coordenada < 0)
        {
            negativo = true;
            coordenada = coordenada * (-1);
        }
        else
        {
            negativo = false;
        }
        double grados = Convert.ToDouble(coordenada.ToString().Substring(0, 2));
        double minutos = (Convert.ToDouble(coordenada.ToString().Substring(2)) / 60);
        double coordenadaFinal = grados + minutos;

        if (negativo)
        {
            coordenadaFinal = coordenadaFinal * (-1);
        }
        return coordenadaFinal;
    }

    public static List<Response_spot> analizarListaSpot(List<Response_spot> list)
    {
        List<Response_spot> res = null;
        Console.WriteLine("PUNTOS DE INTERES ENTRANTES: " + list.Count());
        try
        {
            res = new List<Response_spot>();
            foreach (Response_spot item in list)
            {
                if (res.Count == 0)
                {
                    Console.WriteLine("SE AGREGO A LA LISTA FINAL: " + item.nombre);
                    res.Add(item);
                }
                else
                {
                    Boolean entro = false;
                    foreach (Response_spot item2 in res)
                    {
                        if ((item.latitudes == item2.latitudes) & (item.longitudes == item2.longitudes))
                        {
                            Console.WriteLine("SE ENCONTRO IGUALDAD EN EL ELEMENTO " + item.nombre + " y " + item2.nombre);
                            item2.isPuntoDoble = true;
                            item2.punto2 = item;
                            item2.orientacionInicial2 = item.orientacionInicial;
                            item2.orientacionFinal2 = item.orientacionFinal;
                            entro = true;
                            break;
                        }
                    }
                    if (!entro)
                    {
                        Console.WriteLine("SE AGREGO A LA LISTA FINAL: " + item.nombre);
                        res.Add(item);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        Console.WriteLine("PUNTOS DE INTERES SALIENTES: " + res.Count());
        return res;
    }
}


