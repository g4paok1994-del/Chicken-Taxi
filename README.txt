Chicken Taxi — Unity Starter (drop-in)

1) Create a NEW Unity 2D project.
2) Drag this folder's Assets into your project's Assets.
3) Create a scene called Game and build these objects:
   - _Systems: GameManager, UIManager, SoundRouter, InputPause
   - Main Camera: CameraFollow2D
   - Taxi: SpriteRenderer (sprite_taxi_512), Rigidbody2D, BoxCollider2D, TaxiController, MobileInputRouter
   - Road: Create a SpriteRenderer using tile_road_512 (set layer 'Road') and add BoxCollider2D.
   - Spawner: SmartPassengerSpawner (assign chickenPrefab, set roadMask = Road layer)
   - Chicken prefab: SpriteRenderer (sprite_chicken_512), CircleCollider2D(isTrigger), ChickenPassenger
   - Canvas: HUD/MainMenu/Pause/GameOver panels + Texts and arrow Image. Assign to UIManager & TargetArrow.
4) Build & Run to Android.
