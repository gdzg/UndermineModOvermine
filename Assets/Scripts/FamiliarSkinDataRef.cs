using Thor;
using Overmine.API.Assets.References;
using UnityEngine;

[CreateAssetMenu(fileName = "FamiliarSkinDataRef", menuName = "References/Familiar Skin Data")] 
public class FamiliarSkinDataRef : FamiliarSkinData ,IDataObjectReference
{
    public string Guid => guid;
}