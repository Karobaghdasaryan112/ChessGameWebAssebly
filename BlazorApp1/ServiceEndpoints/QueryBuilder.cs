using WebAssemblyChessGame.UI.Contracts;

namespace WebAssemblyChessGame.UI.ServiceEndpoints
{
    public class QueryBuilder : IQueryBuilder
    {
        /// <summary>
        /// Builds a new URI by appending query parameters to the given base URI.
        /// </summary>
        /// <param name="action">The base URI to which the query parameters will be appended.</param>
        /// <param name="queryParametrsNameAndValue">A list of key-value pairs representing the query parameters.</param>
        /// <returns>
        /// A new <see cref="Uri"/> that includes the original base path and the constructed query string.
        /// If no query parameters are provided, returns the original URI.
        /// </returns>

        public Uri BuildPath(Uri action, List<KeyValuePair<string, string>> queryParametrsNameAndValue)
        {
            if (queryParametrsNameAndValue == null || queryParametrsNameAndValue.Count == 0)
                return action;

            var query = string.Join("&", queryParametrsNameAndValue
                                    .Select(p => $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));

            var separator = action.ToString().Contains("?") ? "&" : "?";

            return new Uri($"{action}{separator}{query}", UriKind.RelativeOrAbsolute);
        }

    }
}
