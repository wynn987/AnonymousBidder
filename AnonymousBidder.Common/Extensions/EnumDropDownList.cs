using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;


namespace AnonymousBidder.Common.Extensions
{
    public static class EnumDropDownList
    {
        /// <summary>
        /// Returns an HTML select element for each property in the object that is represented
        /// by the specified expression using the specified enum items and HTML attributes.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="expression">An expression that identifies the object that contains the properties to display.</param>
        /// <param name="htmlAttributes">An object containing a list of html sttributes to apply to the select element.</param>
        /// <returns>
        /// An HTML select element for each property in the object that is represented
        /// by the expression.
        /// </returns>
        public static MvcHtmlString EnumDropDownListForExt<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, object htmlAttributes = null)
        {

            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

            Type enumType = GetNonNullableModelType(metadata);
            Type baseEnumType = Enum.GetUnderlyingType(enumType);
            List<SelectListItem> items = new List<SelectListItem>();

            foreach (FieldInfo field in enumType.GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public))
            {
                string text = field.Name;
                string value = Convert.ChangeType(field.GetValue(null), baseEnumType).ToString();
                bool selected = field.GetValue(null).Equals(metadata.Model);

                foreach (DisplayAttribute displayAttribute in field.GetCustomAttributes(true).OfType<DisplayAttribute>())
                {
                    text = displayAttribute.GetName();
                }

                items.Add(new SelectListItem()
                {
                    Text = text,
                    Value = value,
                    Selected = selected
                });
            }

            //if (metadata.IsNullableValueType)
            //{
            //    items.Insert(0, new SelectListItem { Text = "", Value = "" });
            //}
            items.Insert(0, new SelectListItem { Text = "", Value = "" });
            var result = items.OrderBy(x => x.Text);
            return SelectExtensions.DropDownListFor(htmlHelper, expression, result, htmlAttributes);

        }

        /// <summary>
        /// Checks for nullable types and return the base type.
        /// </summary>
        /// <param name="modelMetadata">
        /// Provides a container for common metadata, for the <see ref="T:System.Web.Mvc.ModelMetadataProvider"/>
        //  class, and for the <see ref="T:System.Web.Mvc.ModelValidator"/> class for a data model.
        /// </param>
        /// <returns></returns>
        private static Type GetNonNullableModelType(ModelMetadata modelMetadata)
        {
            Type realModelType = modelMetadata.ModelType;
            Type underlyingType = Nullable.GetUnderlyingType(realModelType);

            if (underlyingType != null)
            {
                realModelType = underlyingType;
            }

            return realModelType;
        }
    }
}
