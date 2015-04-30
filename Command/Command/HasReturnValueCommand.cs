namespace Dotpay.Command
{
    public class Command<T> : DFramework.Command
    {
        public T CommandResult { get; set; }
    }
}
