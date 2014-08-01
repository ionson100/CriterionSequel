using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using CriterionMore;

namespace TestCriterion.CriterionModel
{
    /// <summary>
    /// Тип отвечающий за поставку  для фильтра
    /// </summary>
    public class BodyListItem : IListItem
    {
        Dictionary<string, int?> f = new Dictionary<string, int?> { { "-----------", null }, { "adidas", 1 }, { "nike", 2 }, { "puma", 3 } };

        public IEnumerable<ListItem> GetListItems()
        {
            return f.Select(i => new ListItem(i.Key, i.Value == null ? "null" : i.Value.ToString()));
        }
    }
}