using OrderFlow.Shared.Exceptions;
using OrderFlow.Shared.Models.Ordering;

namespace OrderFlow.Ordering.Interfaces;

public interface IOrdersRepository
{
    /// <summary>
    ///     Создаёт новый заказ в базе.
    /// </summary>
    /// <param name="order">Модель заказа. Поле Id заполняется автоматически.</param>
    /// <returns>Возвращает идентификатор заказа</returns>
    /// <remarks>Поле Id у модели Order заполняется автоматически. При передачи в репозиторий следует оставить его пустым.</remarks>
    Task<Order> CreateAsync(Order order);

    /// <summary>
    ///     Обновляет сведения о заказе в базе данных
    /// </summary>
    /// <param name="order">Модель заказа с обновлёнными свойствами</param>
    /// <returns>Возвращает заказ</returns>
    /// <exception cref="EntityNotFoundException">Исключение при ненайденном заказе</exception>
    Task<Order> UpdateAsync(Order order);

    Task<Order?> FindByIdAsync(int id);
    Task<Order> GetByIdAsync(int id);
}