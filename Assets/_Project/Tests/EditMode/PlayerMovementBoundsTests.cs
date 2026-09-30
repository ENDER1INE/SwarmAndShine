using System.Reflection;
using Game.Combat;
using Game.Player;
using Game.World;
using NUnit.Framework;
using UnityEngine;

namespace Game.Tests.EditMode
{
    public class FixedMovementInputSource : IMovementInputSource
    {
        private readonly Vector2 _input;

        public FixedMovementInputSource(Vector2 input)
        {
            _input = input;
        }

        public Vector2 GetMovementInput() => _input;
    }

    public class PlayerMovementBoundsTests
    {
        private GameObject _playerObject;

        [TearDown]
        public void TearDown()
        {
            if (_playerObject != null)
            {
                Object.DestroyImmediate(_playerObject);
            }
        }

        private PlayerMovement CreateMovement(Vector2 input, ArenaBounds bounds)
        {
            _playerObject = new GameObject("TestPlayer", typeof(Rigidbody2D));
            var movement = _playerObject.AddComponent<PlayerMovement>();
            typeof(PlayerMovement).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(movement, null);

            var stats = new CharacterStats(ScriptableObject.CreateInstance<CharacterBaseStatsData>());
            movement.Init(stats, new FixedMovementInputSource(input), bounds);
            return movement;
        }

        private static void RunFixedUpdate(PlayerMovement movement)
        {
            typeof(PlayerMovement).GetMethod("FixedUpdate", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(movement, null);
        }

        [Test]
        public void FixedUpdate_PlayerAtBoundaryPushingOutward_HasNoOutwardVelocity()
        {
            var bounds = ArenaBounds.FromCenterAndSize(Vector2.zero, new Vector2(20f, 20f));
            var movement = CreateMovement(Vector2.right, bounds);
            var body = _playerObject.GetComponent<Rigidbody2D>();
            body.position = new Vector2(10f, 0f);

            RunFixedUpdate(movement);

            Assert.AreEqual(0f, body.linearVelocity.x, 0.0001f);
        }

        [Test]
        public void FixedUpdate_PlayerNearBoundary_VelocityStopsAtBoundary()
        {
            var bounds = ArenaBounds.FromCenterAndSize(Vector2.zero, new Vector2(20f, 20f));
            var movement = CreateMovement(Vector2.right, bounds);
            var body = _playerObject.GetComponent<Rigidbody2D>();
            body.position = new Vector2(9.99f, 0f);

            RunFixedUpdate(movement);

            float distanceCoveredInStep = body.linearVelocity.x * Time.fixedDeltaTime;
            Assert.AreEqual(0.01f, distanceCoveredInStep, 0.0001f);
        }

        [Test]
        public void FixedUpdate_PlayerInsideArena_MovesAtFullSpeed()
        {
            var bounds = ArenaBounds.FromCenterAndSize(Vector2.zero, new Vector2(20f, 20f));
            var movement = CreateMovement(Vector2.right, bounds);
            var body = _playerObject.GetComponent<Rigidbody2D>();
            body.position = Vector2.zero;

            RunFixedUpdate(movement);

            Assert.Greater(body.linearVelocity.x, 0f);
        }
    }
}
