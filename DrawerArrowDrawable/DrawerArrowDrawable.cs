/*
* Copyright (C) 2014 Chris Renke
* Copyright (C) 2014 Tomasz Cielecki
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;

namespace dk.ostebaronen.dad
{
    public class DrawerArrowDrawable
        : Drawable
    {
        /** Paths were generated at a 3px/dp density; this is the scale factor for different densities. */
        private const float PathGenDensity = 3;

        /** Paths were generated with at this size for {@link DrawerArrowDrawable#PATH_GEN_DENSITY}. */
        private const float DimenDp = 23.5f;

        /**
        * Paths were generated targeting this stroke width to form the arrowhead properly, modification
        * may cause the arrow to not for nicely.
        */
        private const float StrokeWidthDp = 2;

        private readonly float[] _coordsA = {0f, 0f};
        private readonly float[] _coordsB = {0f, 0f};
        private readonly bool _rounded;
        private readonly BridgingLine _bottomLine;

        private readonly Rect _bounds;
        private readonly float _halfStrokeWidthPixel;
        private readonly Paint _linePaint;
        private readonly BridgingLine _middleLine;
        private readonly BridgingLine _topLine;
        private bool _flip;
        private float _parameter, _magnitude, _paramA, _paramB, _vX, _vY;

        public DrawerArrowDrawable(Resources resources)
            : this(resources, false)
        { }

        public DrawerArrowDrawable(Resources resources, bool rounded)
        {
            _rounded = rounded;
            float density = resources.DisplayMetrics.Density;
            float strokeWidthPixel = StrokeWidthDp * density;
            _halfStrokeWidthPixel = strokeWidthPixel / 2;

            _linePaint = new Paint(PaintFlags.SubpixelText | PaintFlags.AntiAlias)
            {
                StrokeCap = rounded ? Paint.Cap.Round : Paint.Cap.Butt,
                Color = Color.Black,
                StrokeWidth = strokeWidthPixel
            };
            _linePaint.SetStyle(Paint.Style.Stroke);

            var dimen = (int) (DimenDp * density);
            _bounds = new Rect(0, 0, dimen, dimen);

            //Top
            var first = new Path();
            first.MoveTo(5.042f, 20f);
            first.RCubicTo(8.125f, -16.317f, 39.753f, -27.851f, 55.49f, -2.765f);
            var second = new Path();
            second.MoveTo(60.531f, 17.235f);
            second.RCubicTo(11.301f, 18.015f, -3.699f, 46.083f, -23.725f, 43.456f);
            ScalePath(first, density);
            ScalePath(second, density);
            var joinedA = new JoinedPath(first, second);

            first = new Path();
            first.MoveTo(64.959f, 20f);
            first.RCubicTo(4.457f, 16.75f, 1.512f, 37.982f, -22.557f, 42.699f);
            second = new Path();
            second.MoveTo(42.402f, 62.699f);
            second.CubicTo(18.333f, 67.418f, 8.807f, 45.646f, 8.807f, 32.823f);
            ScalePath(first, density);
            ScalePath(second, density);
            var joinedB = new JoinedPath(first, second);
            _topLine = new BridgingLine(joinedA, joinedB, this);

            // Middle
            first = new Path();
            first.MoveTo(5.042f, 35f);
            first.CubicTo(5.042f, 20.333f, 18.625f, 6.791f, 35f, 6.791f);
            second = new Path();
            second.MoveTo(35f, 6.791f);
            second.RCubicTo(16.083f, 0f, 26.853f, 16.702f, 26.853f, 28.209f);
            ScalePath(first, density);
            ScalePath(second, density);
            joinedA = new JoinedPath(first, second);

            first = new Path();
            first.MoveTo(64.959f, 35f);
            first.RCubicTo(0f, 10.926f, -8.709f, 26.416f, -29.958f, 26.416f);
            second = new Path();
            second.MoveTo(35f, 61.416f);
            second.RCubicTo(-7.5f, 0f, -23.946f, -8.211f, -23.946f, -26.416f);
            ScalePath(first, density);
            ScalePath(second, density);
            joinedB = new JoinedPath(first, second);
            _middleLine = new BridgingLine(joinedA, joinedB, this);

            // Bottom
            first = new Path();
            first.MoveTo(5.042f, 50f);
            first.CubicTo(2.5f, 43.312f, 0.013f, 26.546f, 9.475f, 17.346f);
            second = new Path();
            second.MoveTo(9.475f, 17.346f);
            second.RCubicTo(9.462f, -9.2f, 24.188f, -10.353f, 27.326f, -8.245f);
            ScalePath(first, density);
            ScalePath(second, density);
            joinedA = new JoinedPath(first, second);

            first = new Path();
            first.MoveTo(64.959f, 50f);
            first.RCubicTo(-7.021f, 10.08f, -20.584f, 19.699f, -37.361f, 12.74f);
            second = new Path();
            second.MoveTo(27.598f, 62.699f);
            second.RCubicTo(-15.723f, -6.521f, -18.8f, -23.543f, -18.8f, -25.642f);
            ScalePath(first, density);
            ScalePath(second, density);
            joinedB = new JoinedPath(first, second);
            _bottomLine = new BridgingLine(joinedA, joinedB, this);
        }

        public override int IntrinsicHeight
        {
            get { return _bounds.Height(); }
        }

        public override int IntrinsicWidth
        {
            get { return _bounds.Width(); }
        }


        public override int Opacity
        {
            get { return (int) Format.Translucent; }
        }

        public bool Flip
        {
            get { return _flip; }
            set
            {
                _flip = value;
                InvalidateSelf();
            }
        }

        public float Parameter
        {
            get { return _parameter; }
            set
            {
                if (value > 1 || value < 0)
                    throw new ArgumentOutOfRangeException("value", "Argument must be between 1 and zero inclusive");
                _parameter = value;
                InvalidateSelf();
            }
        }

        public Color StrokeColor
        {
            get { return _linePaint.Color; }
            set
            {
                _linePaint.Color = value;
                InvalidateSelf();
            }
        }

        public override void Draw(Canvas canvas)
        {
            if (Flip)
            {
                canvas.Save();
                canvas.Scale(1f, -1f, IntrinsicWidth / 2f, IntrinsicHeight / 2f);
            }

            _topLine.Draw(canvas);
            _middleLine.Draw(canvas);
            _bottomLine.Draw(canvas);

            if (Flip) canvas.Restore();
        }

        public override void SetAlpha(int alpha)
        {
            _linePaint.Alpha = alpha;
            InvalidateSelf();
        }

        public override void SetColorFilter(ColorFilter cf)
        {
            _linePaint.SetColorFilter(cf);
            InvalidateSelf();
        }

        private static void ScalePath(Path path, float density)
        {
            if (NearlyEqual(density, PathGenDensity, 0.00001f)) return;
            var scaleMatrix = new Matrix();
            scaleMatrix.SetScale(density / PathGenDensity, density / PathGenDensity, 0, 0);
            path.Transform(scaleMatrix);
        }

        public static bool NearlyEqual(float a, float b, float epsilon)
        {
            float absA = Math.Abs(a);
            float absB = Math.Abs(b);
            float diff = Math.Abs(a - b);

            if (a == b)
            {
                // shortcut, handles infinities
                return true;
            }
            if (a == 0 || b == 0 || diff < float.MinValue)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < (epsilon * float.MinValue);
            } // use relative error
            return diff / (absA + absB) < epsilon;
        }

        private class BridgingLine
        {
            private readonly DrawerArrowDrawable _drawable;
            private readonly JoinedPath _pathA;
            private readonly JoinedPath _pathB;

            public BridgingLine(JoinedPath pathA, JoinedPath pathB, DrawerArrowDrawable drawable)
            {
                _drawable = drawable;
                _pathA = pathA;
                _pathB = pathB;
            }

            public void Draw(Canvas canvas)
            {
                _pathA.GetPointOnLine(_drawable._parameter, _drawable._coordsA);
                _pathB.GetPointOnLine(_drawable._parameter, _drawable._coordsB);
                if (_drawable._rounded) InsetPointsForRoundedCaps();
                canvas.DrawLine(_drawable._coordsA[0], _drawable._coordsA[1], _drawable._coordsB[0],
                    _drawable._coordsB[1], _drawable._linePaint);
            }

            private void InsetPointsForRoundedCaps()
            {
                _drawable._vX = _drawable._coordsB[0] - _drawable._coordsA[0];
                _drawable._vY = _drawable._coordsB[1] - _drawable._coordsA[1];
                _drawable._magnitude = (float) Math.Sqrt((_drawable._vX * _drawable._vX + _drawable._vY * _drawable._vY));
                _drawable._paramA = (_drawable._magnitude - _drawable._halfStrokeWidthPixel) / _drawable._magnitude;
                _drawable._paramB = _drawable._halfStrokeWidthPixel / _drawable._magnitude;
                _drawable._coordsA[0] = _drawable._coordsB[0] - (_drawable._vX * _drawable._paramA);
                _drawable._coordsA[1] = _drawable._coordsB[1] - (_drawable._vY * _drawable._paramA);
                _drawable._coordsB[0] = _drawable._coordsB[0] - (_drawable._vX * _drawable._paramB);
                _drawable._coordsB[1] = _drawable._coordsB[1] - (_drawable._vY * _drawable._paramB);
            }
        }

        private class JoinedPath
        {
            private readonly float _lengthFirst;
            private readonly float _lengthSecond;
            private readonly PathMeasure _measureFirst;
            private readonly PathMeasure _measureSecond;

            public JoinedPath(Path pathFirst, Path pathSecond)
            {
                _measureFirst = new PathMeasure(pathFirst, false);
                _measureSecond = new PathMeasure(pathSecond, false);
                _lengthFirst = _measureFirst.Length;
                _lengthSecond = _measureSecond.Length;
            }

            public void GetPointOnLine(float parameter, float[] coords)
            {
                if (parameter <= 0.5f)
                {
                    parameter *= 2;
                    _measureFirst.GetPosTan(_lengthFirst * parameter, coords, null);
                }
                else
                {
                    parameter -= .5f;
                    parameter *= 2;
                    _measureSecond.GetPosTan(_lengthSecond * parameter, coords, null);
                }
            }
        }
    }
}