#if UNITY_EDITOR
using UnityEditor;

namespace EditorGUITable
{
	
	[CustomPropertyDrawer(typeof(ReorderableTableAttribute))]
	public class ReorderableTableDrawer : TableDrawer 
	{

		protected override GUITableOption[] forcedTableOptions
		{
			get 
			{
				return new GUITableOption[] {GUITableOption.AllowScrollView(false), GUITableOption.Reorderable(true)};
			}
		}

	}

}
#endif