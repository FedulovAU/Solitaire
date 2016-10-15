using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(Card))]
public class CustomInspectorForCard : Editor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Update"))
        {
            if (target.GetType() == typeof(Card))
            {
                Card card = (Card)target;
                //card.UpdateCard();
            Debug.Log("update");
            }
        }
    }

}
