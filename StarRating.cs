using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System;

public enum GradientDirection
{
    Horizontal,
    Vertical,
    ForwardDiagonal,
    BackwardDiagonal
}

public enum FillStyle
{
    Solid,
    Gradient
}

public partial class StarRating : UserControl
{
    private int m_leftMargin = 2;
    private int m_rightMargin = 2;
    private int m_topMargin = 2;
    private int m_bottomMargin = 2;
    private int m_hoverStar = 0;
    private int m_selectedStar = 0;

    private int starCount = 5;
    private int starSpacing = 5;
    private static Rectangle[] m_starAreas;
    private static Rectangle[] cachedAreas;
    private static PointF[] p = new PointF[10];
    private bool m_hovering = false;
    private bool layout_changed = false;

    private Color outlineColor = Color.Gray;
    private float outlineThickness = 2F;
    private Color hoverColor = Color.Yellow;
    private Color selectedColor = Color.Yellow;
    private Color gradientColor = Color.White;

    private GradientDirection gradientDirection = GradientDirection.ForwardDiagonal;
    private FillStyle fillBrushStyle = FillStyle.Gradient;

    public StarRating()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        SetStyle(ControlStyles.UserPaint, true);
        SetStyle(ControlStyles.DoubleBuffer, true);
        SetStyle(ControlStyles.ResizeRedraw, true);

        Width = 120;
        Height = 18;

