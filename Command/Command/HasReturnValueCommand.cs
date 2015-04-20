namespace Dotpay.Command
{
    public class HasReturnValueCommand<T> : DFramework.Command
    {
        public T CommandResult { get; set; }
    }
}
