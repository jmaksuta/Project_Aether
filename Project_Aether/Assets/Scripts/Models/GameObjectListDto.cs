using ProjectAether.Objects.Net._2._1.Standard.Models;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class GameObjectListDto : List<GameObjectDto>
    {
        //public List<GameObjectDto> objects;

        public List<GameObject> ToListOfGameObject()
        {
            List<GameObject> result = new List<GameObject>();
            foreach (GameObjectDto goDto in this)
            {
                result.Add(goDto.ToGameObject());
            }
            return result;
        }

    }

}
