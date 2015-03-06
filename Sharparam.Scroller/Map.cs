namespace Sharparam.Scroller
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

        // One vertex array per layer. TODO: Support multiple tilesets?
        private readonly VertexArray[] _vertexArrays;

        private readonly View _view;

        // Temp tex cache
        private Texture _texture;

        public Map(string path)
        {
            Filename = Path.GetFullPath(path);
            var mapDir = Path.GetDirectoryName(Filename);
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

            Log.Debug("Creating vertex arrays.");

            _vertexArrays = new VertexArray[_tmxMap.Layers.Count];
            for (var layerIndex = 0; layerIndex < _tmxMap.Layers.Count; layerIndex++)
            {
                var layer = _tmxMap.Layers[layerIndex];
                var tiles = layer.Tiles.Where(t => t.Gid != 0).ToArray();
                var vertArray = new VertexArray(PrimitiveType.Quads, (uint)tiles.Length * 4);
                Log.DebugFormat("Creating {0} vertices (quads) for layer index {1} ({2})", vertArray.VertexCount, layerIndex, layer.Name);
                Log.Debug("Populating tiles (vertices)");

                // For now, use the only tileset
                var tileset = _tilesets.First().Value;

                for (var tileIndex = 0; tileIndex < tiles.Length; tileIndex++)
                {
                    var tile = tiles[tileIndex];
                    var vertexStartIndex = (uint)tileIndex * 4;
                    var texCoord = tileset.GetTileCoordinates(tile.Gid);
                    var x = tile.X * TileWidth;
                    var y = tile.Y * TileHeight;
                    vertArray[vertexStartIndex++] = new Vertex(new Vector2f(x, y), texCoord);
                    vertArray[vertexStartIndex++] = new Vertex(new Vector2f(x + TileWidth, y), new Vector2f(texCoord.X + TileWidth, texCoord.Y));
                    vertArray[vertexStartIndex++] = new Vertex(new Vector2f(x + TileWidth, y + TileHeight), new Vector2f(texCoord.X + TileWidth, texCoord.Y + TileHeight));
                    vertArray[vertexStartIndex] = new Vertex(new Vector2f(x, y + TileHeight), new Vector2f(texCoord.X, texCoord.Y + TileHeight));
                }

                _vertexArrays[layerIndex] = vertArray;
            }

            Log.Debug("Creating view.");
            _view = new View(new Vector2f(400, 300), new Vector2f(800, 600));

            _texture = _tilesets.First().Value.Texture;
        }

        public string Filename { get; private set; }

        public string Name { get; private set; }

        public int TileHeight { get; private set; }

        public int TileWidth { get; private set; }

        public View View { get { return _view; } }

        public void Draw(RenderTarget target, RenderStates states)
        {
            target.SetView(_view);
            states.Transform *= Transform;
            states.Texture = _texture;
            states.BlendMode = BlendMode.Alpha;
            foreach (var arr in _vertexArrays)
                if (arr.VertexCount > 0)
                    target.Draw(arr, states);
            target.SetView(target.DefaultView);
        }
    }
}
