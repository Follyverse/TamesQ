using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[CustomEditor(typeof(Markers.MarkerArea)), CanEditMultipleObjects]
public class AreaInEditor : Editor
{
    //    SerializedProperty thisIsArea;
    SerializedProperty geometry;
    SerializedProperty range;
    SerializedProperty update;
    SerializedProperty mode;
    SerializedProperty appliesTo;
    SerializedProperty applyToSelf;
    SerializedProperty autoPosition;
    SerializedProperty control;

    void OnEnable()
    {
        //      thisIsArea = serializedObject.FindProperty("thisIsArea");
        geometry = serializedObject.FindProperty("geometry");
        range = serializedObject.FindProperty("range");
        update = serializedObject.FindProperty("update");
        mode = serializedObject.FindProperty("mode");
        applyToSelf = serializedObject.FindProperty("applyToSelf");
        appliesTo = serializedObject.FindProperty("appliesTo");
        autoPosition = serializedObject.FindProperty("autoPosition");
        control = serializedObject.FindProperty("control");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //      EditorGUILayout.PropertyField(thisIsArea);
        EditorGUILayout.PropertyField(geometry);
        EditorGUILayout.PropertyField(range);
        EditorGUILayout.PropertyField(update);
        EditorGUILayout.PropertyField(mode);
        EditorGUILayout.PropertyField(applyToSelf);
        EditorGUILayout.PropertyField(appliesTo);
        EditorGUILayout.PropertyField(autoPosition);
        EditorGUILayout.PropertyField(control);

        serializedObject.ApplyModifiedProperties();
    }
}
[CustomEditor(typeof(Markers.MarkerOrigin))]
public class OriginInEditor : Editor
{
    //    SerializedProperty thisIsArea;
    SerializedProperty origin;
    void OnEnable()
    {
        origin = serializedObject.FindProperty("origin");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(origin);
        serializedObject.ApplyModifiedProperties();
    }
}
[CustomEditor(typeof(Markers.MarkerDynamic))]
public class DynamicInEditor : Editor
{
    //    SerializedProperty thisIsArea;
    SerializedProperty type;
    SerializedProperty up;
    SerializedProperty mover;
    void OnEnable()
    {
        type = serializedObject.FindProperty("type");
        up = serializedObject.FindProperty("up");
        mover = serializedObject.FindProperty("mover");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(type);
        EditorGUILayout.PropertyField(up);
        EditorGUILayout.PropertyField(mover);
        serializedObject.ApplyModifiedProperties();
        Markers.MarkerDynamic dyn = (Markers.MarkerDynamic)target;
        dyn.ChangeDynamic();
        if (GUILayout.Button("Remove"))
        {
            dyn.Remove();
        }
    }
}


[CustomEditor(typeof(Markers.MarkerSettings))]
class MarkerSettingsEditor : Editor
{
    SerializedProperty autoSaveMode;
    SerializedProperty navMode;
    SerializedProperty eyeHeights;
    SerializedProperty email;
    SerializedProperty subject;
    SerializedProperty sendBy;
    //    SerializedProperty materialEmission;
    SerializedProperty replay;
    void OnEnable()
    {
        autoSaveMode = serializedObject.FindProperty("autoSaveMode");
        navMode = serializedObject.FindProperty("navMode");
        replay = serializedObject.FindProperty("replay");
        eyeHeights = serializedObject.FindProperty("eyeHeights");
        email = serializedObject.FindProperty("email");
        subject = serializedObject.FindProperty("subject");
        sendBy = serializedObject.FindProperty("sendBy");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(autoSaveMode);
        EditorGUILayout.PropertyField(navMode);
        EditorGUILayout.PropertyField(replay);
        EditorGUILayout.PropertyField(eyeHeights);
        EditorGUILayout.PropertyField(email);
        EditorGUILayout.PropertyField(subject);
        EditorGUILayout.PropertyField(sendBy);
        Markers.MarkerSettings settings = (Markers.MarkerSettings)target;
        Markers.MarkerSettings.AutoSaveMode = settings.autoSaveMode;
        if (GUILayout.Button("Save intensity"))
        {
            settings.FreezeIntensity();
        }
        if (GUILayout.Button("Reset intensity"))
        {
            settings.ResetIntensity();
        }
        if (GUILayout.Button("Save"))
        {
            settings.Save();
        }
        if (GUILayout.Button("Load"))
        {
            settings.Load();
        }
        serializedObject.ApplyModifiedProperties();
    }
}
[CustomEditor(typeof(Markers.MarkerLink)), CanEditMultipleObjects]
public class MarkerLinkEditor : Editor
{
    SerializedProperty type;
    SerializedProperty childrenNames;
    SerializedProperty childrenOf;
    SerializedProperty parent;
    SerializedProperty offsetBase;
    SerializedProperty speedBase;
    SerializedProperty offset;
    SerializedProperty factor;

