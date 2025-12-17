using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

/// <summary>
/// Provides static methods and layout data for drawing the grid, layout lines, and UI elements in the simulation.
/// </summary>
public static class Lines
{
    // Main and sub grid line colors
    private static Color mainLines = new Color(255, 255, 255, 60);
    private static Color subLines = new Color(255, 255, 255, 30);
    // Size of each grid cell
    private static int cellSize = 50;

    /// <summary>
    /// Stores layout-related coordinates and sizes for the UI and graph.
    /// </summary>
    public static class Layout
    {
        // Key points for layout lines and graph blocks
        public static Vector2 THlineTop;
        public static Vector2 line2Top;
        public static Vector2 line2L;
        public static Vector2 line2R;
        public static Vector2 THlineBot;
        public static float graphBlockSizeX;
        public static float TopInfoSizeX;
        public static float BotInfoSizeX;

        // Additional layout and graph parameters
        public static float xBlockGraph;
        public static Vector2 GPorigin;
        public static Vector2 line1Top;
        public static float graphBlockSizeY;
        public static float y2;
        public static float TFxAxis;
        // Parts layout
        public static Vector2 line1L;
        public static Vector2 line1R;
        public static float partsXdimension;
        public static float partsYdimension;
    }

    /// <summary>
    /// Font sizes for different UI elements.
    /// </summary>
    public static class FontSizes
    {
        public const int Main = 90;
        public const int Info = 50;
        public const int Mini = 30;
    }

    /// <summary>
    /// Draws the main and sub grid lines on the background.
    /// </summary>
    public static void DrawGrid(int screenWidth, int screenHeight)
    {
        // Main grid lines (vertical and horizontal)
        for (int x = 0; x <= screenWidth; x += cellSize)
            DrawLine(x, 0, x, screenHeight, mainLines);

        for (int y = 0; y <= screenHeight; y += cellSize)
            DrawLine(0, y, screenWidth, y, mainLines);

        // Sub grid lines (offset by half a cell)
        for (int subX = cellSize / 2; subX <= screenWidth; subX += cellSize)
            DrawLine(subX, 0, subX, screenHeight, subLines);

        for (int subY = cellSize / 2; subY <= screenHeight; subY += cellSize)
            DrawLine(0, subY, screenWidth, subY, subLines);
    }

    /// <summary>
    /// Updates the layout coordinates and sizes based on the current window size.
    /// </summary>
    public static void UpdateLayoutDimensions(int screenWidth, int screenHeight)
    {
        // Calculate key layout points for lines and graph blocks
        Layout.line2Top = new Vector2(screenWidth * 0.3f + 10, 0);
        Layout.line2L = new Vector2(Layout.line2Top.X, screenHeight * 0.6f);
        Layout.line2R = new Vector2(screenWidth, screenHeight * 0.6f);
        Layout.THlineTop = new Vector2(screenWidth * 0.4527f, screenHeight * 0.6f);
        Layout.THlineBot = new Vector2(screenWidth * 0.4527f, screenHeight * 0.8f);
        Layout.graphBlockSizeX = Raymath.Vector2Distance(Layout.line2L, Layout.line2R);
        Layout.graphBlockSizeY = Raymath.Vector2Distance(Layout.line2Top, Layout.line2L);

        // Info box sizes for top and bottom UI
        Layout.TopInfoSizeX = Layout.graphBlockSizeX / 4;
        Layout.BotInfoSizeX = Layout.graphBlockSizeX / 4;
        Layout.xBlockGraph = Layout.graphBlockSizeX;
        Layout.GPorigin = Layout.line1Top;
        Layout.y2 = Layout.graphBlockSizeY / 16;
        Layout.TFxAxis = Layout.xBlockGraph / 16;

        // Dimensions for parts drawing
        Layout.partsXdimension = Raymath.Vector2Distance(Layout.line1L, Layout.line1R);
        Layout.partsYdimension = Raymath.Vector2Distance(Layout.line1R, new Vector2(Layout.line1R.X, screenHeight));
    }

    /// <summary>
    /// Draws the main layout lines that divide the window into sections for graphs and UI.
    /// </summary>
    public static void DrawLayoutLines(int screenWidth, int screenHeight)
    {
        // Update layout before drawing
        UpdateLayoutDimensions(screenWidth, screenHeight);

        // Calculate key points for layout lines
        Layout.line1Top = new Vector2(screenWidth * 0.3f + 10, 0);
        Vector2 line1Bot = new Vector2(screenWidth * 0.3f + 10, screenHeight);

        // Stage line (horizontal)
        Layout.line1L = new Vector2(0, screenHeight * 0.08f);
        Layout.line1R = new Vector2(Layout.line1Top.X, screenHeight * 0.08f);

        // Info line (horizontal, bottom)
        Vector2 line3L = new Vector2(Layout.line2Top.X, screenHeight * 0.8f);
        Vector2 line3R = new Vector2(screenWidth, screenHeight * 0.8f);

        // Draw all main layout lines
        DrawLineEx(Layout.line1Top, line1Bot, 2, Color.White);
        DrawLineEx(Layout.line1L, Layout.line1R, 2, Color.White);
        DrawLineEx(Layout.line2L, Layout.line2R, 5, Color.White);
        DrawLineEx(line3L, line3R, 5, Color.White);
        //DrawLineEx(Layout.THlineTop, Layout.THlineBot, 2, Color.White); // (optional) vertical line for TH
    }

