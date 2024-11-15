using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Visus.Robotics.RosBridge
{
    public class ROSConfigurationWindow: EditorWindow
    {
        public static readonly float HEADER_TEXT_SIZE = 12f;
        
        [MenuItem("VISUS/Robotics/Configure ROS Bridge")]
        public static void ConfigureROS () {
            ROSConfigurationWindow wnd = GetWindow<ROSConfigurationWindow>();
            wnd.titleContent = new GUIContent("ROS Bridge Configuration");
        }

        private static ROSConfiguration config;
        private static string asset_path;
        public void OnEnable()
        {
            if (config == null)
            {
                config = Resources.Load<ROSConfiguration>("_ros_configuration");
                asset_path = AssetDatabase.GetAssetPath(config);
            }
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            Foldout config_root = createFoldout("Configuration");
            Foldout net_config = createFoldout("Network");

            SerializedObject config_obj = new SerializedObject(config);
            
            net_config.Add(createTextField("Host Address", config_obj.FindProperty("IPAddress")));
            net_config.Add(createToggle("Use TCP", config_obj.FindProperty("useTCP")));
            net_config.Add(createIntegerField("TCP Port", config_obj.FindProperty("TCPPort")));
            net_config.Add(createToggle("Connect on Startup", config_obj.FindProperty("connectOnStartup")));
            
            config_root.Add(net_config);

            Foldout log_config = createFoldout("Logging");
            
            log_config.Add(createToggle("Auto Forward Logs",config_obj.FindProperty("autoForwardLogs")));
            log_config.Add(createTextField("Component Name", config_obj.FindProperty("defaultComponentName")));
            log_config.Add(createDropdownField("Minimum Log Level", config_obj.FindProperty("minimumForwardLevel"),
                new []{"Any", "Log", "Warning", "Error", "Exception", "Assert"}));
            
            config_root.Add(log_config);

            ScrollView scrollView = new ScrollView(ScrollViewMode.Vertical);
            
            scrollView.Add(config_root);
            
            
            Foldout net_stat = createFoldout("Network Status");
            
            net_stat.Add(createInfoEntry("TCP State", ROSConfiguration.client_states, 
                ROSConfiguration.client_state_colors, config_obj.FindProperty("TCPClientState")));

            scrollView.Add(net_stat);
            
            root.Add(scrollView);
        }

        private VisualElement createInfoEntry(string name, string[] texts, Color[] colors, SerializedProperty property, 
            float font_size = 12.0f)
        {
            StatusWidget status = new StatusWidget(name, font_size, texts, colors);
            
            status.BindProperty(property);
            
            return status;
        }

        private Foldout createFoldout(string label)
        {
            Foldout foldout = new Foldout();
            foldout.text = label;
            foldout.style.unityFontStyleAndWeight = FontStyle.Bold;
            foldout.style.fontSize = HEADER_TEXT_SIZE;
            return foldout;
        }

#if UNITY_2021_3_OR_NEWER
        /// <summary>
        /// Creates a DropDown field with the given choices for the given property
        /// </summary>
        /// <param name="property">Property to create the field for</param>
        /// <param name="choices">List of choices to display in the dropdown field</param>
        /// <param name="unit">Unit of the values in the drop down field.</param>
        /// <returns>the created drop down field</returns>
        public static DropdownField createDropdownField(string label, SerializedProperty property, string[] choices)
        {
            DropdownField field = new DropdownField();
            field.label = label;
            field.choices =  new List<string>(choices);
            field.BindProperty(property);
            
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                field.index = property.intValue;
                field.value = choices[property.intValue];
            }
            else
            {
                field.value = property.stringValue;
            }
            
            

            field.tooltip = property.tooltip;
            field.style.unityFontStyleAndWeight = FontStyle.Normal;
            
            return field;
        }
#else
        /// <summary>
        /// Creates a DropDown field with the given choices for the given property
        /// </summary>
        /// <param name="property">Property to create the field for</param>
        /// <param name="choices">List of choices to display in the dropdown field</param>
        /// <param name="unit">Unit of the values in the drop down field.</param>
        /// <returns>a label to inform the user to update their Unity version</returns>
        public static VisualElement createDropdownField(SerializedProperty property, string[] choices, string unit = "")
        {
            return createLabel("Please Update to Unity 2021.3 or newer for DropdownField support");
        }
#endif
        
        private TextField createTextField(string label, SerializedProperty property)
        {
            TextField field = new TextField();
            field.label = label;
            field.value = property.stringValue;
            field.BindProperty(property);

            return field;
        }

        private IntegerField createIntegerField(string label, SerializedProperty property)
        {
            IntegerField field = new IntegerField();
            field.label = label;
            field.value = property.intValue;
            field.BindProperty(property);

            return field;
        }
        
        private Toggle createToggle(string label, SerializedProperty property)
        {
            Toggle field = new Toggle();
            field.label = label;
            field.value = property.boolValue;
            field.BindProperty(property);

            return field;
        }
    }
}