    void OnEnable()
    {
        type = serializedObject.FindProperty("type");
        childrenNames = serializedObject.FindProperty("childrenNames");
        childrenOf = serializedObject.FindProperty("childrenOf");
        parent = serializedObject.FindProperty("parent");
        offsetBase = serializedObject.FindProperty("offsetBase");
        offset = serializedObject.FindProperty("offset");
        speedBase = serializedObject.FindProperty("speedBase");
        factor = serializedObject.FindProperty("factor");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(type);
        EditorGUILayout.PropertyField(childrenNames);
        EditorGUILayout.PropertyField(childrenOf);
        EditorGUILayout.PropertyField(parent);
        EditorGUILayout.PropertyField(offsetBase);
        EditorGUILayout.PropertyField(offset);
        EditorGUILayout.PropertyField(speedBase);
        EditorGUILayout.PropertyField(factor);

        serializedObject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(Markers.ExportOption))]
public class ExportOptionEditor : Editor
{
    SerializedProperty folder;
    SerializedProperty time;
    SerializedProperty onlyIfChanged;
    SerializedProperty personIndex;
    SerializedProperty headPosition;
    SerializedProperty lookDirection;
    SerializedProperty handPosition;
    SerializedProperty handRotation;
    SerializedProperty bothHands;
    SerializedProperty actionKeys;
    SerializedProperty actionMouse;
    SerializedProperty actionGamePad;
    SerializedProperty actionVRController;

    void OnEnable()
    {
        folder = serializedObject.FindProperty("folder");
        time = serializedObject.FindProperty("time");
        onlyIfChanged = serializedObject.FindProperty("onlyIfChanged");
        personIndex = serializedObject.FindProperty("personIndex");
        headPosition = serializedObject.FindProperty("headPosition");
        lookDirection = serializedObject.FindProperty("lookDirection");
        handPosition = serializedObject.FindProperty("handPosition");
        handRotation = serializedObject.FindProperty("handRotation");
        bothHands = serializedObject.FindProperty("bothHands");
        actionKeys = serializedObject.FindProperty("actionKeys");
        actionMouse = serializedObject.FindProperty("actionMouse");
        actionGamePad = serializedObject.FindProperty("actionGamePad");
        actionVRController = serializedObject.FindProperty("actionVRController");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(folder);
        EditorGUILayout.PropertyField(time);
        EditorGUILayout.PropertyField(onlyIfChanged);
        EditorGUILayout.PropertyField(personIndex);
        EditorGUILayout.PropertyField(headPosition);
        EditorGUILayout.PropertyField(lookDirection);
        EditorGUILayout.PropertyField(handPosition);
        EditorGUILayout.PropertyField(handRotation);
        EditorGUILayout.PropertyField(bothHands);
        EditorGUILayout.PropertyField(actionKeys);
        EditorGUILayout.PropertyField(actionMouse);
        EditorGUILayout.PropertyField(actionGamePad);
        EditorGUILayout.PropertyField(actionVRController);
        Markers.ExportOption option = (Markers.ExportOption)target;
        if (GUILayout.Button("Export To CSV"))
        {
            option.Export();
        }
        serializedObject.ApplyModifiedProperties();
    }
}

