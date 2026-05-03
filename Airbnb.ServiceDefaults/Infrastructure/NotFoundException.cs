namespace Airbnb.ServiceDefaults.Infrastructure;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}
