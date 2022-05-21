using System.Collections.Generic;

namespace AllOverIt.Pagination
{
    public interface IContinuationTokenEncoder
    {
        // The caller can create a previous/next page token as desired - the first/last row is selected based on the direction
        string EncodePreviousPage<TEntity>(IReadOnlyCollection<TEntity> references) where TEntity : class;
        string EncodeNextPage<TEntity>(IReadOnlyCollection<TEntity> references) where TEntity : class;

        // Allows a continuation token to be created based on an individual reference row. Any object with the
        // required columns are available as properties.
        //
        // Note: Cannot use <TEntity> because the compiler may choose this overload when it should choose the
        //       IReadOnlyCollection<TEntity> version (when TEntity is an anonymous object and the caller
        //       does not (cannot) explicitly declare using <TEntity>.
        string EncodePreviousPage(object reference);        
        string EncodeNextPage(object reference);

        string EncodeFirstPage();
        string EncodeLastPage();
    }
}
