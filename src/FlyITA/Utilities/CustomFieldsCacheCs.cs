using System.Collections;
using System.Web;
using System.Web.Caching;
using System.Xml;

public static class CustomFieldsCache
{
    #region " ### Sample ### "
    /// <summary>
    /// This is just an example of how to access
    /// the data that is read from the config file.
    /// </summary>
    /// <remarks></remarks>

    private static void _sample()
    {
        try
        {
            Hashtable cfHash = CurrentCFCache;
            string cfName = "Shirt Size";
            string sort = null;

            sort = GetCustomFieldAttribute(cfName, "sort");

            if (sort.Length > 0)
            {
                //// Apply the sort method
            }

        }
        catch
        {
            //// There is not sort attribute for that custom field
        }
    }

    #endregion

    #region " ### Private Variables ### "
    static Cache _cache;
    static string _key = string.Empty;
    static string _path = string.Empty;
    #endregion

    static Hashtable _currentCache = null;

    #region " ### Public Accessor Functions ### "

    public static int CountOfCustomFieldsInConfig()
    {
        try
        {
            if (CurrentCFCache == null) return 0;
            return ((Hashtable)CurrentCFCache).Keys.Count;
        }
        catch
        {
            return 0;
        }
    }

    public static bool IsCustomFieldInConfig(string cfName)
    {
        try
        {
            if (CurrentCFCache == null) return false;
            return (Hashtable)CurrentCFCache[cfName] != null;
        }
        catch
        {
            return false;
        }
    }

    public static string GetCustomFieldAttribute(string cfName, string cfAttrib)
    {
        try
        {
            if (CurrentCFCache == null) return string.Empty;
            return string.Concat(((Hashtable)CurrentCFCache[cfName])[cfAttrib], string.Empty);
        }
        catch
        {
            return string.Empty;
        }
    }

    #endregion

    #region " ### Properties ### "
    /// <summary>
    /// This property is how to access the application cache
    /// from other pages.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks></remarks>
    public static Hashtable CurrentCFCache
    {
        get
        {
            if (_currentCache == null)
            {
                _loadCurrentCache();
            }
            return _currentCache;
        }
    }
    #endregion

    #region " ### Private Methods ### "
    /// <summary>
    /// This is called at the beginning of all code to setup
    /// variables
    /// </summary>
    /// <remarks></remarks>
    private static void initSelf()
    {
        _path = Utilities.GetCustomFieldsConfig();

        _cache = System.Web.HttpRuntime.Cache;
        _key = "CustomFieldsApplicationCacheKey";
    }

    /// <summary>
    /// Stub sub to call more detailed version
    /// </summary>
    /// <remarks></remarks>
    private static void _loadCurrentCache()
    {
        _loadCurrentCache(string.Empty, null, CacheItemRemovedReason.DependencyChanged);
    }

    /// <summary>
    /// This function saves the Custom Field config settings to the cache
    /// </summary>
    /// <param name="key"></param>
    /// <param name="item"></param>
    /// <param name="reason"></param>
    /// <remarks>Calls itself when the file changes</remarks>
    private static void _loadCurrentCache(string key, object item, CacheItemRemovedReason reason)
    {
        _currentCache = null;
        initSelf();

        // Determine path to config file
        string xPath = string.Empty;

        try
        {
            xPath = HttpContext.Current.Server.MapPath(_path);
        }
        catch
        {
            xPath = string.Concat(HttpRuntime.AppDomainAppPath, _path);
        }

        // Now load the data from the config file
        _loadDataFromConfig(xPath);

        // Remove the item from cache before entering it again
        if (_cache[_key] != null)
        {
            _cache.Remove(_key);
        }

        CacheDependency dep = new CacheDependency(xPath);
        CacheItemRemovedCallback cal = new CacheItemRemovedCallback(_loadCurrentCache);

        if (_currentCache != null)
        {
            _cache.Add(_key, _currentCache, dep, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Default, cal);
        }
    }

    /// <summary>
    /// Reads the config file and maps the information into a hashtable.
    /// </summary>
    /// <remarks></remarks>
    private static void _loadDataFromConfig(string xPath)
    {
        initSelf();

        XmlDocument xDoc = new XmlDocument();

        try
        {
            string xQuery = null;
            XmlNode xNode = null;

            //xPath = HttpContext.Current.Server.MapPath(_path)
            xQuery = "CustomFields";

            //// Load the config file
            xDoc.Load(xPath);
            //// Find the starting node
            xNode = xDoc.SelectSingleNode(xQuery);

            Hashtable cfHash = new Hashtable();
            Hashtable cfHashAttribs = new Hashtable();
            string cfName = string.Empty;

            if (xNode != null && xNode.ChildNodes.Count > 0)
            {
                //// Loop through the nodes once per CustomField
                foreach (XmlNode xNodeCF in xNode.ChildNodes)
                {
                    //// Loop through each attribute of a CustomField
                    foreach (XmlNode xNodeCFAttrib in xNodeCF.ChildNodes)
                    {
                        if (string.Compare(xNodeCFAttrib.Name.ToLower(), "name", true) == 0)
                        {
                            //// Store the name of the custom field
                            cfName = xNodeCFAttrib.InnerText;
                        }
                        else
                        {
                            //// Add an attribute for each other than the name
                            cfHashAttribs.Add(xNodeCFAttrib.Name, xNodeCFAttrib.InnerText);
                        }
                    }
                    if (cfHashAttribs.Keys.Count > 0)
                    {
                        //// Add the custom field to the main hashtable
                        cfHash.Add(cfName, cfHashAttribs);
                    }

                    //// Reset the values once per CustomField
                    cfName = string.Empty;
                    cfHashAttribs = new Hashtable();
                }

                _currentCache = cfHash;
            }
        }
        catch
        {
            _currentCache = null;
        }
        finally
        {
            xDoc = null;
        }
    }

    #endregion
}