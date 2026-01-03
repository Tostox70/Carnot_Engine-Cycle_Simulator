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
        int x = (int)Visual.BOXpos.X;
        int y = (int)Visual.BOXpos.Y;
        int screenWidth = Program.screenWidth;
        float verticalSubdivision = Visual.BOXsize.Y / 6;
        float horizontalSubdivision = Visual.BOXsize.X / 30;
        float boxWidth = Visual.BOXsize.X;
        int fontSize = (int)Math.Clamp(boxWidth * 0.06f, 12f, 48f);
        Vector2 textSize = MeasureTextEx(GetFontDefault(), $"Current Pressure: {Points.gasPressure / 1000f:0,0.0} kPa", fontSize, 0);

        DrawText($"Current Volume: {Points.gasVolumeNow:N5}  m^3", x + (int)horizontalSubdivision, y + (int)verticalSubdivision - (int)textSize.Y / 2, fontSize, Color.Yellow);
        DrawText($"Current Pressure: {Points.gasPressure / 1000f:0,0.0} kPa", x + (int)horizontalSubdivision, y + (int)verticalSubdivision * 2 - (int)textSize.Y / 2, fontSize, Color.Yellow);
        DrawText($"Current Temp: {Points.gasTemp:N0} K", x + (int)horizontalSubdivision, y + (int)verticalSubdivision * 3 - (int)textSize.Y / 2, fontSize, Color.Yellow);
        DrawText($"Total Work: {Math.Round(Points.totWork, 1)} J", x + (int)horizontalSubdivision, y + (int)verticalSubdivision * 4 - (int)textSize.Y / 2, fontSize, Color.Yellow);
        DrawText($"Carnot Efficiency: {Points.eff:N2} %", x + (int)horizontalSubdivision, y + (int)verticalSubdivision * 5 - (int)textSize.Y / 2, fontSize, Color.Yellow);

        DrawText("by Tostox", screenWidth / 100 * 95, 5, 15, new Color(0, 255, 255, 255));
    }

    public static Color gasTempColor(float currentTemp, float temp1, float temp2)
    {
        // Normalize temperature to 0-1 range between temp1 and temp2
        float t = (currentTemp - temp1) / (temp2 - temp1);

        // Clamp t to 0-1 range to handle temperatures outside the range
        t = Math.Max(0, Math.Min(1, t));

        // Interpolate between red (temp1) and blue (temp2)
        byte red = (byte)(255 * (1 - t));   // Red decreases as temp increases
        byte blue = (byte)(255 * t);        // Blue increases as temp increases
        byte green = 0;                     // Keep green at 0

        return new Color(red, green, blue, (byte)85);
    }

    public static bool TryGetNumberKeyAsFloat(GraphManager graph, out float seconds)
    {
        int screenWidth = Program.screenWidth;

        seconds = 0f;
        int key = GetKeyPressed();
        switch (key)
        {
            case (int)KeyboardKey.One:
                seconds = 0.5f;
                break;
            case (int)KeyboardKey.Two:
                seconds = 2f;
                break;
            case (int)KeyboardKey.Three:
                seconds = 5f;
                break;
            default:
                return false;
        }

        graph.ClearPoints();
        Points.Reset();
        return true;
    }
}
