namespace Sharparam.Scroller.Mapping
{
    using System.Collections.Generic;
    using System.Linq;

    using SFML.Graphics;
    using SFML.Window;

    using TiledSharp;

    public class TileLayer : Drawable
    {
        private class VerticesLayer : Drawable
        {
            private readonly Color _color;

            private readonly Texture _texture;

            private readonly int _tileHeight;

            private readonly int _tileWidth;

            private readonly VertexArray _vertices;

            public VerticesLayer(Texture texture, int tileHeight, int tileWidth, double opacity = 1.0)
            {
                _texture = texture;
                _tileHeight = tileHeight;
                _tileWidth = tileWidth;
                _color = new Color(255, 255, 255, (byte)(255 * opacity));
                _vertices = new VertexArray(PrimitiveType.Quads);
            }

            public void AddTile(float x, float y, float textureX, float textureY)
            {
                AddTile(x, y, new Vector2f(textureX, textureY));
            }

            public void AddTile(float x, float y, Vector2f texPos)
            {
                var startIndex = _vertices.VertexCount;
                if (_vertices.VertexCount < startIndex + 4)
                    _vertices.Resize(startIndex + 4);
                var pos = new Vector2f(x, y);
                // Top left
                _vertices[startIndex++] = new Vertex(pos, _color, texPos);
                // Top right
                var offset = new Vector2f(_tileWidth, 0);
                _vertices[startIndex++] = new Vertex(pos + offset, _color, texPos + offset);
                // Bottom right
                offset.Y = _tileHeight;
                _vertices[startIndex++] = new Vertex(pos + offset, _color, texPos + offset);
                // Bottom left
                offset.X = 0;
                _vertices[startIndex] = new Vertex(pos + offset, _color, texPos + offset);
            }

            public void Draw(RenderTarget target, RenderStates states)
            {
                states.Texture = _texture;
                target.Draw(_vertices, states);
            }
        }

        private readonly string _name;

        private readonly double _opacity;

        private readonly TmxLayer _tmxLayer;

        private readonly VerticesLayer[] _verticesLayers;

        public TileLayer(Map map, TmxLayer tmxLayer)
        {
            _tmxLayer = tmxLayer;

            Visible = _tmxLayer.Visible;
            _name = _tmxLayer.Name;
            _opacity = _tmxLayer.Opacity;

            // For each different texture used in the layer, we need one vertices layer entry.
            // We'll use a dictionary to organize tiles based on texture name, and then
            // compress it to an array for more efficient iterating.
            var tilesetLayerMapping = new Dictionary<Tileset, VerticesLayer>();

            foreach (var tile in _tmxLayer.Tiles.Where(t => t.Gid != 0))
            {
                var tileset = map.GetTilesetFromGid(tile.Gid);

                if (!tilesetLayerMapping.ContainsKey(tileset))
                    tilesetLayerMapping[tileset] = new VerticesLayer(tileset.Texture, tileset.TileHeight, tileset.TileWidth, _opacity);

                var texPos = tileset.GetTileCoordinates(tile.Gid);
                tilesetLayerMapping[tileset].AddTile(tile.X * tileset.TileWidth, tile.Y * tileset.TileHeight, texPos);
            }

            _verticesLayers = tilesetLayerMapping.Values.ToArray();
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public bool Visible { get; set; }

        public void Draw(RenderTarget target, RenderStates states)
        {
            if (!Visible)
                return;

            foreach (var verticesLayer in _verticesLayers)
                target.Draw(verticesLayer, states);
        }
    }
}
