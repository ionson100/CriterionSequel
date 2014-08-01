using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using CriterionMore.Attributes;


namespace CriterionMore
{
    /// <summary>
    /// Выборка на основе регулярного выражения
    /// </summary>
    struct GroupItem
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GroupItem(string name1, String name2)
            : this()
        {
            FirstName = name1;
            LastName = name2;
        }

    }
    /// <summary>
    /// Тип, объект которого глобально привязывается к целевому типу
    /// </summary>
    public static class MapCriterion<T>
    {
        private static readonly Lazy<List<CriterionBaseAttribute>> BaseAttributes = new Lazy<List<CriterionBaseAttribute>>(
            () =>
            {
                var list = new List<CriterionBaseAttribute>();
                foreach (var propertyInfo in typeof(T).GetProperties())
                {
                    var htmlAttributes = propertyInfo.GetCustomAttributes(typeof(CriterionHtmlAttributeAttribute), true);
                    var i = propertyInfo.GetCustomAttributes(typeof(CriterionBaseAttribute), true);
                    if (!i.Any()) continue;
                    var ii = (CriterionBaseAttribute)i.Single();
                    if (string.IsNullOrWhiteSpace(ii.Id))
                    {
                        ii.Id = propertyInfo.Name.ToLower();
                    }
                    ii.PropertyInfo = propertyInfo;

                    if (htmlAttributes.Any())
                    {
                        ii.HtmlAttributes.AddRange(htmlAttributes.Cast<CriterionHtmlAttributeAttribute>());
                    }
                    list.Add(ii);


                }
                return list;
            }, LazyThreadSafetyMode.PublicationOnly);
       
        private static Lazy<List<GroupItem>> ItemsNameList = new Lazy<List<GroupItem>>(() => PathTemplate == null ? null : InnerTenplateList(HtmlPageDirty.Value, typeof(T)), LazyThreadSafetyMode.PublicationOnly);
        //private readonly Type _type;

        private static Lazy<string> HtmlPageDirty = new Lazy<string>(() => PathTemplate == null ? "" : InnerHtmlPageDirty(PathTemplate.Value, typeof(T)), LazyThreadSafetyMode.PublicationOnly
            );




        private static readonly Lazy<string> PathTemplate = new Lazy<string>(() =>
                                                                    {
                                                                        var t = typeof(T).GetCustomAttributes(typeof(CriterionTemplateAttribute), false);
                                                                        return t.Any() ? HttpContext.Current.Server.MapPath(((CriterionTemplateAttribute)t.Single()).Url) : string.Empty;
                                                                    }, LazyThreadSafetyMode.PublicationOnly);



        internal static string GetIdHtml(string properyName)
        {
            var tt = BaseAttributes.Value.Where(a => a.PropertyInfo.Name == properyName).Select(s => s.Id).SingleOrDefault();
            return tt == null ? "" : string.Format("#{0}#", tt);
        }


        /// <summary>
        /// Получение грязной разметки замещая включения
        /// </summary>
        /// <param name="pathTemplate">Путь для файла шаблонв</param>
        /// <param name="type">Целевой тип</param>
        /// <returns></returns>

        internal static string InnerHtmlPageDirty(string pathTemplate, Type type)
        {
            string htmlpatch;
            try
            {
                htmlpatch = File.ReadAllText(pathTemplate);
            }
            catch (Exception ex)
            {
                throw new Exception("Возможно не правильно указан url путь к шаблону  а атрибуте CriterionTemplateAttribute у типа:" + type.Name + " - " + ex);
            }

            return htmlpatch;

        }




        private static List<GroupItem> InnerTenplateList(string htmlDirti, Type type)
        {
            var list = new List<GroupItem>();
            var items = new Regex("#(.*)#").Matches(htmlDirti);
            for (int i = 0; i < items.Count; i++)
            {
                var val = items[i].Groups[1].Value;
                if (list.Any(a => a.FirstName == val))
                {
                    throw new ArgumentException(string.Format("В шаблоне: {0} типа,  оказались дублирующие значения: {1}", type.FullName, val));
                }

                list.Add(new GroupItem(val.ToLower(), items[i].Groups[0].Value));
            }
            return list;
        }

        /// <summary>
        /// Получение Html разметки из шаблона html 
        /// </summary>
        /// <param name="urlTemplate">при urlTemplate = null, разметка берется из атрибута CriterionTemplate типа </param>
        /// <returns></returns>
        internal static string RenderingPartPage(string urlTemplate = null)
        {
            if (string.IsNullOrWhiteSpace(urlTemplate))
            {
                return InnerRenderingPartPage(HtmlPageDirty.Value, BaseAttributes.Value, ItemsNameList.Value, typeof(T), false);
            }

            var path = HttpContext.Current.Server.MapPath(urlTemplate);
            var htmlPageDirty = InnerHtmlPageDirty(path, typeof(T));
            var itemsNameList = InnerTenplateList(htmlPageDirty, typeof(T));
            return InnerRenderingPartPage(htmlPageDirty, BaseAttributes.Value, itemsNameList, typeof(T), false);
        }

        /// <summary>
        /// Получение разметки для шаблона Razor
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="htmlPageDirty">Грязная разметка для замещения</param>
        /// <returns></returns>
        internal static string RenderingPartViewRazor(HtmlHelper helper, string htmlPageDirty)
        {
            var itemsNameList = InnerTenplateList(htmlPageDirty, typeof(T));
            return InnerRenderingPartPage(htmlPageDirty, BaseAttributes.Value, itemsNameList, typeof(T), true);
        }




        private static string InnerRenderingPartPage(string htmlPageDirty, List<CriterionBaseAttribute> baseAttributes, IEnumerable<GroupItem> itemsNameList, Type type, bool isOne)
        {
            if (itemsNameList == null)
            {
                throw new Exception(string.Format("ВОЗМОЖНО: для типа {0} не определен атрибут CriterionTemplateAttribute  с url шаблоном", type.Name));
            }
            var res = htmlPageDirty;
            var forms = HttpContext.Current.Request.Form;
            Validate(new FormCollection(forms), baseAttributes);
            foreach (var text in itemsNameList)
            {
                var text1 = text;
                var r = baseAttributes.Where(a => a.Id == text1.FirstName);
                var criterionBaseAttributes = r as CriterionBaseAttribute[] ?? r.ToArray();
                if (!criterionBaseAttributes.Any()) continue;
                var atr = criterionBaseAttributes.Single();
                var formValue = forms[text.FirstName];


                if (atr as CriterionListAttribute != null)
                {
                    res = res.Replace(text.LastName, RenderingList((CriterionListAttribute)atr, formValue));
                }
                if (atr as CriterionRadioButtonAttribute != null)
                {
                    res = res.Replace(text.LastName, RenderingRadio((CriterionRadioButtonAttribute)atr, formValue));//
                }

                if (atr as CriterionCheckBoxAttribute != null)
                {
                    var str = RenderingCheckBox((CriterionCheckBoxAttribute)atr, formValue);
                    res = res.Replace(text.LastName, str);
                }

                if (atr as CriterionDropDownAttribute != null)
                {
                    var str = RenderingDropDown((CriterionDropDownAttribute)atr, formValue);
                    res = res.Replace(text.LastName, str);
                }
                if (atr as CriterionBetweenDateAttribute != null)
                {
                    var str = RenderingBetweenDate((CriterionBetweenDateAttribute)atr, formValue);
                    res = res.Replace(text.LastName, str);
                }
                if (atr as CriterionSliderAttribute != null)
                {
                    var str = RenderingSlider((CriterionSliderAttribute)atr, formValue);
                    res = res.Replace(text.LastName, str);
                }

                if (atr as CriterionAutoCompleteAttribute != null)
                {
                    var str = RenderingAutoComplete((CriterionAutoCompleteAttribute)atr, formValue);
                    res = res.Replace(text.LastName, str);
                }

                if (atr as CriterionMyControlAttribute != null)
                {
                    var str = RenderinMyControl((CriterionMyControlAttribute)atr, formValue);
                    res = res.Replace(text.LastName, str);
                }
            }
            if (isOne == false)
            {
                res = RenameScripts(res);

                var finishres = AddinHtml(res, type);

                return finishres;
            }
            return res;
        }




        internal static string AddinHtml(string res, Type type)
        {
            return string.Concat(res, "<input   value=\"" + type.AssemblyQualifiedName + "\" name=\"" + CriterionActivator.NameType + "\" type=\"hidden\" />" +
                                     "<div id=\"dialog\"></div> ");//
        }

        private static string RenameScripts(string res)
        {
            var sb = new StringBuilder(Properties.Resources.Base);
            const string regtemplate = "<script\\b[^>]*>([\\s\\S]*?)<\\/script>";
            var col = Regex.Matches(res, regtemplate, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            foreach (Match val in col)
            {
                var vals = val.Groups[1];
                sb.Append(vals);
            }
            var str = sb.ToString();
#if(!DEBUG)
            var com=new JavaScriptCompressor();
            str= com.Compress(sb.ToString());
#endif
            return "<script type=\"text/javascript\">" + str + "</script>" + Regex.Replace(res, regtemplate, string.Empty);
        }



        #region рендеринг по атрибуту
        private static string RenderinMyControl(CriterionMyControlAttribute atr, string formValue)
        {
            if (atr.MyType == null)
            {
                throw new Exception(string.Format("У свойства {0}  в атрибуте:CriterionMyControlAttribute не определено сойство MyType, оно отвечает за генерацию списка ",
                    atr.PropertyInfo.Name));
            }
            var instans = Activator.CreateInstance(atr.MyType);
            var html = ((IMyType)instans).Rendering(atr, formValue);
            return html;
        }

        private static string RenderingAutoComplete(CriterionAutoCompleteAttribute atr, string formValue)
        {
            return
                Properties.Resources.AutoComplete.Replace("#id#", atr.Id)
                    .Replace("#name#", atr.Name)
                    .Replace("#url#", atr.Url)
                    .Replace("#minlength#", atr.MinLength.ToString(CultureInfo.InvariantCulture))
                    .Replace("#delay#", atr.Delay.ToString(CultureInfo.InvariantCulture))
                    .Replace("#value#", string.IsNullOrWhiteSpace(formValue) ? "" : formValue)
                    .Replace("#help#", GetHelpImage(atr.Id));
        }

        private static string GetHtmlAttributes(CriterionBaseAttribute atrBaseAttribute)
        {
            IEnumerable<CriterionHtmlAttributeAttribute> attributes = atrBaseAttribute.HtmlAttributes;
            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(atrBaseAttribute.Description))
            {
                sb.AppendFormat(" title = \"{0}\"", atrBaseAttribute.Description);
            }
            foreach (var atr in attributes)
            {
                sb.AppendFormat(" {0} = \"{1}\" ", atr.Key, atr.Value);
            }

            return sb.ToString();
        }

        private static string RenderingSlider(CriterionSliderAttribute atr, string formValue)
        {
            if (string.IsNullOrWhiteSpace(formValue))
                if (atr.ISliderData == null)
                {
                    throw new Exception(string.Format("У свойства {0}  в атрибуте:CriterionSliderAttribute не определено сойство ISliderData," +
                                                      " оно отвечает за генерацию начально и конечного значения слайдера ", atr.PropertyInfo.Name));
                }
            var selList = new List<string>();
            var instance = (ISliderData)Activator.CreateInstance(atr.ISliderData);
            if (!string.IsNullOrWhiteSpace(formValue))
            {
                selList.AddRange(formValue.Split(','));
            }
            else
            {
                selList.Add(instance.GetMin().ToString(CultureInfo.InvariantCulture));
                selList.Add(instance.GetMax().ToString(CultureInfo.InvariantCulture));


            }

            var res = Properties.Resources.Slider
                .Replace("#name#", atr.Name)
                .Replace("#id#", atr.Id)
                .Replace("#max#", instance.GetMax().ToString(CultureInfo.InvariantCulture))
                .Replace("#min#", instance.GetMin().ToString(CultureInfo.InvariantCulture)).
                 Replace("#step#", (int)atr.Step == 0 ? "10" : atr.Step.ToString(CultureInfo.InvariantCulture));

            return res.Replace("#value1#", selList[0])
                .Replace("#value2#", selList[1])
                .Replace("#formvalue#", formValue)
                .Replace("#atr#", GetHtmlAttributes(atr)).
                Replace("#help#", GetHelpImage(atr.Id));
        }

        private static string RenderingBetweenDate(CriterionBetweenDateAttribute atr, string formValue)
        {
            var selList = new List<string>();
            if (formValue != null)
            {
                selList.AddRange(formValue.Split(','));
            }
            if (selList.Count() > 2)
            {
                throw new Exception("Не верный диапазон дат, значение больше 2");
            }
            var res = Properties.Resources.Between
                .Replace("#id#", atr.Id)
                .Replace("#name#", atr.Name)
                .Replace("#atr#", GetHtmlAttributes(atr))
                .Replace("#help#", GetHelpImage(atr.Id));
            if (selList.Count < 2)
            {
                return res.Replace("#value1#", string.Empty).Replace("#value2#", string.Empty);
            }

            return res
                .Replace("#value1#", selList[0])
                .Replace("#value2#", selList[1]);


        }

        private static string RenderingDropDown(CriterionDropDownAttribute atr, string formValue)
        {
            if (atr.IListItem == null)
            {
                throw new Exception(string.Format("У свойства {0}  в атрибуте:CriterionDropDownAttribute не определено сойство IListItem, оно отвечает за генерацию списка ", atr.PropertyInfo.Name));
            }
            var itemIListItem = (IListItem)Activator.CreateInstance(atr.IListItem);
            var list = new List<string>();
            const string row = "<option  value=\"#value#\" #select# >#key#</option>";
            foreach (var it in itemIListItem.GetListItems())
            {
                list.Add(row.Replace("#value#", it.Value)
                    .Replace("#key#", it.Text)
                    .Replace("#select#", it.Value == formValue ? "selected=\"selected\"" : ""));

            }

            var templist =
            Properties.Resources.DropDown.
                Replace("#id#", atr.Id).
                Replace("#name#", atr.Name).
                Replace("#option#", string.Join(" ", list))
                .Replace("#atr#", GetHtmlAttributes(atr))
                .Replace("#help#", GetHelpImage(atr.Id));
            return templist;
        }

        private static string RenderingCheckBox(CriterionCheckBoxAttribute atr, string formValue)//selected="selected"
        {
            if (atr.ListItem == null)
            {
                throw new Exception(string.Format("У свойства {0}  в атрибуте:CriterionCheckBoxAttribute не определено сойство IListItem, оно отвечает за генерацию списка ", atr.PropertyInfo.Name));
            }

            var selList = new List<string>();
            if (formValue != null)
            {
                selList.AddRange(formValue.Split(','));
            }


            const string row = "<input data-criterion=\"1\" type=\"checkbox\" value=\"#value#\" class=\"checkbox icr\" #atr# name=\"#id#\" #checked#><label>#text#</label>";//id=\"#id#\"for=\"#id#\"


            var itemIListItem = (IListItem)Activator.CreateInstance(atr.ListItem);
            var sb = new StringBuilder("<table class=\"checktable\" style=\"border:0\">");
            if (atr.IsHorizontally)
            {
                sb.Append("<tr>");
            }

            foreach (var it in itemIListItem.GetListItems())
            {
                if (!atr.IsHorizontally)
                    sb.Append("<tr>");//checked="checked"
                sb.Append("<td>");

                sb.Append(row.Replace("#id#", atr.Id)
                    .Replace("#text#", it.Text)
                    .Replace("#value#", it.Value)
                    .Replace("#checked#", GetValueForOptions(selList, it.Selected, it.Value, "checked=\"checked\"")))
                    .Replace("#atr#", GetHtmlAttributes(atr));

                sb.Append("</td>");
                if (!atr.IsHorizontally)
                    sb.Append("</tr>");
            }
            if (atr.IsHorizontally)
            {
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            var res = Properties.Resources.CheckBox
                .Replace("#name#", atr.Name)
                .Replace("#body#", sb.ToString())
                .Replace("#help#", GetHelpImage(atr.Id));

            return res;

        }

        private static string RenderingRadio(CriterionRadioButtonAttribute atr, string formValue)
        {
            if (atr.ListItem == null)
            {
                throw new Exception(string.Format("У свойства {0}  в атрибуте:CriterionRadioButtonAttribute не определено сойство IListItem, оно отвечает за генерацию списка ", atr.PropertyInfo.Name));
            }
            const string row = " <input type=\"radio\" name=\"#id#\" data-criterion=\"1\" class=\"radio icr\" #atr# value=\"#value#\" #checked# />#text#";//id=\"#id#\"
            var itemIListItem = (IListItem)Activator.CreateInstance(atr.ListItem);
            var sb = new StringBuilder("<table class=\"radiotable\" style=\"border:0\" >");
            if (atr.IsHorizontally)
            {
                sb.Append("<tr>");
            }

            foreach (var it in itemIListItem.GetListItems())
            {
                if (!atr.IsHorizontally)
                    sb.Append("<tr>");//checked="checked"
                sb.Append("<td>");

                sb.Append(row
                    .Replace("#id#", atr.Id)
                    .Replace("#text#", it.Text)
                    .Replace("#value#", it.Value)
                    .Replace("#checked#", it.Value == formValue ? "checked=\"checked\"" : ""))
                    .Replace("#atr#", GetHtmlAttributes(atr));

                sb.Append("</td>");
                if (!atr.IsHorizontally)
                    sb.Append("</tr>");
            }
            if (atr.IsHorizontally)
            {
                sb.Append("</tr>");
            }
            sb.Append("</table>");

            var res = Properties.Resources.RadioButton
                .Replace("#name#", atr.Name)
                .Replace("#body#", sb.ToString())
                .Replace("#help#", GetHelpImage(atr.Id));


            return res;
        }

        private static string RenderingList(CriterionListAttribute atr, string formValue)
        {


            var selList = new List<string>();
            if (formValue != null)
            {
                selList.AddRange(formValue.Split(','));
            }

            if (atr.IListItem == null)
            {
                throw new Exception(string.Format("У свойства {0}  в атрибуте:CriterionListAttribute не определено сойство IListItem, оно отвечает за генерацию списка ", atr.PropertyInfo.Name));
            }
            const string row = "<option  value=\"#value#\" #select# >#key#</option>";

            var itemIListItem = (IListItem)Activator.CreateInstance(atr.IListItem);
            var list = new List<string>();
            foreach (var it in itemIListItem.GetListItems())
            {
                list.Add(row
                    .Replace("#value#", it.Value)
                    .Replace("#key#", it.Text)
                    .Replace("#select#", GetValueForOptions(selList, it.Selected, it.Value, "selected=\"selected\"")));
            }

            var templist =
            Properties.Resources.List.
                Replace("#id#", atr.Id).
                Replace("#size#", atr.Size.ToString(CultureInfo.InvariantCulture)).
                Replace("#multiple#", atr.IsMultiple ? "multiple=\"multiple\"" : string.Empty).
                Replace("#name#", atr.Name).
                Replace("#option#", string.Join(" ", list))
                .Replace("#atr#", GetHtmlAttributes(atr))
                .Replace("#help#", GetHelpImage(atr.Id));
            return templist;

        }

        #endregion


        private static string GetValueForOptions(ICollection<string> selectorlist, bool isSelected, string value, string pattern)
        {

            if (!selectorlist.Any())
            {
                return string.Empty;
            }
            if (selectorlist.Contains(value))
            {
                return pattern;//"selected=\"selected\"";
            }
            if (isSelected)
            {
                return pattern;// "selected=\"selected\"";
            }
            return string.Empty;
        }



        /// <summary>
        /// Получение результирующего выражения для запроса по типу Where
        /// </summary>
        /// <param name="collection">Колекция форм с клиента</param>
        /// <typeparam name="T">Тип объекта запроса</typeparam>
        /// <returns>Expression выражение</returns>
        internal static Expression<Func<T, bool>> GetExpressions(FormCollection collection)
        {
            Expression<Func<T, bool>> expr = null;
            var dict = Validate(collection, BaseAttributes.Value);

            foreach (var kv in dict)
            {

                var tt = BaseAttributes.Value.Single(a => a.Id == kv.Key);

                if (tt as CriterionAutoCompleteAttribute != null)
                {
                    Expression<Func<T, bool>> innerexpr = null;
                    foreach (var value in kv.Value)
                    {
                        Expression<Func<T, bool>> e;
                        if (value == null)
                        {
                            e = System.Linq.Dynamic.DynamicExpression.ParseLambda<T, bool>(tt.ExpressionTemplate.Replace("@0", "null"));
                        }
                        else
                        {
                            e = System.Linq.Dynamic.DynamicExpression.ParseLambda<T, bool>(tt.ExpressionTemplate, value);
                        }

                        innerexpr = e.Or(innerexpr);
                    }
                    expr = innerexpr.And(expr);
                }

                if (tt as CriterionListAttribute != null)
                {
                    Expression<Func<T, bool>> innerexpr = null;
                    foreach (var value in kv.Value)
                    {
                        Expression<Func<T, bool>> e;
                        if (value == null)
                        {
                            e = System.Linq.Dynamic.DynamicExpression.ParseLambda<T, bool>(tt.ExpressionTemplate.Replace("@0", "null"));
                        }
                        else
                        {
                            e = System.Linq.Dynamic.DynamicExpression.ParseLambda<T, bool>(tt.ExpressionTemplate, value);
                        }

                        innerexpr = e.Or(innerexpr);
                    }
                    expr = innerexpr.And(expr);
                }

                if (tt as CriterionRadioButtonAttribute != null)
                {
                    Expression<Func<T, bool>> innerexpr = null;
                    foreach (var value in kv.Value)
                    {
                        Expression<Func<T, bool>> e;
                        if (value == null)
                        {
                            e = System.Linq.Dynamic.DynamicExpression.ParseLambda<T, bool>(tt.ExpressionTemplate.Replace("@0", "null"));
                        }
                        else
                        {
                            e = System.Linq.Dynamic.DynamicExpression.ParseLambda<T, bool>(tt.ExpressionTemplate, value);
                        }

                        innerexpr = e.Or(innerexpr);
                    }
                    expr = innerexpr.And(expr);
                }
                if (tt as CriterionCheckBoxAttribute != null)
                {
                    Expression<Func<T, bool>> innerexpr = null;
                    foreach (var value in kv.Value)
                    {
                        Expression<Func<T, bool>> e;
                        if (value == null)
                        {
                            e = System.Linq.Dynamic.DynamicExpression.ParseLambda<T, bool>(tt.ExpressionTemplate.Replace("@0", "null"));
                        }
                        else
                        {
                            e = System.Linq.Dynamic.DynamicExpression.ParseLambda<T, bool>(tt.ExpressionTemplate, value);
                        }

                        innerexpr = e.Or(innerexpr);
                    }
                    expr = innerexpr.And(expr);
                }
                if (tt as CriterionDropDownAttribute != null)
                {
                    Expression<Func<T, bool>> innerexpr = null;
                    foreach (var value in kv.Value)
                    {
                        Expression<Func<T, bool>> e;
                        if (value == null)
                        {
                            e = System.Linq.Dynamic.DynamicExpression.ParseLambda<T, bool>(tt.ExpressionTemplate.Replace("@0", "null"));
                        }
                        else
                        {
                            e = System.Linq.Dynamic.DynamicExpression.ParseLambda<T, bool>(tt.ExpressionTemplate, value);
                        }

                        innerexpr = e.Or(innerexpr);
                    }
                    expr = innerexpr.And(expr);
                }
                if (tt as CriterionSliderAttribute != null)
                {
                    Expression<Func<T, bool>> innerexpr = null;

                    for (var i = 0; i < kv.Value.Count; i++)
                    {
                        var sql = string.Format(" {0} {1} @0", tt.PropertyInfo.Name, i == 0 ? ">=" : "<=");
                        var e = System.Linq.Dynamic.DynamicExpression.ParseLambda<T, bool>(sql, kv.Value[i]);
                        innerexpr = e.And(innerexpr);
                    }
                    expr = innerexpr.And(expr);

                }

                if (tt as CriterionBetweenDateAttribute != null)
                {
                    Expression<Func<T, bool>> innerexpr = null;

                    for (int i = 0; i < kv.Value.Count; i++)
                    {
                        var sql = string.Format(" {0} {1} @0 ", tt.PropertyInfo.Name, i == 0 ? ">=" : "<=");

                        var e = System.Linq.Dynamic.DynamicExpression.ParseLambda<T, bool>(sql, kv.Value[i] ?? DateTime.Now);
                        innerexpr = e.And(innerexpr);
                    }
                    expr = innerexpr.And(expr);

                }
                if (tt as CriterionMyControlAttribute != null)
                {
                    var instans = (IMyType)Activator.CreateInstance(((CriterionMyControlAttribute)tt).MyType);
                    var e = instans.GetExpression<T>(kv.Value.First().ToString());

                    expr = e.And(expr);
                }
            }
            return expr;

        }
        /// <summary>
        /// Проверка данных с клиента, с одновременным заполнением листа для получения выражения запроса
        /// </summary>
        /// <param name="collection">Коллекция данных с клиента</param>
        /// <param name="baseAttributes">Атрибуты типа</param>
        /// <returns></returns>

        private static Dictionary<string, List<object>> Validate(FormCollection collection, List<CriterionBaseAttribute> baseAttributes)
        {
            var varDict = new Dictionary<string, List<object>>();
            foreach (var str in collection.Keys)
            {
                var list = new List<object>();
                var stri = str.ToString();
                var val = collection.GetValue(str.ToString()).AttemptedValue;

                var myatr = baseAttributes.SingleOrDefault(a => a.Id == stri);

                if (myatr == null) continue;


                if (myatr as CriterionMyControlAttribute != null)
                {
                    var instans = Activator.CreateInstance(((CriterionMyControlAttribute)myatr).MyType);
                    ((IMyType)instans).Validate(val, (CriterionMyControlAttribute)myatr);
                    list.Add(val);
                    varDict.Add(str.ToString(), list);
                    continue;
                }

                Debug.WriteLine("__________________________" + str + "_________" + val);

                var type = myatr.PropertyInfo.PropertyType;

                if (string.IsNullOrWhiteSpace(val) || (type == typeof(DateTime) && val == "," && myatr as CriterionBetweenDateAttribute != null))
                {
                    continue;
                }

                if (type == typeof(string))
                {
                    list.AddRange(val.Split(','));
                    varDict.Add(str.ToString(), list);
                    continue;
                }
                var o = Activator.CreateInstance(type);
                var parse = type.GetMethod("Parse", new[] { typeof(String) });

                foreach (var s1 in val.Split(','))
                {
                    if (type.IsGenericType && type.GetGenericArguments().Count() == 1 && type.IsValueType &&
                        type.GetGenericArguments()[0].IsValueType && Nullable.GetUnderlyingType(type) != null)
                    {
                        if (string.IsNullOrWhiteSpace(s1)) continue;
                        if (s1.ToLower() == "null")//string.IsNullOrWhiteSpace(s1) ||
                        {
                            list.Add(null);
                            continue;
                        }

                        var tg = type.GetGenericArguments()[0];
                        var parse2 = tg.GetMethod("Parse", new[] { typeof(String) });
                        var valres = parse2.Invoke(o, new object[] { s1 });
                        list.Add(valres);

                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(s1)) continue;
                        if (s1.ToLower() == "null")//string.IsNullOrWhiteSpace(s1) ||
                        {
                            list.Add(null);
                            continue;
                        }

                        var valres2 = parse.Invoke(o, new object[] { s1 });
                        list.Add(valres2);
                    }

                }
                varDict.Add(str.ToString(), list);
            }
            return varDict;
        }
        /// <summary>
        /// Добавляет иконку с вопросительным знаком, при наличии постващика для справки
        /// </summary>
        /// <param name="id"> Индентификатор справки</param>
        /// <returns></returns>
        private static string GetHelpImage(string id)
        {
            if (CriterionActivator.TypeHelp == null)
            {
                return string.Empty;
            }
            var img = string.Format("<img src=\"{0}\" class=\"helpcr\" style=\"cursor:pointer\" onClick=\"viewHelp('{1}')\" alt=\"?\" />",
                string.IsNullOrWhiteSpace(CriterionActivator.UrlImage) ? "/ss/Help/Index/" + id : CriterionActivator.UrlImage, id);
            return img;
        }
    }
}
