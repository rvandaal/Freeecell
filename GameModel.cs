using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Timers;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;

namespace FreeCell
{
	public class GameModel : INotifyPropertyChanged
	{
		private bool isValidGame;
		private Timer timer;
		private TimeSpan gameTimeSpan;
        private RandomCardGenerator randomCardGenerator;
        private Card selectedCard = null;
        private int numberOfCardsLeft = 52;
		private bool isValidMove = true;
		private string message = "";

		public bool IsValidGame
		{
			get { return isValidGame; }
			set 
			{ 
				isValidGame = value;
				this.onPropertyChanged("IsValidGame");
			}
		}

		public string Message
		{
			get { return this.message; }
			set
			{
				this.message = value;
				this.onPropertyChanged("Message");
			}
		}

		public bool IsValidMove
		{
			get { return this.isValidMove; }
			set
			{
				this.isValidMove = value;
				this.onPropertyChanged("IsValidMove");
			}
		}

		public NewGameCommand NewGame
		{
			get { return new NewGameCommand(this); }
		}
		
		public string TimeElapsed
		{
			get { return gameTimeSpan.ToString(); }
		}

        private ObservableCollection<Cell> homeCells = new ObservableCollection<Cell>();
        public ObservableCollection<Cell> HomeCells
        {
            get { return this.homeCells; }
        }

        private ObservableCollection<Cell> freeCells = new ObservableCollection<Cell>();
        public ObservableCollection<Cell> FreeCells
        {
            get { return this.freeCells; }
        }

        private IList<ObservableCollection<Card>> cardStacks = new List<ObservableCollection<Card>>(8);
        public IList<ObservableCollection<Card>> CardStacks
        {
            get { return this.cardStacks; }
        }

		public GameModel()
		{
			this.IsValidGame = false;
			timer = new Timer(1000);
			timer.AutoReset = true;
			timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);

            for (int index = 0; index < 4; ++index)
            {
                this.HomeCells.Add(new Cell(CellState.Home, index));
                this.FreeCells.Add(new Cell(CellState.Free, index));
            }

            for (int index = 0; index < 8; ++index)
            {
                this.CardStacks.Add(new ObservableCollection<Card>());
            }

            randomCardGenerator = new RandomCardGenerator();
		}

        public void StartNewGame()
		{
            //Reset the free and home cells
            for (int index = 0; index < 4; ++index)
            {
                this.HomeCells[index].ContainsCard = false;
                this.FreeCells[index].ContainsCard = false;
            }
            
            //Initialize the card stacks
            randomCardGenerator.GetEnumerator().Reset();

            int cardStackIndex;
            for (cardStackIndex = 0; cardStackIndex < 8; ++cardStackIndex)
            {
                this.CardStacks[cardStackIndex].Clear();
            }

            for (cardStackIndex = 0; cardStackIndex < 4; ++cardStackIndex)
            {
                for (int cardIndex = 0; cardIndex < 7; ++cardIndex)
                {
                    randomCardGenerator.GetEnumerator().MoveNext();
                    this.CardStacks[cardStackIndex].Add(new Card((int)randomCardGenerator.GetEnumerator().Current, cardStackIndex, cardIndex));
                }
            }

            for (cardStackIndex = 4; cardStackIndex < 8; ++cardStackIndex)
            {
                for (int cardIndex = 0; cardIndex < 6; ++cardIndex)
                {
                    randomCardGenerator.GetEnumerator().MoveNext();
                    this.CardStacks[cardStackIndex].Add(new Card((int)randomCardGenerator.GetEnumerator().Current, cardStackIndex, cardIndex));
                }
            }

            gameTimeSpan = new TimeSpan(0, 0, 0);
            this.IsValidGame = true;
            timer.Stop();
            timer.Start();
            this.selectedCard = null;
            numberOfCardsLeft = 52;
			this.IsValidMove = true;
		}

        public void HitCardStack(int cardStackIndex)
        {
            if (this.IsValidGame && this.selectedCard != null)
            {
                if (this.CardStacks[cardStackIndex].Count == 0)
                {
                    //TODO: Need to support column movements
                    //TODO: Need to also impose maximum column count
                    this.CardStacks[cardStackIndex].Add(new Card(selectedCard.CardNumber, cardStackIndex, 0));
                    this.RemoveCardFromSource(selectedCard);

                    selectedCard.IsSelected = false;
                    selectedCard = null;
                }
            }
        }

        public void HitCard(Card hitCard)
        {
            Debug.Assert(hitCard.CardState == CardState.Stack);

            if (selectedCard != null)
            {
                //In the middle of a move
                Debug.Assert(hitCard.CardState != CardState.Stack);

                if (selectedCard != hitCard)
                {
                    //TODO: Need to support column movements
                    //TODO: Need to also impose maximum column count
                    if (this.IsValidStackDrop(selectedCard, hitCard))
                    {
                        this.CardStacks[hitCard.CardStackNumber].Add(new Card(selectedCard.CardNumber, hitCard.CardStackNumber, hitCard.PositionInStack + 1));
                        this.RemoveCardFromSource(selectedCard);
                    }
                    else
                    {
                        this.ShowMessage("That move is not allowed.", "Move error");
                    }
                }

                selectedCard.IsSelected = false;
                selectedCard = null;
            }
            else
            {
                if ((hitCard.PositionInStack + 1) == this.CardStacks[hitCard.CardStackNumber].Count)
                {
                    hitCard.IsSelected = true;
                    selectedCard = hitCard;
                }
            }
        }

