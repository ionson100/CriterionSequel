using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace CriterionMore
{
    /// <summary>
    /// Интерфейс, который должны реализовывать все поставщики данных для списочных контролов
    /// </summary>
    public interface IListItem
    {
        /// <summary>
        /// Перечисление ListItem
        /// </summary>
        /// <returns></returns>
        IEnumerable<ListItem> GetListItems();
    }
}