using System;
using System.Collections.Generic;

namespace DefaultNamespace
{
    public class ListWithSwapping<T>
    {
        private List<T> elements;

        public bool isEmpty()
        {
            return elements.Count == 0;
        }
        
        public ListWithSwapping()
        {
            elements = new List<T>();
        }

        public void Add(T element)
        {
            elements.Add(element);
        }

        public T Get(int index)
        {
            if (isEmpty())
            {
                throw new InvalidOperationException("List is empty.");
            }

            if (index >= elements.Count || index < 0)
            {
                throw new InvalidOperationException("Invalid index.");
            }
            T element = elements[index];
            return element;
        }


        public void Remove(int index)
        {
            if (isEmpty())
            {
                throw new InvalidOperationException("List is empty.");
            }

            if (index >= elements.Count || index < 0)
            {
                throw new InvalidOperationException("Invalid index.");
            }
            elements.RemoveAt(index);
        }
        
        

        public void SwapTwoElements(int index1, int index2)
        {
            if (index1 < 0 || index1 >= elements.Count || index2 < 0 || index2 >= elements.Count)
            {
                throw new IndexOutOfRangeException("Invalid index for swap operation.");
            }

            if (index1 == index2 || elements.Count < 2)
                return;
            
            (elements[index1], elements[index2]) = (elements[index2], elements[index1]);
        } 

        public int Count()
        {
            return elements.Count;
        }

        public QueueIterator<T> getIterator()
        {
            return new QueueIterator<T>(this);
        }
    }
}