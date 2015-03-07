namespace Sharparam.Scroller
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using log4net;

    using SFML.Graphics;
    using SFML.Window;

    using TiledSharp;

    public class Tileset
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Tileset));

        private readonly Dictionary<int, Vector2f> _coordCache;
        
        private readonly int _firstGid;
        
        private readonly int _height;

        private readonly int _lastGid;

        private readonly string _name;

        private readonly bool _preloaded;

        private readonly Texture _texture;

        private readonly int _tileCount;
        
        private readonly int _tilesX;
        
        private readonly int _tilesY;
        
        private readonly TmxTileset _tmxTileset;

        private readonly int _width;

        public Tileset(TmxTileset tmxTileset, bool preload = true)
        {
            _tmxTileset = tmxTileset;
            _name = _tmxTileset.Name;
            var tmxImage = _tmxTileset.Image;
            Log.InfoFormat("Loading tileset {0}, image: {1}", _name, tmxImage.Source ?? "data stream");

            _firstGid = _tmxTileset.FirstGid;
            Log.DebugFormat("First GID is {0}", _firstGid);

            Log.Debug("Loading tileset texture from TMX tileset image...");

            if (!tmxImage.Height.HasValue)
                throw new TilesetException("Tileset image has no height!");
            if (!tmxImage.Width.HasValue)
                throw new TilesetException("Tileset image has no width!");
            _height = tmxImage.Height.Value;
            _width = tmxImage.Width.Value;
            Log.DebugFormat("Tileset image is {0}x{1}", _width, _height);

            Image image;
            if (tmxImage.Source == null)
            {
                Log.Debug("Loading image from embedded data string.");
                image = new Image(tmxImage.Data);
            }
            else
            {
                Log.Debug("Loading image from file.");
                image = new Image(_tmxTileset.Image.Source);
            }

            Debug.Assert(image.Size.X == _width, "Loaded image width is different from source image width.");
            Debug.Assert(image.Size.Y == _height, "Loaded image height is different from source image height.");

            var trans = _tmxTileset.Image.Trans;
            if (trans != null)
                image.CreateMaskFromColor(new Color((byte)trans.R, (byte)trans.G, (byte)trans.B));

            _texture = new Texture(image);

            Debug.Assert(_texture.Size.X == _width, "Loaded texture width is different from source image width.");
            Debug.Assert(_texture.Size.Y == _height, "Loaded texture height is different from source image height.");

            Log.DebugFormat("Tileset texture loaded, dimensions: {0},{1}", _width, _height);
            _tilesX = _width / _tmxTileset.TileWidth;
            _tilesY = _height / _tmxTileset.TileHeight;
            _tileCount = _tmxTileset.Tiles.Count;
            Debug.Assert(_tileCount == _tilesX * _tilesY, "Tile count given by TMX is different from calculated count.");
            Log.DebugFormat("Tileset has {0}x{1} ({2}) tiles.", _tilesX, _tilesY, _tileCount);
            _lastGid = _firstGid + _tileCount - 1;
            Log.Debug("Creating coord cache.");
            _coordCache = new Dictionary<int, Vector2f>(_tileCount);
            if (!preload)
            {
                _preloaded = false;
                return;
            }
            Log.Debug("Populating coord cache.");
            for (var id = _firstGid; id <= _lastGid; id++)
                GetTileCoordinates(id);
            _preloaded = true;
        }

        public int FirstGid
        {
            get
            {
                return _firstGid;
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
        }

        public Texture Texture
        {
            get
            {
                return _texture;
            }
        }

        public int TileCount
        {
            get
            {
                return _tileCount;
            }
        }

        public int TileHeight
        {
            get
            {
                return _tmxTileset.TileHeight;
            }
        }

        public int TileWidth
        {
            get
            {
                return _tmxTileset.TileWidth;
            }
        }

        public int TilesX
        {
            get
            {
                return _tilesX;
            }
        }

        public int TilesY
        {
            get
            {
                return _tilesY;
            }
        }

        public int Width
        {
            get
            {
                return _width;
            }
        }

        public Vector2f GetTileCoordinates(int gid)
        {
            if (gid < _firstGid || gid > _lastGid)
                throw new ArgumentOutOfRangeException("gid", gid, "GID is outside the range of defined tileset GIDs.");
            if (_coordCache.ContainsKey(gid))
                return _coordCache[gid];
            if (_preloaded)
                Log.WarnFormat(
                    "Tile coordinates are preloaded but GetTileCoordinates for {0} is calculating tile coordinates for GID {1}",
                    _name,
                    gid);
            var sheetIndex = gid - _firstGid;
            var row = sheetIndex / _tilesX;
            var col = sheetIndex % _tilesX;
            var coords = new Vector2f(col * _tmxTileset.TileWidth, row * _tmxTileset.TileHeight);
            _coordCache[gid] = coords;
            return coords;
        }
    }
}
