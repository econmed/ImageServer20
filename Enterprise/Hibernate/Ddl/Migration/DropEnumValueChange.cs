using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
	class DropEnumValueChange : RelationalModelChange
	{
		private readonly EnumerationMemberInfo _value;

		public DropEnumValueChange(TableInfo table, EnumerationMemberInfo value)
			:base(table)
		{
			_value = value;
		}

		public EnumerationMemberInfo Value
		{
			get { return _value; }
		}

		public override Statement[] GetStatements(IRenderer renderer)
		{
			return renderer.Render(this);
		}
	}
}