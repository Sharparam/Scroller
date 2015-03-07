namespace Sharparam.Scroller.Mapping
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using log4net;

    using SFML.Graphics;
    using SFML.Window;

    using TiledSharp;

    public class Map : Transformable, Drawable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Map));

        private readonly TmxMap _tmxMap;

        private readonly Dictionary<string, Tileset> _tilesets;

        private readonly TileLayer[] _tileLayers;

        private readonly View _view;

        public Map(string path)
        {
            Filename = Path.GetFullPath(path);
            Log.DebugFormat("Loading map from {0}", Filename);
            _tmxMap = new TmxMap(path);
            if (!_tmxMap.Properties.ContainsKey("name"))
                throw new MapLoadException("Map file does not contain mandatory name property.");
            
            Name = _tmxMap.Properties["name"];
            TileHeight = _tmxMap.TileHeight;
            TileWidth = _tmxMap.TileWidth;
            Log.InfoFormat("Loading map {0}.", Name);
            Log.DebugFormat("Map has {0} layers.", _tmxMap.Layers.Count);

            Log.Debug("Creating map tilesets.");
            _tilesets = new Dictionary<string, Tileset>(_tmxMap.Tilesets.Count);
            foreach (var tileset in _tmxMap.Tilesets)
            {
                Log.DebugFormat("Loading tileset {0}", tileset.Name);
                _tilesets[tileset.Name] = new Tileset(tileset);
            }

            Log.Debug("Initializing layers.");
            var tmxLayers = _tmxMap.Layers;
            var layerCount = tmxLayers.Count;
            _tileLayers = new TileLayer[_tmxMap.Layers.Count];
            for (var layerIndex = 0; layerIndex < layerCount; layerIndex++)
                _tileLayers[layerIndex] = new TileLayer(this, tmxLayers[layerIndex]);

            Log.Debug("Creating view.");
            _view = new View(new Vector2f(400, 300), new Vector2f(800, 600));
        }

        public string Filename { get; private set; }

        public string Name { get; private set; }

        public int TileHeight { get; private set; }

        public int TileWidth { get; private set; }

        public Dictionary<string, Tileset> Tilesets
        {
            get
            {
                return _tilesets;
            }
        }

        public View View { get { return _view; } }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.SetView(_view);

            states.Transform *= Transform;
            states.BlendMode = BlendMode.Alpha;

            foreach (var layer in _tileLayers)
                target.Draw(layer, states);

            target.SetView(target.DefaultView);
        }

        public Tileset GetTilesetFromGid(int gid)
        {
            return _tilesets.Values.First(t => t.HasGid(gid));
        }
    }
}
