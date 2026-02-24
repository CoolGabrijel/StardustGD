namespace Stardust;

public class Replay
{
    public bool IsMultiplayer { get; set; }
    public PawnType[] Characters  { get; set; }
    public RoomType[] Rooms  { get; set; }
    public string[] Objectives  { get; set; }
    public IUndoableAction[] Actions  { get; set; }
}