using System;
using System.Collections;
using System.Collections.Generic;

namespace FreeCell
{
    public class RandomCardGenerator : IEnumerable
    {
        private RandomCardGeneratorEnumerator randomCardGenertorEnumerator = new RandomCardGeneratorEnumerator();

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this.randomCardGenertorEnumerator;
        }

        #endregion

        private class RandomCardGeneratorEnumerator : IEnumerator
        {
            private IList<int> availableCards = new List<int>();
            private int currentCard = -1;
            Random random = new Random();

            public RandomCardGeneratorEnumerator()
            {
                this.Reset();
            }

            #region IEnumerator Members

            public object Current
            {
                get 
                {
                    if (currentCard == -1)
                    {
                        throw new InvalidOperationException("Should call MoveNext() on the enumerator before getting current value");
                    }
                    return currentCard; 
                }
            }

            public bool MoveNext()
            {
                if (availableCards.Count != 0)
                {
                    currentCard = availableCards[random.Next(0, availableCards.Count)];
                    availableCards.Remove(currentCard);
                }

                if (availableCards.Count == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            public void Reset()
            {
                availableCards.Clear();

                for (int i = 1; i <= 52; ++i)
                {
                    availableCards.Add(i);
                }

                currentCard = -1;
            }

            #endregion
        }
    }
}