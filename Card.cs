using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace FreeCell
{
    public enum Suite
    {
        Hearts = 0,
        Spades,
        Dice,
        Clubs
    }

    public enum CardState
    {
        Free,
        Home,
        Stack
    }

    /// <summary>
    /// Cards are ordered in the following sequence: Hearts, Spades, Dice, Clubs
    /// i.e, Card number 1 corresponsonds to Ace of Hearts and Card number 52 corresponds to King of Clubs.
    /// </summary>
	public class Card : INotifyPropertyChanged
	{
        private int cardNumber;
        private static int numberOfCardsInSuite = 13;

        public int CardNumber
        {
            get { return cardNumber; }
            set { cardNumber = value; }
        }

        public Suite Suite
        {
            get
            {
                int classOfCard = (this.CardNumber - 1) / numberOfCardsInSuite;
                return (Suite)classOfCard;
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return this.isSelected; }
            set 
            { 
                this.isSelected = value;
                this.OnPropertyChanged("IsSelected");
            }
        }

        private CardState cardState;

        public CardState CardState
        {
            get { return cardState; }
            set { cardState = value; }
        }
	

        private int cardStackNumber;

        public int CardStackNumber
        {
            get { return cardStackNumber; }
            set { cardStackNumber = value; }
        }

        private int positionInStack;

        public int PositionInStack
        {
            get { return positionInStack; }
            set { positionInStack = value; }
        }

        public int SuiteIndependentCardNumber
        {
            get
            {
                int number = this.CardNumber % numberOfCardsInSuite;
                if (number == 0)
                {
                    return numberOfCardsInSuite;
                }
                else
                {
                    return number;
                }
            }
        }

        public string CardName
        {
            get
            {
                int number = this.SuiteIndependentCardNumber;
                string name;
                switch (number)
                {
                    case 1:
                        name = "A";
                        break;
                    case 13:
                        name = "K";
                        break;
                    case 12:
                        name = "Q";
                        break;
                    case 11:
                        name = "J";
                        break;
                    default:
                        name = number.ToString();
                        break;
                }

                return name;
            }
        }

        public string DisplayName
        {
            get { return this.CardName + this.Suite; }
        }
	
        public Card(int cardNumber, int cardStackNumber, int positionInStack)
        {
            this.CardNumber = cardNumber;
            this.CardState = CardState.Stack;
            this.CardStackNumber = cardStackNumber;
            this.PositionInStack = positionInStack;
        }

        public Card(int cardNumber, CardState cardState)
        {
            this.CardNumber = cardNumber;
            this.CardState = cardState;
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
