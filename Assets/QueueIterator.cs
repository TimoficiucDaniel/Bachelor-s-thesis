using System;

namespace DefaultNamespace
{
    public class QueueIterator<T>
    {
        private ListWithSwapping<T> list;

        public QueueIterator(ListWithSwapping<T> list)
        {
            this.list = list;
        }

        public T getNext(){
            if (!hasMore())
            {
                throw new InvalidOperationException("List empty");
            }

            T element = list.Get(0);
            list.Remove(0);
            return element;
        }

        public bool hasMore()
        {
            return !list.isEmpty();
        }
    }
}