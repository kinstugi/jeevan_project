namespace backend.Models;

public class Child{
    public int ChildId { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}