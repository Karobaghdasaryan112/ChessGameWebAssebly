using System;
using System.Collections.Generic;

namespace ChessGameBlazorClient.Contracts
{
    public interface IQueryBuilder
    {
        public Uri BuildPath(Uri action, List<KeyValuePair<string, string>> queryParametrsNameAndValue);
    }
}
