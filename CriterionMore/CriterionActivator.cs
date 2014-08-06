using System;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CriterionMore;
using CriterionMore.Properties;

[assembly: PreApplicationStartMethod(typeof(PreApplicationStart), "Start")]
namespace CriterionMore
{

    /// <summary>
    /// Add rout class
    /// </summary>
    public static class PreApplicationStart
    {
      
        const string Namerouter = "CriterionHelpText";
        private const string UrlPouter = "ss/{controller}/{action}/{id}";


        /// <summary>
        /// Add Rout method
        /// </summary>
        public static void Start()
        {
            RouteTable.Routes.MapRoute(Namerouter, UrlPouter,
                        namespaces: new[] { "CriterionMore" },
                        defaults: new { controller = "Help", action = "Index", id = UrlParameter.Optional });

        }
    }
    /// <summary>
    /// 
    /// </summary>
    public static class CriterionActivator
    {

        /// <summary>
        ///  Имя атрибута [name] для скрытого поля
        /// </summary>

        internal const string NameType = "nametypecriterion";




        /// <summary>
        /// Выпадающий список для выбора сортировки по полям
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="name">Название блока списка</param>
        /// <param name="orderBys">Контейнер опции для выпадающего списка</param>
        /// <typeparam name="T">Целевой тип</typeparam>
        /// <returns></returns>
        public static MvcHtmlString CriterionOrderBy<T>(this HtmlHelper helper, string name, params OrderBy<T>[] orderBys) where T : class
        {
            var res = MapCriterion<T>.GetOrderByHtml(name, orderBys, new FormCollection(HttpContext.Current.Request.Form));
            return MvcHtmlString.Create(res);
        }



        /// <summary>
        /// Получение HTML разметки шаблонного показа, шаблон назначается через атрибут CriterionTemplateAttribute
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static MvcHtmlString CriterionHtmlTemplate<T>(this HtmlHelper helper) where T : class
        {
            var res = MapCriterion<T>.RenderingPartPage();
            return MvcHtmlString.Create(res);
        }

        /// <summary>
        /// Получение HTML разметки для шаблонного показа,  шаблон назначается пользователем
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="nameTemplate">url шаблона</param>
        /// <returns></returns>
        public static MvcHtmlString CriterionHtmlTemplate<T>(this HtmlHelper helper, string nameTemplate) where T : class
        {
            var res = MapCriterion<T>.RenderingPartPage(nameTemplate);
            return MvcHtmlString.Create(res);
        }


        /// <summary>
        /// Получение частичной разметки HTML для представления Razor включая JS
        /// </summary>
        /// <param name="helper">HtmlHelper helper</param>
        /// <param name="expression">вырадение свойства типа ( bvody=>body.id)</param>
        /// <typeparam name="T">Целевой тип</typeparam>
        /// <returns>Разметка HTML включая Jscript</returns>
        public static MvcHtmlString Criterion<T>(this HtmlHelper helper, Expression<Func<T, object>> expression) where T : class
        {
            var name = MapCriterion<T>.GetNamePropery(expression);
            var dirtyHtml = MapCriterion<T>.GetIdHtml(name);

            var res = MapCriterion<T>.RenderingPartViewRazor(helper, dirtyHtml);
            var key = helper.ViewData["assa312312assa"];
            if (key == null)
            {
                res = String.Concat(res, String.Format("<script type=\"text/javascript\">{0}</script>", Resources.Base));
                res = MapCriterion<T>.AddinHtml(res);
                helper.ViewData["assa312312assa"] = 1;
            }
            return MvcHtmlString.Create(res);
        }

        /// <summary>
        /// Получение результирующего выражения, для подстановки в Where
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetExpression<T>() where T : class
        {
            var exp = MapCriterion<T>.GetExpressions();
            return exp;
        }


        /// <summary>
        /// Получение результирующего выражения, для подстановки в OrderBy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, object>> GetExpressionOrderBy<T>() where T : class
        {
               var exp=  MapCriterion<T>.GetExpressionOrderBy();
            return exp;
        }

        /// <summary>
        /// Получение результирующего выражения, для подстановки в OrderBy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, object>> GetExpressionOrderBy<T>(FormCollection collection) where T : class
        {

            var exp = MapCriterion<T>.GetExpressionOrderBy(collection);
            return exp;
        }




        /// <summary>
        ///  Получение результирующего выражения, для подстановки в Where
        /// </summary>
        /// <param name="collection">Коллекция значений Forms с клиента</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetExpression<T>(FormCollection collection) where T : class
        {
            var exp = MapCriterion<T>.GetExpressions(collection);
            return exp;
        }



        internal static string UrlImage { get; set; }

        internal static Type TypeHelp { get; set; }


        /// <summary>
        /// Назначение url иконки для показа кнопки справки
        /// </summary>
        /// <param name="url">url иконки</param>
        public static void SetUrlImageHelp(string url)
        {
            UrlImage = url;
        }

        /// <summary>
        /// Назначение типа поставщика данных для показа справки,тип должен наследовать ITypeHelp, и иметь конструктор по умолчанию
        /// </summary>
        /// <param name="type"></param>
        public static void SetTypeHelp(Type type)
        {
            TypeHelp = type;
        }
    }

    /// <summary>
    /// Структура контейнер для опции выпадающего списка для сортировки показа
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct OrderBy<T>
    {

        /// <summary>
        /// ctor.
        /// </summary>
        /// <param name="name">Текст опции</param>
        /// <param name="expression">Выражение для  определение свойства,  по которому будет производиться сортировка</param>
        /// <param name="isselect">Выбор опции по умолчанию</param>
        public OrderBy(string name, Expression<Func<T, object>> expression,bool isselect) : this()
        {
            Expression = expression;
            Name = name;
            IsSelect = isselect;

        }
        /// <summary>
        /// Выражение поля к торому относится сортировка
        /// </summary>
        public Expression<Func<T, object>> Expression { get; set; }
        /// <summary>
        /// Название выпадающей опции
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Выьрана ли опция при формировании
        /// </summary>
        public bool IsSelect { get; set; }
    }
}