        m_starAreas = new Rectangle[StarCount];
        cachedAreas = new Rectangle[StarCount];
    }

    public int StarCount
    {
        get { return starCount; }
        set
        {
            if (value >= 1)
            {
                starCount = value;
                m_starAreas = new Rectangle[starCount];
                cachedAreas = new Rectangle[StarCount];
                layout_changed = true;
                Invalidate();
            }
        }
    }

    public GradientDirection GradientDirection
    {
        get { return gradientDirection; }
        set
        {
            if (gradientDirection != value)
            {
                gradientDirection = value;
                Invalidate();
            }
        }
    }

    public FillStyle FillBrushStyle
    {
        get { return fillBrushStyle; }
        set
        {
            if (fillBrushStyle != value)
            {
                fillBrushStyle = value;
                Invalidate();  // Redraw the stars when the style changes
            }
        }
    }

    public int SelectedStar
    {
        get { return m_selectedStar; } 
        set
        {
            if (value >= 0 && value <= StarCount)
            {
                m_selectedStar = value;
            }
        }
    }

    public int LeftMargin
    {
        get { return m_leftMargin; }
        set
        {
            if (m_leftMargin != value)
            {
                m_leftMargin = value;
                layout_changed = true;
                Invalidate();
            }
        }
    }

    public int RightMargin
    {
        get { return m_rightMargin; }
        set
        {
            if (m_rightMargin != value)
            {
                m_rightMargin = value;
                layout_changed = true;
                Invalidate();
            }
        }
    }

    public int TopMargin
    {
        get { return m_topMargin; }
        set
        {
            if (m_topMargin != value)
            {
                m_topMargin = value;
                layout_changed = true;
                Invalidate();
            }
        }
    }

    public int BottomMargin
    {
        get { return m_bottomMargin; }
        set
        {
            if (m_bottomMargin != value)
            {
                m_bottomMargin = value;
                layout_changed = true;
                Invalidate();
            }
        }
    }

    public int StarSpacing
    {
        get { return starSpacing; }
        set
        {
            if (starSpacing != value && value >= 1)
            {
                starSpacing = value;
                layout_changed = true;
                Invalidate();
            }
        }
    }

    public Color OutlineColor
    {
        get { return outlineColor; }
        set
        {
            outlineColor = value;
            layout_changed = true;
            Invalidate();
        }
    }

    public Color StarColor
    {
        get { return gradientColor; }
        set
        {
            gradientColor = value;
            layout_changed = true;
            Invalidate();
        }
    }

    public float OutlineThickness
    {
        get { return outlineThickness; }
        set
        {
            outlineThickness = value;
            layout_changed = true;
            Invalidate();
        }
    }

    public Color HoverStarColor
    {
        get { return hoverColor; }
        set
        {
            hoverColor = value;
            layout_changed = true;
            Invalidate();
        }
    }

    public Color SelectedStarColor
    {
        get { return selectedColor; }
        set
        {
            selectedColor = value;
            layout_changed = true;
            Invalidate();
        }
    }

    protected override void OnPaint(PaintEventArgs pe)
    {
        pe.Graphics.Clear(BackColor);
        pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        if (layout_changed)
        {
            int starWidth = (Width - (LeftMargin + RightMargin + (StarSpacing * (StarCount - 1)))) / StarCount;
            int starHeight = (Height - (TopMargin + BottomMargin));

            Rectangle drawArea = new Rectangle(LeftMargin, TopMargin, starWidth, starHeight);

            for (int i = 0; i < StarCount; ++i)
            {
                m_starAreas[i].X = drawArea.X - StarSpacing / 2;
                m_starAreas[i].Y = drawArea.Y;
                m_starAreas[i].Width = drawArea.Width + StarSpacing / 2;
                m_starAreas[i].Height = drawArea.Height;

                DrawStar(pe.Graphics, drawArea, i);
                cachedAreas[i] = drawArea;

                drawArea.X += drawArea.Width + StarSpacing;
            }

            layout_changed = false;
        }
        else
        {
            for (int i = 0; i < StarCount; ++i)
            {
                DrawStar(pe.Graphics, cachedAreas[i], i);
            }
        }

        base.OnPaint(pe);
    }

    protected void DrawStar(Graphics g, Rectangle rect, int starAreaIndex)
    {
        Brush fillBrush;
        Pen outlinePen = new Pen(OutlineColor, OutlineThickness);

        // Determine the gradient direction based on the GradientDirection property
        LinearGradientMode gradientMode = LinearGradientMode.ForwardDiagonal; // Default

        switch (gradientDirection)
        {
            case GradientDirection.Horizontal:
                gradientMode = LinearGradientMode.Horizontal;
                break;
            case GradientDirection.Vertical:
                gradientMode = LinearGradientMode.Vertical;
                break;
            case GradientDirection.ForwardDiagonal:
                gradientMode = LinearGradientMode.ForwardDiagonal;
                break;
            case GradientDirection.BackwardDiagonal:
                gradientMode = LinearGradientMode.BackwardDiagonal;
                break;
        }

        if (fillBrushStyle == FillStyle.Gradient)
        {
            if (m_hovering && starAreaIndex < m_selectedStar)
            {
                fillBrush = new LinearGradientBrush(rect,
                    SelectedStarColor, StarColor, gradientMode);
            }
            else if (m_hovering && m_hoverStar > starAreaIndex)
            {
                fillBrush = new LinearGradientBrush(rect,
                    HoverStarColor, StarColor, gradientMode);
            }
            else if ((!m_hovering) && m_selectedStar > starAreaIndex)
            {
                fillBrush = new LinearGradientBrush(rect,
                    SelectedStarColor, StarColor, gradientMode);
            }
            else
            {
                fillBrush = new SolidBrush(StarColor);
            }
        }
        else
        {
            if (m_hovering && starAreaIndex < m_selectedStar)
            {
                fillBrush = new SolidBrush(SelectedStarColor);
            }
            else if (m_hovering && m_hoverStar > starAreaIndex)
            {
                fillBrush = new SolidBrush(HoverStarColor);
            }
            else if (!m_hovering && m_selectedStar > starAreaIndex)
            {
                fillBrush = new SolidBrush(SelectedStarColor);
            }
            else
            {
                fillBrush = new SolidBrush(StarColor);
            }
        }

        p[0].X = rect.X + (rect.Width / 2);
        p[0].Y = rect.Y;
        p[1].X = rect.X + (42 * rect.Width / 64);
        p[1].Y = rect.Y + (19 * rect.Height / 64);
        p[2].X = rect.X + rect.Width;
        p[2].Y = rect.Y + (22 * rect.Height / 64);
        p[3].X = rect.X + (48 * rect.Width / 64);
        p[3].Y = rect.Y + (38 * rect.Height / 64);
        p[4].X = rect.X + (52 * rect.Width / 64);
        p[4].Y = rect.Y + rect.Height;
        p[5].X = rect.X + (rect.Width / 2);
        p[5].Y = rect.Y + (52 * rect.Height / 64);
        p[6].X = rect.X + (12 * rect.Width / 64);
        p[6].Y = rect.Y + rect.Height;
        p[7].X = rect.X + rect.Width / 4;
        p[7].Y = rect.Y + (38 * rect.Height / 64);
        p[8].X = rect.X;
        p[8].Y = rect.Y + (22 * rect.Height / 64);
        p[9].X = rect.X + (22 * rect.Width / 64);
        p[9].Y = rect.Y + (19 * rect.Height / 64);

        g.FillPolygon(fillBrush, p);
        g.DrawPolygon(outlinePen, p);
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        layout_changed = true;
        Invalidate();
    }

    protected override void OnMouseEnter(System.EventArgs ea)
    {
        m_hovering = true;
        Invalidate();
        base.OnMouseEnter(ea);
    }

    protected override void OnMouseLeave(System.EventArgs ea)
    {
        m_hovering = false;
        Invalidate();
        base.OnMouseLeave(ea);
    }

    protected override void OnMouseMove(MouseEventArgs args)
    {
        int start = m_selectedStar > 0 ? m_selectedStar - 1 : 0;
        for (int i = start; i < StarCount; ++i)
        {
            if (m_starAreas[i].Contains(args.X, args.Y))
            {
                m_hoverStar = i + 1;
                m_hovering = true;
                Invalidate();
                break;
            }
        }

        base.OnMouseMove(args);
    }

    protected override void OnMouseDown(MouseEventArgs args)
    {
        for (int i = 0; i < StarCount; ++i)
        {
            if (m_starAreas[i].Contains(args.X, args.Y))
            {
                m_selectedStar = i + 1;
                m_hovering = false;
                Invalidate();
                break;
            }
        }

        base.OnMouseDown(args);
    }
}
