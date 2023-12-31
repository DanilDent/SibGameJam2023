﻿using Config;
using Helpers;
using UnityEngine;

namespace Game
{
    [DefaultExecutionOrder(-1)]
    public class ConfigContainer : MonoSingleton<ConfigContainer>
    {
        public GameConfigSO Value => _config;

        [SerializeField] private GameConfigSO _config;
    }
}
