using BoilerplateGenerator.Collections;
using BoilerplateGenerator.Contracts;
using BoilerplateGenerator.Models.TreeView;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BoilerplateGenerator.Templates
{
    public class DirectoryViewTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (!(item is ITreeNode<IBaseGeneratedAsset> treeNode))
            {
                throw new Exception("Not a valid type for Tree View");
            }

            switch (treeNode.Current.GetType().Name)
            {
                case nameof(GeneratedDirectory):
                    return element.FindResource("DirectorySelector") as DataTemplate;

                default:
                    return element.FindResource("FileSelector") as DataTemplate;
            }
        }
    }
}
