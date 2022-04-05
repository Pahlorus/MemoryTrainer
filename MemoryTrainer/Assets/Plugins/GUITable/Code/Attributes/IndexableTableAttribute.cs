namespace EditorGUITable
{
	/// <summary>
	/// Attribute that automatically draws a collection as a table with the ShowIndex option enabled
	/// </summary>
	public class IndexableTableAttribute : ReorderableTableAttribute
	{
		/// <summary>
		/// This attribute will display the collection in a indexable table, instead of the classic Unity list.
		/// </summary>
		public IndexableTableAttribute() {}

		/// <summary>
		/// This attribute will display the collection's chosen properties in a indexable table, instead of the classic Unity list.
		/// </summary>
		/// <param name="properties"> The properties to display in the table </param>
		public IndexableTableAttribute(params string[] properties) : base (properties) {}

		/// <summary>
		/// This attribute will display the collection's chosen properties in a indexable table, with the chosen column sizes, instead of the classic Unity list.
		/// </summary>
		/// <param name="properties"> The properties to display in the table</param>
		/// <param name="widths"> The widths of the table's columns</param>
		public IndexableTableAttribute(string[] properties, float[] widths, params string[] tableOptions) : base (properties, widths, tableOptions) {}

		/// <summary>
		/// This attribute will display the collection's chosen properties in a indexable table, with the chosen column sizes, instead of the classic Unity list.
		/// </summary>
		/// <param name="properties"> The properties to display in the table</param>
		/// <param name="widths"> The widths of the table's columns</param>
		public IndexableTableAttribute(string[] properties, params string[] tableOptions) : base (properties, tableOptions) {}
	}

}