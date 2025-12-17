using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

/// <summary>
/// Manages the drawing and data for the PV (Pressure-Volume) and TS (Temperature-Entropy) graphs.
/// </summary>
public class GraphManager
{
    // Queues to store points for the PV and TS graphs (with stage info for PV)
    private readonly Queue<(Vector2 Point, int Stage)> pvPoints = new();
    private readonly Queue<Vector2> tsPoints = new();

    // Graph dimensions and origins
    private int pvWidth, pvHeight, tsWidth, tsHeight;
    private Vector2 pvOrigin, tsOrigin;

    // Graph scaling and offset parameters
    private float pvMaxX, pvMaxY, tsMaxX, tsMaxY;
    private float pvHorOffset, pvVerOffset, tsHorOffset, tsVerOffset;

    // Static properties for graph axes, based on layout
    public static float TSxAxisL => Lines.Layout.GPorigin.X + Lines.Layout.TFxAxis;
    public static float TSxAxisR => Lines.Layout.GPorigin.X + (Lines.Layout.TFxAxis * 7);
    public static float yAxisTop => Lines.Layout.GPorigin.Y + (Lines.Layout.y2 * 2);
    public static float yAxisBot => Lines.Layout.GPorigin.Y + (Lines.Layout.y2 * 14);

    public static float PVxAxisL => Lines.Layout.GPorigin.X + (Lines.Layout.TFxAxis * 9);
    public static float PVxAxisR => Lines.Layout.GPorigin.X + (Lines.Layout.TFxAxis * 15);

    /// <summary>
    /// Converts a PV graph point to screen coordinates.
    /// </summary>
    private Vector2 ToScreenPV(Vector2 graphPoint)
    {
        float x = pvOrigin.X + (graphPoint.X / pvMaxX) * pvWidth - pvHorOffset;
        float y = pvOrigin.Y - (graphPoint.Y / pvMaxY) * pvHeight + pvVerOffset;
        return new Vector2(x, y);
    }

    /// <summary>
    /// Converts a TS graph point to screen coordinates.
    /// </summary>
    private Vector2 ToScreenTS(Vector2 graphPoint)
    {
        float x = tsOrigin.X + (graphPoint.X / tsMaxX) * tsWidth - tsHorOffset;
        float y = tsOrigin.Y - (graphPoint.Y / tsMaxY) * tsHeight + tsVerOffset;
        return new Vector2(x, y);
    }

    /// <summary>
    /// Returns a color for each stage of the PV graph.
    /// </summary>
    private Color GetStageColor(int stage) => stage switch
    {
        1 => Color.Red,
        2 => Color.Green,
        3 => Color.Blue,
        4 => Color.Green,
        _ => Color.White
    };

    /// <summary>
    /// Adds new points to the PV and TS graph queues, maintaining a maximum length.
    /// </summary>
    public void AddPoints(Vector2 pv, Vector2 ts, int stage)
    {
        if (pv.X != 0 && pv.Y != 0)
        {
            pvPoints.Enqueue((pv, stage));
            // Limit the number of points to 4 cycles worth
            if (pvPoints.Count > Points.framesPerStage * 4) pvPoints.Dequeue();
        }

        if (ts.X != 0 && ts.Y != 0)
        {
            tsPoints.Enqueue(ts);
            if (tsPoints.Count > Points.framesPerStage * 4) tsPoints.Dequeue();
        }
    }

    /// <summary>
    /// Clears all points from both PV and TS graphs.
    /// </summary>
    public void ClearPoints()
    {
        pvPoints.Clear();
        tsPoints.Clear();
    }

