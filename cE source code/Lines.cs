using System.Numerics;
using System;
using Raylib_cs;
using static Raylib_cs.Raylib;

public static class Lines
{
   private static Raylib_cs.Color mainLines = new Color(255, 255, 255, 60);
   private static Raylib_cs.Color subLines = new Color(255, 255, 255, 30);
   private static int cellSize = 50;

   // Layout dimensions
   public static class Layout
   {
       public static Vector2 THlineTop;
       public static Vector2 line2Top;
       public static Vector2 line2L;
       public static Vector2 line2R;
       public static Vector2 THlineBot;
       public static float graphBlockSizeX;
       public static float TopInfoSizeX;
       public static float BotInfoSizeX;

       //ADDED
       public static float xBlockGraph;
       public static Vector2 GPorigin;
       public static Vector2 line1Top;
       public static float graphBlockSizeY;
       public static float y2;
       public static float TFxAxis;
       //PARTS
       public static Vector2 line1L;
       public static Vector2 line1R;
       public static float partsXdimension;
       public static float partsYdimension;

   }

   // Font sizes
   public static class FontSizes
   {
       public const int Main = 80;
       public const int Info = 40;
       public const int Mini = 30;
   }

   public static void DrawGrid(int screenWidth, int screenHeight)
   {
       // Main grid lines
       for (int x = 0; x <= screenWidth; x += cellSize)
           DrawLine(x, 0, x, screenHeight, mainLines);

       for (int y = 0; y <= screenHeight; y += cellSize)
           DrawLine(0, y, screenWidth, y, mainLines);

       // Sub grid lines
       for (int subX = cellSize / 2; subX <= screenWidth; subX += cellSize)
           DrawLine(subX, 0, subX, screenHeight, subLines);

       for (int subY = cellSize / 2; subY <= screenHeight; subY += cellSize)
           DrawLine(0, subY, screenWidth, subY, subLines);
   }

   public static void UpdateLayoutDimensions(int screenWidth, int screenHeight)
   {
       // Update layout points
       Layout.line2Top = new Vector2(screenWidth * 0.3f + 10, 0);
       Layout.line2L = new Vector2(Layout.line2Top.X, screenHeight * 0.6f);
       Layout.line2R = new Vector2(screenWidth, screenHeight * 0.6f);
       Layout.THlineTop = new Vector2(screenWidth * 0.4527f, screenHeight * 0.6f);
       Layout.THlineBot = new Vector2(screenWidth * 0.4527f, screenHeight * 0.8f);
       Layout.graphBlockSizeX = Raymath.Vector2Distance(Layout.line2L, Layout.line2R);
       Layout.graphBlockSizeY = Raymath.Vector2Distance(Layout.line2Top, Layout.line2L);

       Layout.TopInfoSizeX = Layout.graphBlockSizeX / 5;
       Layout.BotInfoSizeX = Layout.graphBlockSizeX / 4;
       Layout.xBlockGraph = Layout.graphBlockSizeX;
       Layout.GPorigin = Layout.line1Top;
       Layout.y2 = Layout.graphBlockSizeY / 16;
       Layout.TFxAxis = Layout.xBlockGraph / 16;

       Layout.partsXdimension = Raymath.Vector2Distance(Layout.line1L, Layout.line1R);
       Layout.partsYdimension = Raymath.Vector2Distance(Layout.line1R, new Vector2(Layout.line1R.X, screenHeight));
   }

   public static void DrawLayoutLines(int screenWidth, int screenHeight)
   {
       UpdateLayoutDimensions(screenWidth, screenHeight);

       // Dividing Left & right
       Layout.line1Top = new Vector2(screenWidth * 0.3f + 10, 0);
       Vector2 line1Bot = new Vector2(screenWidth * 0.3f + 10, screenHeight);

       // STAGE LINE
       Layout.line1L = new Vector2(0, screenHeight * 0.08f);
       Layout.line1R = new Vector2(Layout.line1Top.X, screenHeight * 0.08f);

       // INFO LINE
       Vector2 line3L = new Vector2(Layout.line2Top.X, screenHeight * 0.8f);
       Vector2 line3R = new Vector2(screenWidth, screenHeight * 0.8f);

       // Draw all lines
       DrawLineEx(Layout.line1Top, line1Bot, 2, Raylib_cs.Color.White);
       DrawLineEx(Layout.line1L, Layout.line1R, 2, Raylib_cs.Color.White);
       DrawLineEx(Layout.line2L, Layout.line2R, 5, Raylib_cs.Color.White);
       DrawLineEx(line3L, line3R, 5, Raylib_cs.Color.White);
       //DrawLineEx(Layout.THlineTop, Layout.THlineBot, 2, Raylib_cs.Color.White);
   }

