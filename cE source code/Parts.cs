using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

/// <summary>
/// Represents a rectangular part of the visual simulation, with customizable position, size, color, and outline.
/// </summary>
public class Parts
{
    // Position of the part (top-left corner)
    public Vector2 Position { get; set; }
    // Size of the part (width, height)
    public Vector2 Size { get; set; }
    // Rectangle structure for Raylib drawing
    public Rectangle Rectangle { get; private set; }
    // Thickness of the outline lines
    public int LineThickness { get; set; }
    // Margin around the part for drawing outlines
    public int Margin { get; set; }
    // Fill color of the part
    public Color FillColor { get; set; }
    // Outline color of the part
    public Color OutlineColor { get; set; }

    // Default values for line thickness and margin
    public static int DefaultLineThickness = 5;
    public static int DefaultMargin = DefaultLineThickness + 3;

    // Tuples representing the four outline lines (start, end)
    private (Vector2, Vector2) topLine;
    private (Vector2, Vector2) rightLine;
    private (Vector2, Vector2) leftLine;
    private (Vector2, Vector2) botLine;

    /// <summary>
    /// Constructor for Parts using position and size vectors.
    /// </summary>
    public Parts(Vector2 position, Vector2 size, int lineThickness = 0, int margin = 0)
    {
        Position = position;
        Size = size;
        LineThickness = lineThickness == 0 ? DefaultLineThickness : lineThickness;
        Margin = margin == 0 ? DefaultMargin : margin;
        FillColor = new Color(255, 0, 0, 115);      // Default semi-transparent red
        OutlineColor = new Color(255, 0, 0, 255);   // Default solid red

        UpdateLines();
    }

    /// <summary>
    /// Constructor for Parts using float values for position and size.
    /// </summary>
    public Parts(float x, float y, float width, float height, int lineThickness = 0, int margin = 0)
        : this(new Vector2(x, y), new Vector2(width, height), lineThickness, margin)
    {
    }

    /// <summary>
    /// Updates the rectangle and outline lines based on current position and size.
    /// </summary>
    private void UpdateLines()
    {
        Rectangle = new Rectangle(Position, Size);

        topLine = (new Vector2(Position.X - Margin, Position.Y - Margin),
                   new Vector2(Position.X + Size.X + Margin, Position.Y - Margin));

        rightLine = (new Vector2(Position.X + Size.X + Margin, Position.Y - Margin),
                     new Vector2(Position.X + Size.X + Margin, Position.Y + Size.Y + Margin));

        leftLine = (new Vector2(Position.X - Margin, Position.Y - Margin),
                    new Vector2(Position.X - Margin, Position.Y + Size.Y + Margin));

        botLine = (new Vector2(Position.X - Margin, Position.Y + Size.Y + Margin),
                   new Vector2(Position.X + Size.X + Margin, Position.Y + Size.Y + Margin));
    }

    /// <summary>
    /// Sets a new position for the part and updates outline lines.
    /// </summary>
    public void SetPosition(Vector2 newPosition)
    {
        Position = newPosition;
        UpdateLines();
    }

    /// <summary>
    /// Sets a new size for the part and updates outline lines.
    /// </summary>
    public void SetSize(Vector2 newSize)
    {
        Size = newSize;
        UpdateLines();
    }

    /// <summary>
    /// Draws the part: filled rectangle, outline lines, and corner circles.
    /// </summary>
    public void Draw()
    {
        // Draw filled rectangle
        DrawRectangleRec(Rectangle, FillColor);

        // Draw outline lines
        DrawLineEx(topLine.Item1, topLine.Item2, LineThickness, OutlineColor);
        DrawLineEx(leftLine.Item1, leftLine.Item2, LineThickness, OutlineColor);
        DrawLineEx(rightLine.Item1, rightLine.Item2, LineThickness, OutlineColor);
        DrawLineEx(botLine.Item1, botLine.Item2, LineThickness, OutlineColor);

        // Draw circles at the corners for a rounded effect
        DrawCircleV(topLine.Item1, LineThickness / 2f, OutlineColor);
        DrawCircleV(topLine.Item2, LineThickness / 2f, OutlineColor);
        DrawCircleV(botLine.Item1, LineThickness / 2f, OutlineColor);
        DrawCircleV(rightLine.Item2, LineThickness / 2f, OutlineColor);
    }

