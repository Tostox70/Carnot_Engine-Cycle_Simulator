using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

public class GraphManager
{
    private readonly Queue<(Vector2 Point, int Stage)> pvPoints = new();
    private readonly Queue<Vector2> pvPath = new();
    private readonly Queue<Vector2> tsPoints = new();
    private readonly Queue<Vector2> tsPath = new();

    private int pvWidth, pvHeight, tsWidth, tsHeight;
    private Vector2 pvOrigin, tsOrigin;

    private float pvMaxX, pvMaxY, tsMaxX, tsMaxY;
    private float pvHorOffset, pvVerOffset, tsHorOffset, tsVerOffset;

    //GRAPH AXIS
    public static float TSxAxisL => Lines.Layout.GPorigin.X + Lines.Layout.TFxAxis;
    public static float TSxAxisR => Lines.Layout.GPorigin.X + (Lines.Layout.TFxAxis * 7);
    public static float yAxisTop => Lines.Layout.GPorigin.Y + (Lines.Layout.y2 * 2);
    public static float yAxisBot => Lines.Layout.GPorigin.Y + (Lines.Layout.y2 * 14);

    public static float PVxAxisL => Lines.Layout.GPorigin.X + (Lines.Layout.TFxAxis * 9); 
    public static float PVxAxisR => Lines.Layout.GPorigin.X + (Lines.Layout.TFxAxis * 15);


    private Vector2 ToScreenPV(Vector2 graphPoint)
    {
        float x = pvOrigin.X + (graphPoint.X / pvMaxX) * pvWidth - pvHorOffset;
        float y = pvOrigin.Y - (graphPoint.Y / pvMaxY) * pvHeight + pvVerOffset;
        return new Vector2(x, y);
    }

    private Vector2 ToScreenTS(Vector2 graphPoint)
    {
        float x = tsOrigin.X + (graphPoint.X / tsMaxX) * tsWidth - tsHorOffset;
        float y = tsOrigin.Y - (graphPoint.Y / tsMaxY) * tsHeight + tsVerOffset;
        return new Vector2(x, y);
    }

    private Color GetStageColor(int stage) => stage switch
    {
        1 => Color.Red,
        2 => Color.Blue,
        3 => Color.Green,
        4 => Color.Yellow,
        _ => Color.White
    };

    public void AddPoints(Vector2 pv, Vector2 ts, int stage)
    {
        if (pv.X != 0 && pv.Y != 0)
        {
            pvPoints.Enqueue((pv, stage));
            pvPath.Enqueue(pv);
            if (Points.isComplete) pvPath.Dequeue();
            if (pvPoints.Count > Points.framesPerStage * 2) pvPoints.Dequeue();
        }

        if (ts.X != 0 && ts.Y != 0)
        {
            tsPoints.Enqueue(ts);
            tsPath.Enqueue(ts);
            if (Points.isComplete) tsPath.Dequeue();
            if (tsPoints.Count > Points.framesPerStage * 2) tsPoints.Dequeue();
        }
    }


    public void Draw()
    {
        // PV SCALE
        pvOrigin = new Vector2(PVxAxisL, yAxisBot);
        pvWidth = (int) Raymath.Vector2Distance(new Vector2 (PVxAxisL, yAxisBot), new Vector2 (PVxAxisR, yAxisBot));
        pvHeight = (int) Raymath.Vector2Distance(new Vector2 (PVxAxisL, yAxisTop), new Vector2 (PVxAxisL, yAxisBot));
        // PV graph parameters
        float v1 = (float)Points.gasVolume1;
        float v3 = (float)Points.gasVolume3;
        float pMax = Points.n * Points.R * Points.TH / v1;
        float pMin = Points.n * Points.R * Points.TC / v3;
        pvMaxX = v3 * 1.13f;
        pvMaxY = pMax * 1.13f;
        pvHorOffset = ((v1 / pvMaxX) * pvWidth) - 50;
        pvVerOffset = ((pMin / pvMaxY) * pvHeight) - 50;

        //TS SCALE
        tsOrigin = new Vector2(TSxAxisL, yAxisBot);
        tsWidth = (int) Raymath.Vector2Distance(new Vector2 (TSxAxisL, yAxisBot), new Vector2 (TSxAxisR, yAxisBot));
        tsHeight = (int) Raymath.Vector2Distance(new Vector2 (TSxAxisL, yAxisTop), new Vector2 (TSxAxisL, yAxisBot));
        // TS graph parameters
        float entropyBase = 5;
        float maxEntropy = entropyBase + ((float)Points.QH / Points.TH);
        tsMaxX = maxEntropy * 1.13f;
        tsMaxY = Points.TH * 1.3f;   

        tsHorOffset = ((entropyBase / tsMaxX) * tsWidth) - 50;
        tsVerOffset = ((Points.TC / tsMaxY) * tsHeight) - 50; 


        // Axes
        DrawLineEx(new Vector2(PVxAxisL, yAxisTop), new Vector2(PVxAxisL, yAxisBot), 10, Color.White); // PV Y
        DrawLineEx(new Vector2(PVxAxisL, yAxisBot), new Vector2(PVxAxisR, yAxisBot), 10, Color.White); // PV X
        DrawCircleV(new Vector2(PVxAxisL, yAxisBot), 5, Color.White); //PV
        

        DrawLineEx(new Vector2(TSxAxisL, yAxisTop), new Vector2(TSxAxisL, yAxisBot), 10, Color.White);   // TS Y
        DrawLineEx(new Vector2(TSxAxisL, yAxisBot), new Vector2(TSxAxisR, yAxisBot), 10, Color.White);  // TS X
        DrawCircleV(new Vector2(TSxAxisL, yAxisBot), 5, Color.White); //TS        

        //TEXT DONT FOLLOW GRAPHs!!!

        DrawText("P", (int)PVxAxisL-16, (int)yAxisTop  -50 , 50, Color.White);
        DrawText("V", (int)PVxAxisR + 12, (int)yAxisBot -20, 50, Color.White);
        DrawText("T", (int)TSxAxisL-16, (int)yAxisTop  -50, 50, Color.White);
        DrawText("S", (int)TSxAxisR + 12, (int)yAxisBot -20, 50, Color.White);

        // PV dots and path
        foreach (var (pt, stage) in pvPoints)
            DrawCircleV(ToScreenPV(pt), 2, GetStageColor(stage));

        Vector2? prevPV = null;
        foreach (var pt in pvPath)
        {
            var screen = ToScreenPV(pt);
            if (prevPV.HasValue)
                DrawLineV(ToScreenPV(prevPV.Value), screen, new Color(255, 255, 255, 150));
            prevPV = pt;
        }

        // TS dots and path
        foreach (var pt in tsPoints)
            DrawCircleV(ToScreenTS(pt), 2, Color.Orange);

        Vector2? prevTS = null;
        foreach (var pt in tsPath)
        {
            var screen = ToScreenTS(pt);
            if (prevTS.HasValue)
                DrawLineV(ToScreenTS(prevTS.Value), screen, new Color(255, 255, 255, 150));
            prevTS = pt;
        }
    }

    public void DrawTracer(Vector2 pv, Vector2 ts)
    {
        if (pv.X != 0 && pv.Y != 0)
            DrawCircleV(ToScreenPV(pv), 6, Color.White);
        if (ts.X != 0 && ts.Y != 0)
            DrawCircleV(ToScreenTS(ts), 6, Color.White);
    }
}
