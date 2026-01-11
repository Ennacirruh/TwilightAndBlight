using System.Drawing;
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
    }
}
