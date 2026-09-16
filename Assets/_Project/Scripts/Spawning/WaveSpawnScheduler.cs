using System;
using System.Collections.Generic;
using Game.Enemies;

namespace Game.Spawning
{
    public class WaveSpawnScheduler
    {
        private class EntryState
        {
            public readonly WaveSpawnEntry Entry;
            public int SpawnedCount;
            public float Timer;

            public EntryState(WaveSpawnEntry entry)
            {
                Entry = entry;
                Timer = entry.SpawnInterval;
            }
        }

        private readonly List<EntryState> _entryStates = new();

        public bool IsWaveComplete
        {
            get
            {
                foreach (var state in _entryStates)
                {
                    if (state.SpawnedCount < state.Entry.Count)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public void StartWave(WaveConfig waveConfig)
        {
            _entryStates.Clear();
            foreach (var entry in waveConfig.SpawnEntries)
            {
                _entryStates.Add(new EntryState(entry));
            }
        }

        public void Tick(float deltaTime, Action<EnemyData> onSpawnRequested)
        {
            foreach (var state in _entryStates)
            {
                if (state.SpawnedCount >= state.Entry.Count)
                {
                    continue;
                }

                state.Timer -= deltaTime;
                if (state.Timer > 0f)
                {
                    continue;
                }

                state.Timer = state.Entry.SpawnInterval;
                state.SpawnedCount++;
                onSpawnRequested(state.Entry.EnemyData);
            }
        }
    }
}
