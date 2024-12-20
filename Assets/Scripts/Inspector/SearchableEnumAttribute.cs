using System.Linq;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.IMGUI.Controls;
#endif
using UnityEngine;

public class SearchableEnumAttribute : PropertyAttribute
{
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SearchableEnumAttribute))]
public class SearchableEnumDrawer : PropertyDrawer
{
    private string _enumName;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.type != "Enum")
        {
            EditorGUI.PropertyField(position, property, label);
            return;
        }

        try
        {
            _enumName = property.enumNames[property.enumValueIndex];
        }
        catch
        {
            _enumName = "Invalid Index";
        }

        var newRect = EditorGUI.PrefixLabel(position, label);
        if (GUI.Button(newRect, _enumName, EditorStyles.popup))
        {
            Popup.Show(newRect, property);
        }
    }

    private class Popup : PopupWindowContent
    {
        private readonly SearchField _searchField;
        private readonly TreeView _treeView;
        private readonly float _height;

        public static void Show(Rect position, SerializedProperty property)
        {
            PopupWindow.Show(position, new Popup(property));
        }

        private Popup(SerializedProperty property)
        {
            _height = GetHeight(property);

            _treeView = new PopupTreeView(new TreeViewState(), this, property);
            _searchField = new SearchField();
            _searchField.downOrUpArrowKeyPressed += _treeView.SetFocusAndEnsureSelectedItem;
            _treeView.Reload();
            _searchField.SetFocus();
        }

        public override void OnGUI(Rect rect)
        {
            rect.height = EditorGUIUtility.singleLineHeight;
            _treeView.searchString = _searchField.OnGUI(rect, _treeView.searchString);
            rect.y = EditorGUIUtility.singleLineHeight;
            rect.height = GetWindowSize().y - EditorGUIUtility.singleLineHeight;
            _treeView.OnGUI(rect);
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(200f, _height);
        }

        private static float GetHeight(SerializedProperty property)
        {
            return Mathf.Min(property.enumNames.Length, 8) * EditorGUIUtility.singleLineHeight;
        }

        private class PopupTreeView : TreeView
        {
            private readonly SerializedProperty _property;
            private readonly Popup _popup;

            public PopupTreeView(TreeViewState state, Popup popup, SerializedProperty property) : base(state)
            {
                _property = property;
                _popup = popup;
            }

            public PopupTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
            {
            }

            private void UpdateEnumValue()
            {
                if (state.selectedIDs.Count <= 0)
                    return;

                _property.serializedObject.Update();
                _property.enumValueIndex = state.selectedIDs.First();
                _property.serializedObject.ApplyModifiedProperties();
                _popup.editorWindow.Close();
            }

            protected override TreeViewItem BuildRoot()
            {
                var root = new TreeViewItem(-1, -1, "root");
                for (var i = 0; i < _property.enumNames.Length; ++i)
                    root.AddChild(new TreeViewItem(i, 0, _property.enumNames[i]));
                return root;
            }

            protected override void KeyEvent()
            {
                if (Event.current.keyCode == KeyCode.Return)
                    UpdateEnumValue();
            }

            protected override void SingleClickedItem(int id)
            {
                UpdateEnumValue();
            }
        }
    }
}

#endif
