using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;
using SpaceEngineer.GameObjects;
using SpaceEngineer.GameObjects.Ship;
using SpaceEngineer.GUI;
using SpaceEngineer.Hud;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceEngineer
{
    /// <summary>
    /// Handles game rules and mechanics
    /// </summary>
    public class World
    {
        #region PrivateVariables
        private readonly MainGame _game;
        // List of GameObjects; Also used by CollisionComponent for Collision detection
        private readonly List<ShipComponent> _entities = new List<ShipComponent>();
        private Player _player;
        private CollisionComponent _collisionComponent;

        // Map boundaries defined by TileMap dimensions and used by the CollisionComponent
        private int MapWidth;
        private int MapHeight;

        TiledMap _tiledMap;
        TiledMapRenderer _tiledMapRenderer;

        InventoryHUD playerInventory;
        InventoryHUD toolboxInventory;

        // Camera; Moved with Player
        OrthographicCamera _camera;
        float _cameraScale;
        #endregion

        public World(MainGame game, BoxingViewportAdapter viewport)
        {
            _game = game;
            _camera = new OrthographicCamera(viewport);
            _cameraScale = 4f;
            _camera.ZoomIn(_cameraScale);
        }

        public void LoadContent()
        {
            GenerateTileMap();
            GeneratePlayer();
            GenerateHud();
            GenerateShipObjects();

            foreach (IEntity entity in _entities)
            {
                _collisionComponent.Insert(entity);
            }
        }

        public void Update(GameTime gameTime)
        {
            _player.Update(gameTime);
            // Updates all gmae objects on the screen
            foreach (ShipComponent entity in _entities)
            {
                UpdateShipComponent(entity, gameTime);
            }
            // Update for TileMap
            _tiledMapRenderer.Update(gameTime);
            // Update used for Collision Detection
            _collisionComponent.Update(gameTime);
            playerInventory.Update(gameTime);
            toolboxInventory.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Matrix transform = _camera.GetViewMatrix();

            _tiledMapRenderer.Draw(_tiledMap.GetLayer("Space"), transform);
            _tiledMapRenderer.Draw(_tiledMap.GetLayer("ShipFloor"), transform);
            _tiledMapRenderer.Draw(_tiledMap.GetLayer("Collidable"), transform);

            spriteBatch.Begin(transformMatrix: transform, samplerState: SamplerState.PointClamp);
            // Draws all game objects to screen
            foreach (ShipComponent entity in _entities)
                entity.Draw(spriteBatch);
            _player.Draw(spriteBatch);

            spriteBatch.End();

            _tiledMapRenderer.Draw(_tiledMap.GetLayer("UpperLayer"), transform);

            // HUD Section
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            playerInventory.Draw(spriteBatch);
            toolboxInventory.Draw(spriteBatch);
            spriteBatch.End();
        }

        #region HelperMethods

        private void GenerateTileMap()
        {
            _tiledMap = _game.Content.Load<TiledMap>("Test");
            _tiledMapRenderer = new TiledMapRenderer(_game.GraphicsDevice, _tiledMap);

            MapWidth = _tiledMap.WidthInPixels;
            MapHeight = _tiledMap.HeightInPixels;

            _collisionComponent = new CollisionComponent(new RectangleF(0, 0, MapWidth, MapHeight));

            TiledMapTileLayer collidables = _tiledMap.GetLayer<TiledMapTileLayer>("Collidable");

            if (collidables != null)
            {
                foreach (TiledMapTile t in collidables.Tiles)
                {
                    // Global Identifier 0 means tile is empty
                    if (t.GlobalIdentifier == 0) continue;

                    Vector2 pos = Vector2.Zero;

                    pos.X = t.X * collidables.TileWidth;
                    pos.Y = t.Y * collidables.TileHeight;

                    Size2 size = new Size2(collidables.TileWidth, collidables.TileHeight);

                    Wall w = new Wall(pos, size);
                    _collisionComponent.Insert(w);
                }
            }
        }

        private void GeneratePlayer()
        {
            // Hard-coded player starting position
            // TODO - Obtain starting point from TileMap
            Vector2 spawn = new Vector2(100f, 100f);

            Sprite progressFill = new Sprite(_game.Content.Load<Texture2D>("GUI/progressbar_fill"));
            Sprite progressBorder = new Sprite(_game.Content.Load<Texture2D>("GUI/progressbar_border"));

            ProgressBar fixProgress = new ProgressBar(progressFill, spawn, new Vector2(1f, 1f), progressBorder);

            _player = new Player(_game.Content.Load<SpriteSheet>("player.sf", new JsonContentLoader()), spawn, _camera, fixProgress);
            _collisionComponent.Insert(_player);
        }

        private void GenerateShipObjects()
        {
            TiledMapObject[] objects = _tiledMap.GetLayer<TiledMapObjectLayer>("Objects").Objects;

            // Loops through Objects created in Tiled
            // TODO - Refactor into reusable code for other objects
            foreach (TiledMapObject obj in objects)
            {
                string assetName;
                if (!obj.Properties.TryGetValue("sprite", out assetName)) continue;

                Sprite sprite = new Sprite(_game.Content.Load<Texture2D>(assetName));
                Vector2 pos = Vector2.Zero;
                pos.X = obj.Position.X + sprite.Origin.X;
                pos.Y = obj.Position.Y + sprite.Origin.Y;

                ShipComponent component = null;
                switch (obj.Name)
                {
                    case "O2Component":
                        component = new BreakableComponent(sprite, pos, ItemType.O2Filter);
                        break;
                    case "Toolbox":
                        component = new Toolbox(sprite, pos, playerInventory, toolboxInventory);
                        break;
                }
                if (component != null)
                    _entities.Add(component);

            }
        }

        /// <summary>
        /// Generates the player inventory and HUD logic
        /// </summary>
        private void GenerateHud()
        {

            Vector2 guiScale = new Vector2(_cameraScale, _cameraScale);
            Sprite btnSprite = new Sprite(_game.Content.Load<Texture2D>("GUI/inventory_slot"));

            Vector2 inventoryPos = new Vector2(900f, 100f);
            Inventory pInventory = new Inventory();

            Vector2 toolboxInventoryPos = new Vector2(400f, 400f);
            Inventory tInventory = new Inventory();
            tInventory.slots = 4;

            Sprite drillSprite = new Sprite(_game.Content.Load<Texture2D>("Items/drill"));
            Item drill = new Item(drillSprite, ItemType.Drill);
            tInventory.AddItem(drill);

            Sprite filterSprite = new Sprite(_game.Content.Load<Texture2D>("Items/O2Filter"));
            Item filter = new Item(filterSprite, ItemType.O2Filter);
            tInventory.AddItem(filter);

            Sprite swSprite = new Sprite(_game.Content.Load<Texture2D>("Items/screwdriver"));
            Item sw = new Item(swSprite, ItemType.Screwdriver);
            tInventory.AddItem(sw);

            Sprite wrenchSprite = new Sprite(_game.Content.Load<Texture2D>("Items/wrench"));
            Item wrench = new Item(wrenchSprite, ItemType.Wrench);
            tInventory.AddItem(wrench);
            tInventory.SetDepositInventory(pInventory);
            pInventory.SetDepositInventory(tInventory);

            playerInventory = new InventoryHUD(inventoryPos, btnSprite, guiScale, pInventory);
            toolboxInventory = new InventoryHUD(toolboxInventoryPos, btnSprite, guiScale, tInventory);

            _player.SetInventory(pInventory);
        }

        /// <summary>
        /// Updates each ship component and checks if player is close enough to interact with it
        /// </summary>
        /// <param name="component">Ship Object</param>
        /// <param name="gameTime">GameTime variable</param>
        private void UpdateShipComponent(ShipComponent component, GameTime gameTime)
        {
            component.Update(gameTime);

            Vector2 compPos = component.GetPosition();
            float distance = Vector2.Distance(compPos, _player.GetPosition());

            if (distance <= _player.interactRad)
            {
                _player.SetFocus(component);
            }
        }

        #endregion

    }
}
