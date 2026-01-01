using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

public class Visual
{
    private static string phase = "N/A";
    private static int DefaultLineThickness = 5;
    private static int DefaulfMargin = 8;
    private static int constantDrawing = DefaultLineThickness * 3;

    private static Vector2 topL, topR, botL, botR; //RECTANGLE CONTAINER
    private static Vector2 periTopL, periTopR, periBotL, periBotR;  //PERIMETER

    private static float TopLenght, SideLenght, TopPeriLength, chanmberLength, chamberSubdivision;

    private static float A, B, C, D, E, F; int verticalDivision = 32; // VERTICAL REFERENCE POINTS
    private static float A2, B2, C2, D2, E2; int horizontalDivision = 30; // HORIZONTAL REFERENCE POINTS

    private static Color LineColor = Color.White;
    public static Color TCshader = new Color(0, 0, 255, 115);
    public static Color TClines = new Color(0, 0, 255, 255);
    public static Color THshader = new Color(255, 0, 0, 115);
    public static Color THlines = new Color(255, 0, 0, 255);
    public static Color SEMIshader = new Color(200, 200, 200, 115);
    private static Color SEMIlines = new Color(100, 100, 100, 255);
    public static Color SEMIshaderTemp = new Color(200, 200, 200, 115);
    private static Color gasColor;

    private static Vector2 TCsize, THsize;
    private static Vector2 TCpos, THpos;
    private static Vector2 semiTCpos, semiTHpos, SEMIpos;
    private static Vector2 semiTCsize, semiTHsize, SEMIsize;
    private static Vector2 rodTopL, rodTopR, rodBotL, rodBotR;
    public static Vector2 BOXpos, BOXsize;
    private static Vector2 gasPos, gasSize;
    private static Vector2 pistonPos, pistonSize;
    private static float gasHeight, minVolume, maxVolume, normalizedVolume, gasAtMinimumLength, gasAtMaximumLength, standardGasLength;