    /// <summary>
    /// Returns a color interpolated between red and blue based on the current temperature.
    /// </summary>
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
}

/// <summary>
/// Manages the visual representation of the Carnot engine, including all parts and their layout.
/// </summary>
public class Visual
{
    // Parts representing different components of the engine
    private Parts TCpart, semiTC, semiTH, semi, THpart, supportC, supportH, piston, workCalc;
    // Rectangles for gas and shading effects
    private Rectangle gas, ShaderGas, ShaderTC, ShaderTH, ShaderSemi;
    // Colors for various visual elements
    private Color gasColor, shaderTC, shaderTH, shaderSemi, temperatureColor;
    // Positions and sizes for layout
    private Vector2 TCpos, TCsize, semiSize, pistonPos, pistonSize, rodSize, rodPos, HRbotLineR, LtopRod, LbotRod, RtopRod, RbotRod;

    // Variables for dynamic visual calculations
    private float gasHeight, semiTTsizeX, semiTCposX, semiposX, semiTHposX, THposX, supportposY, supportSizeY, gasPosX, gasPosY, LtopAngleX,
    LtopAngleY, LbotAngleX, LbotAngleY, RtopAngleX, HRbotLineLx, topLength, workCalcSizeX, workCalcSizeY, minVolume,
    maxVolume, maxGasHeight, normalizedVolume, minHeight, maxHeight, pistonY, rodY;
    public static float workCalcPosX, workCalcPosY;
    
    private (Vector2, Vector2) rightLine;
    public string phase = "N/A";
    private int constantDr;