   public static (Vector2 position, Vector2 size) TopHoverPositionSize(int slot = 0)
   {
       var size = new Vector2(Layout.TopInfoSizeX, Layout.THlineBot.Y - Layout.THlineTop.Y);

       // Base starting X position for the first rectangle
       float baseX = Layout.line2Top.X;

       // Each additional rectangle shifts by its width
       baseX += slot * Layout.TopInfoSizeX;

       var position = new Vector2(baseX, Layout.line2L.Y);

       return (position, size);
   }

   public static (Vector2 position, Vector2 size) BotHoverPositionSize(int slot = 0)
   {
       var size = new Vector2(Layout.BotInfoSizeX, Layout.THlineBot.Y - Layout.THlineTop.Y);

       // Base starting X position for the first rectangle
       float baseX = Layout.line2Top.X;

       // Each additional rectangle shifts by its width
       baseX += slot * Layout.BotInfoSizeX;

       var position = new Vector2(baseX, Layout.THlineBot.Y);

       return (position, size);
   }

   public static void DrawHoverInofs(Vector2 position, Vector2 size, string number,
   Raylib_cs.Color textColor, string label, string sub, float numberPosX, float numberPosY, int textSize, float labelPosX, float labelPosY)
   {
       DrawText($"{number}",
     (int)(position.X + size.X / numberPosX),
     (int)(position.Y + size.Y / 8 * numberPosY), //*1.6
     textSize, textColor);
       // Kelvin unit
       DrawText($"{sub}",
           (int)(position.X + (size.X / 5) + MeasureText($"{number}", textSize)),
           (int)(position.Y + (size.Y / 8 * 3.5)),
           FontSizes.Mini, Raylib_cs.Color.White);

       DrawText(label,
(int)(position.X + size.X / 7 * labelPosX), // /7*3
(int)(position.Y + size.Y / 8 * labelPosY),
FontSizes.Info, Raylib_cs.Color.White);
   }

   public static void DrawHoverInofsPlus(Vector2 position, Vector2 size, float number1,
   float number2, float number3, float number4, Raylib_cs.Color textColor, float numberPos, int textSize, string suffix, string unit)
   {
       DrawText($"{suffix}1: {number1} {unit}",
     (int)(position.X + size.X / numberPos),
     (int)(position.Y + size.Y / 8 * 1.5),
     textSize, textColor);

       DrawText($"{suffix}2: {number2} {unit}",
(int)(position.X + size.X / numberPos),
(int)(position.Y + size.Y / 8 * 3),
textSize, textColor);

       DrawText($"{suffix}3: {number3} {unit}",
(int)(position.X + size.X / numberPos),
(int)(position.Y + size.Y / 8 * 4.5),
textSize, textColor);

       DrawText($"{suffix}4: {number4} {unit}",
(int)(position.X + size.X / numberPos),
(int)(position.Y + size.Y / 8 * 6),
textSize, textColor);
   }

   public static void DrawHoverInofsGamma(Vector2 position, Vector2 size, string number,
Raylib_cs.Color textColor, string label, string sub, float numberPos, int textSize, float labelPos)
   {
       DrawText($"{number}",
     (int)(position.X + size.X / numberPos),
     (int)(position.Y + size.Y / 8 * 1.8),
     textSize, textColor);
       // Kelvin unit
       DrawText($"{sub}",
           (int)(position.X + (size.X / 5) + MeasureText($"{number}", textSize)),
           (int)(position.Y + (size.Y / 8 * 3.5)),
           FontSizes.Mini, Raylib_cs.Color.White);

       DrawText(label,
(int)(position.X + size.X / 7 * labelPos), // /7*3
(int)(position.Y + size.Y / 8 * 5.6),
FontSizes.Info, Raylib_cs.Color.White);
   }

   public static void DrawTemperatureDisplay(Vector2 position, Vector2 size, int temperature,
       Raylib_cs.Color textColor, string label)
   {
       // Main temperature value
       DrawText($"{temperature}",
           (int)(position.X + size.X / 9),
           (int)(position.Y + size.Y / 8 * 1.7),
           FontSizes.Main, textColor);

       // Kelvin unit
       DrawText("K",
           (int)(position.X + (size.X / 7.5) + MeasureText($"{temperature}", FontSizes.Main)),
           (int)(position.Y + (size.Y / 8 * 3.2)),
           FontSizes.Mini, Raylib_cs.Color.White);

       // Temperature label (TH/TC)
       DrawText(label,
           (int)(position.X + size.X / 8 * 2),
           (int)(position.Y + size.Y / 8 * 5.7),
           FontSizes.Info, Raylib_cs.Color.White);
   }

   public static void DrawDebugInfo(Vector2 mousePosition)
   {
       DrawText($"{mousePosition}", 10, 200, 60, Raylib_cs.Color.Green);
   }

   public static void SetGridSettings(int newCellSize, Raylib_cs.Color? newMainLines = null, Raylib_cs.Color? newSubLines = null)
   {
       cellSize = newCellSize;
       if (newMainLines.HasValue) mainLines = newMainLines.Value;
       if (newSubLines.HasValue) subLines = newSubLines.Value;
   }
}