    private static Vector2 stagesBoxSize, stageFontSize;
    private static float stageHorSub, stageVerSub;
    private static int stagefontSize;
    public void UpdateVisualCalc()
    {
        topL = Lines.Layout.line1L;
        topR = Lines.Layout.line1R;
        botL = new Vector2(0, Program.screenHeight);
        botR = new Vector2(Program.screenWidth * 0.3f + 10, Program.screenHeight);

        TopLenght = Raymath.Vector2Distance(topL, topR);
        SideLenght = Raymath.Vector2Distance(topL, botL);

        periTopL = new Vector2(topL.X + TopLenght / 20, topL.Y + SideLenght / horizontalDivision);
        periTopR = new Vector2(topL.X + TopLenght / 20 * 19, topR.Y + SideLenght / horizontalDivision);
        periBotL = new Vector2(topL.X + TopLenght / 20, topL.Y + SideLenght / horizontalDivision * 18.5f);
        periBotR = new Vector2(topL.X + TopLenght / 20 * 19, topL.Y + SideLenght / horizontalDivision * 18.5f);

        TopPeriLength = Raymath.Vector2Distance(periTopL, periBotR);
        A = periTopL.X + TopPeriLength / verticalDivision;
        B = periTopL.X + TopPeriLength / verticalDivision * 3.5f;
        C = periTopL.X + TopPeriLength / verticalDivision * 5;
        D = periTopR.X - TopPeriLength / verticalDivision * 5;
        E = periTopR.X - TopPeriLength / verticalDivision * 3.5f;
        F = periTopR.X - TopPeriLength / verticalDivision;

        A2 = topL.Y + SideLenght / horizontalDivision * 2;
        B2 = periTopL.Y + SideLenght / horizontalDivision * 3.5f;
        C2 = topL.Y + SideLenght / horizontalDivision * 17.5f;
        D2 = topL.Y + SideLenght / horizontalDivision * 23;
        E2 = topL.Y + SideLenght / horizontalDivision * 29;

        chanmberLength = Raymath.Vector2Distance(new Vector2(C, 0), new Vector2(D, 0));
        chamberSubdivision = chanmberLength / 3;

        //TH AND TC
        TCpos = new Vector2(A + DefaulfMargin, A2 + DefaulfMargin);
        THpos = new Vector2(E + DefaulfMargin, A2 + DefaulfMargin);
        TCsize = new Vector2(Raymath.Vector2Distance(new Vector2(A, 0), new Vector2(B, 0)) - constantDrawing, Raymath.Vector2Distance(new Vector2(0, A2), new Vector2(0, C2)) - constantDrawing);
        THsize = new Vector2(Raymath.Vector2Distance(new Vector2(E, 0), new Vector2(F, 0)) - constantDrawing, Raymath.Vector2Distance(new Vector2(0, A2), new Vector2(0, C2)) - constantDrawing);
        //SEMI TH AND TC
        semiTCpos = new Vector2(B + DefaulfMargin, A2 + DefaulfMargin);
        semiTHpos = new Vector2(D + DefaulfMargin, A2 + DefaulfMargin);
        semiTCsize = new Vector2(Raymath.Vector2Distance(new Vector2(B, 0), new Vector2(C, 0)) - constantDrawing, Raymath.Vector2Distance(new Vector2(0, A2), new Vector2(0, B2)) - constantDrawing);
        semiTHsize = new Vector2(Raymath.Vector2Distance(new Vector2(D, 0), new Vector2(E, 0)) - constantDrawing, Raymath.Vector2Distance(new Vector2(0, A2), new Vector2(0, B2)) - constantDrawing);
        //SEMI
        SEMIpos = new Vector2(C + DefaulfMargin, A2 + DefaulfMargin);
        SEMIsize = new Vector2(chanmberLength - constantDrawing, Raymath.Vector2Distance(new Vector2(0, A2), new Vector2(0, B2)) - constantDrawing);
        //BOX
        BOXpos = new Vector2(periTopL.X, D2);
        BOXsize = new Vector2(Raymath.Vector2Distance(periTopL, periTopR), Raymath.Vector2Distance(new Vector2(0, D2), new Vector2(0, E2)));


        //GAS
        gasPos = new Vector2(C + DefaulfMargin, B2 + DefaulfMargin);
        standardGasLength = SEMIsize.Y;
        minVolume = (float)Points.gasVolume1;
        maxVolume = (float)Points.gasVolume3;
        normalizedVolume = ((float)Points.gasVolumeNow - minVolume) / (maxVolume - minVolume);
        normalizedVolume = Math.Clamp(normalizedVolume, 0f, 1f);
        gasAtMinimumLength = standardGasLength * 1;
        gasAtMaximumLength = standardGasLength * 4;
        gasHeight = gasAtMinimumLength + normalizedVolume * (gasAtMaximumLength - gasAtMinimumLength);
        gasSize = new Vector2(SEMIsize.X, gasHeight);
        gasColor = Functions.gasTempColor((float)Points.gasTemp, Points.TH, Points.TC);


        //PISTON
        pistonPos = new Vector2(C + DefaulfMargin, B2 + DefaulfMargin * 2.2f + gasHeight);
        pistonSize = new Vector2(SEMIsize.X, SEMIsize.Y * 2);
        //ROD
        rodBotL = new Vector2(C + chamberSubdivision, D2);
        rodBotR = new Vector2(D - chamberSubdivision, D2);
        rodTopL = new Vector2(C + chamberSubdivision, pistonPos.Y + pistonSize.Y);
        rodTopR = new Vector2(D - chamberSubdivision, pistonPos.Y + pistonSize.Y);

        // Set phase and shading based on current stage
        int currentStage = Points.GetCurrentStage();
        switch (currentStage)
        {
            case 1:
                phase = "I - Isothermal Expansion";
                SEMIshaderTemp = THshader;
                SEMIlines = THlines;
                break;
            case 2:
                phase = "II - Adiabatic Expansion";
                SEMIshaderTemp = Color.Blank;
                SEMIlines = new Color(200, 200, 200, 115);
                break;
            case 3:
                phase = "III - Isothermal Compression";
                SEMIshaderTemp = TCshader;
                SEMIlines = TClines;
                break;
            case 4:
                phase = "IV - Adiabatic Compression";
                SEMIshaderTemp = Color.Blank;
                SEMIlines = new Color(200, 200, 200, 115);
                break;
            default:
                phase = "N/A";
                SEMIshaderTemp = Color.LightGray;
                SEMIlines = new Color(200, 200, 200, 115);
                break;
        }
        stagesBoxSize = topR;
        stageVerSub = stagesBoxSize.Y / 2;
        stageHorSub = stagesBoxSize.X / 30;
        stagefontSize = (int)Math.Clamp(stagesBoxSize.X * 0.065f, 12f, 48f);
        stageFontSize = MeasureTextEx(GetFontDefault(), "III - Isothermal Compression", stagefontSize, 0);

    }