[CustomEditor(typeof(Markers.MarkerScore)), CanEditMultipleObjects]
public class MarkerScoreditor : Editor
{
    SerializedProperty isBasket;
    SerializedProperty score;
    SerializedProperty count;
    SerializedProperty passScore;
    SerializedProperty basket;
    SerializedProperty interval;
    SerializedProperty onlyAfter;
    SerializedProperty activate;
    SerializedProperty show;
    SerializedProperty control;
    SerializedProperty showAfter;
    SerializedProperty activateAfter;
    SerializedProperty choiceScore;

    void OnEnable()
    {
        isBasket = serializedObject.FindProperty("isBasket");
        score = serializedObject.FindProperty("score");
        count = serializedObject.FindProperty("count");
        passScore = serializedObject.FindProperty("passScore");
        basket = serializedObject.FindProperty("basket");
        interval = serializedObject.FindProperty("interval");
        onlyAfter = serializedObject.FindProperty("onlyAfter");
        activate = serializedObject.FindProperty("activate");
        show = serializedObject.FindProperty("show");
        control = serializedObject.FindProperty("control");
        activateAfter = serializedObject.FindProperty("activateAfter");
        showAfter = serializedObject.FindProperty("showAfter");
        choiceScore = serializedObject.FindProperty("choiceScore");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Markers.MarkerScore ms = (Markers.MarkerScore)target;
        EditorGUILayout.PropertyField(isBasket);
        if (ms.isBasket)
        {
            EditorGUILayout.PropertyField(passScore);
        }
        else
        {
            EditorGUILayout.PropertyField(score);
            EditorGUILayout.PropertyField(count);
            EditorGUILayout.PropertyField(interval);
            EditorGUILayout.PropertyField(basket);
            EditorGUILayout.PropertyField(onlyAfter);
            EditorGUILayout.PropertyField(control);
            EditorGUILayout.PropertyField(showAfter);
            EditorGUILayout.PropertyField(choiceScore);
        }
        EditorGUILayout.PropertyField(activateAfter);
        EditorGUILayout.PropertyField(activate);
        EditorGUILayout.PropertyField(show);
        serializedObject.ApplyModifiedProperties();
    }
}
[CustomEditor(typeof(Markers.PublishProject)), CanEditMultipleObjects]
public class PublishProjectEditor : Editor
{
    SerializedProperty email;
    SerializedProperty title;
    SerializedProperty description;
    SerializedProperty author;
    SerializedProperty ID;
    SerializedProperty serverIP;
    SerializedProperty serverPort;
    SerializedProperty password;

