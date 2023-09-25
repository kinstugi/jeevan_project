namespace backend.Models;

public class Payment{
    public int PaymentId { get; set; }
    public double Amount { get; set; }
    public User User { get; set; } = null!;
    public DateTime DateOfPayment { get; set; }
    public bool Confirmed { get; set; } = false; // this will be done my parent of 
}