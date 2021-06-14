using BoilerplateGenerator.Collections;
using BoilerplateGenerator.Contracts.Generators;
using BoilerplateGenerator.Models.TreeView;
using System.Linq;
using System.Threading.Tasks;

namespace BoilerplateGenerator.Helpers
{
    public static class GeneratedAssetsTreeExtensions
    {
        public static async Task ExportGeneratedFiles(this ITreeNode<IBaseGeneratedAsset> treeNode)
        {
            if (treeNode.Current is GeneratedClass generatedClass)
            {
                await generatedClass.ExportFile().ConfigureAwait(false);
                return;
            }

            foreach (ITreeNode<IBaseGeneratedAsset> node in treeNode.Children)
            {
                await ExportGeneratedFiles(node).ConfigureAwait(false);
            }
        }

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
