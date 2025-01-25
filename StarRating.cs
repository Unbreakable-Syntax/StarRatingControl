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

public class StarSelectedEventArgs : EventArgs
{
    public int SelectedStar { get; }

    public StarSelectedEventArgs(int selectedStar)
    {
        SelectedStar = selectedStar;
    }
};

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
    private float outlineThickness = 2F;

    private bool m_hovering = false;
    private bool layout_changed = false;

    private Rectangle[] cachedAreas;
    private PointF[][] p;

    private Color outlineColor = Color.Gray;
    private Color removeColor = Color.Red;
    private Color hoverColor = Color.Blue;
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

        cachedAreas = new Rectangle[StarCount];
        p = new PointF[StarCount][];
        for (int i = 0; i < StarCount; i++) { p[i] = new PointF[10]; }
    }

    public event EventHandler<StarSelectedEventArgs> OnStarSelected;

    public int StarCount
    {
        get { return starCount; }
        set
        {
            if (value >= 1)
            {
                starCount = value;
                cachedAreas = new Rectangle[StarCount];
                p = new PointF[StarCount][];
                for (int i = 0; i < StarCount; i++) { p[i] = new PointF[10]; }
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

    public int SelectedStar
    {
        get { return m_selectedStar; }
        set
        {
            m_selectedStar = value <= StarCount && value >= 0 ? value : 0;
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

    public Color OutlineColor
    {
        get { return outlineColor; }
        set
        {
            outlineColor = value;
            Invalidate();
        }
    }
    public float OutlineThickness
    {
        get { return outlineThickness; }
        set
        {
            outlineThickness = value;
            Invalidate();
        }
    }

    public Color StarColor
    {
        get { return gradientColor; }
        set
        {
            gradientColor = value;
            Invalidate();
        }
    }

    public Color RemoveStarColor
    {
        get { return removeColor; }
        set
        {
            removeColor = value;
            Invalidate();
        }
    }

    public Color HoverStarColor
    {
        get { return hoverColor; }
        set
        {
            hoverColor = value;
            Invalidate();
        }
    }

    public Color SelectedStarColor
    {
        get { return selectedColor; }
        set
        {
            selectedColor = value;
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
        Brush fillBrush = new SolidBrush(StarColor);
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
            if (m_hovering && starAreaIndex >= m_hoverStar && starAreaIndex < m_selectedStar)
            {
                fillBrush = new LinearGradientBrush(rect,
                    RemoveStarColor, StarColor, gradientMode);
            }
            else if (m_hovering && starAreaIndex < m_selectedStar)
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
        }
        else
        {
            if (m_hovering && starAreaIndex >= m_hoverStar && starAreaIndex < m_selectedStar)
            {
                fillBrush = new SolidBrush(RemoveStarColor);
            }
            else if (m_hovering && starAreaIndex < m_selectedStar)
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
        }

        if (layout_changed)
        {
            p[starAreaIndex][0].X = rect.X + (rect.Width / 2);
            p[starAreaIndex][0].Y = rect.Y;
            p[starAreaIndex][1].X = rect.X + (42 * rect.Width / 64);
            p[starAreaIndex][1].Y = rect.Y + (19 * rect.Height / 64);
            p[starAreaIndex][2].X = rect.X + rect.Width;
            p[starAreaIndex][2].Y = rect.Y + (22 * rect.Height / 64);
            p[starAreaIndex][3].X = rect.X + (48 * rect.Width / 64);
            p[starAreaIndex][3].Y = rect.Y + (38 * rect.Height / 64);
            p[starAreaIndex][4].X = rect.X + (52 * rect.Width / 64);
            p[starAreaIndex][4].Y = rect.Y + rect.Height;
            p[starAreaIndex][5].X = rect.X + (rect.Width / 2);
            p[starAreaIndex][5].Y = rect.Y + (52 * rect.Height / 64);
            p[starAreaIndex][6].X = rect.X + (12 * rect.Width / 64);
            p[starAreaIndex][6].Y = rect.Y + rect.Height;
            p[starAreaIndex][7].X = rect.X + rect.Width / 4;
            p[starAreaIndex][7].Y = rect.Y + (38 * rect.Height / 64);
            p[starAreaIndex][8].X = rect.X;
            p[starAreaIndex][8].Y = rect.Y + (22 * rect.Height / 64);
            p[starAreaIndex][9].X = rect.X + (22 * rect.Width / 64);
            p[starAreaIndex][9].Y = rect.Y + (19 * rect.Height / 64);
        }

        g.FillPolygon(fillBrush, p[starAreaIndex]);
        g.DrawPolygon(outlinePen, p[starAreaIndex]);
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
        for (int i = 0; i < StarCount; ++i)
        {
            if (cachedAreas[i].Contains(args.X, args.Y))
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
        if (m_selectedStar == 1 && cachedAreas[0].Contains(args.X, args.Y)) 
        {
            m_selectedStar = 0;
            OnStarSelected?.Invoke(this, new StarSelectedEventArgs(m_selectedStar));
            return; 
        }

        for (int i = 0; i < StarCount; ++i)
        {
            if (cachedAreas[i].Contains(args.X, args.Y))
            {
                m_selectedStar = i + 1;
                m_hovering = false;
                Invalidate();
                OnStarSelected?.Invoke(this, new StarSelectedEventArgs(m_selectedStar));
                break;
            }
        }

        base.OnMouseDown(args);
    }
}
