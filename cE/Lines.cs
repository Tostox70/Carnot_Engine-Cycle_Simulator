using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

public static class Lines
{
    private static Color mainLines = new Color(255, 255, 255, 60);
    private static Color subLines = new Color(255, 255, 255, 25);
    private static int cellSize = 100;

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
        //ADDED
        public static float xBlockGraph;
        public static Vector2 GPorigin;
        public static Vector2 line1Top;
        public static float graphBlockSizeY;
        public static float y2;
        public static float TFxAxis;

    }

    // Font sizes
    public static class FontSizes
    {
        public const int Main = 100;
        public const int Info = 50;
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


        Layout.TopInfoSizeX = Layout.graphBlockSizeX / 4;
        Layout.xBlockGraph = Layout.graphBlockSizeX;
        Layout.GPorigin = Layout.line1Top;
        Layout.y2 = Layout.graphBlockSizeY / 16;
        Layout.TFxAxis = Layout.xBlockGraph / 16;
    }

    public static void DrawLayoutLines(int screenWidth, int screenHeight)
    {
        UpdateLayoutDimensions(screenWidth, screenHeight);

        // Dividing Left & right
        Layout.line1Top = new Vector2(screenWidth * 0.3f + 10, 0);
        Vector2 line1Bot = new Vector2(screenWidth * 0.3f + 10, screenHeight);

        // STAGE LINE
        Vector2 line1L = new Vector2(0, screenHeight * 0.08f);
        Vector2 line1R = new Vector2(Layout.line1Top.X, screenHeight * 0.08f);

        // INFO LINE
        Vector2 line3L = new Vector2(Layout.line2Top.X, screenHeight * 0.8f);
        Vector2 line3R = new Vector2(screenWidth, screenHeight * 0.8f);

        // Draw all lines
        DrawLineEx(Layout.line1Top, line1Bot, 2, Color.White);
        DrawLineEx(line1L, line1R, 2, Color.White);
        DrawLineEx(Layout.line2L, Layout.line2R, 5, Color.White);
        DrawLineEx(line3L, line3R, 5, Color.White);
        //DrawLineEx(Layout.THlineTop, Layout.THlineBot, 2, Color.White);
    }

    public static (Vector2 position, Vector2 size) GetTemperatureRectangle(bool isCold = false) // WHAT DOES IT DO????
    {
        var size = new Vector2(Layout.TopInfoSizeX, Layout.THlineBot.Y - Layout.THlineTop.Y);
        var baseX = Layout.line2Top.X;
        if (isCold) baseX += Layout.TopInfoSizeX;
        
        var position = new Vector2(baseX, Layout.line2L.Y);
        return (position, size);
    }

    public static void DrawTemperatureDisplay(Vector2 position, Vector2 size, int temperature, 
        Color textColor, string label)
    {
        // Main temperature value
        DrawText($"{temperature}", 
            (int)(position.X + size.X / 6), 
            (int)(position.Y + size.Y / 8 * 1.5), 
            FontSizes.Main, textColor);
            
        // Kelvin unit
        DrawText("K", 
            (int)(position.X + (size.X / 5) + MeasureText($"{temperature}", FontSizes.Main)), 
            (int)(position.Y + (size.Y / 8 * 3.5)), 
            FontSizes.Mini, Color.White);
            
        // Temperature label (TH/TC)
        DrawText(label, 
            (int)(position.X + size.X / 8 * 3), 
            (int)(position.Y + size.Y / 8 * 5), 
            FontSizes.Info, Color.White);
    }

    public static void DrawDebugInfo(Vector2 mousePosition)
    {
        DrawText($"{mousePosition}", 10, 200, 60, Color.Green);
    }

    public static void SetGridSettings(int newCellSize, Color? newMainLines = null, Color? newSubLines = null)
    {
        cellSize = newCellSize;
        if (newMainLines.HasValue) mainLines = newMainLines.Value;
        if (newSubLines.HasValue) subLines = newSubLines.Value;
    }
}