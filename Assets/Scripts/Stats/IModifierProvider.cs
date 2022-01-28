using System.Collections.Generic;

namespace RPG.Stats
{
    public interface IModifierProvider
    {
        public IEnumerable<float> GetAdditiveModifier(StatType statType);
    }
}