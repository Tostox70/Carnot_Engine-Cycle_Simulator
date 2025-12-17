using Raylib_cs;
using System.Numerics;
using static Raylib_cs.Raylib;

/// <summary>
/// Utility class for drawing custom UI elements and displaying simulation values.
/// </summary>
public static class Functions
{
    /// <summary>
    /// Draws a text info box that automatically sizes itself to fit the provided text.
    /// Each line of text is measured and the box is sized accordingly.
    /// </summary>
    /// <param name="text">The text to display (can include newlines).</param>
    /// <param name="fontSize">Font size for the text.</param>
    /// <param name="Pos">Position (bottom-right corner) for the info box.</param>
    public static void DrawAutoSizedInfoBox(string text, int fontSize, Vector2 Pos)
    {
        float lineSpacing = 5f;
        int padding = 10;

        // Split text into lines and find the maximum width
        string[] lines = text.Split('\n');
        int maxWidth = 0;

        foreach (string line in lines)
        {
            int width = MeasureText(line, fontSize);
            if (width > maxWidth) maxWidth = width;
        }

        // Calculate total height for all lines
        int totalHeight = (int)(lines.Length * (fontSize + lineSpacing));
        Rectangle textBox = new Rectangle(
            Pos.X - (maxWidth + padding * 2),    // Shift left by box width
            Pos.Y - (totalHeight + padding * 2), // Position above
            maxWidth + padding * 2,
            totalHeight + padding * 2
        );
        // Draw background and border
        DrawRectangleRec(textBox, Color.Gray);
        DrawRectangleLinesEx(textBox, 1, Color.White);

        // Draw each line of text inside the box
        for (int i = 0; i < lines.Length; i++)
        {
            int x = (int)textBox.X + padding;
            int y = (int)textBox.Y + padding + i * (int)(fontSize + lineSpacing);
            DrawText(lines[i], x, y, fontSize, Color.White);
        }
    }

    /// <summary>
    /// Draws the current simulation values (volume, pressure, temperature, work, efficiency)
    /// in the work calculation box area.
    /// </summary>
    public static void eValues()
    {
        int x = (int)Visual.workCalcPosX;
        int y = (int)Visual.workCalcPosY;
        int width = Program.screenWidth;

        // Display current state values in the work box
        DrawText($"Current Volume: {Points.gasVolumeNow:N4}  m^3", x+9, y+6, 20, Color.Yellow); 
        DrawText($"Current Pressure: {Points.gasPressure / 1000f:0,0.0} kPa", x+9, y+34, 20, Color.Yellow);
        DrawText($"Current Temp: {Points.gasTemp:N0} K", x+9, y+59, 20, Color.Yellow);
        DrawText($"Total Work: {Points.totWork:N0} J", x+9, y+84, 20, Color.Yellow);
        DrawText($"Carnot Efficiency: {Points.eff:N2} %", x+9, y+109, 20, Color.Yellow); // y diff of 25

        // Display author/version in the top-right corner
        DrawText("by Tostox", width/100*97, 5, 15, new Color(0, 255, 255, 255) );
    }
}
