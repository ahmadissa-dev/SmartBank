using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SmartBank.WinForms.Controls
{
    public class CirclePictureBox : PictureBox
    {
        private bool _isResizing;
        private Color _borderColor = Color.FromArgb(180, 190, 200);
        private int _borderThickness = 1;

        [Category("Circle PictureBox")]
        [DefaultValue(typeof(Color), "180, 190, 200")]
        public Color CircleBorderColor
        {
            get
            {
                return _borderColor;
            }
            set
            {
                _borderColor = value;
                Invalidate();
            }
        }

        [Category("Circle PictureBox")]
        [DefaultValue(1)]
        public int CircleBorderThickness
        {
            get
            {
                return _borderThickness;
            }
            set
            {
                _borderThickness = Math.Max(0, value);
                Invalidate();
            }
        }

        public CirclePictureBox()
        {
            SizeMode = PictureBoxSizeMode.Zoom;
            BorderStyle = BorderStyle.None;
            BackColor = Color.FromArgb(245, 247, 250);

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            MakeSizeSquare();
            ApplyCircleRegion();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_borderThickness <= 0)
                return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (Pen pen = new Pen(_borderColor, _borderThickness))
            {
                float inset = _borderThickness / 2f + 1f;

                RectangleF borderRectangle = new RectangleF(
                    inset,
                    inset,
                    Width - (_borderThickness + 2),
                    Height - (_borderThickness + 2)
                );

                e.Graphics.DrawEllipse(pen, borderRectangle);
            }
        }

        private void MakeSizeSquare()
        {
            if (_isResizing)
                return;

            int size = Math.Min(Width, Height);

            if (size <= 0)
                return;

            if (Width == size && Height == size)
                return;

            _isResizing = true;
            Size = new Size(size, size);
            _isResizing = false;
        }

        private void ApplyCircleRegion()
        {
            if (Width <= 0 || Height <= 0)
                return;

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(0, 0, Width - 1, Height - 1);

                Region oldRegion = Region;
                Region = new Region(path);
                oldRegion?.Dispose();
            }
        }
    }
}