    void OnEnable()
    {
        email = serializedObject.FindProperty("email");
        author = serializedObject.FindProperty("author");
        title = serializedObject.FindProperty("title");
        ID = serializedObject.FindProperty("id");
        serverIP = serializedObject.FindProperty("serverIP");
        serverPort = serializedObject.FindProperty("serverPort");
        password = serializedObject.FindProperty("password");
        description = serializedObject.FindProperty("description");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(email);
        EditorGUILayout.PropertyField(author);
        EditorGUILayout.PropertyField(title);
        EditorGUILayout.PropertyField(ID);
        EditorGUILayout.PropertyField(serverIP);
        EditorGUILayout.PropertyField(serverPort);
        EditorGUILayout.PropertyField(password);
        EditorGUILayout.PropertyField(description);
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("New ID"))
        {
            (target as Markers.PublishProject).NewID();
        }
        if (GUILayout.Button("Test publish"))
        {
            Markers.PublishProject pp = target as Markers.PublishProject;
            pp.PublishType = false;
            //   DLCManager.DLCCreate(pp);
            // pp.Save();
        }
        if (GUILayout.Button("Final publish"))
        {
            Markers.PublishProject pp = target as Markers.PublishProject;
            pp.PublishType = true;
            //        DLCManager.DLCCreate(pp);
        }
    }
}
/*
[CustomEditor(typeof(Markers.MarkerControl)), CanEditMultipleObjects]
public class MarkerControlEditor : Editor
{
    SerializedProperty feature;
    SerializedProperty type;
    SerializedProperty initial;
    SerializedProperty interval;
    SerializedProperty control;
    SerializedProperty parent;
    SerializedProperty trigger;
    SerializedProperty withPeople;
    SerializedProperty withPeoploids;
    SerializedProperty trackables;

    void OnEnable()
    {
        feature = serializedObject.FindProperty("mode");
        type = serializedObject.FindProperty("effect");
        initial = serializedObject.FindProperty("initial");
        interval = serializedObject.FindProperty("interval");
        control = serializedObject.FindProperty("control");
        parent = serializedObject.FindProperty("parent");
        trigger = serializedObject.FindProperty("trigger");
        withPeople = serializedObject.FindProperty("withPeople");
        withPeoploids = serializedObject.FindProperty("withPeoploids");
        trackables = serializedObject.FindProperty("trackables");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Markers.MarkerControl mc = (Markers.MarkerControl)target;
        EditorGUILayout.PropertyField(feature);
        EditorGUILayout.PropertyField(type);
        if (mc.effect == Markers.ControlTarget.Activation || mc.effect == Markers.ControlTarget.Visibility)
            EditorGUILayout.PropertyField(initial);
        if (mc.effect == Markers.ControlTarget.Alter || mc.mode == Markers.ControlType.Time)
            EditorGUILayout.PropertyField(interval);
        if (mc.mode == Markers.ControlType.Manual)
            EditorGUILayout.PropertyField(control);
        if (mc.mode == Markers.ControlType.Element)
        {
            EditorGUILayout.PropertyField(parent);
            EditorGUILayout.PropertyField(trigger);
        }
        if (mc.mode == Markers.ControlType.Time)
            EditorGUILayout.PropertyField(trigger);

        if (mc.mode == Markers.ControlType.Element)
        {
            EditorGUILayout.PropertyField(withPeople);
            EditorGUILayout.PropertyField(withPeoploids);
            EditorGUILayout.PropertyField(trackables);
        }

        serializedObject.ApplyModifiedProperties();

    }
}*/
[CustomEditor(typeof(BakingCheck)), CanEditMultipleObjects]
public class BakingEditor : Editor
{
    SerializedProperty notGI;
    SerializedProperty giRoots;


    void OnEnable()
    {
        notGI = serializedObject.FindProperty("notGI");
        giRoots = serializedObject.FindProperty("GIRoots");

    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(giRoots);
        EditorGUILayout.PropertyField(notGI);
        serializedObject.ApplyModifiedProperties();

        if (GUILayout.Button("Bake Mode"))
        {
            (target as BakingCheck).Enforce();
        }
        if (GUILayout.Button("Play Mode"))
        {
            (target as BakingCheck).Revert();
        }

    }
}
[CustomEditor(typeof(Tames.IntroPage))]
public class EIntro : Editor
{
    SerializedProperty show;
    SerializedProperty image;
    SerializedProperty background;

    SerializedProperty foreNext;
    SerializedProperty backNext;
    SerializedProperty questions;
    SerializedProperty answers;
    SerializedProperty required;
    SerializedProperty selected;
    SerializedProperty lineCount;
    SerializedProperty items;



