using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    [Serializable]
    public class PlayerData
    {
        public int CurrentHealth;
        public int MaxHealth;

        public PlayerData() : base()
        {
            this.CurrentHealth = 100; // Default health value
            this.MaxHealth = 100; // Default max health value        
        }
    }
}
