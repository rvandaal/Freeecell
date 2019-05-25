using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace FreeCell
{
    public enum CellState
    {
        Home,
        Free
    }

	public class Cell : INotifyPropertyChanged
	{
        private bool containsCard;

        public bool ContainsCard
        {
            get { return containsCard; }
            set 
            { 
                containsCard = value;
                this.OnPropertyChanged("ContainsCard");
            }
        }

        private Card card;

        public Card Card
        {
            get { return card; }
            set 
            { 
                card = value;
                this.OnPropertyChanged("Card");
            }
        }

        private CellState cellState;

        public CellState CellState
        {
            get { return cellState; }
            set { cellState = value; }
        }
	
        private int cellIndex;

        public int CellIndex
        {
            get { return cellIndex; }
            set { cellIndex = value; }
        }

        public Cell(CellState cellState, int cellIndex)
        {
            this.CellState = cellState;
            this.CellIndex = cellIndex;
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
