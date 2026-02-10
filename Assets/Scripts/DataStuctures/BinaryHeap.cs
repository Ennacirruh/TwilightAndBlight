using System;
using System.Collections.Generic;
using UnityEngine;
namespace TwilightAndBlight.Collections
{
    public class BinaryHeap<TCollection> where TCollection: IComparable
    {
        private List<TCollection> heapList;

        public int Count { get { return heapList.Count; } }

        private int current_heap_size;

        //public BinaryHeap()
        //{
        //    heapList = new List<TCollection>();
        //}

        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs; ;
            lhs = rhs;  
            rhs = temp;
        }

        private int Parent(int key)
        {
            return (key - 1) / 2;
        }
        private int Left(int key)
        {
            return 2 * key + 1;
        }
        private int Right(int key)
        {
            return 2 * key + 2;
        }

        public bool InsertKey(TCollection key)
        {
            int i = heapList.Count;
            heapList.Add(key);
            while( i != 0 && heapList[i].CompareTo(heapList[Parent(i)]) < 0)
            {
                //Swap(ref heapList[])

            }


            return false;
        }


    }
}