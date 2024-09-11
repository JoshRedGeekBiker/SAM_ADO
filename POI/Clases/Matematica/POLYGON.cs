using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class POLYGON
{
    const float MARGEN_ERROR = 2f;

    #region "Propiedades"       
    public List<POINT> Vertex { get; set; }
    public List<ARISTA> LAristas { get; set; }
    #endregion

    #region "Constructores"  
    /// <summary>
    /// Constructor basico
    /// </summary>
    public POLYGON()
    {
        this.Vertex = new List<POINT>();
    }

    /// <summary>
    /// Constructor tomando en cuenta un arreglo de vertices
    /// </summary>
    /// <param name="vertex"></param>
    public POLYGON(POINT[] vertex)
    {
        if (this.Vertex == null)
        {
            this.Vertex = new List<POINT>();
        }
        this.Vertex.AddRange(vertex);
    }
    public void getAristas()
    {
        if (this.Vertex.Count > 0)
        {
            ARISTA arista = null;
            if (this.LAristas == null)
            {
                this.LAristas = new List<ARISTA>();
            }
            for (int i = 0; i < this.Vertex.Count() - 1; i++)
            {
                arista = new ARISTA();
                arista.vertice_A = new POINT(this.Vertex.ElementAt(i).Latitud, this.Vertex.ElementAt(i).Longitud, 0);
                arista.vertice_B = new POINT(this.Vertex.ElementAt(i + 1).Latitud, this.Vertex.ElementAt(i + 1).Longitud, 0);
                this.LAristas.Add(arista);
            }
            arista = new ARISTA();
            arista.vertice_A = new POINT(this.Vertex.ElementAt(this.Vertex.Count - 1).Latitud, this.Vertex.ElementAt(this.Vertex.Count - 1).Longitud, 0);
            arista.vertice_B = new POINT(this.Vertex.ElementAt(0).Latitud, this.Vertex.ElementAt(0).Longitud, 0);
            this.LAristas.Add(arista);
        }
    }
    #endregion

    #region "Metodos publicos"  
    /// <summary>
    /// Agregamos un punto especificando el orden
    /// </summary>
    /// <param name="index"></param>
    /// <param name="coordinate"></param>
    public void ADD(int index, POINT point)
    {
        this.Vertex.Insert(index, point);
    }

    /// <summary>
    /// Agregamos  un punto al final del arreglo
    /// </summary>
    /// <param name="point"></param>
    public void ADD(POINT point)
    {
        this.Vertex.Add(point);
    }

    /// <summary>
    /// Obtenemos la sumatoria de los angulos dentro de un poligono.
    /// </summary>
    /// <param name="point"></param>
    public float IN_POLYGON_ANGLE(POINT point)
    {
        float fltAngulo = 0;
        List<float> fltAngulos = new List<float>();
        //Verificamos que se tengan al menos 3 poligonos
        if (this.Vertex.Count >= 3)
        {
            POINT pntAnterior = null;
            POINT pntPrimero = null;
            int intIteracion = 0;
            float fltAnguloX;


            foreach (POINT p in this.Vertex)
            {
                //Validamos primera ejecución
                if (pntAnterior == null)
                {
                    //Si es la primera ejecución solo guardamos el punto anterior.
                    pntAnterior = p;
                    pntPrimero = p;
                }
                else
                {
                    fltAnguloX = Math.Abs(MATH.GET_ANGLE(point, pntAnterior, p));

                    fltAngulos.Add(fltAnguloX);
                    //Calculamos el angulo
                    fltAngulo += fltAnguloX;
                    //Almacenamos el punto anterior
                    pntAnterior = p;
                }
                intIteracion += 1;
            }

            fltAnguloX = MATH.GET_ANGLE(point, pntAnterior, pntPrimero);
            fltAngulos.Add(fltAnguloX);
            //Sacamos el angulo entre el ultimo punto y el primero
            fltAngulo += fltAnguloX;
        }
        else
        {
            //No se puede comparar con un poligono de menos de tres vertices
            fltAngulo = -1;
        }

        return fltAngulo;
    }

    /// <summary>
    /// Determinamos si un punto se encuentra dentro del poligono
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool IN_POLYGON(POINT point)
    {
        bool bolResultado = false;

        //Determinamos si estamos rodeados por los poligonos
        //if (this.IN_POLYGON_ANGLE(point) >= (360f - MARGEN_ERROR))
        //{
        //    bolResultado = true;
        //}
        //else
        //{
        //    bolResultado = false;
        //}
        int cuenta = 0;
        foreach (ARISTA ar in this.LAristas)
        {
            cuenta = ray_intersect(point, ar.vertice_A, ar.vertice_B) ? cuenta += 1 : cuenta;
        }
        bolResultado = (cuenta % 2) == 1 ? true : false;
        return bolResultado;
    }
    public static Boolean ray_intersect(POINT punto, POINT p1, POINT p2)
    {
        Double m_blue;
        Double m_red;
        const double EPSILON = 0.0001;
        POINT p_min = new POINT();
        POINT p_max = new POINT();
        if (p1.Longitud < p2.Longitud)
        {
            p_min = p1;
            p_max = p2;
        }
        else
        {
            p_min = p2;
            p_max = p1;
        }
        Double puntoy = punto.Longitud;
        if (punto.Longitud == p_min.Longitud || punto.Longitud == p_max.Longitud)
        {
            puntoy += EPSILON;
        }
        if (puntoy < p_min.Longitud || puntoy > p_max.Longitud)
        {
            return false;
        }
        else if (punto.Latitud >= Math.Max(p_min.Latitud, p_max.Latitud))
        {
            return false;
        }
        else if (punto.Latitud < Math.Min(p_min.Latitud, p_max.Latitud))
        {
            return true;
        }
        else
        {
            if (p_min.Latitud != p_max.Latitud)
            {
                m_red = (p_max.Longitud - p_min.Longitud) / (p_max.Longitud - p_min.Longitud);
            }
            else
            {
                m_red = Int32.MaxValue;
            }
            if (p_min.Latitud != punto.Latitud)
            {
                m_blue = (puntoy - p_min.Longitud) / (punto.Latitud - p_min.Latitud);
            }
            else
            {
                m_blue = Int32.MaxValue;
            }
            if (m_blue >= m_red)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    
    /// <summary>
    /// Este es el metodo bueno de colisión 
    /// Powered ByRED 24OCT2023
    /// </summary>
    /// <param name="pt"></param>
    /// <param name="noneZeroMode"></param>
    /// <returns></returns>
    public Boolean in_poligone(POINT pt, bool noneZeroMode)
    {
        int ptNum = Vertex.Count();
        if (ptNum < 3)
        {
            return false;
        }
        int j = ptNum - 1;
        bool oddNodes = false;
        int zeroState = 0;
        for (int k = 0; k < ptNum; k++)
        {
            POINT ptK = Vertex[k];
            POINT ptJ = Vertex[j];
            if (((ptK.Latitud > pt.Latitud) != (ptJ.Latitud > pt.Latitud)) && (pt.Longitud < (ptJ.Longitud - ptK.Longitud) * (pt.Latitud - ptK.Latitud) / (ptJ.Latitud - ptK.Latitud) + ptK.Longitud))
            {
                oddNodes = !oddNodes;
                if (ptK.Latitud > ptJ.Latitud)
                {
                    zeroState++;
                }
                else
                {
                    zeroState--;
                }
            }
            j = k;
        }
        return noneZeroMode ? zeroState != 0 : oddNodes;
    }




    public float angulo_orientacion(POINT p1, POINT p2)
    {
        Console.WriteLine("ANALIZANDO P1: " + p1.Latitud + ", " + p1.Longitud);
        Console.WriteLine("ANALIZANDO P2: " + p2.Latitud + ", " + p2.Longitud);
        float res = 0.0f;
        float a = p2.Latitud - p1.Latitud;
        float b = p2.Longitud - p1.Longitud;
        float c = (float)Math.Sqrt(Convert.ToDouble(Math.Pow(a, 2) + Math.Pow(b, 2)));
        double m = Math.Tan(a / b);
        float[] vector = new float[2] { a, b };
        //Cambiando de indice el 1 y el 0 se cambia de lugar el norte
        float[] vy = new float[2] { 0, 1 };
        float p = 0;
        for (int i = 0; i < vector.Length; i++)
        {
            p += vector[i] * vy[i];
        }
        float r = (float)Math.Acos(p / c);
        //Si se cambia el signo de mayor que a menor que cambia el sentido en el q se forma el circulo
        if (a > 0)
        {
            r = (float)Math.PI * 2 - r;
        }
        res = (float)Convert.ToDouble((180 / Math.PI) * r);
        return res;
    }

    const double Rad2Deg = 180.0 / Math.PI;
    const double Deg2Rad = Math.PI / 180.0;

    /// <summary>
    /// Powered ByRED 24OCT2023
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public float CalculeAngle(POINT origin, POINT target)
    {
        var n = 0f;
        try
        {
            n = (float) (270 - (Math.Atan2(origin.Latitud - target.Latitud, origin.Longitud - target.Longitud)) * 180 / Math.PI);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message + " " + ex.StackTrace);
        }
        return n % 360;
    }

    /// <summary>
    /// Version 2
    /// Miguel Olguin 25OCT2023
    /// No productivo
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public float CalculeAngle2(POINT origin, POINT target)
    {
        double lat1 = origin.Latitud * Deg2Rad;
        double lon1 = origin.Longitud * Deg2Rad;
        double lat2 = target.Latitud * Deg2Rad;
        double lon2 = target.Longitud * Deg2Rad;

        double y = Math.Sin(lon2 - lon1) * Math.Cos(lat2);
        double x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(lon2 - lon1);
        double angleInRadians = Math.Atan2(y, x);

        float angleInDegrees = (float)(angleInRadians * Rad2Deg);

        return (360 + angleInDegrees) % 360;
    }
    #endregion
}
