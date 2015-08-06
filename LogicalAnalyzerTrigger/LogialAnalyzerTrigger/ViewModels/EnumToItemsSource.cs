﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace LogialAnalyzerTrigger.ViewModels
{
    // tom.maruska http://stackoverflow.com/questions/6145888/how-to-bind-an-enum-to-a-combobox-control-in-wpf

    public class EnumToItemsSource : MarkupExtension
    {
        private readonly Type _type;

        public EnumToItemsSource(Type type)
        {
            _type = type;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(_type)
                .Cast<object>()
                .Select(e => new { Value = (int)e, DisplayName = e.ToString() });
        }
    }
}
