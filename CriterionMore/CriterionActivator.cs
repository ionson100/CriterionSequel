using System;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using CriterionMore.Properties;

namespace CriterionMore
{
    /// <summary>
    /// 
    /// </summary>
    public static class CriterionActivator
    {

        /// <summary>
        ///  Имя атрибута [name] для скрытого поля
        /// </summary>
        public const string NameType = "nametypecriterion";
        private const string UrlPouter = "ss/{controller}/{action}/{id}";
        internal static bool KeyAddUrl;


        private static void ActiveUrl(HtmlHelper helper)
        {
            if (!KeyAddUrl)
            {
                helper.RouteCollection.MapRoute("CriterionHelpText", UrlPouter,
                                            namespaces: new[] { "CriterionMore" },
                                            defaults: new { controller = "Help", action = "Index", id = UrlParameter.Optional });
                KeyAddUrl = true;
            }
        }



        /// <summary>
        /// Получение HTML разметки шаблонного показа, шаблон назначается через атрибут CriterionTemplateAttribute
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static MvcHtmlString CriterionHtmlTemplate<T>(this HtmlHelper helper) where T : class
        {
            ActiveUrl(helper);

            var res = MapCriterion<T>.RenderingPartPage();
            return  MvcHtmlString.Create(res);
        }

        /// <summary>
        /// Получение HTML разметки для шаблонного показа,  шаблон назначается пользователем
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="nameTemplate">url шаблона</param>
        /// <returns></returns>
        public static MvcHtmlString CriterionHtmlTemplate<T>(this HtmlHelper helper, string nameTemplate) where T : class
        {
            ActiveUrl(helper);

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
            ActiveUrl(helper);

            string name = GetNamePropery(expression);

          
            var dirtyHtml = MapCriterion<T>.GetIdHtml(name);

            var res = MapCriterion<T>.RenderingPartViewRazor(helper, dirtyHtml);
            var key = helper.ViewData["assa312312assa"];
            if (key == null)
            {
                res= String.Concat(res, String.Format("<script type=\"text/javascript\">{0}</script>", Resources.Base));
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
            var s = HttpContext.Current.Request.Form[NameType];
            if (s == null) return null;
            var exp =
                 MapCriterion<T>.GetExpressions(new FormCollection(HttpContext.Current.Request.Form));
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
       
            var exp =
                 MapCriterion<T>.GetExpressions(collection);
            return exp;
        }

        private static string GetNamePropery<T>(Expression<Func<T, object>> expression) where T : class
        {
            var body = expression.Body as MemberExpression ??  ((UnaryExpression)expression.Body).Operand as MemberExpression;
                     
            if (body != null) return  body.Member.Name;
            throw new Exception("Не могу получить значение  имя свойзсва из выражения");
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
}
