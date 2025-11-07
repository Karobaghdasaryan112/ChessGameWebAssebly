using System;
using System.Collections.Generic;

namespace ServerSideClientUI.Contracts
{
    public interface IQueryBuilder
    {
        public Uri BuildPath(Uri action, List<KeyValuePair<string, string>> queryParametrsNameAndValue);
    }
}
