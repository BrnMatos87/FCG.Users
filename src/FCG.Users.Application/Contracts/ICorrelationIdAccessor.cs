namespace FCG.Users.Application.Contracts;

public interface ICorrelationIdAccessor
{
    Guid Get();
    void Set(Guid correlationId);
}