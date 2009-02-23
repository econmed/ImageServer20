using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Hibernate.Ddl.Model;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration
{
    class DropUniqueConstraintChange : Change
    {
    	private readonly ConstraintInfo _constraint;

        public DropUniqueConstraintChange(TableInfo table, ConstraintInfo constraint)
			: base(table)
		{
        	_constraint = constraint;
        }

    	public ConstraintInfo Constraint
    	{
			get { return _constraint; }
    	}

		public override Statement[] GetStatements(IRenderer renderer)
		{
			return renderer.Render(this);
		}
	}
}
