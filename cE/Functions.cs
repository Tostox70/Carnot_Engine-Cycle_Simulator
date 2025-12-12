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
        Rectangle textBox = new Rectangle(Pos.X, Pos.Y - totalHeight - padding * 2, maxWidth + padding * 2, totalHeight + padding * 2);

        DrawRectangleRec(textBox, Color.Gray);
        DrawRectangleLinesEx(textBox, 1, Color.White);

        for (int i = 0; i < lines.Length; i++)
        {
            int x = (int)textBox.X + padding;
            int y = (int)textBox.Y + padding + i * (int)(fontSize + lineSpacing);
            DrawText(lines[i], x, y, fontSize, Color.White);
        }
    }
}
