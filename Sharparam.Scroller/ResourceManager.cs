namespace Sharparam.Scroller
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using log4net;

    using SFML.Graphics;

    using Sharparam.Scroller.Mapping;

    public static class ResourceManager
    {
        private const string BaseResourcePath = "res";

        private const string MapExtension = "tmx";

        private static readonly string FontPath = Path.Combine(BaseResourcePath, "fonts");

        private static readonly string ImagePath = Path.Combine(BaseResourcePath, "images");

        private static readonly ILog Log = LogManager.GetLogger(typeof(ResourceManager));

        private static readonly string MapPath = Path.Combine(BaseResourcePath, "maps");

        private static readonly Dictionary<Type, Dictionary<string, object>> Resources =
            new Dictionary<Type, Dictionary<string, object>>();

        // Type specific loaders

        public static Font LoadFont(string file)
        {
            var path = Path.Combine(FontPath, file);
            Log.InfoFormat("Loading font: {0}", path);
            var font = new Font(path);
            return Add(Path.GetFileNameWithoutExtension(path), font);
        }

        public static Map LoadMap(string name)
        {
            var path = Path.Combine(MapPath, name);
            Log.InfoFormat("Loading map  {0}:{1}", name, path);
            path = Path.ChangeExtension(path, MapExtension);
            var map = new Map(path);
            return Add(map.Name, map);
        }

        // Generic methods

        public static T Add<T>(string name, T resource) where T : class
        {
            Log.DebugFormat("Adding resource {0} : {1}", name, typeof(T));

            if (Has<T>(name))
            {
                Log.Debug("Resource already added, returning existing resource.");
                return Get<T>(name);
            }

            GetResources<T>()[name] = resource;
            return resource;
        }

        public static bool Has<T>(string name) where T : class
        {
            return GetResources<T>().ContainsKey(name);
        }

        public static T Get<T>(string name) where T : class
        {
            return Has<T>(name) ? (T)GetResources<T>()[name] : null;
        }

        private static Dictionary<string, object> GetResources<T>() where T : class
        {
            return GetResources(typeof(T));
        }

        private static Dictionary<string, object> GetResources(Type type)
        {
            if (Resources.ContainsKey(type))
                return Resources[type];
            Log.DebugFormat("Creating new resource dictionary for {0}", type);
            var dict = new Dictionary<string, object>();
            Resources[type] = dict;
            return dict;
        }
    }
}