    /// <summary>
    /// Draws the PV and TS graphs, including axes, labels, and all points.
    /// </summary>
    public void Draw()
    {
        // --- PV Graph Scaling and Axes ---
        pvOrigin = new Vector2(PVxAxisL, yAxisBot);
        pvWidth = (int)Raymath.Vector2Distance(new Vector2(PVxAxisL, yAxisBot), new Vector2(PVxAxisR, yAxisBot));
        pvHeight = (int)Raymath.Vector2Distance(new Vector2(PVxAxisL, yAxisTop), new Vector2(PVxAxisL, yAxisBot));

        // Get PV graph parameters from Points
        float v1 = (float)Points.gasVolume1;
        float v3 = (float)Points.gasVolume3;
        float v2 = (float)Points.gasVolume2;
        float v4 = (float)Points.gasVolume4;

        float pMax = Points.n * Points.R * Points.TH / v1;
        float p2 = Points.n * Points.R * Points.TH / v2;
        float pMin = Points.n * Points.R * Points.TC / v3;
        float p4 = Points.n * Points.R * Points.TC / v4;

        pvMaxX = v3 * 1.13f;
        pvMaxY = pMax * 1.13f;
        pvHorOffset = ((v1 / pvMaxX) * pvWidth) - 50;
        pvVerOffset = ((pMin / pvMaxY) * pvHeight) - 50;

        // --- TS Graph Scaling and Axes ---
        tsOrigin = new Vector2(TSxAxisL, yAxisBot);
        tsWidth = (int)Raymath.Vector2Distance(new Vector2(TSxAxisL, yAxisBot), new Vector2(TSxAxisR, yAxisBot));
        tsHeight = (int)Raymath.Vector2Distance(new Vector2(TSxAxisL, yAxisTop), new Vector2(TSxAxisL, yAxisBot));

        // Get TS graph parameters from Points
        float entropyBase = 5;
        float maxEntropy = entropyBase + ((float)Points.QH / Points.TH);
        tsMaxX = maxEntropy * 1.13f;
        tsMaxY = Points.TH * 1.3f;

        tsHorOffset = ((entropyBase / tsMaxX) * tsWidth) - 50;
        tsVerOffset = ((Points.TC / tsMaxY) * tsHeight) - 50;

        // --- Draw PV Axes, Ticks, and Labels ---
        DrawLineEx(new Vector2(PVxAxisL, yAxisTop), new Vector2(PVxAxisL, yAxisBot), 10, Color.White); // PV Y
        DrawLineEx(new Vector2(PVxAxisL, yAxisBot), new Vector2(PVxAxisR, yAxisBot), 10, Color.White); // PV X
        DrawCircleV(new Vector2(PVxAxisL, yAxisBot), 5, Color.White); // Center
        DrawCircleV(new Vector2(PVxAxisL, yAxisTop), 5, Color.White); // Top
        DrawCircleV(new Vector2(PVxAxisR, yAxisBot), 5, Color.White); // Right

        // Draw tick marks for volumes
        float visualXmaxPV = pvOrigin.X + (v3 / pvMaxX) * pvWidth - pvHorOffset;
        DrawLineEx(new Vector2(visualXmaxPV, yAxisBot - 10), new Vector2(visualXmaxPV, yAxisBot + 10), 3, Color.White);
        float visualXminPV = pvOrigin.X + (v1 / pvMaxX) * pvWidth - pvHorOffset;
        DrawLineEx(new Vector2(visualXminPV, yAxisBot - 10), new Vector2(visualXminPV, yAxisBot + 10), 3, Color.White);
        float visualXv2PV = pvOrigin.X + (v2 / pvMaxX) * pvWidth - pvHorOffset;
        DrawLineEx(new Vector2(visualXv2PV, yAxisBot - 10), new Vector2(visualXv2PV, yAxisBot + 10), 3, Color.White);
        float visualXv4PV = pvOrigin.X + (v4 / pvMaxX) * pvWidth - pvHorOffset;
        DrawLineEx(new Vector2(visualXv4PV, yAxisBot - 10), new Vector2(visualXv4PV, yAxisBot + 10), 3, Color.White);

        // Draw tick marks for pressures
        float visualYmaxPV = pvOrigin.Y - (pMax / pvMaxY) * pvHeight + pvVerOffset;
        DrawLineEx(new Vector2(PVxAxisL - 10, visualYmaxPV), new Vector2(PVxAxisL + 10, visualYmaxPV), 3, Color.White);
        float visualYminPV = pvOrigin.Y - (pMin / pvMaxY) * pvHeight + pvVerOffset;
        DrawLineEx(new Vector2(PVxAxisL - 10, visualYminPV), new Vector2(PVxAxisL + 10, visualYminPV), 3, Color.White);
        float visualYp2PV = pvOrigin.Y - (p2 / pvMaxY) * pvHeight + pvVerOffset;
        DrawLineEx(new Vector2(PVxAxisL - 10, visualYp2PV), new Vector2(PVxAxisL + 10, visualYp2PV), 3, Color.White);
        float visualYp4PV = pvOrigin.Y - (p4/ pvMaxY) * pvHeight + pvVerOffset;
        DrawLineEx(new Vector2(PVxAxisL - 10, visualYp4PV), new Vector2(PVxAxisL + 10, visualYp4PV), 3, Color.White);

        // Draw volume and pressure labels
        DrawText("V1", (int)(visualXminPV -7), (int)(yAxisBot + 20), 15, Color.White);
        DrawText("V2", (int)(visualXv2PV - 7), (int)(yAxisBot + 20), 15, Color.White);
        DrawText("V3", (int)(visualXmaxPV -7), (int)(yAxisBot + 20), 15, Color.White);
        DrawText("V4", (int)(visualXv4PV - 7), (int)(yAxisBot + 20), 15, Color.White);
        
        DrawText("P1", (int)(PVxAxisL - 35), (int)(visualYmaxPV - 7), 15, Color.White);
        DrawText("P2", (int)(PVxAxisL - 35), (int)(visualYp2PV - 7), 15, Color.White);
        DrawText("P3", (int)(PVxAxisL - 35), (int)(visualYminPV - 7), 15, Color.White);
        DrawText("P4", (int)(PVxAxisL - 35), (int)(visualYp4PV - 7), 15, Color.White);

        // --- Draw TS Axes, Ticks, and Labels ---
        DrawLineEx(new Vector2(TSxAxisL, yAxisTop), new Vector2(TSxAxisL, yAxisBot), 10, Color.White);   // TS Y
        DrawLineEx(new Vector2(TSxAxisL, yAxisBot), new Vector2(TSxAxisR, yAxisBot), 10, Color.White);  // TS X
        DrawCircleV(new Vector2(TSxAxisL, yAxisBot), 5, Color.White); // Center
        DrawCircleV(new Vector2(TSxAxisL, yAxisTop), 5, Color.White); // Top
        DrawCircleV(new Vector2(TSxAxisR, yAxisBot), 5, Color.White); // Right

        // Draw tick marks for temperature and entropy
        float visualYmaxTS = tsOrigin.Y - (Points.TH / tsMaxY) * tsHeight + tsVerOffset;
        DrawLineEx(new Vector2(TSxAxisL - 10, visualYmaxTS), new Vector2(TSxAxisL + 10, visualYmaxTS), 3, Color.White);
        float visualYminTS = tsOrigin.Y - (Points.TC / tsMaxY) * tsHeight + tsVerOffset;
        DrawLineEx(new Vector2(TSxAxisL - 10, visualYminTS), new Vector2(TSxAxisL + 10, visualYminTS), 3, Color.White);

        float viusalXmaxTS = tsOrigin.X + (maxEntropy / tsMaxX) * tsWidth - tsHorOffset;
        DrawLineEx(new Vector2(viusalXmaxTS, yAxisBot - 10), new Vector2(viusalXmaxTS, yAxisBot + 10), 3, Color.White);
        float visualXminTS = tsOrigin.X + (entropyBase / tsMaxX) * tsWidth - tsHorOffset;
        DrawLineEx(new Vector2(visualXminTS, yAxisBot - 10), new Vector2(visualXminTS, yAxisBot + 10), 3, Color.White);
        
        // Draw temperature and entropy labels
        DrawText("TH", (int)(TSxAxisL - 35), (int)(visualYmaxTS - 7), 15, Color.White);
        DrawText("TC", (int)(TSxAxisL - 35), (int)(visualYminTS - 7), 15, Color.White);
        DrawText("change in Entropy", (int)(visualXminTS + 10), (int)(yAxisBot + 20), 15, Color.White);

        // Draw axis labels
        DrawText("P", (int)PVxAxisL - 16, (int)yAxisTop - 50, 50, Color.White);
        DrawText("V", (int)PVxAxisR + 12, (int)yAxisBot - 20, 50, Color.White);
        DrawText("T", (int)TSxAxisL - 16, (int)yAxisTop - 50, 50, Color.White);
        DrawText("S", (int)TSxAxisR + 12, (int)yAxisBot - 20, 50, Color.White);

        // --- Draw PV and TS Points ---
        foreach (var (pt, stage) in pvPoints)
            DrawCircleV(ToScreenPV(pt), 2, GetStageColor(stage));

        foreach (var pt in tsPoints)
            DrawCircleV(ToScreenTS(pt), 2, Color.Orange);
    }

    /// <summary>
    /// Draws a tracer (highlighted point) for the current PV and TS positions.
    /// </summary>
    public void DrawTracer(Vector2 pv, Vector2 ts)
    {
        if (pv.X != 0 && pv.Y != 0)
            DrawCircleV(ToScreenPV(pv), 6, Color.White);
        if (ts.X != 0 && ts.Y != 0)
            DrawCircleV(ToScreenTS(ts), 6, Color.White);
    }
}