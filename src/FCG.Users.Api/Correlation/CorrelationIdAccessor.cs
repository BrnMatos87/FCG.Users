using FCG.Users.Application.Contracts;

namespace FCG.Users.Api.Correlation;

public class CorrelationIdAccessor : ICorrelationIdAccessor
{
    private Guid _correlationId;

    public Guid Get()
    {
        return _correlationId;
    }

    public void Set(Guid correlationId)
    {
        _correlationId = correlationId;
    }
}