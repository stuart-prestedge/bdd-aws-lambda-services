using System;

namespace BDDReferenceService
{
    
    internal static class RandomHelper
    {

        /**
         * The first random number.
         */
        private const Int64 RANDOM_NUMBER_START = 10001;

        /**
         * The next pseudo-random number.
         */
        private static Int64 NextRandom = RANDOM_NUMBER_START;

        /**
         * Reset the next pseudo-random number.
         */
        internal static void Reset() {
            Debug.Tested();

            NextRandom = RANDOM_NUMBER_START;
        }

        /**
         * Get the next pseudo-random number.
         * If maxValue is not specified, the number will be unique, starting from RANDOM_NUMBER_START.
         * If maxValue is specified, the number will be unique from zero to (maxValue-1).
         * If a unique number cannot be generated, an exception is thrown.
         */
        internal static Int64 NextNumber(Int64 maxValue = Int64.MaxValue) {
            Debug.Tested();

            Int64 nextRandom = NextRandom++;
            if (maxValue < Int64.MaxValue) {
                Debug.Tested();
                nextRandom -= RANDOM_NUMBER_START;
                if (nextRandom >= maxValue) {
                    Debug.Untested();
                    throw new Exception();
                } else {
                    Debug.Tested();
                }
            } else {
                Debug.Tested();
            }
            return nextRandom;
        }

        /**
         * Get the next pseudo-random number.
         * If maxValue is not specified, the number will be unique, starting from RANDOM_NUMBER_START.
         * If maxValue is specified, the number will be unique from zero to (maxValue-1).
         * If a unique number cannot be generated, an exception is thrown.
         */
        internal static string Next(Int64 maxValue = Int64.MaxValue) {
            Debug.Tested();

            return NextNumber().ToString();
        }

    }   // RandomHelper

}   // BDDReferenceService
