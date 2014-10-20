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

using Android.App;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Android.OS;
using dk.ostebaronen.dad;

namespace Sample
{
    [Activity(Label = "Sample", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private DrawerArrowDrawable _drawerArrowDrawable;
        private float _offset;
        private bool _flipped;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            var imageView = FindViewById<ImageView>(Resource.Id.drawer_indicator);
            
            _drawerArrowDrawable = new DrawerArrowDrawable(Resources)
            {
                StrokeColor = Resources.GetColor(Resource.Color.light_gray)
            };
            imageView.SetImageDrawable(_drawerArrowDrawable);

            drawer.DrawerSlide += (sender, args) =>
            {
                _offset = args.SlideOffset;

                if (_offset >= .995)
                {
                    _flipped = true;
                    _drawerArrowDrawable.Flip = _flipped;
                }
                else if (_offset <= .005)
                {
                    _flipped = false;
                    _drawerArrowDrawable.Flip = _flipped;
                }

                _drawerArrowDrawable.Parameter = _offset;
            };

            imageView.Click += (sender, args) =>
            {
                if (drawer.IsDrawerVisible((int)GravityFlags.Start))
                    drawer.CloseDrawer((int)GravityFlags.Start);
                else
                    drawer.OpenDrawer((int)GravityFlags.Start);
            };

            var styleButton = FindViewById<TextView>(Resource.Id.indicator_style);
            var rounded = false;
            styleButton.Click += (sender, args) =>
            {
                styleButton.Text = rounded ? "See Rounded" : "See Squared";
                rounded = !rounded;

                _drawerArrowDrawable = new DrawerArrowDrawable(Resources, rounded)
                {
                    Parameter = _offset,
                    Flip = _flipped,
                    StrokeColor = Resources.GetColor(Resource.Color.light_gray)
                };

                imageView.SetImageDrawable(_drawerArrowDrawable);
            };
        }
    }
}

