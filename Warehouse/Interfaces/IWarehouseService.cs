using OrderFlow.Shared.Exceptions;
using OrderFlow.Shared.Models.Ordering;
using OrderFlow.Shared.Models.Warehouse;

namespace OrderFlow.Stock.Interfaces;

public interface IWarehouseService
{
    /// <summary>
    ///     Добавление прихода продукта на склад
    /// </summary>
    /// <param name="productId">Идентификатор продукта резерва</param>
    /// <param name="quantity">Количество прихода на склад</param>
    /// <returns>Возвращает текущие резервы со склада</returns>
    Task<ProductReserve> AddProductReserveAsync(int productId, double quantity);

    /// <summary>
    ///     Изъятие со склада продукта
    /// </summary>
    /// <param name="productId">Идентификатор продукта резерва</param>
    /// <param name="quantity">Количество изъятия на склад</param>
    /// <returns>Возвращает текущие резервы со склада</returns>
    Task<ProductReserve> TakeProductReserveAsync(int productId, double quantity);

    /// <summary>
    ///     Поиск резервов на складе по продукту
    /// </summary>
    /// <param name="productId">Искомый продукт</param>
    /// <returns>Возвращает текущие резервы со склада</returns>
    Task<ProductReserve?> FindProductReserveAsync(int productId);

    /// <summary>
    ///     Получает текущие резервы на складе.
    /// </summary>
    /// <param name="productId">Идентификатор продукта резерва</param>
    /// <returns>Возвращает текущие резервы со склада</returns>
    /// <exception cref="EntityNotFoundException">Исключение при нахождении неизвестного продукта</exception>
    Task<ProductReserve> GetProductReserveAsync(int productId);
}