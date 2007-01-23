using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Validation
{
    /// <summary>
    /// Implemenation of <see cref="IValidationRuleSet"/>
    /// </summary>
    public class ValidationRuleSet : IValidationRuleSet
    {
        private List<IValidationRule> _rules;

        /// <summary>
        /// Constructor
        /// </summary>
        public ValidationRuleSet()
        {
            _rules = new List<IValidationRule>();
        }

        #region IValidationRuleSet members

        public void Add(IValidationRule rule)
        {
            _rules.Add(rule);
        }

        public void Remove(IValidationRule rule)
        {
            _rules.Remove(rule);
        }

        public List<ValidationResult> GetResults(IApplicationComponent component)
        {
            return GetResults(component, _rules);
        }

        public List<ValidationResult> GetResults(IApplicationComponent component, string propertyName)
        {
            return GetResults(component, _rules.FindAll(delegate(IValidationRule v) { return v.PropertyName == propertyName; }));
        }

        #endregion

        private List<ValidationResult> GetResults(IApplicationComponent component, List<IValidationRule> validators)
        {
            return validators.ConvertAll<ValidationResult>(delegate(IValidationRule v) { return v.GetResult(component); });
        }
    }
}
