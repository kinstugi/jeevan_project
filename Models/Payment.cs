namespace backend.Models;

public class Payment{
    public int PaymentId { get; set; }
    public double Amount { get; set; }
    public User User { get; set; } = null!;
    public DateTime DateOfPayment { get; set; }
}