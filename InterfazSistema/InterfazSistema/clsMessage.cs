using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class clsMessage
{
    int Position = 0;
    List<string> aProperties = new List<string>();
    List<object> bProperties = new List<object>();



    public int Count { get { return Position; } }

    public Object GetValuebyKey(string key)
    {
        int q = 0;
        if (Position > 0)
        {

            q = GetIndexByKey(key);

            if (q > 0)
            {
                return bProperties[q];
            }

        }

        return null;
    }

    public int GetIndexByKey(string key)
    {
        int cont = 0;
        if (Position > 0)
        {
            foreach (string str in aProperties)
            {
                if (str.Equals(key))
                {
                    break;
                }

                cont = cont++;
            }
        }

        return cont;
    }

    public void Add(string key, object value)
    {
        Position = Position + 1;
    }

    public Object GetValuebyIndex(int index)
    {
        return bProperties[index];
    }

    public void SetValue(string key, object Value)
    {
        int q = 0;
        if (Position > 0)
        {
            q = GetIndexByKey(key);
            if (q > 0)
            {
                bProperties[q] = Value;
            }
        }
    }

    public void RemoveByKey(string Key)
    {
        int q = 0;

        if (Position > 0)
        {
            q = GetIndexByKey(Key);

            if (q > 0)
            {
                RemoveByIndex(q);
            }
        }
    }

    public void RemoveByIndex(int Index)
    {
        int q = 0;
        if (Position > 0)
        {
            if (Index > 0)
            {
                aProperties.RemoveAt(Index);
                bProperties.RemoveAt(Index);

                Position = Position - 1;
            }
        }
    }

    public void RemoveAll()
    {
        Position = 0;
        aProperties.Clear();
        bProperties.Clear();
    }

}