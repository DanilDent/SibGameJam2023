using System;
using System.Collections;
using UnityEngine;

namespace Helpers
{
    public static class CoroutineHelpers
    {
        public static IEnumerator InvokeWithDelay(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
    }
}
