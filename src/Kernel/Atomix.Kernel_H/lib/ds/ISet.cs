﻿/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* Copyright (c) 2015, Atomix Development, Inc - All Rights Reserved                                        *
*                                                                                                          *
* Unauthorized copying of this file, via any medium is strictly prohibited                                 *
* Proprietary and confidential                                                                             *
* Written by Aman Priyadarshi <aman.eureka@gmail.com>, March 2016                                          *
*                                                                                                          *
*   Namespace     ::  Atomix.Kernel_H.lib                                                                  *
*   File          ::  IDictionary.cs                                                                       *
*                                                                                                          *
*   Description                                                                                            *
*       File Contains Dictionary implementation                                                            *
*                                                                                                          *
*   History                                                                                                *
*       24-03-2016      Aman Priyadarshi      Implementation                                               *
*                                                                                                          *
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;

using Atomix.Kernel_H.core;

namespace Atomix.Kernel_H.lib
{
    public class ISet<_type>
    {
        const uint Capacity = (1 << 5);//Should be a power of 2

        uint mModulo;
        Bucket[] mBuckets;

        HashFunction<_type> mFunction;
        EqualityFunction<_type> mEquality;

        class Bucket
        {
            public _type mKey;
            public Bucket mNext;
        }

        public ISet(HashFunction<_type> aFunction, EqualityFunction<_type> aEquality)
        {
            mFunction = aFunction;
            mEquality = aEquality;
            mModulo = Capacity - 1;
            mBuckets = new Bucket[Capacity];
        }

        public bool ContainsKey(_type mKey)
        {
            uint Index = mFunction(mKey) & mModulo;
            Bucket Current = mBuckets[Index];

            while (Current != null && !mEquality(Current.mKey, mKey))
                Current = Current.mNext;

            return (Current != null);
        }
        
        public void RemoveKey(_type mKey)
        {
            uint Index = mFunction(mKey) & mModulo;
            Bucket Current = mBuckets[Index];

            if (Current == null)
                throw new Exception("[ISet]: Key not present!");

            if (mEquality(Current.mKey, mKey))
                mBuckets[Index] = Current.mNext;

            while (Current.mNext != null && !mEquality(Current.mNext.mKey, mKey))
                Current = Current.mNext;

            if (Current.mNext == null)
                throw new Exception("[ISet]: Key not present!");

            var ToDelete = Current.mNext;
            Current.mNext = ToDelete.mNext;
            Heap.Free(ToDelete);//Free bucket
        }
    }
}