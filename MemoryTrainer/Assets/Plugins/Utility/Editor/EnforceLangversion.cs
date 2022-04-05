#if UNITY_EDITOR

using UnityEditor;

namespace Utility.EditorExtensions
{
    [InitializeOnLoad]
    public class EnforceLangversion
    {
        static EnforceLangversion()
        {
#if ENABLE_VSTU
            SyntaxTree.VisualStudio.Unity.Bridge.ProjectFilesGenerator.ProjectFileGeneration += (string name, string content) =>
            {
                return content.Replace
                ("<LangVersion>latest</LangVersion>",
                    "<LangVersion>7.3</LangVersion>");
            };
#endif
        }
    }
}
#endif