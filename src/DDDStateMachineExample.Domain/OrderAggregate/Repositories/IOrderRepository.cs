using JustPlatform.Domain;

namespace DDDStateMachineExample.Domain.OrderAggregate.Repositories;

/// <summary>
/// Репозиторий для управления сущностью заявки.
/// </summary>
public interface IOrderRepository : IRepository<Order>
{
    /// <summary>
    /// Получение информации о заявке.
    /// </summary>
    /// <param name="orderId">Идентификатор заявки.</param>
    /// <param name="token">Токен отмены операции.</param>
    /// <returns>Сущность заявки.</returns>
    Task<Order> Get(long orderId, CancellationToken token);
    
    /// <summary>
    /// Обновления сущности заявки.
    /// </summary>
    /// <param name="order">Заявка для обновления.</param>
    /// <param name="token">Токен отмены операции.</param>
    /// <returns>Задача.</returns>
    Task Update(Order order, CancellationToken token);
}