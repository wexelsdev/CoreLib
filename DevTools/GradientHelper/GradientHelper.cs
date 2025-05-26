using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CoreLib.DevTools.GradientHelper
{
    public static class GradientHelper
    {
        private struct GradientStop
        {
            public readonly float Position;
            public readonly Color Color;

            public GradientStop(float position, Color color)
            {
                Position = position;
                Color = color;
            }
        }

        public static string ApplyGradient(this string text, List<(string hex, float stop)> gradientStops)
        {
            if (string.IsNullOrEmpty(text) || gradientStops.Count < 2)
                return text;
        
            var stops = new List<GradientStop>(gradientStops.Count);
            foreach (var (hex, stop) in gradientStops)
            {
                if (!ColorUtility.TryParseHtmlString(NormalizeHex(hex), out var color))
                    throw new ArgumentException($"Invalid HEX: {hex}");
                stops.Add(new GradientStop(stop, color));
            }
            stops.Sort((a, b) => a.Position.CompareTo(b.Position));

            StringBuilder sb = new StringBuilder(text.Length * 30);

            int len = text.Length;
            for (int i = 0; i < len; i++)
            {
                float percent = (i / (float)(len - 1)) * 100f;

                GradientStop left = stops[0], right = stops[stops.Count - 1];
                for (int j = 0; j < stops.Count; j++)
                {
                    if (stops[j].Position > percent)
                    {
                        right = stops[j];
                        if (j > 0) left = stops[j - 1];
                        break;
                    }
                }

                float t = Mathf.InverseLerp(left.Position, right.Position, percent);
                Color col = Color.Lerp(left.Color, right.Color, t);
                string hex = ColorUtility.ToHtmlStringRGB(col);
                sb.Append("<color=#").Append(hex).Append('>').Append(text[i]).Append("</color>");
            }

            return sb.ToString();
        }

        private static string NormalizeHex(string hex) => hex.StartsWith("#") ? hex : $"#{hex}";
    }
}