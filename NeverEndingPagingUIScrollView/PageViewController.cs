
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace NeverEndingPagingUIScrollView
{
	public partial class PageViewController : UIViewController
	{
		int index;
		public int Index {
			get {
				return index;
			}
			set {
				if (index != value) {
					index = value;
					IndexLabel.Text = index.ToString ();
				}
			}
		}

		static readonly UIColor[] Colors = new UIColor[]{
			UIColor.Green,
			UIColor.Gray,
			UIColor.Blue,
			UIColor.Cyan,
		};
		readonly Random rng = new Random ();

		public PageViewController () : base ("PageViewController", null)
		{
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
			View.BackgroundColor = Colors [rng.Next (0, Colors.Length)];
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Clear any references to subviews of the main view in order to
			// allow the Garbage Collector to collect them sooner.
			//
			// e.g. myOutlet.Dispose (); myOutlet = null;
			
			ReleaseDesignerOutlets ();
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
	}
}

