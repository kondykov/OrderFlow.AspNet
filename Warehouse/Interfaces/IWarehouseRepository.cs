using OrderFlow.Shared.Exceptions;
using OrderFlow.Shared.Models.Ordering;
using OrderFlow.Shared.Models.Warehouse;

namespace OrderFlow.Stock.Interfaces;

public interface IWarehouseRepository
{
    /// <summary>
    ///     Добавление прихода продукта на склад
    /// </summary>
    /// <param name="product">Продукт резерва</param>
    /// <param name="quantity">Количество прихода на склад</param>
    /// <returns>Возвращает текущие резервы со склада</returns>
    Task<ProductReserve> AddProductReserveAsync(Product product, double quantity);

    /// <summary>
    ///     Изъятие со склада продукта
    /// </summary>
    /// <param name="product">Продукт резерва</param>
    /// <param name="quantity">Количество изъятия на склад</param>
    /// <returns>Возвращает текущие резервы со склада</returns>
    Task<ProductReserve> TakeProductReserveAsync(Product product, double quantity);

    /// <summary>
    ///     Поиск резервов на складе по продукту
    /// </summary>
    /// <param name="product">Искомый продукт</param>
    /// <returns>Возвращает текущие резервы со склада</returns>
    Task<ProductReserve?> FindProductReserveAsync(Product product);

    /// <summary>
    ///     Получает текущие резервы на складе.
    /// </summary>
    /// <param name="product"></param>
    /// <returns>Возвращает текущие резервы со склада</returns>
    /// <exception cref="EntityNotFoundException">Исключение при нахождении неизвестного продукта</exception>
    Task<ProductReserve> GetProductReserveAsync(Product product);
}