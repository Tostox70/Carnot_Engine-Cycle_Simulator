using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

public class GraphManager
{
    private readonly Queue<(Vector2 Point, int Stage)> pvPoints = new();
    private readonly Queue<Vector2> tsPoints = new();

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

    private Raylib_cs.Color GetStageColor(int stage) => stage switch
    {
        1 => Raylib_cs.Color.Red,
        2 => Raylib_cs.Color.Green,
        3 => Raylib_cs.Color.Blue,
        4 => Raylib_cs.Color.Green,
        _ => Raylib_cs.Color.White
    };

    public void AddPoints(Vector2 pv, Vector2 ts, int stage)
    {
        if (pv.X != 0 && pv.Y != 0)
        {
            pvPoints.Enqueue((pv, stage));

            if (pvPoints.Count > Points.framesPerStage * 16) pvPoints.Dequeue();
        }

        if (ts.X != 0 && ts.Y != 0)
        {
            tsPoints.Enqueue(ts);
            if (tsPoints.Count > Points.framesPerStage * 16) tsPoints.Dequeue();
        }
    }

    public void ClearPoints()
    {
        pvPoints.Clear();
        tsPoints.Clear();
    }

    public void Draw()
    {
        // PV SCALE
        pvOrigin = new Vector2(PVxAxisL, yAxisBot);
        pvWidth = (int)Raymath.Vector2Distance(new Vector2(PVxAxisL, yAxisBot), new Vector2(PVxAxisR, yAxisBot));
        pvHeight = (int)Raymath.Vector2Distance(new Vector2(PVxAxisL, yAxisTop), new Vector2(PVxAxisL, yAxisBot));
        // PV graph parameters
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

        //TS SCALE
        tsOrigin = new Vector2(TSxAxisL, yAxisBot);
        tsWidth = (int)Raymath.Vector2Distance(new Vector2(TSxAxisL, yAxisBot), new Vector2(TSxAxisR, yAxisBot));
        tsHeight = (int)Raymath.Vector2Distance(new Vector2(TSxAxisL, yAxisTop), new Vector2(TSxAxisL, yAxisBot));
        // TS graph parameters
        float entropyBase = 5;
        float maxEntropy = entropyBase + ((float)Points.QH / Points.TH);
        tsMaxX = maxEntropy * 1.13f;
        tsMaxY = Points.TH * 1.3f;

        tsHorOffset = ((entropyBase / tsMaxX) * tsWidth) - 50;
        tsVerOffset = ((Points.TC / tsMaxY) * tsHeight) - 50;


        // Axes
        DrawLineEx(new Vector2(PVxAxisL, yAxisTop), new Vector2(PVxAxisL, yAxisBot), 10, Raylib_cs.Color.White); // PV Y
        DrawLineEx(new Vector2(PVxAxisL, yAxisBot), new Vector2(PVxAxisR, yAxisBot), 10, Raylib_cs.Color.White); // PV X
        DrawCircleV(new Vector2(PVxAxisL, yAxisBot), 5, Raylib_cs.Color.White); //center circle
        DrawCircleV(new Vector2(PVxAxisL, yAxisTop), 5, Raylib_cs.Color.White); //circle bot r
        DrawCircleV(new Vector2(PVxAxisR, yAxisBot), 5, Raylib_cs.Color.White); //circe top l

        float visualXmaxPV = pvOrigin.X + (v3 / pvMaxX) * pvWidth - pvHorOffset;
        DrawLineEx(new Vector2(visualXmaxPV, yAxisBot - 10), new Vector2(visualXmaxPV, yAxisBot + 10), 3, Raylib_cs.Color.White);
        float visualXminPV = pvOrigin.X + (v1 / pvMaxX) * pvWidth - pvHorOffset;
        DrawLineEx(new Vector2(visualXminPV, yAxisBot - 10), new Vector2(visualXminPV, yAxisBot + 10), 3, Raylib_cs.Color.White);
        float visualXv2PV = pvOrigin.X + (v2 / pvMaxX) * pvWidth - pvHorOffset;
        DrawLineEx(new Vector2(visualXv2PV, yAxisBot - 10), new Vector2(visualXv2PV, yAxisBot + 10), 3, Raylib_cs.Color.White);
        float visualXv4PV = pvOrigin.X + (v4 / pvMaxX) * pvWidth - pvHorOffset;
        DrawLineEx(new Vector2(visualXv4PV, yAxisBot - 10), new Vector2(visualXv4PV, yAxisBot + 10), 3, Raylib_cs.Color.White);

        float visualYmaxPV = pvOrigin.Y - (pMax / pvMaxY) * pvHeight + pvVerOffset;
        DrawLineEx(new Vector2(PVxAxisL - 10, visualYmaxPV), new Vector2(PVxAxisL + 10, visualYmaxPV), 3, Raylib_cs.Color.White);
        float visualYminPV = pvOrigin.Y - (pMin / pvMaxY) * pvHeight + pvVerOffset;
        DrawLineEx(new Vector2(PVxAxisL - 10, visualYminPV), new Vector2(PVxAxisL + 10, visualYminPV), 3, Raylib_cs.Color.White);
        float visualYp2PV = pvOrigin.Y - (p2 / pvMaxY) * pvHeight + pvVerOffset;
        DrawLineEx(new Vector2(PVxAxisL - 10, visualYp2PV), new Vector2(PVxAxisL + 10, visualYp2PV), 3, Raylib_cs.Color.White);
        float visualYp4PV = pvOrigin.Y - (p4/ pvMaxY) * pvHeight + pvVerOffset;
        DrawLineEx(new Vector2(PVxAxisL - 10, visualYp4PV), new Vector2(PVxAxisL + 10, visualYp4PV), 3, Raylib_cs.Color.White);

        DrawText("V1", (int)(visualXminPV -7), (int)(yAxisBot + 20), 15, Raylib_cs.Color.White);
        DrawText("V2", (int)(visualXv2PV - 7), (int)(yAxisBot + 20), 15, Raylib_cs.Color.White);
        DrawText("V3", (int)(visualXmaxPV -7), (int)(yAxisBot + 20), 15, Raylib_cs.Color.White);
        DrawText("V4", (int)(visualXv4PV - 7), (int)(yAxisBot + 20), 15, Raylib_cs.Color.White);
        
        DrawText("P1", (int)(PVxAxisL - 35), (int)(visualYmaxPV - 7), 15, Raylib_cs.Color.White);
        DrawText("P2", (int)(PVxAxisL - 35), (int)(visualYp2PV - 7), 15, Raylib_cs.Color.White);
        DrawText("P3", (int)(PVxAxisL - 35), (int)(visualYminPV - 7), 15, Raylib_cs.Color.White);
        DrawText("P4", (int)(PVxAxisL - 35), (int)(visualYp4PV - 7), 15, Raylib_cs.Color.White);


        DrawLineEx(new Vector2(TSxAxisL, yAxisTop), new Vector2(TSxAxisL, yAxisBot), 10, Raylib_cs.Color.White);   // TS Y
        DrawLineEx(new Vector2(TSxAxisL, yAxisBot), new Vector2(TSxAxisR, yAxisBot), 10, Raylib_cs.Color.White);  // TS X
        DrawCircleV(new Vector2(TSxAxisL, yAxisBot), 5, Raylib_cs.Color.White); //TS   
        DrawCircleV(new Vector2(TSxAxisL, yAxisTop), 5, Raylib_cs.Color.White); //circle bot r
        DrawCircleV(new Vector2(TSxAxisR, yAxisBot), 5, Raylib_cs.Color.White); //circe top l     

        float visualYmaxTS = tsOrigin.Y - (Points.TH / tsMaxY) * tsHeight + tsVerOffset;
        DrawLineEx(new Vector2(TSxAxisL - 10, visualYmaxTS), new Vector2(TSxAxisL + 10, visualYmaxTS), 3, Raylib_cs.Color.White);
        float visualYminTS = tsOrigin.Y - (Points.TC / tsMaxY) * tsHeight + tsVerOffset;
        DrawLineEx(new Vector2(TSxAxisL - 10, visualYminTS), new Vector2(TSxAxisL + 10, visualYminTS), 3, Raylib_cs.Color.White);

        float viusalXmaxTS = tsOrigin.X + (maxEntropy / tsMaxX) * tsWidth - tsHorOffset;
        DrawLineEx(new Vector2(viusalXmaxTS, yAxisBot - 10), new Vector2(viusalXmaxTS, yAxisBot + 10), 3, Raylib_cs.Color.White);
        float visualXminTS = tsOrigin.X + (entropyBase / tsMaxX) * tsWidth - tsHorOffset;
        DrawLineEx(new Vector2(visualXminTS, yAxisBot - 10), new Vector2(visualXminTS, yAxisBot + 10), 3, Raylib_cs.Color.White);
        
        DrawText("TH", (int)(TSxAxisL - 35), (int)(visualYmaxTS - 7), 15, Raylib_cs.Color.White);
        DrawText("TC", (int)(TSxAxisL - 35), (int)(visualYminTS - 7), 15, Raylib_cs.Color.White);
        DrawText("change in Entropy", (int)(visualXminTS + 15), (int)(yAxisBot + 20), 15, Raylib_cs.Color.White);



        DrawText("P", (int)PVxAxisL - 16, (int)yAxisTop - 50, 50, Raylib_cs.Color.White);
        DrawText("V", (int)PVxAxisR + 12, (int)yAxisBot - 20, 50, Raylib_cs.Color.White);
        DrawText("T", (int)TSxAxisL - 16, (int)yAxisTop - 50, 50, Raylib_cs.Color.White);
        DrawText("S", (int)TSxAxisR + 12, (int)yAxisBot - 20, 50, Raylib_cs.Color.White);


        // PV dots and path
        foreach (var (pt, stage) in pvPoints)
            DrawCircleV(ToScreenPV(pt), 2, GetStageColor(stage));

        // TS dots and path
        foreach (var pt in tsPoints)
            DrawCircleV(ToScreenTS(pt), 2, Raylib_cs.Color.Orange);
    }

    public void DrawTracer(Vector2 pv, Vector2 ts)
    {
        if (pv.X != 0 && pv.Y != 0)
            DrawCircleV(ToScreenPV(pv), 6, Raylib_cs.Color.White);
        if (ts.X != 0 && ts.Y != 0)
            DrawCircleV(ToScreenTS(ts), 6, Raylib_cs.Color.White);
    }
}
