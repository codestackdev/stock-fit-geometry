using System.Linq;

namespace System.Collections.Generic
{
    public static class EnumerableExtension
    {
        public static bool AreEqualItemWise<T>(this IEnumerable<T> thisEnumerable,
            IEnumerable<T> other, IEqualityComparer<T> comparer = null)
        {
            if (thisEnumerable != null && other != null)
            {
                var thisEnum = thisEnumerable.GetEnumerator();
                var otherEnum = other.GetEnumerator();

                var thisHasValues = false;
                var otherHasValues = false;

                do
                {
                    thisHasValues = thisEnum.MoveNext();
                    otherHasValues = otherEnum.MoveNext();

                    if (!thisHasValues && !otherHasValues)
                    {
                        return true;//last element checked - enumerables are equal
                    }
                    else if(thisHasValues != otherHasValues)
                    {
                        return false; //different sizes
                    }

                    bool equal;

                    if (comparer == null)
                    {
                        equal = object.Equals(thisEnum.Current, otherEnum.Current);
                    }
                    else
                    {
                        equal = comparer.Equals(thisEnum.Current, otherEnum.Current);
                    }

                    if (!equal)
                    {
                        return false;
                    }
                } while (thisHasValues == otherHasValues);
            }

            return false;
        }
    }
}