    void OnEnable()
    {
        show = serializedObject.FindProperty("opening");
        image = serializedObject.FindProperty("image");
        background = serializedObject.FindProperty("background");
        foreNext = serializedObject.FindProperty("foreNext");
        backNext = serializedObject.FindProperty("backNext");
        questions = serializedObject.FindProperty("questions");
        answers = serializedObject.FindProperty("answers");
        required = serializedObject.FindProperty("required");
        selected = serializedObject.FindProperty("selected");
        lineCount = serializedObject.FindProperty("lineCount");
        items = serializedObject.FindProperty("items");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        Tames.IntroPage tp = (Tames.IntroPage)target;
        for (int i = 0; i < tp.items.Length; i++)
            if (tp.items[i] != null)
            {
                tp.items[i].content.type = tp.items[i].option.type;
                int c = tp.items[i].option.type switch
                {
                    Tames.AnswerType.Checkbox => 2,
                    Tames.AnswerType.DropDown => tp.items[i].content.question.Split(';', System.StringSplitOptions.RemoveEmptyEntries).Length,
                    _ => 0
                };
                tp.items[i].links.count = c > 0 ? c + 2 : 0;
            }
        EditorGUILayout.PropertyField(show);
        EditorGUILayout.PropertyField(image);
        EditorGUILayout.PropertyField(background);
        EditorGUILayout.PropertyField(foreNext);
        EditorGUILayout.PropertyField(backNext);
        EditorGUILayout.PropertyField(questions);
        EditorGUILayout.PropertyField(answers);
        EditorGUILayout.PropertyField(required);
        EditorGUILayout.PropertyField(selected);
        EditorGUILayout.PropertyField(lineCount);
        if (GUILayout.Button("Load"))
            tp.Read();
        EditorGUILayout.PropertyField(items);

        serializedObject.ApplyModifiedProperties();

    }

}
[CustomPropertyDrawer((typeof(StateSender)))]
public class Sending : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        //    property.serializedObject.Update();
        float total = 70, w = 0;
        EditorGUI.BeginProperty(position, label, property);
        var asIsRect = new Rect(position.x + 70, position.y, w = 15, position.height);
        //   position.x += w + 5;
        //   position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;


        //     var asIsLabel = new Rect(position.x, position.y, w = 30, position.height);

        var byLabel = new Rect(position.x + (total += w + 5), position.y, w = 70, position.height);
        var byRect = new Rect(position.x + (total += w + 5), position.y, w = 40, position.height);

        string n = property.name;
        n = n.ToUpper()[0] + n.Substring(1);
        EditorGUI.LabelField(position, n);
        EditorGUI.PropertyField(asIsRect, property.FindPropertyRelative("on"), GUIContent.none);
        EditorGUI.LabelField(byLabel, "Interval (s):");
        EditorGUI.PropertyField(byRect, property.FindPropertyRelative("interval"), GUIContent.none);
        EditorGUI.EndProperty();
        property.serializedObject.ApplyModifiedProperties();
    }
}
[CustomPropertyDrawer((typeof(Tames.IntroItemOption)))]
public class IntroItemOption : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        //    property.serializedObject.Update();
        float total = 10, w = 0;
        EditorGUI.BeginProperty(position, label, property);

        //   var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;


        //     var asIsLabel = new Rect(position.x, position.y, w = 30, position.height);

        var asterisk = new Rect(position.x + total, position.y, w = 15, position.height);
        var nam = new Rect(position.x + (total += w + 5), position.y, w = 60, position.height);
        var type = new Rect(position.x + (total += w + 5), position.y, w = 50, position.height);
        var align = new Rect(position.x + (total += w + 5), position.y, w = 50, position.height);
        var other = new Rect(position.x + (total += w + 5), position.y, w = 50, position.height);
        var lafter = new Rect(position.x + (total += w + 5), position.y, w = 10, position.height);
        var after = new Rect(position.x + (total += w + 5), position.y, position.width - total, position.height);


        EditorGUI.LabelField(position, "*");
        EditorGUI.PropertyField(asterisk, property.FindPropertyRelative("required"), GUIContent.none);
        EditorGUI.PropertyField(nam, property.FindPropertyRelative("name"), GUIContent.none);
        // EditorGUI.LabelField(byLabel, "Interval (s):");
        SerializedProperty sp = property.FindPropertyRelative("type");
        EditorGUI.PropertyField(type, sp, GUIContent.none);
        EditorGUI.PropertyField(align, property.FindPropertyRelative("alignment"), GUIContent.none);
        if (sp.enumValueIndex == (int)Tames.AnswerType.Image || sp.enumValueIndex == (int)Tames.AnswerType.Line)
            EditorGUI.PropertyField(other, property.FindPropertyRelative("lineCount"), GUIContent.none);
        else
            EditorGUI.PropertyField(other, property.FindPropertyRelative("style"), GUIContent.none);
        //?¬???
        EditorGUI.LabelField(lafter, "_");
        EditorGUI.PropertyField(after, property.FindPropertyRelative("lineAfter"), GUIContent.none);
        EditorGUI.EndProperty();
        property.serializedObject.ApplyModifiedProperties();
    }
}
[CustomPropertyDrawer((typeof(Tames.IntroContent)))]
public class IntroContent : PropertyDrawer
{
    int rows = 0;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var p = property.FindPropertyRelative("type");
        rows = p.enumValueIndex switch
        {
            (int)Tames.AnswerType.Image => 1,
            (int)Tames.AnswerType.Text => 4,
            (int)Tames.AnswerType.DropDown => 4,
            (int)Tames.AnswerType.Typable => 4,
            (int)Tames.AnswerType.Checkbox => 4,
            _ => 0
        };
        return base.GetPropertyHeight(property, label) * rows;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        var p = property.FindPropertyRelative("type");
        string ps = p.enumValueIndex switch
        {
            (int)Tames.AnswerType.Image => "texture",
            (int)Tames.AnswerType.Text => "question",
            (int)Tames.AnswerType.DropDown => "question",
            (int)Tames.AnswerType.Typable => "question",
            (int)Tames.AnswerType.Checkbox => "question",
            _ => ""
        };
        if (ps.Length > 0)
            EditorGUI.PropertyField(position, property.FindPropertyRelative(ps), GUIContent.none);
        EditorGUI.EndProperty();
        property.serializedObject.ApplyModifiedProperties();
    }
}
[CustomPropertyDrawer((typeof(Tames.IntroLink)))]
public class IntroLink : PropertyDrawer
{
    int rows = 0;
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var s = property.FindPropertyRelative("count");
        //    rows = s.intValue;
        return base.GetPropertyHeight(property, label) * rows;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        var s = property.FindPropertyRelative("count");
        //if (s.intValue > 0)            EditorGUI.PropertyField(position, property.FindPropertyRelative("pages"), GUIContent.none);
        EditorGUI.EndProperty();
        property.serializedObject.ApplyModifiedProperties();
    }
}
[CustomEditor(typeof(ProjectData))]
public class ProjectDataEditor : Editor
{
    SerializedProperty ip;
    SerializedProperty port;
    SerializedProperty token;
    SerializedProperty minutes;
    SerializedProperty title;


