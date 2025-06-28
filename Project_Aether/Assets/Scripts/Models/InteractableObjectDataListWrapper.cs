using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    [System.Serializable] // Wrapper for deserializing a list of objects if your API returns a root array
    public class InteractableObjectDataListWrapper
    {
        public List<InteractableObjectData> objects;
    }
}
