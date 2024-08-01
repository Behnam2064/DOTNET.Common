namespace DOTNET.Common.Extensions.Collections
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> observableCollection, IEnumerable<T> rangeList)
        {
            foreach (T item in rangeList)
            {
                observableCollection.Add(item);
            }
        }
    }
}
