namespace Sharparam.Scroller
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using log4net;

    using SFML.Graphics;
    using SFML.Window;

    using TiledSharp;

    public class Tileset
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Tileset));

        private readonly TmxTileset _tmxTileset;

        private readonly string _imageFile;

        private readonly Texture _texture;

        private readonly int _firstGid;

        private readonly uint _width;

        private readonly uint _height;

        private readonly uint _tilesX;

        private readonly uint _tilesY;

        private readonly Dictionary<int, Vector2f> _coordCache; 

        public Tileset(TmxTileset tmxTileset)
        {
            _tmxTileset = tmxTileset;
            Log.InfoFormat("Loading tileset {0}, image: {1}", _tmxTileset.Name, _tmxTileset.Image.Source);
            Log.Debug("Loading tileset texture from TMX tileset image stream.");
            var image = new Image(_tmxTileset.Image.Source);
            var trans = _tmxTileset.Image.Trans;
            if (trans != null)
                image.CreateMaskFromColor(new Color((byte)trans.R, (byte)trans.G, (byte)trans.B));
            _texture = new Texture(image);
            _firstGid = _tmxTileset.FirstGid;
            _width = _texture.Size.X;
            _height = _texture.Size.Y;
            Log.DebugFormat("Tileset texture loaded, dimensions: {0},{1}", _width, _height);
            _tilesX = _width / (uint)_tmxTileset.TileWidth;
            _tilesY = _height / (uint)_tmxTileset.TileHeight;
            _coordCache = new Dictionary<int, Vector2f>(_tmxTileset.Tiles.Count);
        }

        public Texture Texture
        {
            get
            {
                return _texture;
            }
        }

        public Vector2f GetTileCoordinates(int gid)
        {
            if (_coordCache.ContainsKey(gid))
                return _coordCache[gid];
            var sheetIndex = gid - _firstGid;
            var row = sheetIndex / _tilesX;
            var col = sheetIndex % _tilesX;
            var coords = new Vector2f(col * _tmxTileset.TileWidth, row * _tmxTileset.TileHeight);
            _coordCache[gid] = coords;
            return coords;
        }
    }
}
