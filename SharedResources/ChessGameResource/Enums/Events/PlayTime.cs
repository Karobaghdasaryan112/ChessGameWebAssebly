namespace SharedResources.ChessGameResource.Enums.Events
{
    //default Uses classical
    public enum PlayEvent
    {
        None = 0,
        Bullet = 60,     // 1 min
        Blitz = 180,     // 3 min
        Rapid = 900,     // 15 min
        Classical = 1800 // 30 min
    }
}
