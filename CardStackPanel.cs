using System.Windows;
using System.Windows.Controls;

namespace FreeCell
{
	public class CardStackPanel : StackPanel
	{
        public double ArrangeOffset
        {
            get { return (double)base.GetValue(ArrangeOffsetProperty); }
            set { base.SetValue(ArrangeOffsetProperty, value); }
        }

        public static readonly DependencyProperty ArrangeOffsetProperty = DependencyProperty.Register("ArrangeOffset", typeof(double), typeof(CardStackPanel), new PropertyMetadata(10.0));
	
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            Rect childArrangeRect = new Rect(arrangeSize);
            childArrangeRect.Y = -1 * this.ArrangeOffset;

            for (int i = 0; i < this.InternalChildren.Count; i++)
            {
                UIElement child = this.InternalChildren[i];
                childArrangeRect.Y += this.ArrangeOffset;
                childArrangeRect.Height = child.DesiredSize.Height;
                child.Arrange(childArrangeRect);
            }

            return arrangeSize;
        }
	}
}
