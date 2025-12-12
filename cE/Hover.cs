using System.Numerics;
using Raylib_cs;
using System;

public class Hover
{
    private Rectangle rect;
    private int count;
    private int minCount;
    private int maxCount;
    private Color innColor;
    private Color edgeColor;

    public Color txtColor = Color.White;
    
    private float lastKeyUpdateTime = 0f;
    private float keyHoldTime = 0f;

    public InfoCircle infoCircle;

    public Hover(Vector2 position, Vector2 size, int initialCount, int min, int max)
    {
        rect = new Rectangle(position.X, position.Y, size.X, size.Y);
        count = initialCount;
        minCount = min;
        maxCount = max;
        innColor = Color.Blank;
        edgeColor = Color.White;
        infoCircle = new InfoCircle(this);
    }

    public void UpdatePosition(Vector2 position, Vector2 size)
    {
        rect.X = position.X;
        rect.Y = position.Y;
        rect.Width = size.X;
        rect.Height = size.Y;
        infoCircle.UpdatePosition();
    }

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

            if (anyKeyDown)
            {
                keyHoldTime += Raylib.GetFrameTime();
                float interval = Math.Max(0.04f, 0.2f - keyHoldTime * 0.1f);

                if (currentTime - lastKeyUpdateTime >= interval)
                {
                    if (Raylib.IsKeyDown(KeyboardKey.Up)) count++;
                    if (Raylib.IsKeyDown(KeyboardKey.Down)) count--;
                    if (Raylib.IsKeyDown(KeyboardKey.Left)) count -= 10;
                    if (Raylib.IsKeyDown(KeyboardKey.Right)) count += 10;
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

    public void Draw(string text)
    {
        Raylib.DrawRectangleRec(rect, innColor);
        Raylib.DrawRectangleLinesEx(rect, 2, edgeColor);   
        infoCircle.Draw(text);
    }

    public int GetCount() => count;
    public void SetCount(int newCount)
    {
        count = Math.Max(minCount, Math.Min(maxCount, newCount));
    }

    // Add public getters for InfoCircle to access
    public int MinCount => minCount;
    public int MaxCount => maxCount;

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

        public void UpdatePosition()
        {
            position = new Vector2(
                parentHover.rect.X + parentHover.rect.Width / 8f * 7,
                parentHover.rect.Y + parentHover.rect.Height / 8f * 6.5f
            );
        }

        public void Update()
        {
            Vector2 mousePos = Raylib.GetMousePosition();
            isMouseInside = IsMouseInsideCircle(mousePos);
            UpdateClickState();
        }

        private void UpdateClickState()
        {
            bool mousePressed = Raylib.IsMouseButtonPressed(MouseButton.Left);
            
            // Check for click on circle
            if (isMouseInside && mousePressed)
            {
                isClicked = !isClicked; // Toggle the clicked state
            }
            
            // Close the rectangle if clicked outside while it's open
            if (isClicked && mousePressed && !isMouseInside && !IsMouseInsideInfoRect())
            {
                isClicked = false;
            }
        }

        private bool IsMouseInsideInfoRect()
        {
            if (!isClicked) return false;
            
            Vector2 mousePos = Raylib.GetMousePosition();
            Rectangle infoRect = new Rectangle(position.X + 25, position.Y - 100, 200, 120);
            return Raylib.CheckCollisionPointRec(mousePos, infoRect);
        }

        public bool IsMouseInsideCircle(Vector2 mousePos)
        {
            float distance = Vector2.Distance(mousePos, position);
            return distance <= radius;
        }


        private void DrawClickedInfo(string text)
        {
            if (isClicked)            
                Functions.DrawAutoSizedInfoBox(text, 20, position);
            
        }

        public void Draw(string text)
        {
            Color circleColor = Color.Gray; // Standard outer circle is always gray
            Color innerColor;
            
            // Determine inner circle color based on state
            if (isClicked)
            {
                innerColor = Color.Black; // Black when clicked
            }
            else if (isMouseInside)
            {
                innerColor = Color.Gray; // Gray when hovered
            }
            else
            {
                innerColor = Color.White; // White as default
            }

            Raylib.DrawCircleV(position, radius, circleColor);
            Raylib.DrawCircleV(position, radius / 2, innerColor);

            DrawClickedInfo(text); // Draw the clicked rectangle
            
        }

        public Vector2 Position => position;
        public bool IsHovered => isMouseInside;
        public bool IsClicked => isClicked;
    }
}