using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common;
using ClearCanvas.Common.Specifications;
using System.Reflection;
using ClearCanvas.Common.Utilities;
using System.Xml;

namespace ClearCanvas.Desktop.Validation
{
    /// <summary>
    /// Caches custom validation rules for application components.
    /// </summary>
    public static class ValidationCache
    {
        private static readonly Dictionary<Type, List<IValidationRule>> _rulesCache = new Dictionary<Type, List<IValidationRule>>();
        private static readonly IConfigurationStore _configStore;
        private static readonly object _syncLock = new object();

        static ValidationCache()
        {
            try
            {
                _configStore = ConfigurationStoreFactory.GetDefaultStore();
            }
            catch (NotSupportedException e)
            {
                Platform.Log(LogLevel.Debug, e);
                _configStore = null;
            }
        }

        /// <summary>
        /// Gets the rules, including both attribute-based and custom XML-based rules, for the specified
        /// class of application component.
        /// </summary>
        /// <param name="applicationComponentClass"></param>
        /// <returns></returns>
        public static IList<IValidationRule> GetRules(Type applicationComponentClass)
        {
            lock (_syncLock)
            {

                // try to get it from the cache
                List<IValidationRule> rules;
                if (_rulesCache.TryGetValue(applicationComponentClass, out rules))
                    return rules;

                // build the validation rules and cache
                rules = new List<IValidationRule>();
                rules.AddRange(ProcessAttributeRules(applicationComponentClass));
                rules.AddRange(ProcessCustomRules(applicationComponentClass));

                _rulesCache.Add(applicationComponentClass, rules);

                return rules;
            }
        }

        /// <summary>
        /// Invalidates the cache for the specified application component class, causing the rules
        /// to be re-compiled the next time <see cref="GetRules"/> is called.
        /// </summary>
        /// <param name="applicationComponentClass"></param>
        public static void Invalidate(Type applicationComponentClass)
        {
            lock (_syncLock)
            {
                if (_rulesCache.ContainsKey(applicationComponentClass))
                    _rulesCache.Remove(applicationComponentClass);
            }
        }

        private static List<IValidationRule> ProcessCustomRules(Type applicationComponentClass)
        {
            List<IValidationRule> customRules = new List<IValidationRule>();

            // if there is no config store, there are no custom rules
            if (_configStore == null)
                return customRules;

            try
            {
                string documentName = string.Format("{0}.val.xml", applicationComponentClass.FullName);
                using (TextReader reader = _configStore.GetDocument(
                    documentName, applicationComponentClass.Assembly.GetName().Version, null, null))
                {
                    XmlValidationCompiler compiler = new XmlValidationCompiler();
                    customRules = compiler.CompileRules(reader);
                }
            }
            catch (ConfigurationDocumentNotFoundException e)
            {
                // no validation document exists for this application component class
                // this is not an error, but might be useful to know this for debugging
                Platform.Log(LogLevel.Debug, e);
            }
            catch(Exception e)
            {
                // some error occured in accessing or reading the validation document
                // the question is, to swallow the exception or not to swallow it??
                // most authors of application components do not expect exceptions from the constructor,
                // which is where the rules are built - therefore, it seems reasonable
                // to swallow and log the exception, although this means than an app component may
                // execute without having the custom validation rules in place
                Platform.Log(LogLevel.Error, e);
            }

            return customRules;
        }

        private static List<IValidationRule> ProcessAttributeRules(Type applicationComponentClass)
        {
            List<IValidationRule> rules = new List<IValidationRule>();
            foreach (PropertyInfo property in applicationComponentClass.GetProperties())
            {
                foreach (ValidationAttribute a in property.GetCustomAttributes(typeof(ValidationAttribute), true))
                {
                    rules.Add(a.CreateRule(property, new ResourceResolver(applicationComponentClass.Assembly)));
                }
            }
            return rules;
        }
    }
}
