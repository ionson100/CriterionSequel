namespace CriterionMore
{
    /// <summary>
    /// Интрефейс, который должны реализовывать типы, которые отвечаю за доставку минимального и максимального значения для слайдерного показа
    /// </summary>
    public interface ISliderData
    {
        /// <summary>
        /// Максимальное значение
        /// </summary>
        /// <returns></returns>
        decimal GetMax();

        /// <summary>
        /// Минимальное значение
        /// </summary>
        /// <returns></returns>
        decimal GetMin();
    }
}