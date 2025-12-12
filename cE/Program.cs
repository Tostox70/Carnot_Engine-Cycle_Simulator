using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

public class Program
{
    // Configuration constants
    private const int TARGET_FPS = 60;
    public static int screenWidth = 2200;
    public static int screenHeight = 1350;
    static void Main()
    {
        // Initialize window
        SetConfigFlags(ConfigFlags.ResizableWindow);
        InitWindow(screenWidth, screenHeight, "");
        SetTargetFPS(TARGET_FPS);

        Lines.UpdateLayoutDimensions(screenWidth, screenHeight);
        Lines.DrawLayoutLines(screenWidth, screenHeight);
        var Graph = new GraphManager();

        Vector2 PVpointGraph;
        Vector2 TSpointGraph;
        int currentStage;

        // Initialize temperature values
        int TH = Points.TH;
        int TC = Points.TC;

        // Initialize hover controls
        var hoverTH = new Hover(Vector2.Zero, Vector2.Zero, TH, 100, 1000);
        var hoverTC = new Hover(Vector2.Zero, Vector2.Zero, TC, 100, 1000);

        Points.SetTimePerStage(2.0f);
        Points.SetFrameRate(TARGET_FPS);

        // Main game loop
        while (!WindowShouldClose())
        {
            // Update screen dimensions
            screenWidth = GetScreenWidth();
            screenHeight = GetScreenHeight();
            Vector2 mousePosition = GetMousePosition();

            // Get temperature rectangle positions and sizes
            var (thPosition, thSize) = Lines.GetTemperatureRectangle(isCold: false);
            var (tcPosition, tcSize) = Lines.GetTemperatureRectangle(isCold: true);

            // Update hover controls
            hoverTH.UpdatePosition(thPosition, thSize);
            hoverTH.Update();
            TH = hoverTH.GetCount();

            hoverTC.UpdatePosition(tcPosition, tcSize);
            hoverTC.Update();
            TC = hoverTC.GetCount();

            Points.SetTemperatures(TH, TC); // <-- Add this line

            var pointData = Points.Point();
            PVpointGraph = pointData.Item1;
            TSpointGraph = pointData.Item2;
            currentStage = Points.GetCurrentStage();
            Graph.AddPoints(PVpointGraph, TSpointGraph, currentStage);


            // Begin drawing
            BeginDrawing();
            ClearBackground(Color.Black);

            Lines.DrawGrid(screenWidth, screenHeight);
            Lines.DrawLayoutLines(screenWidth, screenHeight);

            Lines.DrawTemperatureDisplay(tcPosition, tcSize, TC, hoverTC.txtColor, "TC");
            hoverTC.Draw("Cold reservoir's\ntemperature in\nKelvin");

            Lines.DrawTemperatureDisplay(thPosition, thSize, TH, hoverTH.txtColor, "TH");
            hoverTH.Draw("Hot reservoir's\ntemperature in\nKelvin");

            Graph.Draw();
            Graph.DrawTracer(PVpointGraph, TSpointGraph);

            // Draw debug info
            Lines.DrawDebugInfo(mousePosition);
            EndDrawing();
        }

        CloseWindow();
    }
}