    /// <summary>
    /// Returns the position and size for a top info/hover box, based on its slot index.
    /// </summary>
    public static (Vector2 position, Vector2 size) TopHoverPositionSize(int slot = 0)
    {
        var size = new Vector2(Layout.TopInfoSizeX, Layout.THlineBot.Y - Layout.THlineTop.Y);

        // Calculate X position for the slot
        float baseX = Layout.line2Top.X + slot * Layout.TopInfoSizeX;
        var position = new Vector2(baseX, Layout.line2L.Y);

        return (position, size);
    }

    /// <summary>
    /// Returns the position and size for a bottom info/hover box, based on its slot index.
    /// </summary>
    public static (Vector2 position, Vector2 size) BotHoverPositionSize(int slot = 0)
    {
        var size = new Vector2(Layout.BotInfoSizeX, Layout.THlineBot.Y - Layout.THlineTop.Y);

        // Calculate X position for the slot
        float baseX = Layout.line2Top.X + slot * Layout.BotInfoSizeX;
        var position = new Vector2(baseX, Layout.THlineBot.Y);

        return (position, size);
    }

    /// <summary>
    /// Draws a hover info box with a single value and label.
    /// </summary>
    public static void DrawHoverInofs(Vector2 position, Vector2 size, float number,
        Color textColor, string label, string sub, float numberPos, int textSize, float labelPos)
    {
        // Draw the main value
        DrawText($"{number}",
            (int)(position.X + size.X / numberPos),
            (int)(position.Y + size.Y / 8 * 1.5),
            textSize, textColor);

        // Draw the unit/sub-label
        DrawText($"{sub}",
            (int)(position.X + (size.X / 5) + MeasureText($"{number}", textSize)),
            (int)(position.Y + (size.Y / 8 * 3.5)),
            FontSizes.Mini, Color.White);

        // Draw the main label
        DrawText(label,
            (int)(position.X + size.X / 7 * labelPos),
            (int)(position.Y + size.Y / 8 * 5),
            FontSizes.Info, Color.White);
    }

    /// <summary>
    /// Draws a hover info box with four values (e.g., for P1-P4 or V1-V4).
    /// </summary>
    public static void DrawHoverInofsPlus(Vector2 position, Vector2 size, float number1,
        float number2, float number3, float number4, Color textColor, float numberPos, int textSize, string suffix, string unit)
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

    /// <summary>
    /// Draws a hover info box for gamma (heat capacity ratio) with a label.
    /// </summary>
    public static void DrawHoverInofsGamma(Vector2 position, Vector2 size, string number,
        Color textColor, string label, string sub, float numberPos, int textSize, float labelPos)
    {
        DrawText($"{number}",
            (int)(position.X + size.X / numberPos),
            (int)(position.Y + size.Y / 8 * 1.5),
            textSize, textColor);

        DrawText($"{sub}",
            (int)(position.X + (size.X / 5) + MeasureText($"{number}", textSize)),
            (int)(position.Y + (size.Y / 8 * 3.5)),
            FontSizes.Mini, Color.White);

        DrawText(label,
            (int)(position.X + size.X / 7 * labelPos),
            (int)(position.Y + size.Y / 8 * 5),
            FontSizes.Info, Color.White);
    }

    /// <summary>
    /// Draws a temperature display box with a large value and label (e.g., for TH or TC).
    /// </summary>
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

    /// <summary>
    /// Draws debug information (e.g., mouse position) on the screen.
    /// </summary>
    public static void DrawDebugInfo(Vector2 mousePosition)
    {
        DrawText($"{mousePosition}", 10, 200, 60, Color.Green);
    }

    /// <summary>
    /// Sets the grid cell size and optionally updates the grid line colors.
    /// </summary>
    public static void SetGridSettings(int newCellSize, Color? newMainLines = null, Color? newSubLines = null)
    {
        cellSize = newCellSize;
        if (newMainLines.HasValue) mainLines = newMainLines.Value;
        if (newSubLines.HasValue) subLines = newSubLines.Value;
    }
}