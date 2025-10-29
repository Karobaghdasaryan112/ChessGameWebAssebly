namespace WebAssemblyChessGame.UI.Contracts
{
    public interface IQueryBuilder
    {
        public Uri BuildPath(Uri action, List<KeyValuePair<string, string>> queryParametrsNameAndValue);
    }
}
