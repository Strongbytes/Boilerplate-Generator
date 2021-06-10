using BoilerplateGenerator.Collections;
using BoilerplateGenerator.Contracts.RoslynWrappers;
using BoilerplateGenerator.Models.RoslynWrappers;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BoilerplateGenerator.Templates
{
    public class EntityViewTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (!(item is ITreeNode<IBaseSymbolWrapper> treeNode))
            {
                throw new Exception("Not a valid type for Tree View");
            }

            switch (treeNode.Current.GetType().Name)
            {
                case nameof(EntityClassWrapper):
                    return element.FindResource("ClassSelector") as DataTemplate;

                default:
                    return element.FindResource("PropertySelector") as DataTemplate;
            }
        }
    }
}
