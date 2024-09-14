using DOTNET.Common.Extensions.Arrays;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DOTNET.Common.WindowsAppsSettings
{
    public class WindowAppConfigReader
    {
        public readonly string Address;

        private readonly Stream? stream;

        /// <summary>
        /// Default value is "user.config"
        /// </summary>
        public string ConfgFileName { get; set; }

        private XmlDocument xdocument;



        /// <summary>
        /// List<KeyValuePair<string[0], KeyValuePair<string[1],object[2]>>>
        /// string[0] => dll name like 'UtilityDevExpress.Properties.Settings' or  'TebnegarCosmoDoctor.Properties.Settings'
        /// string[1] => Setting name like 'UserName' or 'Password'
        /// object[2] => Settings value like 'Visible' or 'Persian'
        /// </summary>
        private List<KeyValuePair<string, KeyValuePair<string, object>>> settings { get; set; }


        /// <summary>
        /// List<KeyValuePair<string[0], Dictionary<string[1],object[2]>>>
        /// string[0] => dll name like 'UtilityDevExpress.Properties.Settings' or  'TebnegarCosmoDoctor.Properties.Settings'
        /// string[1] => Setting name like 'UserName' or 'Password'
        /// object[2] => Settings value like 'Visible' or 'Persian'
        /// </summary>
        public ReadOnlyCollection<KeyValuePair<string, KeyValuePair<string, object>>> Settings { get => new ReadOnlyCollection<KeyValuePair<string, KeyValuePair<string, object>>>((IList<KeyValuePair<string, KeyValuePair<string, object>>>)settings); }


        public WindowAppConfigReader(string address)
        {
            Address = address;
            ConfgFileName = "user.config";
            this.settings = new List<KeyValuePair<string, KeyValuePair<string, object>>>();
            //Load();
        }

        public WindowAppConfigReader(Stream stream) : this(string.Empty)
        {
            this.stream = stream;
        }

        #region Load

        public void Load()
        {
            xdocument = new XmlDocument();
            if (stream != null)
                xdocument.Load(stream);
            else
                xdocument.Load(this.Address);

            ReadSettings();
        }

        private void ReadSettings()
        {
            XmlNode root = Find("userSettings", null);

            if (root == null || root.ChildNodes.Count < 1)
                throw new NullReferenceException("The dot net settings not found!");

            foreach (System.Xml.XmlElement libraryElement in root)
            {
                string LibraryName = libraryElement.Name; // Like TebnegarCosmoDoctor.Properties.Settings

                foreach (XmlNode item in libraryElement)
                {
                    string name = item.Attributes["name"].Value;

                    XmlNode ValueTag = Find("value", null, item);

                    XmlNode ArrayOfString = null;
                    if ((ArrayOfString = Find("ArrayOfString", null, item)) != null)
                    {
                        string[] StringArray = GetStringArray(ArrayOfString);
                        KeyValuePair<string, object> kp = new KeyValuePair<string, object>(name, StringArray);
                        settings.Add(new KeyValuePair<string, KeyValuePair<string, object>>(LibraryName, kp));
                    }
                    else
                    {
                        KeyValuePair<string, object> kp = new KeyValuePair<string, object>(name, ValueTag?.InnerText);
                        settings.Add(new KeyValuePair<string, KeyValuePair<string, object>>(LibraryName, kp));

                    }
                }



            }

            //XmlNodeList SettingsNode = root.FirstChild.ChildNodes as XmlNodeList;
            // نام نرم افزار میباشد که در اینجا میتواند هر چیزی باشد userSettings بعد از تگ 
            //foreach (XmlNode item in SettingsNode)
            //{
            //    string name = item.Attributes["name"].Value;
            //
            //    XmlNode ValueTag = Find("value", null, item);
            //
            //    XmlNode ArrayOfString = null;
            //    if ((ArrayOfString = Find("ArrayOfString", null, item)) != null)
            //    {
            //        string[] StringArray = GetStringArray(ArrayOfString);
            //        KeyValuePair<string, object> kp = new KeyValuePair<string, object>(name, StringArray);
            //        settings.Add(kp);
            //    }
            //    else
            //    {
            //        KeyValuePair<string, object> kp = new KeyValuePair<string, object>(name, ValueTag?.InnerText);
            //        settings.Add(kp);
            //
            //    }
            //}
        }

        private string[] GetStringArray(XmlNode ArrayOfString)
        {
            IEnumerable<XmlNode> nodes = Finds("string", null, ArrayOfString);
            if (nodes == null)
                return null;

            List<string> list = new List<string>();

            foreach (var item in nodes)
            {
                if (item != null && !string.IsNullOrEmpty(item.InnerText))
                {
                    list.Add(item.InnerText);
                }
            }
            return list.ToArray();
        }

        #endregion

        #region Public method

        public IEnumerable<XmlNode> Finds(string TagName, string AttributName, XmlNode child = null, List<XmlNode> nodes = null)
        {
            if (nodes == null)
            {
                nodes = new List<XmlNode>();
            }



            if (child == null)
            {
                foreach (XmlNode item in xdocument.ChildNodes)
                {
                    if (
                        // در صورتی که کاربر فقط به خواهد یک تگ را پیدا کند نه چیزی بیشتر
                        ((item.Name.Equals(TagName) && AttributName == null) ||
                        // اگر کاربر میخواست یک تگی را همراه با یک ویژگی پیدا کند
                        // برای مثال 
                        // setting with Attribute name RememberMe
                        (item.Name.Equals(TagName) && AttributName != null && item.Attributes["name"] != null && item.Attributes["name"].Value.Equals(AttributName)))
                        )
                        nodes.Add(item);
                    else if (item.HasChildNodes)
                    {
                        XmlNode result = Find(TagName, AttributName, item);
                        if (result != null)
                            nodes.Add(result);

                    }
                }
            }
            else
            {
                foreach (XmlNode item in child.ChildNodes)
                {
                    if (
                        // در صورتی که کاربر فقط به خواهد یک تگ را پیدا کند نه چیزی بیشتر
                        ((item.Name.Equals(TagName) && AttributName == null) ||

                        // اگر کاربر میخواست یک تگی را همراه با یک ویژگی پیدا کند
                        // برای مثال 
                        // setting with Attribute name RememberMe
                        (item.Name.Equals(TagName) && AttributName != null && item.Attributes["name"] != null && item.Attributes["name"].Value.Equals(AttributName)))

                        )
                        nodes.Add(item);
                    else if (item.HasChildNodes)
                    {
                        XmlNode result = Find(TagName, AttributName, item);
                        if (result != null)
                            nodes.Add(result);
                    }
                }
            }

            return nodes;
        }

        public XmlNode Find(string TagName, string AttributName, XmlNode child = null)
        {

            if (child == null)
            {
                foreach (XmlNode item in xdocument.ChildNodes)
                {
                    if (
                        // در صورتی که کاربر فقط به خواهد یک تگ را پیدا کند نه چیزی بیشتر
                        ((item.Name.Equals(TagName) && AttributName == null) ||
                        // اگر کاربر میخواست یک تگی را همراه با یک ویژگی پیدا کند
                        // برای مثال 
                        // setting with Attribute name RememberMe
                        (item.Name.Equals(TagName) && AttributName != null && item.Attributes["name"] != null && item.Attributes["name"].Value.Equals(AttributName)))
                        )
                        return item;
                    else if (item.HasChildNodes)
                    {
                        XmlNode result = Find(TagName, AttributName, item);
                        if (result != null)
                            return result;

                    }
                }
            }
            else
            {
                foreach (XmlNode item in child.ChildNodes)
                {
                    if (
                        // در صورتی که کاربر فقط به خواهد یک تگ را پیدا کند نه چیزی بیشتر
                        ((item.Name.Equals(TagName) && AttributName == null) ||

                        // اگر کاربر میخواست یک تگی را همراه با یک ویژگی پیدا کند
                        // برای مثال 
                        // setting with Attribute name RememberMe
                        (item.Name.Equals(TagName) && AttributName != null && item.Attributes["name"] != null && item.Attributes["name"].Value.Equals(AttributName)))

                        )
                        return item;
                    else if (item.HasChildNodes)
                    {
                        XmlNode result = Find(TagName, AttributName, item);
                        if (result != null)
                            return result;
                    }
                }
            }

            return null;
        }


        #endregion
    }
}
