using System.Numerics;
using Raylib_cs;

/// <summary>
/// Represents an interactive hoverable UI element for adjusting simulation parameters.
/// </summary>
public class Hover
{
    // Rectangle area for the hover control
    private Rectangle rect;
    // Current value, min, and max for the parameter
    private float count;
    private float minCount;
    private float maxCount;
    // Colors for the rectangle and outline
    private Color innColor;
    private Color edgeColor;

    // Text color for the value/label
    public Color txtColor = Color.White;

    // Timing for key repeat logic
    private float lastKeyUpdateTime = 0f;
    private float keyHoldTime = 0f;

    // InfoCircle for displaying help/info
    public InfoCircle infoCircle;

    /// <summary>
    /// Creates a new hover control with position, size, and value range.
    /// </summary>
    public Hover(Vector2 position, Vector2 size, float initialCount, float min, float max)
    {
        rect = new Rectangle(position.X, position.Y, size.X, size.Y);
        count = initialCount;
        minCount = min;
        maxCount = max;
        innColor = Color.Blank;
        edgeColor = Color.White;
        infoCircle = new InfoCircle(this);
    }

    /// <summary>
    /// Updates the rectangle's position and size.
    /// </summary>
    public void UpdatePosition(Vector2 position, Vector2 size)
    {
        rect.X = position.X;
        rect.Y = position.Y;
        rect.Width = size.X;
        rect.Height = size.Y;
        infoCircle.UpdatePosition();
    }

    /// <summary>
    /// Updates the hover control, handling mouse and keyboard input for integer/float values.
    /// </summary>
    public void Update()
    {
        Vector2 mousePos = Raylib.GetMousePosition();
        infoCircle.Update();

        if (infoCircle.IsHovered)
        {
            innColor = Color.Blank;
            edgeColor = Color.White;
            txtColor = Color.White;
            return;
        }

        bool isMouseInside = Raylib.CheckCollisionPointRec(mousePos, rect);

        if (isMouseInside)
        {
            innColor = Color.Blank;
            edgeColor = Color.White;
            txtColor = new Color(255, 255, 255, 200);

            float currentTime = (float)Raylib.GetTime();
            bool anyKeyDown = Raylib.IsKeyDown(KeyboardKey.Up) || Raylib.IsKeyDown(KeyboardKey.Down) ||
                             Raylib.IsKeyDown(KeyboardKey.Left) || Raylib.IsKeyDown(KeyboardKey.Right);

            // Handle key repeat for value adjustment
            if (anyKeyDown)
            {
                keyHoldTime += Raylib.GetFrameTime();
                float interval = Math.Max(0.04f, 0.2f - keyHoldTime * 0.1f);

                if (currentTime - lastKeyUpdateTime >= interval)
                {
                    if (Raylib.IsKeyDown(KeyboardKey.Up)) count += 1f;
                    if (Raylib.IsKeyDown(KeyboardKey.Down)) count -= 1f;
                    if (Raylib.IsKeyDown(KeyboardKey.Left)) count -= 10f;
                    if (Raylib.IsKeyDown(KeyboardKey.Right)) count += 10f;
                    lastKeyUpdateTime = currentTime;
                }
            }
            else
            {
                keyHoldTime = 0f;
            }

            // Clamp value to min/max
            if (count > maxCount) count = maxCount;
            if (count < minCount) count = minCount;
        }
        else
        {
            innColor = Color.Blank;
            edgeColor = Color.White;
            txtColor = Color.White;
        }
    }

    /// <summary>
    /// Updates the hover control for flexible (usually integer) values, e.g., for n.
    /// </summary>
    public void flexUpdate()
    {
        Vector2 mousePos = Raylib.GetMousePosition();
        infoCircle.Update();

        if (infoCircle.IsHovered)
        {
            innColor = Color.Blank;
            edgeColor = Color.White;
            txtColor = Color.White;
            return;
        }

        bool isMouseInside = Raylib.CheckCollisionPointRec(mousePos, rect);

        if (isMouseInside)
        {
            innColor = Color.Blank;
            edgeColor = Color.White;
            txtColor = new Color(255, 255, 255, 200);

            float currentTime = (float)Raylib.GetTime();
            bool anyKeyDown = Raylib.IsKeyDown(KeyboardKey.Up) || Raylib.IsKeyDown(KeyboardKey.Down) ||
                             Raylib.IsKeyDown(KeyboardKey.Left) || Raylib.IsKeyDown(KeyboardKey.Right);

            if (anyKeyDown)
            {
                keyHoldTime += Raylib.GetFrameTime();
                float interval = Math.Max(0.04f, 0.2f - keyHoldTime * 0.1f);

                if (currentTime - lastKeyUpdateTime >= interval)
                {
                    if (Raylib.IsKeyDown(KeyboardKey.Left)) count--;
                    if (Raylib.IsKeyDown(KeyboardKey.Right)) count++;
                    lastKeyUpdateTime = currentTime;
                }
            }
            else
            {
                keyHoldTime = 0f;
            }

            if (count > maxCount) count = maxCount;
            if (count < minCount) count = minCount;
        }
        else
        {
            innColor = Color.Blank;
            edgeColor = Color.White;
            txtColor = Color.White;
        }
    }

    // Options for gamma (heat capacity ratio) and their labels
    private readonly float[] options = { 5f / 3f - 1, 7f / 5f - 1, 4f / 3f - 1 };
    private readonly string[] gammaLabels = { "5/3", "7/5", "4/3" };
    private int optionIndex = 0;

