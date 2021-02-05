using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Content;
using MonoGame.Extended.Sprites;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using SpaceEngineer.GameObjects.Ship;
using MonoGame.Extended.Collisions;
using System;
using SpaceEngineer.GameObjects;
using SpaceEngineer.GUI;
using SpaceEngineer.Hud;

namespace SpaceEngineer
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // List of GameObjects; Also used by CollisionComponent for Collision detection
        private readonly List<ShipComponent> _entities = new List<ShipComponent>();
        private Player player;
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

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            BoxingViewportAdapter viewport = new BoxingViewportAdapter(Window, GraphicsDevice, 1280, 720);

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            _camera = new OrthographicCamera(viewport);
            _cameraScale = 4f;
            _camera.ZoomIn(_cameraScale);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _tiledMap = Content.Load<TiledMap>("Test");
            _tiledMapRenderer = new TiledMapRenderer(GraphicsDevice, _tiledMap);

            MapWidth = _tiledMap.WidthInPixels;
            MapHeight = _tiledMap.HeightInPixels;

            _collisionComponent = new CollisionComponent(new RectangleF(0, 0, MapWidth, MapHeight));

            GenerateTiledCollisions();

            // Hard-coded player starting position
            // TODO - Obtain starting point from TileMap (using Objects above)
            Vector2 spawn = new Vector2(100f, 100f);

            Sprite progressFill = new Sprite(Content.Load<Texture2D>("GUI/progressbar_fill"));
            Sprite progressBorder = new Sprite(Content.Load<Texture2D>("GUI/progressbar_border"));

            ProgressBar fixProgress = new ProgressBar(progressFill, spawn, new Vector2(1f, 1f), progressBorder);

            player = new Player(Content.Load<SpriteSheet>("player.sf", new JsonContentLoader()), spawn, _camera, fixProgress);
            _collisionComponent.Insert(player);

            GenerateHud();
            GenerateShipObjects();

            foreach (IEntity entity in _entities)
            {
                _collisionComponent.Insert(entity);
            }

        }

        private void Button_OnClick(object sender, EventArgs e)
        {
            Console.WriteLine("Clicked inventory button!!");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            player.Update(gameTime);
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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Matrix transform = _camera.GetViewMatrix();

            _tiledMapRenderer.Draw(_tiledMap.GetLayer("Space"), transform);
            _tiledMapRenderer.Draw(_tiledMap.GetLayer("ShipFloor"), transform);
            _tiledMapRenderer.Draw(_tiledMap.GetLayer("Collidable"), transform);

            _spriteBatch.Begin(transformMatrix: transform, samplerState: SamplerState.PointClamp );
            // Draws all game objects to screen
            foreach (ShipComponent entity in _entities)
                entity.Draw(_spriteBatch);
            player.Draw(_spriteBatch);

            _spriteBatch.End();

            _tiledMapRenderer.Draw(_tiledMap.GetLayer("UpperLayer"), transform);

            // HUD Section
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            playerInventory.Draw(_spriteBatch);
            toolboxInventory.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        // Helper methods

        private void GenerateTiledCollisions()
        {
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
                    // _entities.Add(w);

                }
            }
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

                Sprite sprite = new Sprite(Content.Load<Texture2D>(assetName));
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

        private void GenerateHud()
        {

            Vector2 guiScale = new Vector2(_cameraScale, _cameraScale);
            Sprite btnSprite = new Sprite(Content.Load<Texture2D>("GUI/inventory_slot"));

            Vector2 inventoryPos = new Vector2(900f, 100f);
            Inventory pInventory = new Inventory();

            Vector2 toolboxInventoryPos = new Vector2(400f, 400f);
            Inventory tInventory = new Inventory();
            tInventory.slots = 4;

            Sprite drillSprite = new Sprite(Content.Load<Texture2D>("Items/drill"));
            Item drill = new Item(drillSprite, ItemType.Drill);
            tInventory.AddItem(drill);

            Sprite filterSprite = new Sprite(Content.Load<Texture2D>("Items/O2Filter"));
            Item filter = new Item(filterSprite, ItemType.O2Filter);
            tInventory.AddItem(filter);

            Sprite swSprite = new Sprite(Content.Load<Texture2D>("Items/screwdriver"));
            Item sw = new Item(swSprite, ItemType.Screwdriver);
            tInventory.AddItem(sw);

            Sprite wrenchSprite = new Sprite(Content.Load<Texture2D>("Items/wrench"));
            Item wrench = new Item(wrenchSprite, ItemType.Wrench);
            tInventory.AddItem(wrench);
            tInventory.SetDepositInventory(pInventory);
            pInventory.SetDepositInventory(tInventory);

            playerInventory = new InventoryHUD(inventoryPos, btnSprite, guiScale, pInventory);
            toolboxInventory = new InventoryHUD(toolboxInventoryPos, btnSprite, guiScale, tInventory);

            player.SetInventory(pInventory);
        }

        private void UpdateShipComponent(ShipComponent component, GameTime gameTime)
        {
            component.Update(gameTime);

            Vector2 compPos = component.GetPosition();
            float distance = Vector2.Distance(compPos, player.GetPosition());

            if (distance <= player.interactRad)
            {
                player.SetFocus(component);
            }
        }
    }
}
