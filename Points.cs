using System.Numerics;

/// <summary>
/// Static class to manage the thermodynamic state and calculations for the Carnot cycle.
/// </summary>
public static class Points
{
    // Main cycle variables (temperatures, volumes, moles, etc.)
    public static int TH = 300; // Hot reservoir temperature
    public static int TC = 100; // Cold reservoir temperature (cannot be zero)
    public static float gamma = 5f / 3f - 1f; // Heat capacity ratio
    public static float exponent = 1f / gamma;
    public static double gasVolume1 = 0.01; // Initial volume
    public static double gasVolume2 = gasVolume1 * 2; // Volume after isothermal expansion
    public static double gasVolume3 = gasVolume2 * MathF.Pow(TH / TC, exponent); // Max volume
    public static double gasVolume4 = gasVolume1 * MathF.Pow(TH / TC, exponent); // Volume after adiabatic compression
    public static float n = 1; // Number of moles
    public const float R = 8.314f; // Universal gas constant

    // Heat and work calculations
    public static double QH = n * R * TH * Math.Log(gasVolume2 / gasVolume1); // Heat absorbed
    public static double QC = n * R * TC * Math.Log(gasVolume4 / gasVolume3); // Heat released
    public static double gasVolumeNow; // Current gas volume
    public static double gasPressure; // Current gas pressure
    public static double gasTemp; // Current gas temperature

    // Entropy calculations
    public static double EntropyH = QH / TH;
    public static double EntropyC = QC / TC;
    private static double deltaEntropy;
    private static double dE = 0;
    private static double diff;
    public static double entropy = 5;
    private static double QHe;
    private static double QCe;

    // Work and efficiency
    public static double totWork = Math.Abs(QH) - Math.Abs(QC);
    private static float W2 = n * R / gamma * (TH - TC);
    private static float W4 = n * R / gamma * (TC - TH);
    public static double workByGas = QH + W2;
    public static double workBySurr = QC + W4;
    public static double eff = totWork / QH * 100.0;

    // Simulation state variables
    public static int stage = 1; // Current stage of the cycle
    private static bool initialized = false;
    private static bool cycleComplete = false;
    public static bool isComplete = false;

    private static int currentStep = 0; // Current step in the stage
    private static double currentStepSize = 0.0001; // Step size for integration

    // Timing variables for step integration
    private static float timePerStage;
    private static float frameRate;
    public static int framesPerStage;

    /// <summary>
    /// Returns the current PV and TS points for graphing, and updates the simulation state.
    /// </summary>
    public static (Vector2, Vector2) Point()
    {
        // Return empty vector if cycle is complete
        if (cycleComplete)
            return (new Vector2(), new Vector2());

        // Initialize on first call
        if (!initialized)
        {
            gasVolumeNow = gasVolume1;
            gasTemp = TH;
            stage = 1;
            currentStep = 0;
            currentStepSize = GetTimeBasedStepSize(1, timePerStage, frameRate);
            framesPerStage = (int)(timePerStage * frameRate);
            initialized = true;
        }

        switch (stage)
        {
            case 1: // Isothermal expansion
                if (currentStep < framesPerStage)
                {
                    // Calculate exact volume from step number
                    gasVolumeNow = gasVolume1 + (currentStep * currentStepSize);

                    QHe = n * R * TH * Math.Log(gasVolumeNow / gasVolume1);
                    deltaEntropy = QHe / gasTemp;
                    diff = deltaEntropy - dE;
                    entropy += diff;
                    dE = deltaEntropy;

                    gasPressure = n * R * gasTemp / gasVolumeNow;

                    currentStep++;
                    return (new Vector2((float)gasVolumeNow, (float)gasPressure), new Vector2((float)entropy, (float)gasTemp));
                }
                else
                {
                    stage = 2;
                    currentStep = 0;
                    currentStepSize = GetTimeBasedStepSize(2, timePerStage, frameRate);
                    framesPerStage = (int)(timePerStage * frameRate);

                }
                break;

            case 2: // Adiabatic expansion
                if (currentStep < framesPerStage)
                {
                    // Calculate exact volume from step number
                    gasVolumeNow = gasVolume2 + (currentStep * currentStepSize);

                    gasTemp = TH * Math.Pow(gasVolume2 / gasVolumeNow, gamma);
                    gasPressure = n * R * gasTemp / gasVolumeNow;

                    currentStep++;
                    return (new Vector2((float)gasVolumeNow, (float)gasPressure), new Vector2((float)entropy, (float)gasTemp));
                }
                else
                {
                    stage = 3;
                    currentStep = 0;
                    currentStepSize = GetTimeBasedStepSize(3, timePerStage, frameRate);
                    framesPerStage = (int)(timePerStage * frameRate);
                    dE = 0;
                    gasTemp = TC;
                }
                break;

            case 3: // Isothermal compression
                if (currentStep < framesPerStage)
                {
                    // Calculate exact volume from step number (decreasing)
                    gasVolumeNow = gasVolume3 - (currentStep * currentStepSize);

                    gasPressure = n * R * gasTemp / gasVolumeNow;

                    QCe = n * R * TC * Math.Log(gasVolumeNow / gasVolume3);
                    deltaEntropy = QCe / gasTemp;
                    diff = deltaEntropy - dE;
                    entropy += diff;
                    dE = deltaEntropy;

                    currentStep++;
                    return (new Vector2((float)gasVolumeNow, (float)gasPressure), new Vector2((float)entropy, (float)gasTemp));

                }
                else
                {
                    stage = 4;
                    currentStep = 0;
                    currentStepSize = GetTimeBasedStepSize(4, timePerStage, frameRate);
                    framesPerStage = (int)(timePerStage * frameRate);
                }
                break;

            case 4: // Adiabatic compression
                if (currentStep < framesPerStage)
                {
                    // Calculate exact volume from step number (decreasing)
                    gasVolumeNow = gasVolume4 - (currentStep * currentStepSize);

                    gasTemp = TC * Math.Pow(gasVolume4 / gasVolumeNow, gamma);
                    gasPressure = n * R * gasTemp / gasVolumeNow;

                    currentStep++;
                    return (new Vector2((float)gasVolumeNow, (float)gasPressure), new Vector2((float)entropy, (float)gasTemp));
                }
                else
                {
                    stage = 1;
                    currentStep = 0;
                    currentStepSize = GetTimeBasedStepSize(1, timePerStage, frameRate);
                    framesPerStage = (int)(timePerStage * frameRate);
                    gasVolumeNow = gasVolume1;
                    gasTemp = TH;
                    dE = 0;
                    deltaEntropy = 0;
                    entropy = 5;
                    isComplete = true;
                    // Keep entropy accumulated for realistic behavior
                }
                break;
        }

        return (new Vector2(), new Vector2()); // Fallback
    }

