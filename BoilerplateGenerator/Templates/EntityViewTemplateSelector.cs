using BoilerplateGenerator.Collections;
using BoilerplateGenerator.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BoilerplateGenerator.Templates
{
    public class EntityViewTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement elemnt = container as FrameworkElement;

            if (!(item is ITreeNode<IBaseSymbolWrapper> treeNode))
            {
                throw new Exception("Not a valid type for Tree View");
            }

            switch (treeNode.Current.GetType().Name)
            {
                case nameof(EntityClassWrapper):
                    return elemnt.FindResource("ClassSelector") as DataTemplate;

                default:
                    return elemnt.FindResource("PropertySelector") as DataTemplate;
            }
        }
    }
}
