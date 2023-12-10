using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace com.overmine.editor.Editor
{
    public class TrimCanvasEditor : EditorWindow
    {
        [MenuItem("Overmine/TrimCanvas")]
        private static void GetWindow()
        {
            TrimCanvasEditor window = GetWindow<TrimCanvasEditor>(true, "修改画布大小", true);
            window.Show();
        }

        private SerializedObject serializedObject;
        private SerializedProperty texture2DListProperty;
        private Vector2Int trimSize;
        private Vector2Int offset;
        private string savePath;
        private string namePrefix;
        private string namePostfix;

        [SerializeField]
        private List<Texture2D> texture2DList;
        private Vector2 selectScrollPosition = Vector2.zero;

        private void OnEnable()
        {
            texture2DList = new List<Texture2D>();
            savePath = @"D:\\project\\Overmine.Editor\\Assets\\Resources\\Sprite";
            namePostfix = "";
            trimSize = new Vector2Int(112, 112);

            serializedObject = new SerializedObject(this);
            texture2DListProperty = serializedObject.FindProperty("texture2DList");
        }

        private void OnDisable()
        {
            serializedObject.Dispose();
        }

        private void OnGUI()
        {
            selectScrollPosition = EditorGUILayout.BeginScrollView(selectScrollPosition, EditorStyles.helpBox);

            EditorGUILayout.PropertyField(texture2DListProperty, new GUIContent("选择图集"), true);

            EditorGUILayout.Space();

            trimSize = EditorGUILayout.Vector2IntField("目标尺寸", trimSize);
            offset = EditorGUILayout.Vector2IntField("偏移", offset);

            EditorGUILayout.Space();

            for (int i = 0; i < texture2DListProperty.arraySize; i++)
            {
                SerializedProperty texProperty = texture2DListProperty.GetArrayElementAtIndex(i);
            }

            EditorGUILayout.BeginHorizontal();

            savePath = GUILayout.TextField(savePath);

            if (GUILayout.Button("选择保存路径"))
            {
                savePath = EditorUtility.OpenFolderPanel("路径选择", savePath, "");
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("图片前缀");
            namePrefix = EditorGUILayout.TextField(namePrefix);
            EditorGUILayout.LabelField("图片后缀");
            namePostfix = EditorGUILayout.TextField(namePostfix);
           
          
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("保存", GUILayout.MinHeight(24), GUILayout.MinWidth(120)))
            {
                for (int i = 0; i < texture2DListProperty.arraySize; i++)
                {
                    SerializedProperty texProperty = texture2DListProperty.GetArrayElementAtIndex(i);

                    Texture2D item = texProperty.objectReferenceValue as Texture2D;

                    Texture2D texture = new Texture2D(trimSize.x, trimSize.y, TextureFormat.RGBA32, item.streamingMipmaps);

                    if (!item.isReadable)
                    {
                        Debug.LogWarning("请打开图片的 Read/Write Enable");
                        break;
                    }

                    int widthDifference = texture.width - item.width;
                    int heightDifference = texture.height - item.height;

                    widthDifference /= 2;
                    heightDifference /= 2;

                    widthDifference += offset.x;
                    heightDifference += offset.y;

                    Color[] itemPixels = item.GetPixels();
                    Color[] texturePixels = texture.GetPixels();

                    for (int j = 0; j < texturePixels.Length; j++)
                    {
                        int x = j % texture.width;
                        int y = j / texture.width;

                        x -= widthDifference;
                        y -= heightDifference;

                        int itemIndex = y * item.width + x;

                        if (x >= 0 && y >= 0 && x < item.width && y < item.height && itemIndex < itemPixels.Length)
                        {
                            texturePixels[j] = itemPixels[itemIndex];
                        }
                        else
                        {
                            texturePixels[j] = new Color(0, 0, 0, 0);
                        }
                    }

                    texture.SetPixels(0, 0, texture.width, texture.height, texturePixels);
                    texture.Apply();

                    string contents = Application.dataPath;

                    byte[] bytes = texture.EncodeToPNG();
                    if (!Directory.Exists(contents))
                        Directory.CreateDirectory(contents);
                    if (namePostfix == string.Empty)
                    {
                        namePostfix = $"-{trimSize.x}x{trimSize.y}" ;
                    }
                    FileStream file = File.Open(string.Format("{0}/{1}{2}{3}.png", savePath, namePrefix, texProperty.objectReferenceValue.name, namePostfix), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    BinaryWriter writer = new BinaryWriter(file);
                    writer.Write(bytes);
                    file.Close();

                    EditorUtility.DisplayProgressBar("正在处理 : ", name, (float)i / texture2DListProperty.arraySize);
                }

                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
