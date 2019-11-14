using System;
using CoreAnimation;
using CoreGraphics;
using RestaurantApp.CustomControlls;
using RestaurantApp.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ButtonFrame), typeof(ButtonFrameRenderer))]
namespace RestaurantApp.iOS.Renderers
{
    public class ButtonFrameRenderer : FrameRenderer
    {
        public override void LayoutSublayersOfLayer(CALayer layer)
        {
            base.LayoutSublayersOfLayer(layer);

            var path = UIBezierPath.FromRoundedRect(this.Bounds, UIRectCorner.TopRight | UIRectCorner.TopLeft , new CGSize(1, 1));
            var mask = new CAShapeLayer();
            mask.Path = path.CGPath;
            this.Layer.Mask = mask;
            this.ClipsToBounds = true;
            this.Layer.CornerRadius = 20;
            this.Layer.MaskedCorners = CACornerMask.MaxXMaxYCorner | CACornerMask.MaxXMinYCorner;
        }
    }
}
