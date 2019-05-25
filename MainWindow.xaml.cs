using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Collections.ObjectModel;

namespace FreeCell
{
	public partial class MainWindow
	{
        private GameModel gameModel;
		
		public MainWindow()
		{
			// This assumes that you are navigating to this scene.
			// If you will normally instantiate it via code and display it
			// manually, you either have to call InitializeComponent by hand or
			// uncomment the following line.
			// this.InitializeComponent();

			// Insert code required on object creation below this point.
		}
		private void OnCardAreaMouseClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			// TODO: Add event handler implementation here.
            if (e.ClickCount == 1)
            {
                FrameworkElement element = e.OriginalSource as FrameworkElement;
                if (element != null)
                {
                    Card card = element.DataContext as Card;
                    if (card != null)
                    {
                        gameModel.HitCard(card);
                    }

                    Cell cell = element.DataContext as Cell;
                    if (cell != null)
                    {
                        gameModel.HitCell(cell);
                    }

                    CardStackPanel cardStackPanel = element as CardStackPanel;
                    if (cardStackPanel != null)
                    {
                        ObservableCollection<Card> hitCardStack = ((ItemsControl)cardStackPanel.TemplatedParent).DataContext as ObservableCollection<Card>;
                        for(int index = 0; index < gameModel.CardStacks.Count; ++index)
                        {
                            if (hitCardStack == this.gameModel.CardStacks[index])
                            {
                                gameModel.HitCardStack(index);
                                return;
                            }
                        }
                    }
                }
            }
		}

        private void OnWindowLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // TODO: Add event handler implementation here.
            this.gameModel = this.Resources["GameModelDS"] as GameModel;
        }
	}
}
