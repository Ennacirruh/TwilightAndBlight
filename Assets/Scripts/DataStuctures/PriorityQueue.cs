using UnityEngine;
using System.Collections.Generic;
using System;
namespace TwilightAndBlight.Collections
{
    public class PriorityQueue<TCollection> where TCollection : IComparable
    {
        public PriorityQueue() { }
        private List<TCollection> collection = new List<TCollection>();
        private HashSet<TCollection> unorderedCollection = new HashSet<TCollection>();
        private static int Parent(int i) => (i - 1) / 2;
        private static int LeftChild(int i) => 2 * i + 1;
        private static int RightChild(int i) => 2 * i + 2;
        public int Count { get { return collection.Count; } }
        private static void ShiftUp(int i, List<TCollection> arr)
        {
            while (i > 0 && arr[Parent(i)].CompareTo(arr[i]) > 0)
            {
                TCollection temp = arr[Parent(i)];
                arr[Parent(i)] = arr[i];
                arr[i] = temp;
                i = Parent(i);
            }
        }

        private static void ShiftDown(int i, List<TCollection> arr, int size)
        {
            int minIndex = i;
            int l = LeftChild(i);
            if(l < size && arr[l].CompareTo(arr[minIndex]) < 0) minIndex = l;
            int r = RightChild(i);
            if (r < size && arr[r].CompareTo(arr[minIndex]) < 0) minIndex = r;

            if (i != minIndex)
            {
                TCollection temp = arr[i];
                arr[i] = arr[minIndex];
                arr[minIndex] = temp;
                ShiftDown(minIndex, arr, size);
            }
        }
        public bool Contains(TCollection value)
        {
            return unorderedCollection.Contains(value);
        }
        

        public void Insert(TCollection p)
        {
            collection.Add(p);
            ShiftUp(collection.Count - 1, collection);
            unorderedCollection.Add(p);
        }

        public TCollection Pop()
        {
            if (collection.Count == 0) return default;
            TCollection result = collection[0];
            collection[0] = collection[collection.Count - 1];
            collection.RemoveAt(collection.Count - 1);
            ShiftDown(0, collection, collection.Count);
            unorderedCollection.Remove(result);
            return result;
        }
        public TCollection GetFirst()
        {
            if(collection.Count == 0) return default;
            return collection[0];
        }

        
    }
}
