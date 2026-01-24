using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace TwilightAndBlight
{
    public static class GameUtility
    {
        public static string ColorString(string text, int r, int g, int b)
        {

            string hexcolor = $"#{r:X2}{g:X2}{b:X2}";
            string colorString = $"<color={hexcolor}>{text}</color>";
            return colorString;
        }
        public static int FactorialInt(int factorial)
        {
            factorial = Mathf.Max(0, factorial);
            int total = 1;
            for (int i = 1; i <= factorial; i++)
            {
                total *= i;
            }
            return total;
        }
        public static void DebugLogCollection<T>(IEnumerable<T> enumerable) 
        {
            if (enumerable.Count() == 0)
            {
                Debug.Log("[]");
            }
            else
            {
                string fullLog = enumerable.ToString() + "--> [";
                foreach (T item in enumerable)
                {
                    fullLog += item.ToString() + ", ";
                }
                fullLog.Remove(fullLog.Length - 2);
                fullLog += "]";
                Debug.Log(fullLog);
            }
        }

    }
    
}