    SerializedProperty includeIP;
    SerializedProperty progress;
    SerializedProperty choice;
    SerializedProperty surveys;
    SerializedProperty location;
    SerializedProperty elements;
    SerializedProperty alternatives;
    void OnEnable()
    {
        title = serializedObject.FindProperty("title");
        ip = serializedObject.FindProperty("IP");
        port = serializedObject.FindProperty("port");
        token = serializedObject.FindProperty("token");
        minutes = serializedObject.FindProperty("minutes");
        includeIP = serializedObject.FindProperty("includeIP");
        progress = serializedObject.FindProperty("progress");
        choice = serializedObject.FindProperty("alters");
        location = serializedObject.FindProperty("location");
        surveys = serializedObject.FindProperty("surveys");
        elements = serializedObject.FindProperty("elements");
        alternatives = serializedObject.FindProperty("alternatives");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        ProjectData tp = (ProjectData)target;

        EditorGUILayout.PropertyField(title);
        EditorGUILayout.PropertyField(ip);
        EditorGUILayout.PropertyField(port);
        EditorGUILayout.PropertyField(token);
        EditorGUILayout.PropertyField(minutes);
        if (GUILayout.Button("Register Project"))
            tp.RegisterProject(true);
        if (GUILayout.Button("Unregister"))
            tp.RegisterProject(false);
        if (GUILayout.Button("Request Results"))
            tp.RequestResults();
        EditorGUILayout.PropertyField(includeIP);
        EditorGUILayout.PropertyField(progress);
        EditorGUILayout.PropertyField(choice);
        EditorGUILayout.PropertyField(surveys);
        EditorGUILayout.PropertyField(location);
        EditorGUILayout.PropertyField(elements);
        EditorGUILayout.PropertyField(alternatives);

        serializedObject.ApplyModifiedProperties();

    }
}
[CustomPropertyDrawer((typeof(Markers.InfoPosManager)))]
public class InfoPosMgr : PropertyDrawer
{
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        //    property.serializedObject.Update();
        EditorGUI.BeginProperty(rect, label, property);
        EditorGUI.LabelField(rect, "Position");
        EditorGUI.indentLevel = 8;
        Rect position = EditorGUI.IndentedRect(rect);
        EditorGUI.indentLevel = 0;
        float total = 0, w = 0;
        var pos = new Rect(position.x + total, position.y, w = 80, position.height);
        var xl = new Rect(position.x + (total += w + 5), position.y, w = 15, position.height);
        var x = new Rect(position.x + (total += w + 5), position.y, w = 40, position.height);
        var yl = new Rect(position.x + (total += w + 5), position.y, w = 15, position.height);
        var y = new Rect(position.x + (total += w + 5), position.y, w = 40, position.height);
        //   position.x += w + 5;
        //   position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        // Don't make child fields be indented


