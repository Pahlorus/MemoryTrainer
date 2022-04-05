#if UNITY_EDITOR
using UnityEditor;

namespace EditorGUITable
{
	
	[CustomPropertyDrawer(typeof(IndexableTableAttribute))]
	public class IndexableTableDrawer : ReorderableTableDrawer
	{

		protected override GUITableOption[] forcedTableOptions
		{
			get 
			{
				return new GUITableOption[] { GUITableOption.AllowScrollView(false), GUITableOption.Reorderable(true), GUITableOption.ShowIndex(true) };
			}
		}

	}

}
#endif