    /// <summary>
    /// Initializes all parts, layout, and colors for the visual engine.
    /// </summary>
    public Visual()
    {
        // Set up initial layout and sizes for all parts
        gasHeight = 0;
        constantDr = (Parts.DefaultLineThickness * 3) + 1;

        // Calculate positions and sizes for all components
        TCpos = new Vector2(65, 140);
        TCsize = new Vector2(25, 400);
        semiTTsizeX = 10;
        semiTCposX = TCpos.X + TCsize.X + constantDr;
        semiposX = semiTCposX + semiTTsizeX + constantDr;
        semiSize = new Vector2(150, 50);
        semiTHposX = semiposX + semiSize.X + constantDr;
        THposX = semiTHposX + semiTTsizeX + constantDr;

        supportposY = TCpos.Y + semiSize.Y + constantDr;
        supportSizeY = TCsize.Y + 100;
        gasPosX = semiposX;
        gasPosY = TCpos.Y + semiSize.Y + constantDr;

        pistonPos = new Vector2(semiposX + Parts.DefaultMargin, gasHeight);
        pistonSize = new Vector2(semiSize.X - (Parts.DefaultMargin * 2), 100);
        rodSize = new Vector2(50, 500);
        rodPos = new Vector2(pistonPos.X + semiSize.X / 2 - rodSize.X / 2 - Parts.DefaultMargin, gasHeight + pistonSize.Y);

        // Instantiate all parts
        TCpart = new Parts(TCpos, TCsize);
        semiTC = new Parts(semiTCposX, TCpos.Y, semiTTsizeX, semiSize.Y);
        semiTH = new Parts(semiTHposX, TCpos.Y, semiTTsizeX, semiSize.Y);
        semi = new Parts(semiposX, TCpos.Y, semiSize.X, semiSize.Y);
        THpart = new Parts(THposX, TCpos.Y, TCsize.X, TCsize.Y);

        supportC = new Parts(semiTCposX, supportposY, semiTTsizeX, TCsize.Y + 100);
        supportH = new Parts(semiTHposX, supportposY, semiTTsizeX, TCsize.Y + 100);

        piston = new Parts(pistonPos, pistonSize);

        // Set up rectangles for gas and shading
        ShaderTC = new Rectangle(semiTCposX, TCpos.Y, semiTTsizeX, semiSize.Y);
        shaderTC = new Color(0, 0, 255, 115);
        ShaderTH = new Rectangle(semiTHposX, TCpos.Y, semiTTsizeX, semiSize.Y);
        shaderTH = new Color(255, 0, 0, 115);
        ShaderSemi = new Rectangle(semiposX, TCpos.Y, semiSize.X, semiSize.Y);

        // Calculate perimeter and rod lines
        LtopAngleX = TCpos.X - TCsize.X - Parts.DefaultMargin;
        LtopAngleY = TCpos.Y - TCsize.X - Parts.DefaultMargin;
        LbotAngleX = TCpos.X - TCsize.X - Parts.DefaultMargin;
        LbotAngleY = TCpos.Y + TCsize.Y + TCsize.X + Parts.DefaultMargin;
        rightLine = (new Vector2(LtopAngleX, LtopAngleY), new Vector2(LbotAngleX, LbotAngleY));
        RtopAngleX = THposX + TCsize.X + TCsize.X + Parts.DefaultMargin;
        HRbotLineR = new Vector2(LbotAngleX + (TCsize.X + Parts.DefaultMargin) * 2, LbotAngleY);
        HRbotLineLx = RtopAngleX - (TCsize.X + Parts.DefaultMargin) * 2;
        topLength = RtopAngleX - LbotAngleX;

        // Work calculation box
        workCalcPosX = LtopAngleX;
        workCalcPosY = supportposY + supportSizeY + constantDr;
        workCalcSizeX = topLength;
        workCalcSizeY = 140;
        workCalc = new Parts(workCalcPosX, workCalcPosY, workCalcSizeX, workCalcSizeY);

        // Rod lines
        LtopRod = new Vector2(rodPos.X - Parts.DefaultMargin, rodPos.Y - Parts.DefaultMargin);
        LbotRod = new Vector2(rodPos.X - Parts.DefaultMargin, workCalcPosY - Parts.DefaultMargin);
        RtopRod = new Vector2(rodPos.X + rodSize.X + Parts.DefaultMargin, rodPos.Y - Parts.DefaultMargin);
        RbotRod = new Vector2(rodPos.X + rodSize.X + Parts.DefaultMargin, workCalcPosY - Parts.DefaultMargin);

        // Set default colors for each part
        gasColor = new Color(170, 170, 170, 70);

        THpart.FillColor = new Color(255, 0, 0, 115);
        THpart.OutlineColor = new Color(255, 0, 0, 255);

        TCpart.FillColor = new Color(0, 0, 255, 115);
        TCpart.OutlineColor = new Color(0, 0, 255, 255);

        semi.FillColor = new Color(150, 150, 150, 155);
        semi.OutlineColor = new Color(100, 100, 100, 255);

        semiTC.FillColor = new Color(200, 200, 200, 115);
        semiTC.OutlineColor = new Color(0, 0, 255, 255);

        semiTH.FillColor = new Color(200, 200, 200, 115);
        semiTH.OutlineColor = new Color(255, 0, 0, 255);

        supportC.FillColor = Color.Blank;
        supportC.OutlineColor = Color.White;

        supportH.FillColor = Color.Blank;
        supportH.OutlineColor = Color.White;

        piston.FillColor = Color.Blank;
        piston.OutlineColor = Color.White;

        workCalc.FillColor = Color.Black;
        workCalc.OutlineColor = Color.White;
    }

    /// <summary>
    /// (Reserved for future use) Updates the visual dimensions if the window is resized.
    /// </summary>
    public void UpdateVisualDimension()
    {
        // Currently empty, can be used for dynamic resizing
    }

