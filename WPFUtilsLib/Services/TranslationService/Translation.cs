using System.Resources;
using System.Windows;

namespace WPFUtilsLib.Services.TranslationService
{
    public class Translation : DependencyObject
    {
        public static readonly DependencyProperty ResourceManagerProperty =
            DependencyProperty.RegisterAttached("ResourceManager", typeof(ResourceManager), typeof(Translation));

        public static ResourceManager GetResourceManager(DependencyObject dependencyObject)
        {
            return (ResourceManager)dependencyObject.GetValue(ResourceManagerProperty);
        }

        public static void SetResourceManager(DependencyObject dependencyObject, ResourceManager resourceManager)
        {
            dependencyObject.SetValue(ResourceManagerProperty, resourceManager);
        }
    }
}
