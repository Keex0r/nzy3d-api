using nzy3D.Colors;
using nzy3D.Events;
using nzy3D.Maths;
using nzy3D.Plot3D.Rendering.View;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace nzy3D.Plot3D.Primitives
{
    public class GroupedLineScatter : AbstractDrawable, ISingleColorable
    {

        private Color[] _colors;
        private Color _color;
        /// <summary>
        /// Each list is rendered as an individual scatter plot with lines between the points
        /// </summary>
        private List<Coord3d[]> _coordinates;
        private float _pointwidth;
        private float _linewidth;

        public GroupedLineScatter()
        {
            _bbox = new BoundingBox3d();
            PointWidth = 1;
            Color = Color.BLACK;
        }

        public GroupedLineScatter(List<Coord3d[]> coordinates) :
                this(coordinates, Color.BLACK)
        {
        }

        public GroupedLineScatter(List<Coord3d[]> coordinates, Color rgb, float width = 1)
        {
            _bbox = new BoundingBox3d();
            Data = coordinates;
            PointWidth = width;
            Color = rgb;
        }

        public GroupedLineScatter(List<Coord3d[]> coordinates, Color[] colors, float width = 1)
        {
            _bbox = new BoundingBox3d();
            Data = coordinates;
            PointWidth = width;
            Colors = colors;
        }

        public void Clear()
        {
            _coordinates = null;
            _bbox.reset();
        }

        public override void Draw(Camera cam)
        {

            _transform?.Execute();

            
            if (_coordinates != null)
            {
                int k = 0;
                foreach (Coord3d[] c in _coordinates)
                {
                    GL.PointSize(_pointwidth);
                    GL.Begin(BeginMode.Points);
                    if (_colors == null)
                    {
                        GL.Color4(_color.r, _color.g, _color.b, _color.a);
                    }
                    if ((_colors != null))
                    {
                        GL.Color4(_colors[k].r, _colors[k].g, _colors[k].b, _colors[k].a);
                        k++;
                        if (k >= _colors.Length) k = 0; //Roll over the color list
                    }
                    //Draw all points and lines for this series
                    foreach(Coord3d p in c)
                    {
                        GL.Vertex3(p.x, p.y, p.z);
                    }
                    GL.End();
                    GL.LineWidth(LineWidth);
                    GL.Begin(BeginMode.LineStrip);
                    foreach (Coord3d p in c)
                    {
                        GL.Vertex3(p.x, p.y, p.z);
                    }
                    GL.End();
                }
            }

            // doDrawBounds (MISSING)

        }

        public override Transform.Transform Transform
        {
            get => _transform;
            set
            {
                _transform = value;
                UpdateBounds();
            }
        }

        private void UpdateBounds()
        {
            _bbox.reset();
            // Iterate over all groups of points to update the bounds
            foreach (var s in _coordinates)
            {
                foreach (var c in s)
                {
                    _bbox.add(c);
                }
            }
        }

        private List<Coord3d[]> Data
        {
            get => _coordinates;
            set
            {
                _coordinates = value;
                UpdateBounds();
            }
        }

        private Color[] Colors
        {
            get => _colors;
            set
            {
                _colors = value;
                fireDrawableChanged(new DrawableChangedEventArgs(this, DrawableChangedEventArgs.FieldChanged.Color));
            }
        }

        private float PointWidth
        {
            get => _pointwidth;
            set => _pointwidth = value;
        }

        private float LineWidth
        {
            get => _linewidth;
            set => _linewidth = value;
        }

        public Color Color
        {
            get => _color;
            set => _color = value;
        }
    }
}