    /// <summary>
    /// Updates the hover control for gamma, allowing selection from predefined options.
    /// </summary>
    public void OptionUpdate()
    {
        Vector2 mousePos = Raylib.GetMousePosition();
        infoCircle.Update();

        if (infoCircle.IsHovered)
        {
            innColor = Color.Blank;
            edgeColor = Color.White;
            txtColor = Color.White;
            return;
        }

        bool inside = Raylib.CheckCollisionPointRec(mousePos, rect);

        if (inside)
        {
            innColor = Color.Blank;
            edgeColor = Color.White;
            txtColor = new Color(255, 255, 255, 200);

            // Right key = move forward but stop at end
            if (Raylib.IsKeyPressed(KeyboardKey.Right))
            {
                if (optionIndex < options.Length - 1)
                    optionIndex++;

                count = options[optionIndex];
            }

            // Left key = move back but stop at start
            if (Raylib.IsKeyPressed(KeyboardKey.Left))
            {
                if (optionIndex > 0)
                    optionIndex--;

                count = options[optionIndex];
            }
        }
        else
        {
            innColor = Color.Blank;
            edgeColor = Color.White;
            txtColor = Color.White;
        }
    }

    /// <summary>
    /// Updates the hover control for display-only values (no interaction).
    /// </summary>
    public void DisplayUpdate()
    {
        Vector2 mousePos = Raylib.GetMousePosition();
        infoCircle.Update();

        if (infoCircle.IsHovered)
        {
            innColor = Color.Blank;
            edgeColor = Color.White;
            txtColor = Color.White;
            return;
        }
    }

    /// <summary>
    /// Draws the hover control and its info circle.
    /// </summary>
    public void Draw(string text)
    {
        Raylib.DrawRectangleRec(rect, innColor);
        Raylib.DrawRectangleLinesEx(rect, 2, edgeColor);
        infoCircle.Draw(text);
    }

    // Accessors for value and range
    public float GetCount() => count;
    public void SetCount(float value)
    {
        count = value;
    }
    public float MinCount => minCount;
    public float MaxCount => maxCount;
    public string GammaLabel => gammaLabels[optionIndex];

    /// <summary>
    /// Represents a clickable info/help circle attached to a hover control.
    /// </summary>
    public class InfoCircle
    {
        private Vector2 position;
        private float radius;
        private Hover parentHover;
        private bool isMouseInside;

        private bool isClicked = false;

        public InfoCircle(Hover parent)
        {
            parentHover = parent;
            radius = 15f;
            UpdatePosition();
        }

        /// <summary>
        /// Updates the position of the info circle based on the parent hover rectangle.
        /// </summary>
        public void UpdatePosition()
        {
            position = new Vector2(
                parentHover.rect.X + parentHover.rect.Width / 8f * 7,
                parentHover.rect.Y + parentHover.rect.Height / 8f * 6.5f
            );
        }

        /// <summary>
        /// Updates the info circle's hover and click state.
        /// </summary>
        public void Update()
        {
            Vector2 mousePos = Raylib.GetMousePosition();
            isMouseInside = IsMouseInsideCircle(mousePos);
            UpdateClickState();
        }

        /// <summary>
        /// Handles click toggling and closing the info box.
        /// </summary>
        private void UpdateClickState()
        {
            bool mousePressed = Raylib.IsMouseButtonPressed(MouseButton.Left);

            // Toggle clicked state if clicked on the circle
            if (isMouseInside && mousePressed)
            {
                isClicked = !isClicked;
            }

            // Close the info box if clicked outside
            if (isClicked && mousePressed && !isMouseInside && !IsMouseInsideInfoRect())
            {
                isClicked = false;
            }
        }

        /// <summary>
        /// Checks if the mouse is inside the info box rectangle.
        /// </summary>
        private bool IsMouseInsideInfoRect()
        {
            if (!isClicked) return false;

            Vector2 mousePos = Raylib.GetMousePosition();
            Rectangle infoRect = new Rectangle(position.X + 25, position.Y - 100, 200, 120);
            return Raylib.CheckCollisionPointRec(mousePos, infoRect);
        }

        /// <summary>
        /// Checks if the mouse is inside the info circle.
        /// </summary>
        public bool IsMouseInsideCircle(Vector2 mousePos)
        {
            float distance = Vector2.Distance(mousePos, position);
            return distance <= radius;
        }

        /// <summary>
        /// Draws the info box if clicked.
        /// </summary>
        private void DrawClickedInfo(string text)
        {
            if (isClicked)
                Functions.DrawAutoSizedInfoBox(text, 20, position);
        }

        /// <summary>
        /// Draws the info circle and its info box if open.
        /// </summary>
        public void Draw(string text)
        {
            Color circleColor = Color.Gray; // Outer circle
            Color innerColor;

            // Set inner color based on state
            if (isClicked)
            {
                innerColor = Color.Black;
            }
            else if (isMouseInside)
            {
                innerColor = Color.Gray;
            }
            else
            {
                innerColor = Color.White;
            }

            Raylib.DrawCircleV(position, radius, circleColor);
            Raylib.DrawCircleV(position, radius / 2, innerColor);

            DrawClickedInfo(text);
        }

        // Properties for hover/click state
        public Vector2 Position => position;
        public bool IsHovered => isMouseInside;
        public bool IsClicked => isClicked;
    }
}