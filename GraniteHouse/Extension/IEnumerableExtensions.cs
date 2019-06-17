using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ChainStore.Extension
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> items, int selectedValue)
        {
            return from v in items
                select new SelectListItem
                {
                    Text = v.GetPropertyValue("Name"),
                    Value = v.GetPropertyValue("Id"),
                    Selected = v.GetPropertyValue("Id").Equals(selectedValue.ToString())
                };
        }
        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> items, string selectedValue)
        {
            if (selectedValue == null)
            {
                selectedValue = "";
            }
            return from v in items
                select new SelectListItem
                {
                    Text = v.GetPropertyValue("Name"),
                    Value = v.GetPropertyValue("Id"),
                    Selected = v.GetPropertyValue("Id").Equals(selectedValue.ToString())
                };
        }
    }
}
