using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[CreateAssetMenu(fileName = "ControllerType", menuName = "Hanadayori/ControllerType")]
public class G_ControllerType : ScriptableObject
{
    public ControllerPathTexture[] controllerPathes = new ControllerPathTexture[Enum.GetValues(typeof(G_TutorialBar.ButtonType)).Length];

    [Serializable]
    public class ControllerPathTexture
    {
        public G_TutorialBar.ButtonType buttonType;
        public Sprite texture;

        public ControllerPathTexture(G_TutorialBar.ButtonType _buttonType, Sprite _texture = null)
        {
            buttonType = _buttonType;
            texture = _texture;
        }
    }
}
