using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime;
using Editor;
using Overmine.API.Assets;
using Overmine.API.Assets.References;
using UnityEditor;
using Editor.Util;
using UnityEngine;
using Thor;

public class BuildLangauage
{
    [MenuItem("Overmine/BuildLangauage")]
    private static void Build()
    {
        var locDbs = Exporter.FindAssetsByType<LocDB>();
        var itemDatas = Exporter.FindAssetsByType<ItemData>();
        foreach (var itemData in itemDatas)
        {
            if (itemData is IAssetReference || itemData.guid == "")
            {
                continue;
            }

            var displayName = itemData.DisplayName.Text.Value;
            var description = itemData.Description.Text.Value;
            var flavor = itemData.Flavor.Text.Value;

            var name = itemData.name.Replace("Item", "");
            var strings = new[] { "displayName", "description", "flavor" };
            foreach (var property in strings)
            {
                string propertyValue = "";
                switch (property)
                {
                    case "displayName":
                        propertyValue = displayName;
                        break;
                    case "description":
                        propertyValue = description;
                        break;
                    case "flavor":
                        propertyValue = flavor;
                        break;
                }

                var key = name + "." + property;
                if (!propertyValue.Equals(key))
                {
                    itemData.SetSerializedField("m_" + property, new LocID
                    {
                        Text = new SharedString
                        {
                            Value = key
                        }
                    });
                    EditorUtility.SetDirty(itemData);
                    foreach (LocDB locDb in locDbs)
                    {
                        if (locDb.Keys.Contains(key))
                        {
                            continue;
                        }
                        locDb.Entries.Add(new LocDB.Entry
                        {
                            Key = key,
                            Text = locDb.Language == Localizer.LanguageType.English && property == "displayName"? name : propertyValue
                        });
                        EditorUtility.SetDirty(locDb);
                    }
                }
            }
        }
    }
}