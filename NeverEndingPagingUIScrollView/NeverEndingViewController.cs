using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace NeverEndingPagingUIScrollView
{
	public partial class NeverEndingViewController : UIViewController
	{
		HashSet<PageViewController> VisiblePages = new HashSet<PageViewController> ();
		HashSet<PageViewController> RecycledPages = new HashSet<PageViewController> ();
		UIScrollView PagingView;
		bool isAnimating;

		public NeverEndingViewController ()
			: base ("NeverEndingViewController", null)
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
			PagingView = new UIScrollView (View.Bounds) {
				PagingEnabled = true,
				ShowsHorizontalScrollIndicator = false,
			};
			PagingView.Scrolled += (sender, e) => {
				if (!isAnimating)
					isAnimating = true;
				TilePages ();
			};
			PagingView.ScrollAnimationEnded += (sender, e) => {
				isAnimating = false;
			};
			PagingView.DecelerationEnded += (sender, e) => {
				isAnimating = false;
				Console.WriteLine (" VisiblePages: Count={0}; Indexes={1}", VisiblePages.Count, string.Join (", ", VisiblePages.Select (p => p.Index)));
				Console.WriteLine ("RecycledPages: Count={0}; Indexes={1}", RecycledPages.Count, string.Join (", ", RecycledPages.Select (p => p.Index)));
			};
			View.AddSubview (PagingView);
			TilePages ();
		}

		bool IsDisplayingPageForIndex (int index)
		{
			return VisiblePages.Any (p => p.Index == index);
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

		PageViewController DequeueRecycledPage ()
		{
			PageViewController viewController = RecycledPages.FirstOrDefault ();
			if (viewController != null) {
				RecycledPages.Remove (viewController);
			}
			return viewController;
		}
		
		RectangleF FrameForPageAtIndex (int index)
		{
			SizeF pageSize = PagingView.Bounds.Size;
			return new RectangleF (index * pageSize.Width, 0, pageSize.Width, pageSize.Height);
		}
		
		void AddPageWithIndex (int index)
		{
			PageViewController viewController = DequeueRecycledPage ();
			if (viewController == null)
				viewController = new PageViewController ();
			viewController.View.Frame = FrameForPageAtIndex (index);
			viewController.Index = index;
			PagingView.AddSubview (viewController.View);
			VisiblePages.Add (viewController);
		}
		
		int CurrentPageIndex {
			get {
				return (int)((PagingView.ContentOffset.X /
				              PagingView.Bounds.Size.Width));
			}
		}
		
		void ResizePagingViewContentSize ()
		{
			PagingView.ContentSize = new SizeF (
					PagingView.Bounds.Size.Width * (CurrentPageIndex + 2),
					PagingView.Bounds.Size.Height);
		}
		
		void TilePages ()
		{
			var visibleBounds = PagingView.Bounds;
			int firstNeededPageIndex = (int) Math.Floor (visibleBounds.X / visibleBounds.Width);
			int lastNeededPageIndex  = (int) Math.Floor ((visibleBounds.GetMaxX ()-1) / visibleBounds.Width);
			int historyCount = 5;
			firstNeededPageIndex = Math.Max (firstNeededPageIndex-historyCount, 0);
			lastNeededPageIndex  = Math.Max (lastNeededPageIndex+1, 1);
			
			// Recycle unneeded controllers
			foreach (var vc in VisiblePages) {
				if (vc.Index < firstNeededPageIndex || vc.Index > lastNeededPageIndex) {
					RecycledPages.Add (vc);
					vc.View.RemoveFromSuperview ();
				}
			}
			VisiblePages.ExceptWith (RecycledPages);

			// Add missing pages
			for (int i = firstNeededPageIndex; i <= lastNeededPageIndex; ++i) {
				if (!IsDisplayingPageForIndex (i))
					AddPageWithIndex (i);
			}

			ResizePagingViewContentSize ();
		}
	}
}

