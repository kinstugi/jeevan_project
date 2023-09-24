using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class User{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? AffiliateLink { get; set; }
    public byte[] PasswordSalt { get; set; } = null!;
    public byte[] PasswordHash { get; set; } = null!;
    public List<Child> Children { get; } = new List<Child>();
    public List<Payment> Payments { get; } = new List<Payment>();


    public bool AddChild(Child child){
        var res = Children.Where(ch => ch.ChildId == child.ChildId).ToList();
        if (res.Count > 0) return false; // check if user has not already used referral link for this particular user
        Children.Add(child);
        return true;
    }
}

public class UserDTO{
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}