        public void HitCell(Cell hitCell)
        {
            if (selectedCard != null)
            {
                if (hitCell.CellState == CellState.Free)
                {
                    if (hitCell.ContainsCard)
                    {
                        if (hitCell.Card != selectedCard)
                        {
                            //Trying to dock over an existing card.
                            this.ShowMessage("That move is not allowed.", "Move error");
                        }
                    }
                    else
                    {
                        this.RemoveCardFromSource(selectedCard);
                        hitCell.Card = new Card(selectedCard.CardNumber, CardState.Free);
                        hitCell.ContainsCard = true;
                    }
                }
                else if (hitCell.CellState == CellState.Home)
                {
                    if (this.IsValidHomeCellDrop(hitCell, selectedCard))
                    {
                        this.RemoveCardFromSource(selectedCard);
                        hitCell.Card = new Card(selectedCard.CardNumber, CardState.Home);
                        hitCell.ContainsCard = true;
                        numberOfCardsLeft--;
                    }
                    else
                    {
                        this.ShowMessage("That move is not allowed.", "Move error");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Could not determine the state of the cell");
                }

                selectedCard.IsSelected = false;
                selectedCard = null;
            }
            else
            {
                if ((hitCell.CellState == CellState.Free )&& hitCell.ContainsCard)
                {
                    hitCell.Card.IsSelected = true;
                    selectedCard = hitCell.Card;
                }
            }

            if (numberOfCardsLeft <= 0)
            {
				this.timer.Stop();
                this.ShowMessage("Congratulations! You win. Time taken: " + gameTimeSpan.ToString(), "You win!");
            }
        }

        private void RemoveCardFromSource(Card card)
        {
            Debug.Assert(card.CardState != CardState.Home);

            if (card.CardState == CardState.Stack)
            {
                //Card is be removed from the stack.
                this.CardStacks[card.CardStackNumber].RemoveAt(card.PositionInStack);
            }
            else if (card.CardState == CardState.Free)
            {
                //Card is being removed from a Free cell.
                int cellIndex = GetCellIndexForCard(this.FreeCells, card);
                this.FreeCells[cellIndex].ContainsCard = false;
            }
        }

        //TODO: This function could probably be inlined..
        private int GetCellIndexForCard(ObservableCollection<Cell> targetCollection, Card targetCard)
        {
            foreach (Cell cell in targetCollection)
            {
                if (cell.ContainsCard)
                {
                    if (cell.Card.CardNumber == targetCard.CardNumber)
                    {
                        return cell.CellIndex;
                    }
                }
            }

            throw new InvalidOperationException("Could not locate card in collection");
        }

        private bool IsValidHomeCellDrop(Cell targetCell, Card card)
        {
            Debug.Assert(targetCell.CellState == CellState.Home);

            if (targetCell.ContainsCard)
            {
                //Target home cell is not empty, so valid drop has to be onenumerically higher than cell content card number
                if ((targetCell.Card.Suite == card.Suite) && (card.CardNumber == (targetCell.Card.CardNumber + 1)))
                {
                    return true;
                }
            }
            else
            {
                //Target home cell is empty, so only valid drop is an ace.
                if (card.SuiteIndependentCardNumber == 1)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsValidStackDrop(Card sourceCard, Card targetCard)
        {
            Debug.Assert(sourceCard != null);
            Debug.Assert(targetCard != null);

            //Target card has to be of a different color.
            if ((sourceCard.Suite == Suite.Hearts) || (sourceCard.Suite == Suite.Dice))
            {
                if ((targetCard.Suite != Suite.Clubs) && (targetCard.Suite != Suite.Spades))
                {
                    return false;
                }
            }
            else
            {
                if ((targetCard.Suite != Suite.Hearts) && (targetCard.Suite != Suite.Dice))
                {
                    return false;
                }
            }

            //Target card has to be greater than Ace, and has to greater than the source card
            if((targetCard.SuiteIndependentCardNumber <= 1) || ((targetCard.SuiteIndependentCardNumber - 1) != sourceCard.SuiteIndependentCardNumber))
            {
                return false;
            }

            return true;
        }

        private void ShowMessage(string message, string windowTitle)
        {
        	this.Message = message;
        	this.IsValidMove = false;
        	this.IsValidMove = true;
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			gameTimeSpan = gameTimeSpan.Add(new TimeSpan(0, 0, 1));
			this.onPropertyChanged("TimeElapsed");
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		private void onPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public class NewGameCommand : ICommand
		{
			private GameModel gameModel;

			public NewGameCommand(GameModel gameModel)
			{
				this.gameModel = gameModel;
			}

			#region ICommand Members

			public bool CanExecute(object parameter)
			{
				return true;
			}

			public event EventHandler CanExecuteChanged;

			public void Execute(object parameter)
			{
				gameModel.StartNewGame();
			}
			#endregion
		}
	}
}