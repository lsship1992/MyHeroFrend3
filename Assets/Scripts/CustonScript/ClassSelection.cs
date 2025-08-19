using UnityEngine;
using Assets.HeroEditor.Common.Scripts.EditorScripts;

public class ClassSelection : MonoBehaviour
{
    public CharacterEditor characterEditor; // Используем класс из ассета

    public void SelectClass(int classIndex)
    {
        if (characterEditor != null)
        {
            // Вызываем методы characterEditor для изменения класса
        }
    }
}