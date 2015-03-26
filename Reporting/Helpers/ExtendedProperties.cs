using System;
using System.Collections;
using System.Text;

namespace NUnitReporter.Reporting.Helpers
{

    public class ExtendedProperties : Hashtable
    {
        /// <summary>
        /// These are the keys in the order they listed
        ///             in the configuration file. This is useful when
        ///             you wish to perform operations with configuration
        ///             information in a particular order.
        /// 
        /// </summary>
        protected internal ArrayList keysAsListed = new ArrayList();

        private ExtendedProperties defaults;

        public IEnumerable Keys
        {
            get { return keysAsListed; }
        }

        /// <summary>
        /// Creates an empty extended properties object.
        /// 
        /// </summary>
        public ExtendedProperties()
        {
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            foreach (string str in Keys)
            {
                var obj = this[str];
                stringBuilder.AppendFormat("{0} => {1}", str, ValueToString(obj)).Append(Environment.NewLine);
            }
            return stringBuilder.ToString();
        }

        private string ValueToString(object value)
        {
            if (!(value is ArrayList))
                return value.ToString();
            var str = "ArrayList :: ";
            foreach (var obj in (ArrayList) value)
            {
                if (!str.EndsWith(", "))
                    str += ", ";
                str += string.Format("[{0}]", obj);
            }
            return str;
        }

        /// <summary>
        /// Get a string associated with the given configuration key.
        ///              *
        /// 
        /// </summary>
        /// <param name="key">The configuration key.
        ///              </param><param name="defaultValue">The default value.
        ///              </param>
        /// <returns>
        /// The associated string if key is found,
        ///              default value otherwise.
        /// 
        /// </returns>
        /// <exception cref="T:System.InvalidCastException">is thrown if the key maps to an
        ///              object that is not a String.
        /// 
        ///              </exception>
        public string GetString(string key, string defaultValue)
        {
            var obj = this[key];
            if (obj is string)
                return (string) obj;
            if (obj == null)
            {
                return defaults == null ? defaultValue : defaults.GetString(key, defaultValue);
            }
            if (obj is ArrayList)
                return (string) ((ArrayList) obj)[0];
            throw new InvalidCastException(string.Format("{0}{1}' doesn't map to a String object", '\'', key));
        }

        public string GetString(string key)
        {
            var obj = this[key];
            return (string)obj;
        }
    }
}

