
﻿using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

public class Program
{
    // Target frames per second for the simulation
    private const int TARGET_FPS = 60;
    // Initial window dimensions
    public static int screenWidth = 1350;
    public static int screenHeight = 900; 

    static void Main()
    {
        //SetConfigFlags(ConfigFlags.ResizableWindow);
        
        // Initialize Raylib window
        InitWindow(screenWidth, screenHeight, "cE Simulator v1.3");
        SetTargetFPS(TARGET_FPS);
        Points.SetFrameRate(TARGET_FPS);

        // Load and set the window icon
        Image Logo = LoadImage("assets/logo.png");
        SetWindowIcon(Logo);

         // Initialize layout and graph lines
        Lines.UpdateLayoutDimensions(screenWidth, screenHeight);
        Lines.DrawLayoutLines(screenWidth, screenHeight);
        var Graph = new GraphManager();

        // Variables for graph points and stage
        Vector2 PVpointGraph;
        Vector2 TSpointGraph;
        int currentStage;

        // Initialize temperature values
        int TH = Points.TH;
        int TC = Points.TC;
        float ratio = Points.ratio;
        int n = (int)Points.n;
        float gamma = Points.gamma; // 5/3 monoatomic, 7/5 diatomic, 4/3 triatomic gases
        float Wg = (float)Points.workByGas;
        float Ws = (float)Points.workBySurr * -1;

        float v1, v2, v3, v4;
        float pMax, p2, pMin, p4;

        // Initialize hover controls
        var hoverTH = new Hover(Vector2.Zero, Vector2.Zero, TH, 101, 999);
        var hoverTC = new Hover(Vector2.Zero, Vector2.Zero, TC, 100, 998);
        var hoverN = new Hover(Vector2.Zero, Vector2.Zero, n, 1, 4);
        var hoverRatio = new Hover(Vector2.Zero, Vector2.Zero, ratio, 1.4f, 3.1f);
        var hoverGamma = new Hover(Vector2.Zero, Vector2.Zero, gamma, 1f, 3f);
        var hoverWg = new Hover(Vector2.Zero, Vector2.Zero, Wg, 0f, 0f);
        var hoverWs = new Hover(Vector2.Zero, Vector2.Zero, Ws, 0f, 0f);
        var hoverV = new Hover(Vector2.Zero, Vector2.Zero, 0, 0f, 0f);
        var hoverP = new Hover(Vector2.Zero, Vector2.Zero, 0, 0f, 0f);

        // Add these before the main loop
        int prevTH = TH;
        int prevTC = TC;
        float prevRatio = ratio;
        int prevN = n;
        float prevGamma = gamma;

        bool thChanged;
        bool tcChanged;

        Visual visual = new Visual();
        bool isPaused = false;
        float cycleSeconds = 2f;
        Points.SetTimePerStage(cycleSeconds);

        while (!WindowShouldClose())
        {
            if (Functions.TryGetNumberKeyAsFloat(Graph, out float newSeconds))
            {
                cycleSeconds = newSeconds;
                Points.SetTimePerStage(cycleSeconds);
            }

            visual.UpdateVisualCalc();

            // Update screen dimensions
            screenWidth = GetScreenWidth();
            screenHeight = GetScreenHeight();

            // Get temperature rectangle positions and sizes
            var (thPosition, thSize) = Lines.TopHoverPositionSize(0);
            var (tcPosition, tcSize) = Lines.TopHoverPositionSize(1);
            var (ratioPosition, ratioSize) = Lines.TopHoverPositionSize(2);
            var (nPosition, nSize) = Lines.TopHoverPositionSize(3);
            var (gammaPosition, gammaSize) = Lines.TopHoverPositionSize(4);
            var (wgPosition, wgSize) = Lines.BotHoverPositionSize(0);
            var (wsPosition, wsSize) = Lines.BotHoverPositionSize(1);
            var (vPosition, vSize) = Lines.BotHoverPositionSize(3);
            var (pPosition, pSize) = Lines.BotHoverPositionSize(2);

            // Update hover controls
            hoverTH.UpdatePosition(thPosition, thSize);
            hoverTH.Update();
            TH = (int)hoverTH.GetCount();

            hoverTC.UpdatePosition(tcPosition, tcSize);
            hoverTC.Update();
            TC = (int)hoverTC.GetCount();

            hoverRatio.UpdatePosition(ratioPosition, ratioSize);
            hoverRatio.flexUpdate(0.1f);
            ratio = (float)hoverRatio.GetCount();

            hoverN.UpdatePosition(nPosition, nSize);
            hoverN.flexUpdate(1f);
            n = (int)hoverN.GetCount();

            hoverGamma.UpdatePosition(gammaPosition, gammaSize);
            hoverGamma.OptionUpdate();
            gamma = hoverGamma.GetCount();


            hoverWg.UpdatePosition(wgPosition, wgSize);
            hoverWg.DisplayUpdate();

            hoverWs.UpdatePosition(wsPosition, wsSize);
            hoverWs.DisplayUpdate();

            hoverV.UpdatePosition(vPosition, vSize);
            hoverV.DisplayUpdate();

            hoverP.UpdatePosition(pPosition, pSize);
            hoverP.DisplayUpdate();

            //TEMP BUTTON LOGIC
            thChanged = TH != prevTH;
            tcChanged = TC != prevTC;

            if (thChanged && TH <= TC)
            {
                TH = TC + 1;
                hoverTH.SetCount(TH);
            }
            else if (tcChanged && TC >= TH)
            {
                TC = TH - 1;
                hoverTC.SetCount(TC);
            }

            // Detect temperature change and clear points
            if (TH != prevTH || TC != prevTC || n != prevN || gamma != prevGamma || ratio != prevRatio)
            {
                Points.SetNewPointsValues(TH, TC, ratio, n, gamma);
                Graph.ClearPoints();
                prevTH = TH;
                prevTC = TC;
                prevRatio = ratio;
                prevN = n;
                prevGamma = gamma;
            }

            Wg = (float)Points.workByGas;
            Ws = (float)Points.workBySurr * -1f;

            v1 = (float)Points.gasVolume1;
            v3 = (float)Points.gasVolume3;
            v2 = (float)Points.gasVolume2;
            v4 = (float)Points.gasVolume4;

            pMax = Points.n * Points.R * Points.TH / v1;
            p2 = Points.n * Points.R * Points.TH / v2;
            pMin = Points.n * Points.R * Points.TC / v3;
            p4 = Points.n * Points.R * Points.TC / v4;

            var pointData = Points.Point();
            PVpointGraph = pointData.Item1;
            TSpointGraph = pointData.Item2;
            currentStage = Points.GetCurrentStage();
            Graph.AddPoints(PVpointGraph, TSpointGraph, currentStage);

            if (IsKeyPressed(KeyboardKey.Space)) // or your button callback
            {
                isPaused = !isPaused;
                Points.SetPaused(isPaused);
            }

            BeginDrawing();
            ClearBackground(Color.Black);

            visual.UpdateVisual();

            Lines.DrawGrid(screenWidth, screenHeight);
            Lines.DrawLayoutLines(screenWidth, screenHeight);

            Lines.DrawHoverInofsPlus(pPosition, pSize, pMax, p2, (float)Math.Round(pMin, 0), (float)Math.Round(p4, 0), hoverV.txtColor, 15, 20, "P", "Pa");
            hoverP.Draw("Pressures at\ndifferent stages");

            Lines.DrawHoverInofsPlus(vPosition, vSize, (float)Math.Round(v1, 4), (float)Math.Round(v2, 5), (float)Math.Round(v3, 6), (float)Math.Round(v4, 5), hoverV.txtColor, 15, 20, "V", "m^3"); //pos, size, number1, 2, 3, 4, color, numberPos, textSize, suffix, unit
            hoverV.Draw("Volumes at\ndifferent stages");

            Lines.DrawHoverInofs(wsPosition, wsSize, $"{MathF.Round(Ws, 1)}", hoverWs.txtColor, "Ws", "", 6.5f, 2.2f, 50, 2.5f, 5);// "", numberPosX, numberPosY, textSize, labelPosX, labelPosY
            hoverWs.Draw("Work done\nby surrounding\nin Jouls");

            Lines.DrawHoverInofs(wgPosition, wgSize, $"{MathF.Round(Wg, 1)}", hoverWg.txtColor, "Wg", "", 6.5f, 2.2f, 50, 2.5f, 5);
            hoverWg.Draw("Work done\nby gas in Jouls");

            Lines.DrawHoverInofsGamma(gammaPosition, gammaSize, hoverGamma.GammaLabel, hoverGamma.txtColor, "gamma", "", 6, 70, 0.8f);
            hoverGamma.Draw("gas heat\ncapacity ratio ");

            Lines.DrawHoverInofs(nPosition, nSize, n.ToString(), hoverN.txtColor, "n", "", 2.5f, 1.6f, 100, 3, 5.7f);// for th/tc is 6
            hoverN.Draw("# of moles\nin gas");

            Lines.DrawHoverInofs(ratioPosition, ratioSize, $"{MathF.Round(ratio, 1):0.0}", hoverRatio.txtColor, "ratio", "", 5.5f, 1.6f, 90, 1.3f, 5.7f);
            hoverRatio.Draw("how much v2\nbased on v1");

            Lines.DrawTemperatureDisplay(tcPosition, tcSize, TC, hoverTC.txtColor, "TC");
            hoverTC.Draw("Cold reservoir's\ntemperature in\nKelvin");

            Lines.DrawTemperatureDisplay(thPosition, thSize, TH, hoverTH.txtColor, "TH");
            hoverTH.Draw("Hot reservoir's\ntemperature in\nKelvin");

            DrawText($"Cycle duration: {cycleSeconds * 4}s", screenWidth / 100 * 45, 5, 20, new Color(255, 255, 255, 110));
            DrawText($"Calculations per cycle: {Points.framesPerStage * 4}", screenWidth / 100 * 43, 25, 20, new Color(255, 255, 255, 110)); 

            Graph.Draw();
            Graph.DrawTracer(PVpointGraph, TSpointGraph);
            Functions.eValues();

            EndDrawing();
        }

        CloseWindow();
    }
}
