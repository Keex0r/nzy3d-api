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
            LineWidth = 1;
            Color = Color.BLACK;
        }

        /// <summary>
        /// Initializes a new GroupedLineScatter object
        /// </summary>
        /// <param name="coordinates">A list of coordinate arrays. Each list item is rendered as a separate series in a different color.</param>
        public GroupedLineScatter(List<Coord3d[]> coordinates) :
                this(coordinates, Color.BLACK)
        {
        }

        /// <summary>
        /// Initializes a new GroupedLineScatter object
        /// </summary>
        /// <param name="coordinates">A list of coordinate arrays. Each list item is rendered as a separate series in a different color.</param>
        /// <param name="rgb">The default color. It is recommended to use the <see cref="GroupedLineScatter.GroupedLineScatter(List{Coord3d[]}, Color[], float, float)"/> constructor and a provide a list of colors.</param>
        /// <param name="pointwidth">The size of the scatter markers</param>
        /// <param name="linewidth">The width of the line</param>
        public GroupedLineScatter(List<Coord3d[]> coordinates, Color rgb, float pointwidth = 8, float linewidth = 2)
        {
            _bbox = new BoundingBox3d();
            Data = coordinates;
            this.PointWidth = pointwidth;
            this.LineWidth = linewidth;
            Color = rgb;
        }

        /// <summary>
        /// Initializes a new GroupedLineScatter object
        /// </summary>
        /// <param name="coordinates">A list of coordinate arrays. Each list item is rendered as a separate series in a different color.</param>
        /// <param name="colors">A list of colors. The series are colored in order of this list with rollover if more series than colors are provided</param>
        /// <param name="pointwidth">The size of the scatter markers</param>
        /// <param name="linewidth">The width of the line</param>
        public GroupedLineScatter(List<Coord3d[]> coordinates, Color[] colors, float pointwidth = 8, float linewidth = 2)
        {
            _bbox = new BoundingBox3d();
            Data = coordinates;
            this.PointWidth = pointwidth;
            this.LineWidth = linewidth;
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
                GL.PointSize(_pointwidth);
                GL.LineWidth(LineWidth);
                //Enable in order to provide correct z-indexing of the series
                GL.Enable(EnableCap.DepthTest);
                GL.DepthFunc(DepthFunction.Less);
                //Render each set of coordinates
                foreach (Coord3d[] c in _coordinates)
                {
                    //Setup colors
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
                    //Draw points
                    if (PointWidth > 0)
                    {
                        GL.Begin(BeginMode.Points);
                        foreach (Coord3d p in c)
                        {
                            GL.Vertex3(p.x, p.y, p.z);
                        }
                        GL.End();
                    }
                    //Draw lines
                    if (LineWidth > 0)
                    {
                        GL.Begin(BeginMode.LineStrip);
                        foreach (Coord3d p in c)
                        {
                            GL.Vertex3(p.x, p.y, p.z);
                        }
                        GL.End();
                    }
                }
            }
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