    public void UpdateVisual()
    {
        // Draw current phase label
        DrawText($"{phase}", (int)stageHorSub, (int)stageVerSub - (int)stageFontSize.Y / 2, stagefontSize, Color.Yellow);

        //RECTANGLE CONTAINER
        DrawLineV(topL, topR, LineColor); //top
        DrawLineV(botL, botR, LineColor); //bot
        DrawLineV(topR, botR, LineColor); //right
        DrawLineV(topL, botL, LineColor); //left

        //PERIMETER
        DrawLineEx(periTopL, periTopR, DefaultLineThickness, LineColor); //top
        DrawLineEx(periTopL, periBotL, DefaultLineThickness, LineColor); //left side
        DrawLineEx(periTopR, periBotR, DefaultLineThickness, LineColor); //right side
        DrawLineEx(periBotL, new Vector2(B, periBotL.Y), DefaultLineThickness, LineColor); //left bot
        DrawLineEx(periBotR, new Vector2(E, periBotR.Y), DefaultLineThickness, LineColor); //right bot
        DrawCircleV(periTopL, DefaultLineThickness / 2, LineColor);
        DrawCircleV(periTopR, DefaultLineThickness / 2, LineColor);
        DrawCircleV(periBotL, DefaultLineThickness / 2, LineColor);
        DrawCircleV(periBotR, DefaultLineThickness / 2, LineColor);

        //TC
        DrawLineEx(new Vector2(A, A2), new Vector2(B, A2), DefaultLineThickness, TClines); //top
        DrawLineEx(new Vector2(A, A2), new Vector2(A, C2), DefaultLineThickness, TClines);//left
        DrawLineEx(new Vector2(B, A2), new Vector2(B, C2), DefaultLineThickness, TClines);//right
        DrawLineEx(new Vector2(A, C2), new Vector2(B, C2), DefaultLineThickness, TClines); //bot
        DrawRectangleV(TCpos, TCsize, TCshader);
        DrawCircleV(new Vector2(A, A2), DefaultLineThickness / 2, TClines);
        DrawCircleV(new Vector2(A, C2), DefaultLineThickness / 2, TClines);

        //TH
        DrawLineEx(new Vector2(E, A2), new Vector2(F, A2), DefaultLineThickness, THlines); //top
        DrawLineEx(new Vector2(E, A2), new Vector2(E, C2), DefaultLineThickness, THlines);//left
        DrawLineEx(new Vector2(F, A2), new Vector2(F, C2), DefaultLineThickness, THlines);//right
        DrawLineEx(new Vector2(E, C2), new Vector2(F, C2), DefaultLineThickness, THlines); //bot
        DrawRectangleV(THpos, THsize, THshader);
        DrawCircleV(new Vector2(F, A2), DefaultLineThickness / 2, THlines);
        DrawCircleV(new Vector2(F, C2), DefaultLineThickness / 2, THlines);

        //semiTC
        DrawLineEx(new Vector2(B, A2), new Vector2(C, A2), DefaultLineThickness, TClines); //top
        DrawLineEx(new Vector2(C, A2), new Vector2(C, B2), DefaultLineThickness, TClines);//right
        DrawLineEx(new Vector2(B, B2), new Vector2(C, B2), DefaultLineThickness, TClines); //bot
        DrawRectangleV(semiTCpos, semiTCsize, SEMIshader);
        DrawRectangleV(semiTCpos, semiTCsize, TCshader);

        //semiTH
        DrawLineEx(new Vector2(D, A2), new Vector2(E, A2), DefaultLineThickness, THlines); //top
        DrawLineEx(new Vector2(D, A2), new Vector2(D, B2), DefaultLineThickness, THlines);//left
        DrawLineEx(new Vector2(D, B2), new Vector2(E, B2), DefaultLineThickness, THlines); //bot
        DrawRectangleV(semiTHpos, semiTHsize, SEMIshader);
        DrawRectangleV(semiTHpos, semiTHsize, THshader);

        //SEMI
        DrawLineEx(new Vector2(C, A2), new Vector2(D, A2), DefaultLineThickness, SEMIlines); //top
        DrawLineEx(new Vector2(C, B2), new Vector2(D, B2), DefaultLineThickness, SEMIlines); //bot
        DrawRectangleV(SEMIpos, SEMIsize, SEMIshader);
        DrawRectangleV(SEMIpos, SEMIsize, SEMIshaderTemp);

        DrawCircleV(new Vector2(C, A2), DefaultLineThickness / 2, TClines);
        DrawCircleV(new Vector2(D, A2), DefaultLineThickness / 2, THlines);

        //SUPPORT TC
        DrawLineEx(new Vector2(B, B2), new Vector2(C, B2), DefaultLineThickness, LineColor); //top
        DrawLineEx(new Vector2(C, B2), new Vector2(C, D2), DefaultLineThickness, LineColor);//right
        DrawLineEx(new Vector2(B, B2), new Vector2(B, D2), DefaultLineThickness, LineColor);//left
        DrawCircleV(new Vector2(B, B2), DefaultLineThickness / 2, LineColor);
        DrawCircleV(new Vector2(C, B2), DefaultLineThickness / 2, LineColor);

        //SUPPORT TH
        DrawLineEx(new Vector2(D, B2), new Vector2(E, B2), DefaultLineThickness, LineColor); //top
        DrawLineEx(new Vector2(E, B2), new Vector2(E, D2), DefaultLineThickness, LineColor);//right
        DrawLineEx(new Vector2(D, B2), new Vector2(D, D2), DefaultLineThickness, LineColor);//left
        DrawCircleV(new Vector2(D, B2), DefaultLineThickness / 2, LineColor);
        DrawCircleV(new Vector2(E, B2), DefaultLineThickness / 2, LineColor);

        //BOX
        DrawLineEx(BOXpos, new Vector2(BOXpos.X + BOXsize.X, BOXpos.Y), DefaultLineThickness, LineColor); //top
        DrawLineEx(new Vector2(BOXpos.X, BOXpos.Y + BOXsize.Y), new Vector2(BOXpos.X + BOXsize.X, BOXpos.Y + BOXsize.Y), DefaultLineThickness, LineColor); //bot
        DrawLineEx(new Vector2(periTopL.X, D2), new Vector2(periTopL.X, E2), DefaultLineThickness, LineColor); //left
        DrawLineEx(new Vector2(periTopR.X, D2), new Vector2(periTopR.X, E2), DefaultLineThickness, LineColor); //right
        DrawCircleV(BOXpos, DefaultLineThickness / 2, LineColor);
        DrawCircleV(new Vector2(BOXpos.X, BOXpos.Y + BOXsize.Y), DefaultLineThickness / 2, LineColor);
        DrawCircleV(new Vector2(BOXpos.X + BOXsize.X, BOXpos.Y + BOXsize.Y), DefaultLineThickness / 2, LineColor);
        DrawCircleV(new Vector2(BOXpos.X + BOXsize.X, BOXpos.Y), DefaultLineThickness / 2, LineColor);

        //GAS
        DrawRectangleV(gasPos, gasSize, gasColor);

        //PISTON
        DrawLineEx(pistonPos, new Vector2(pistonPos.X + pistonSize.X, pistonPos.Y), DefaultLineThickness, LineColor); //top
        DrawLineEx(new Vector2(pistonPos.X, pistonPos.Y + pistonSize.Y), new Vector2(pistonPos.X + pistonSize.X, pistonPos.Y + pistonSize.Y), DefaultLineThickness, LineColor); // bot
        DrawLineEx(new Vector2(pistonPos.X + pistonSize.X, pistonPos.Y), new Vector2(pistonPos.X + pistonSize.X, pistonPos.Y + pistonSize.Y), DefaultLineThickness, LineColor); //right
        DrawLineEx(pistonPos, new Vector2(pistonPos.X, pistonPos.Y + pistonSize.Y), DefaultLineThickness, LineColor); //left
        DrawCircleV(pistonPos, DefaultLineThickness / 2, LineColor);
        DrawCircleV(new Vector2(pistonPos.X + pistonSize.X, pistonPos.Y), DefaultLineThickness / 2, LineColor);
        DrawCircleV(new Vector2(pistonPos.X + pistonSize.X, pistonPos.Y + pistonSize.Y), DefaultLineThickness / 2, LineColor);
        DrawCircleV(new Vector2(pistonPos.X, pistonPos.Y + pistonSize.Y), DefaultLineThickness / 2, LineColor);

        //ROD
        DrawLineEx(rodTopL, rodBotL, DefaultLineThickness, LineColor); //left
        DrawLineEx(rodTopR, rodBotR, DefaultLineThickness, LineColor); //right

        //REFERENCE
        // DrawCircleV(new Vector2(A, periTopL.Y), 5, Color.Red);
        // DrawCircleV(new Vector2(B, periTopL.Y), 5, Color.Red);
        // DrawCircleV(new Vector2(C, periTopL.Y), 5, Color.Red);
        // DrawCircleV(new Vector2(D, periTopL.Y), 5, Color.Red);
        // DrawCircleV(new Vector2(E, periTopL.Y), 5, Color.Red);
        // DrawCircleV(new Vector2(F, periTopL.Y), 5, Color.Red);

        // DrawCircleV(periBotL, 5, Color.Red);

        // DrawCircleV(new Vector2(periTopL.X, A2), 5, Color.Red);
        // DrawCircleV(new Vector2(periTopL.X, B2), 5, Color.Red);
        // DrawCircleV(new Vector2(periTopL.X, C2), 5, Color.Red);
        // DrawCircleV(new Vector2(periTopL.X, D2), 5, Color.Red);
        // DrawCircleV(new Vector2(periTopL.X, E2), 5, Color.Red);

        // DrawCircleV(rodBotL, 5, THshader);
        // DrawCircleV(rodBotR, 5, THshader);
    }
}

