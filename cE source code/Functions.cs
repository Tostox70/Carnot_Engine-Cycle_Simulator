using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;


public static class Functions
{


   public static void DrawAutoSizedInfoBox(string text, int fontSize, Vector2 Pos)
   {
       float lineSpacing = 5f;
       int padding = 10;

       string[] lines = text.Split('\n');
       int maxWidth = 0;

       foreach (string line in lines)
       {
           int width = MeasureText(line, fontSize);
           if (width > maxWidth) maxWidth = width;
       }

       int totalHeight = (int)(lines.Length * (fontSize + lineSpacing));
       Rectangle textBox = new Rectangle(
           Pos.X - (maxWidth + padding * 2),                  // shift left by box width
           Pos.Y - (totalHeight + padding * 2),               // still position above
           maxWidth + padding * 2,
           totalHeight + padding * 2
       );
       DrawRectangleRec(textBox, Color.Gray);
       DrawRectangleLinesEx(textBox, 1, Color.White);

       for (int i = 0; i < lines.Length; i++)
       {
           int x = (int)textBox.X + padding;
           int y = (int)textBox.Y + padding + i * (int)(fontSize + lineSpacing);
           DrawText(lines[i], x, y, fontSize, Color.White);
       }
   }

   public static void eValues()
   {
       int x = (int)Visual.workCalcPosX;
       int y = (int)Visual.workCalcPosY;
       int width = Program.screenWidth;


       DrawText($"Current Volume: {Points.gasVolumeNow:N5}  m^3", x+9, y+6, 20, Color.Yellow);
       DrawText($"Current Pressure: {Points.gasPressure / 1000f:0,0.0} kPa", x+9, y+34, 20, Color.Yellow);
       DrawText($"Current Temp: {Points.gasTemp:N0} K", x+9, y+59, 20, Color.Yellow);
       DrawText($"Total Work: {Math.Round(Points.totWork, 1)} J", x+9, y+84, 20, Color.Yellow);
       DrawText($"Carnot Efficiency: {Points.eff:N2} %", x+9, y+109, 20, Color.Yellow); // y diff of 25

       DrawText("by Tostox", width/100*97, 5, 15, new Color(0, 255, 255, 255) );
   }
}


