namespace Budget.Interfaces
{
    public interface IExpence :INumber {
       
        Interval Interval { get; set; }
    }

    public interface IBalance :INumber{ }
}