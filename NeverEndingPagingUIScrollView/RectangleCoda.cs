using System;
using System.Drawing;

namespace NeverEndingPagingUIScrollView
{
	public static class RectangleCoda
	{
		public static float GetMaxX (this RectangleF self)
		{
			return self.X + self.Width;
		}
	}
}

