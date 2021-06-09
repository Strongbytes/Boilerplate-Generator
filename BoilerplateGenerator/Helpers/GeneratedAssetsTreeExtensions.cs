using BoilerplateGenerator.Collections;
using BoilerplateGenerator.Contracts;
using BoilerplateGenerator.Models.ClassGeneratorModels.TreeView;
using System.Linq;

namespace BoilerplateGenerator.Helpers
{
    public static class GeneratedAssetsTreeExtensions
    {
        public static void GenerateDirectoryClassTree(this ITreeNode<IBaseGeneratedAsset> rootNode, IGeneratedClass generatedClass)
        {
            foreach (string directory in generatedClass.ParentDirectoryHierarchy)
            {
                rootNode = GenerateParentDirectory(rootNode, directory);
            }

            ITreeNode<IBaseGeneratedAsset> childNode = new TreeNode<IBaseGeneratedAsset>
            {
                Current = generatedClass,
                Parent = rootNode
            };

            rootNode.Children.Add(childNode);
            rootNode.Children.Sort(x => x.Current.AssetName);

            while (rootNode.Parent != null)
            {
                rootNode = rootNode.Parent;
            }
        }

        private static ITreeNode<IBaseGeneratedAsset> GenerateParentDirectory(ITreeNode<IBaseGeneratedAsset> rootNode, string directory)
        {
            ITreeNode<IBaseGeneratedAsset> directoryNode = rootNode.Children.FirstOrDefault(x => x.Current is GeneratedDirectory childDirectory && childDirectory.AssetName.Equals(directory));
            if (directoryNode != null)
            {
                return directoryNode;
            }

            directoryNode = new TreeNode<IBaseGeneratedAsset>
            {
                Current = new GeneratedDirectory(directory),
                Parent = rootNode
            };

            rootNode.Children.Add(directoryNode);
            rootNode.Children.Sort(x => x.Current.AssetName);

            return directoryNode;
        }
    }
}
