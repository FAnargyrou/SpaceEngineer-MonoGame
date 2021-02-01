﻿using System.Collections;
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

        // Player GUI
        StackPanel _inventory;

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

            TiledMapObject[] objects = _tiledMap.GetLayer<TiledMapObjectLayer>("Objects").Objects;

            // Loops through Objects created in Tiled
            // TODO - Refactor into reusable code for other objects
            foreach (TiledMapObject obj in objects)
            {
                if (obj.Name == "O2Component")
                {

                    if (obj.Properties.ContainsKey("sprite"))
                    {
                        string assetName = obj.Properties["sprite"];
                        Sprite sprite = new Sprite(Content.Load<Texture2D>(assetName));
                        Vector2 pos = Vector2.Zero;
                        pos.X = obj.Position.X + sprite.Origin.X;
                        pos.Y = obj.Position.Y + sprite.Origin.Y;
                        BreakableComponent shipComponent = new BreakableComponent(sprite, pos);
                        _entities.Add(shipComponent);
                    }
                        
                }
            }

            GenerateTiledCollisions();

            // Hard-coded player starting position
            // TODO - Obtain starting point from TileMap (using Objects above)
            Vector2 spawn = new Vector2(100f, 100f);

            player = new Player(Content.Load<SpriteSheet>("player.sf", new JsonContentLoader()), spawn, _camera);
            _collisionComponent.Insert(player);

            foreach (IEntity entity in _entities)
            {
                _collisionComponent.Insert(entity);
            }

            Vector2 inventoryPos = new Vector2(900f, 100f);
            Size2 inventorySize = new Size2(0f, 0f);

            _inventory = new StackPanel(inventoryPos, inventorySize, 10f, Orientation.Horizontal);

            // MAGIC NUMBERS

            Sprite btnSprite = new Sprite(Content.Load<Texture2D>("GUI/inventory_slot"));
            Vector2 guiScale = new Vector2(_cameraScale, _cameraScale);
            Button inventorySlot1 = new Button(btnSprite, Vector2.Zero);
            inventorySlot1.SetScale(guiScale);
            Button inventorySlot2 = new Button(btnSprite, Vector2.Zero);
            inventorySlot2.SetScale(guiScale);

            _inventory.AddComponent(inventorySlot1);
            _inventory.AddComponent(inventorySlot2);

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
            _inventory.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Matrix transform = _camera.GetViewMatrix();

            // button.position = player.GetPosition(); // _camera.ScreenToWorld(button.position.X, button.position.Y);

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
            _inventory.Draw(_spriteBatch);

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
