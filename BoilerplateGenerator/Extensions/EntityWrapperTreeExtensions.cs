using BoilerplateGenerator.Collections;
using BoilerplateGenerator.Contracts.RoslynWrappers;
using BoilerplateGenerator.Models.RoslynWrappers;
using BoilerplateGenerator.Models.SyntaxDefinitionModels;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BoilerplateGenerator.Extensions
{
    public static class EntityWrapperTreeExtensions
    {
        public static void SetChildrenSelection(this ITreeNode<IBaseSymbolWrapper> treeNode, bool checkStatus)
        {
            foreach (ITreeNode<IBaseSymbolWrapper> item in treeNode.Children.Where(x => x.Current.IsEnabled))
            {
                item.Current.IsPropertyChanging = true;
                item.Current.IsChecked = checkStatus;

                if (item.Current is BaseSymbolWrapper<INamedTypeSymbol>)
                {
                    item.SetChildrenSelection(checkStatus);
                    SetNodeSelectionStatus(item);
                }

                item.Current.IsPropertyChanging = false;
            }

            SetNodeSelectionStatus(treeNode);
        }

        public static void SetParentsSelection(this ITreeNode<IBaseSymbolWrapper> treeNode)
        {
            while (treeNode.Parent != null)
            {
                SetNodeSelectionStatus(treeNode.Parent);
                treeNode = treeNode.Parent;
            }
        }

        private static void SetNodeSelectionStatus(ITreeNode<IBaseSymbolWrapper> treeNode)
        {
            if (!treeNode.Children.Any())
            {
                return;
            }

            int fullSelectedChildrenCount = treeNode.Children.Count(x => x.Current.IsChecked.HasValue && x.Current.IsChecked.Value);
            int halfSelectedChildrenCount = treeNode.Children.Count(x => !x.Current.IsChecked.HasValue);

            if (fullSelectedChildrenCount == treeNode.Children.Count)
            {
                SetEntitySelectionStatus(treeNode.Current, true);
                return;
            }

            if (fullSelectedChildrenCount > 0 || halfSelectedChildrenCount > 0)
            {
                SetEntitySelectionStatus(treeNode.Current, null);
                return;
            }

            SetEntitySelectionStatus(treeNode.Current, false);
        }

        private static void SetEntitySelectionStatus(IBaseSymbolWrapper symbolWrapper, bool? selectionStatus)
        {
            symbolWrapper.IsPropertyChanging = true;
            symbolWrapper.IsChecked = selectionStatus;
            symbolWrapper.IsPropertyChanging = false;
        }

        public static IEnumerable<PropertyDefinitionModel> FilterTreeProperties(this ITreeNode<IBaseSymbolWrapper> rootNode)
        {
            List<PropertyDefinitionModel> propertyDefinitions = new List<PropertyDefinitionModel>();

            foreach (ITreeNode<IBaseSymbolWrapper> treeNode in rootNode.Children)
            {
                switch (treeNode.Current.GetType().Name)
                {
                    case nameof(EntityClassWrapper):
                        propertyDefinitions.AddRange(treeNode.FilterTreeProperties());
                        break;

                    default:
                        if (!(treeNode.Parent.Current is EntityClassWrapper entityClassWrapper))
                        {
                            throw new Exception("Not a valid Tree Node");
                        }

                        if (!(treeNode.Current is EntityPropertyWrapper entityPropertyWrapper))
                        {
                            break;
                        }

                        if (!entityPropertyWrapper.IsChecked.HasValue || !entityPropertyWrapper.IsChecked.Value)
                        {
                            break;
                        }

                        propertyDefinitions.Add(new PropertyDefinitionModel(entityPropertyWrapper, !entityClassWrapper.IsBaseTypeInheritance));
                        break;
                }
            }

            return propertyDefinitions.OrderByDescending(x => x.Attributes.Any());
        }

        public static string PrimaryEntityType(this ObservableCollection<ITreeNode<IBaseSymbolWrapper>> entityTree)
        {
            return entityTree.First().Current.Name;
        }

        public static string PrimaryEntityNamespace(this ObservableCollection<ITreeNode<IBaseSymbolWrapper>> entityTree)
        {
            return entityTree.First().Current.Namespace;
        }
    }
}