    /// <summary>
    /// Sets the time per stage for the simulation.
    /// </summary>
    public static void SetTimePerStage(float seconds)
    {
        timePerStage = seconds;
        // Recalculate step size for current stage
        if (initialized)
        {
            currentStepSize = GetTimeBasedStepSize(stage, timePerStage, frameRate);
            framesPerStage = (int)(timePerStage * frameRate);
        }
    }

    /// <summary>
    /// Sets the frame rate for the simulation.
    /// </summary>
    public static void SetFrameRate(float fps)
    {
        frameRate = fps;
        if (initialized)
        {
            currentStepSize = GetTimeBasedStepSize(stage, timePerStage, frameRate);
            framesPerStage = (int)(timePerStage * frameRate);
        }
    }

    /// <summary>
    /// Calculates the number of points per cycle.
    /// </summary>
    public static int CalculatePointsPerCycle()
    {
        return (int)(timePerStage * frameRate * 4); // 4 stages × frames per stage
    }

    /// <summary>
    /// Resets the simulation state.
    /// </summary>
    public static void Reset()
    {
        dE = 0;
        initialized = false;
        cycleComplete = false;
        stage = 1;
        currentStep = 0;
        currentStepSize = GetTimeBasedStepSize(1, timePerStage, frameRate);
        framesPerStage = (int)(timePerStage * frameRate);

        // Reset to exact starting values
        gasVolumeNow = gasVolume1;
        gasTemp = TH;
        entropy = 5f; // Reset entropy if desired
    }

    /// <summary>
    /// Calculates the step size for time-based integration.
    /// </summary>
    private static double GetTimeBasedStepSize(int stage, float timePerStage, float frameRate)
    {
        framesPerStage = (int)(timePerStage * frameRate); // frames needed for this stage

        double volumeRange = stage switch
        {
            1 => gasVolume2 - gasVolume1,     // ~0.01
            2 => gasVolume3 - gasVolume2,     // ~0.34  
            3 => gasVolume3 - gasVolume4,     // ~0.18
            4 => gasVolume4 - gasVolume1,     // ~0.17
            _ => 0.01f
        };

        return volumeRange / framesPerStage; // volume change per frame
    }

    public static int GetCurrentStage()
    {
        return stage;
    }

    public static void SetNewPointsValues(int newTH, int newTC, int newN, float newGamma)
    {
        if (TH != newTH || TC != newTC || n != newN || gamma!= newGamma)
        {
            TH = newTH;
            TC = newTC;
            n = newN;
            gamma = newGamma;

            // Recalculate all dependent variables
            exponent = 1f / gamma;
            gasVolume2 = gasVolume1 * 2;
            gasVolume3 = gasVolume2 * MathF.Pow(TH / (float)TC, exponent);
            gasVolume4 = gasVolume1 * MathF.Pow(TH / (float)TC, exponent);

            QH = n * R * TH * Math.Log(gasVolume2 / gasVolume1);
            QC = n * R * TC * Math.Log(gasVolume4 / gasVolume3);

            EntropyH = QH / TH;
            EntropyC = QC / TC;

            totWork = Math.Abs(QH) - Math.Abs(QC);
            W2 = n * R / gamma * (TH - TC);
            W4 = n * R / gamma * (TC - TH);
            workByGas = QH + W2;
            workBySurr = QC + W4;
            eff = totWork / QH * 100.0;

            Reset();
        }
    }
}