        //     var asIsLabel = new Rect(position.x, position.y, w = 30, position.height);
        SerializedProperty p;

        EditorGUI.PropertyField(pos, p = property.FindPropertyRelative("position"), GUIContent.none);
        int i = p.enumValueIndex;
        if (i < 2)
        {
            EditorGUI.LabelField(xl, "X");
            EditorGUI.PropertyField(x, property.FindPropertyRelative("X"), GUIContent.none);
            EditorGUI.LabelField(yl, "Y");
            EditorGUI.PropertyField(y, property.FindPropertyRelative("Y"), GUIContent.none);
        }
        EditorGUI.EndProperty();
        property.serializedObject.ApplyModifiedProperties();
    }
}
[CustomPropertyDrawer((typeof(Markers.BackMargin)))]
public class BackMarg : PropertyDrawer
{
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        //    property.serializedObject.Update();
        EditorGUI.BeginProperty(rect, label, property);
        EditorGUI.LabelField(rect, "Margin");
        EditorGUI.indentLevel = 8;
        Rect position = EditorGUI.IndentedRect(rect);
        EditorGUI.indentLevel = 0;
        float total = 0, w = 0;
        var pos = new Rect(position.x + total, position.y, w = 50, position.height);
        var xl = new Rect(position.x + (total += w + 5), position.y, w = 20, position.height);
        var x = new Rect(position.x + (total += w + 5), position.y, w = 60, position.height);
        //   position.x += w + 5;
        //   position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        // Don't make child fields be indented


        //     var asIsLabel = new Rect(position.x, position.y, w = 30, position.height);
        SerializedProperty p;

        EditorGUI.PropertyField(pos, p = property.FindPropertyRelative("margin"), GUIContent.none);
        EditorGUI.LabelField(xl, "of");
        EditorGUI.PropertyField(x, property.FindPropertyRelative("type"), GUIContent.none);
        EditorGUI.EndProperty();
        property.serializedObject.ApplyModifiedProperties();
    }
}