    /// <summary>
    /// Updates the dynamic visual state based on the current simulation values.
    /// </summary>
    public void UpdateVisualCalc()
    {
        // Get min/max volumes from Points
        minVolume = (float)Points.gasVolume1;
        maxVolume = (float)Points.gasVolume3;
        maxGasHeight = semiSize.Y - 1;

        // Normalize the current volume to [0,1]
        normalizedVolume = ((float)Points.gasVolumeNow - minVolume) / (maxVolume - minVolume);
        normalizedVolume = Math.Clamp(normalizedVolume, 0f, 1f);

        // Calculate gasHeight based on normalized volume
        minHeight = maxGasHeight * 1f;
        maxHeight = maxGasHeight * 5f;
        gasHeight = minHeight + normalizedVolume * (maxHeight - minHeight);

        // Update rectangles for gas and shading
        gas = new Rectangle(gasPosX, gasPosY, semiSize.X - 1, gasHeight);
        ShaderGas = gas;

        // Update piston and rod positions
        pistonY = gasHeight + gasPosY + (Parts.DefaultMargin * 2);
        rodY = pistonY + pistonSize.Y + (Parts.DefaultMargin * 2);
        piston.SetPosition(new Vector2(pistonPos.X, pistonY));
        LtopRod.Y = rodY - Parts.DefaultMargin;
        RtopRod.Y = rodY - Parts.DefaultMargin;

        // Set phase and shading based on current stage
        int currentStage = Points.GetCurrentStage();
        switch (currentStage)
        {
            case 1:
                phase = "I - Isothermal Expansion";
                shaderSemi = shaderTH;
                break;
            case 2:
                phase = "II - Adiabatic Expansion";
                shaderSemi = Color.Blank;
                break;
            case 3:
                phase = "III - Isothermal Compression";
                shaderSemi = shaderTC;
                break;
            case 4:
                phase = "IV - Adiabatic Compression";
                shaderSemi = Color.Blank;
                break;
            default:
                phase = "N/A";
                shaderSemi = Color.LightGray;
                break;
        }
     }

    /// <summary>
    /// Draws all parts and visual elements of the engine.
    /// </summary>
    public void UpdateVisual()
    {
        // Set temperature color for gas shading
        temperatureColor = Parts.gasTempColor((float)Points.gasTemp, Points.TH, Points.TC);

        // Draw all parts
        THpart.Draw();
        TCpart.Draw();
        semi.Draw();
        semiTC.Draw();
        semiTH.Draw();
        supportC.Draw();
        supportH.Draw();
        piston.Draw();
        workCalc.Draw();

        // Draw gas and shading rectangles
        DrawRectangleRec(gas, gasColor);
        DrawRectangleRec(ShaderGas, temperatureColor);
        DrawRectangleRec(ShaderTC, shaderTC);
        DrawRectangleRec(ShaderTH, shaderTH);
        DrawRectangleRec(ShaderSemi, shaderSemi);

        // Draw rod and perimeter lines
        DrawLineEx(LtopRod, LbotRod, Parts.DefaultLineThickness, Color.White);
        DrawLineEx(RtopRod, RbotRod, Parts.DefaultLineThickness, Color.White);
        DrawLineEx(rightLine.Item1, rightLine.Item2, Parts.DefaultLineThickness, Color.White); // left line
        DrawLineEx(rightLine.Item1, new Vector2(RtopAngleX, LtopAngleY), Parts.DefaultLineThickness, Color.White);// top line
        DrawLineEx(new Vector2(RtopAngleX, LtopAngleY), new Vector2(RtopAngleX, LbotAngleY), Parts.DefaultLineThickness, Color.White); //right line
        DrawLineEx(rightLine.Item2, HRbotLineR, Parts.DefaultLineThickness, Color.White); //HR bot line L
        DrawLineEx(new Vector2(RtopAngleX, LbotAngleY), new Vector2(HRbotLineLx, LbotAngleY), Parts.DefaultLineThickness, Color.White); //HR bot line R

        // Draw perimeter corners
        DrawCircleV(new Vector2(LtopAngleX, LtopAngleY), Parts.DefaultLineThickness / 2f, Color.White);
        DrawCircleV(new Vector2(RtopAngleX, LtopAngleY), Parts.DefaultLineThickness / 2f, Color.White);
        DrawCircleV(new Vector2(LbotAngleX, LbotAngleY), Parts.DefaultLineThickness / 2f, Color.White);
        DrawCircleV(new Vector2(RtopAngleX, LbotAngleY), Parts.DefaultLineThickness / 2f, Color.White);

        // Draw current phase label
        DrawText($"{phase}", 15, 30, 25, Color.Yellow);
    }
}