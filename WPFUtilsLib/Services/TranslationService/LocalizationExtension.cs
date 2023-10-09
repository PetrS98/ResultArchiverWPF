using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xaml;
using System.Resources;
using System;

namespace WPFUtilsLib.Services.TranslationService
{
    public class LocalizationExtension : MarkupExtension
    {
        public string StringName { get; }

        public LocalizationExtension(string stringName)
        {
            StringName = stringName;
        }

        private static ResourceManager? GetResourceManager(object? control)
        {
            if (control is DependencyObject dependencyObject)
            {
                object localValue = dependencyObject.ReadLocalValue(Translation.ResourceManagerProperty);

                if (localValue != DependencyProperty.UnsetValue)
                {
                    if (localValue is ResourceManager resourceManager)
                    {
                        TranslationService.Instance.AddResourceManager(resourceManager);

                        return resourceManager;
                    }
                }
            }

            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            // targetObject is the control that is using the LocExtension
            object? targetObject = (serviceProvider as IProvideValueTarget)?.TargetObject;

            if (targetObject?.GetType().Name == "SharedDp") // is extension used in a control template?
                return targetObject; // required for template re-binding

            string baseName = GetResourceManager(targetObject)?.BaseName ?? string.Empty;

            if (string.IsNullOrEmpty(baseName))
            {
                // rootObject is the root control of the visual tree (the top parent of targetObject)
                object? rootObject = (serviceProvider as IRootObjectProvider)?.RootObject;
                baseName = GetResourceManager(rootObject)?.BaseName ?? string.Empty;
            }

            if (string.IsNullOrEmpty(baseName)) // template re-binding
            {
                if (targetObject is FrameworkElement frameworkElement)
                {
                    baseName = GetResourceManager(frameworkElement.TemplatedParent)?.BaseName ?? string.Empty;
                }
            }

            Binding binding = new()
            {
                Mode = BindingMode.OneWay,
                Path = new PropertyPath($"[{baseName}.{StringName}]"),
                Source = TranslationService.Instance,
                FallbackValue = StringName
            };

            return binding.ProvideValue(serviceProvider);
        }
    }
}
