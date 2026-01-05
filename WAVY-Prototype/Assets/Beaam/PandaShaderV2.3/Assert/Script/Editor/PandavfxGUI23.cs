using System;
using UnityEngine;
using UnityEditor;


//����һ��GUI��
public class PandavfxGUI23 : ShaderGUI
{
   
    //�Զ���һ��С��ť

    public GUILayoutOption[] shortButtonStyle = new GUILayoutOption[] { GUILayout.Width(100) };

    //�Զ�������

    public GUIStyle style = new GUIStyle()

        ;



    //�Զ��������˵�����״����
    static bool Foldout(bool display, string title)
    {




        var style = new GUIStyle("ShurikenModuleTitle");
        style.font = new GUIStyle(EditorStyles.boldLabel).font;
        style.border = new RectOffset(15, 7, 4, 4);
        style.fixedHeight = 22;
        style.contentOffset = new Vector2(20f, -2f);
        style.fontSize = 11;
        style.normal.textColor = new Color(0.7f, 0.8f, 0.9f);


        var rect = GUILayoutUtility.GetRect(16f, 25f, style);
        GUI.Box(rect, title, style);

        var e = Event.current;

        var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
        if (e.type == EventType.Repaint)
        {
            EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
        }

        if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
        {
            display = !display;
            e.Use();
        }

        return display;
    }



    //�Զ��������˵�1����״����
    static bool Foldouts(bool display, string title)
    {



        var style = new GUIStyle("ShurikenModuleTitle");
        style.font = new GUIStyle(EditorStyles.boldLabel).font;
        style.border = new RectOffset(15, 7, 4, 4);
        style.fixedHeight = 18;
        style.contentOffset = new Vector2(30f, -2f);
        style.fontSize = 10;
        style.normal.textColor = new Color(0.75f, 0.75f, 0.75f);


        var rect = GUILayoutUtility.GetRect(16f, 15f, style);
        GUI.Box(rect, title, style);

        var e = Event.current;

        var toggleRect = new Rect(rect.x + 15f, rect.y + 2f, 13f, 13f);
        if (e.type == EventType.Repaint)
        {
            EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
        }

        if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
        {
            display = !display;
            e.Use();
        }

        return display;
    }



    //�Զ��������˵�2����״����
    static bool Foldout2(bool display, string title)
    {


        var style = new GUIStyle("ShurikenModuleTitle");
        style.font = new GUIStyle(EditorStyles.boldLabel).font;
        style.border = new RectOffset(15, 7, 4, 4);
        style.fixedHeight = 22;
        style.contentOffset = new Vector2(20f, -2f);
        style.fontSize = 11;
        style.normal.textColor = new Color(0.65f, 0.55f, 0.55f);


        var rect = GUILayoutUtility.GetRect(16f, 25f, style);
        GUI.Box(rect, title, style);

        var e = Event.current;

        var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
        if (e.type == EventType.Repaint)
        {
            EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
        }

        if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
        {
            display = !display;
            e.Use();
        }

        return display;
    }





    //�Զ������



    static bool _Base_Foldout = true;

    static bool _Maintextures_Foldout = true;

    static bool _Mask_Foldout = false;

    static bool _Dissolve_Foldout = false;

    static bool _Distort_Foldout = false;

    static bool _FNL_Foldout = false;

    static bool _VTO_Foldout = false;

    static bool _Main_Foldout = true;

    static bool _Tip_Foldout = false;

    static bool _Mainxx = false;

    static bool _Maskxx = false;

    static bool _MaskPlusxx = false;

    static bool _Dissolvexx = false;

    static bool _Dissolveplusxx = false;

    static bool _VTOxx = false;

    static bool _VTOMaskxx = false;



    static bool _AddTexxx = false;

    static bool _AddTex = false;

     public static bool _tips = false;

    MaterialEditor m_MaterialEditor;

    static bool _NormalTexx = false;

    static bool _NormalTexxx = false;

    static bool _DistortTexxx = false;

    static bool _DistortMaskTexxx = false;

    static bool _shichax = false;

    static bool _mengbanxx = false;


    //�Զ�����Ҫ��ͼ��Ҫ��ʾ������

    MaterialProperty mainTex_Sampler = null;

    MaterialProperty TinColor = null;
 
    MaterialProperty MainUspeed = null;

    MaterialProperty MainVspeed = null;

    MaterialProperty MainTexrotat = null;

    MaterialProperty MainTexC = null;

    MaterialProperty MainTexCV = null;

    MaterialProperty MainTexAR = null;

    MaterialProperty BackFaceColor = null;

    MaterialProperty Face = null;

    MaterialProperty MainOffsetUC1 = null;

    MaterialProperty MainOffsetUC2 = null;

    MaterialProperty MainOffsetVC1 = null;

    MaterialProperty MainOffsetVC2 = null;

    MaterialProperty MainTexUVS = null;

    MaterialProperty TexCenter = null;

    MaterialProperty CenterU = null;

    MaterialProperty CenterV = null;

    MaterialProperty ScreenAsMain = null;

    MaterialProperty MainTexRefine = null;

    MaterialProperty MainTexAC = null;

    //������ͼ


    MaterialProperty AddTexUspeed = null;

    MaterialProperty AddTexVspeed = null;

    MaterialProperty AddTexRo = null;

    MaterialProperty AddTexC = null;

    MaterialProperty AddTexCV = null;

    MaterialProperty AddTexColor = null;

    MaterialProperty AddTex_Sampler = null;

 //   MaterialProperty AddTexChanel = null;

    MaterialProperty AddTexBlendMode = null;

     MaterialProperty AddTexBlend = null;

    MaterialProperty AddTexRefine = null;

    MaterialProperty AddTexAR = null;

    MaterialProperty IfAddTexAlpha = null;

    MaterialProperty IfAddTexColor = null;

    MaterialProperty AddTexUVS = null;
    MaterialProperty CAddTexUVT = null;
    MaterialProperty AddTexAC = null;
    //Mask����

    MaterialProperty Mask_Sampler = null;

    MaterialProperty MaskScale = null;

    MaterialProperty Maskrotat = null;

    MaterialProperty MaskUspeed = null;

    MaterialProperty MaskVspeed = null;

    MaterialProperty MaskAR = null;

    MaterialProperty MaskC = null;

    MaterialProperty MaskCV = null;

    MaterialProperty IfMaskColor = null;

    MaterialProperty MaskTexUVS = null;

    MaterialProperty IfMaskPlusTex = null;

    MaterialProperty MaskPlusTex = null;

    MaterialProperty MaskPlusAR = null;

   MaterialProperty MaskPlusR = null;

    MaterialProperty MaskPlusC = null;

    MaterialProperty MaskPlusCV = null;

    MaterialProperty MaskPlusUspeed = null;

    MaterialProperty MaskPlusVspeed = null;

    MaterialProperty MaskOffsetUC1 = null;

    MaterialProperty MaskOffsetUC2 = null;

    MaterialProperty MaskOffsetVC1 = null;

    MaterialProperty MaskOffsetVC2 = null;
    //�ܽ�����

    MaterialProperty Dissolve_Tex = null;

    MaterialProperty Dissolve_Color = null;

    MaterialProperty Dissolve_Factor = null;

    MaterialProperty Dissolve_Soft = null;

    MaterialProperty Dissolve_Wide = null;

    MaterialProperty Dissolve_Uspeed = null;

    MaterialProperty Dissolve_Vspeed = null;

    MaterialProperty Dissolve_Rotation = null;

    MaterialProperty DissolveAR = null;

    MaterialProperty DissolveC = null;

    MaterialProperty DissolveCV = null;

    MaterialProperty DissloveTexPlus = null;

    MaterialProperty DissolvePlusAR = null;

    MaterialProperty DissolvePlusC = null;

    MaterialProperty DissolvePlusCV = null;

    MaterialProperty IfDissolvePlus = null;

    MaterialProperty DissolveTexDivide = null;

    MaterialProperty DissolvePlusR = null;

    MaterialProperty DissolveTexUVS2 = null;

    MaterialProperty DissolveTexExp = null;

    MaterialProperty IfDissolveColor = null;

    MaterialProperty DissolveFactorC1 = null;

    MaterialProperty DissolveFactorC2 = null;

    MaterialProperty DissolveOffsetUC1 = null;

    MaterialProperty DissolveOffsetUC2 = null;

    MaterialProperty DissolveOffsetVC1 = null;

    MaterialProperty DissolveOffsetVC2 = null;

    MaterialProperty IfDissolveOffsetC = null;

    //UVŤ������

    MaterialProperty Distort_Tex = null;

    MaterialProperty Distort_Factor = null;

    MaterialProperty Distort_Uspeed = null;

    MaterialProperty Distort_Vspeed = null;

    MaterialProperty IfFlowmap = null;

    MaterialProperty DistortTexAR = null;

    MaterialProperty IfNormalDistort = null;

    MaterialProperty DistortMaskTexAR = null;

    MaterialProperty DistortMaskTex = null;

    MaterialProperty DistortMaskTexC = null;

    MaterialProperty DistortMaskTexCV = null;

    MaterialProperty DistortMaskTexR = null;

    MaterialProperty DistortRemap = null;

    MaterialProperty DistortFactorC1 = null;

    MaterialProperty DistortFactorC2 = null;

    //�����

    MaterialProperty Fnl_Scale = null;

    MaterialProperty Fnl_Power = null;

    MaterialProperty Fnl_Color = null;

    MaterialProperty Fnl_Soft = null;

    MaterialProperty Dir = null;

    MaterialProperty IfFNLAlpha = null;

    //VTO

    MaterialProperty Vto_Scale = null;

    MaterialProperty Vto_Tex = null;

    MaterialProperty Vto_Mask = null;

    MaterialProperty Vto_Uspeed = null;

    MaterialProperty Vto_Vspeed = null;

    MaterialProperty VTOAR = null;

    MaterialProperty VTOC = null;

    MaterialProperty VTOCV = null;

    MaterialProperty VTOR = null;

    MaterialProperty VTOMaskAR = null;

    MaterialProperty VTOMaskC = null;

    MaterialProperty VTOMaskCV = null;

    MaterialProperty VTOMaskR = null;

    MaterialProperty VTOTexExp = null;

    MaterialProperty VTORemap = null;

    MaterialProperty VTOFactorC1 = null;

    MaterialProperty VTOFactorC2 = null;

    MaterialProperty IfVAT = null;

    MaterialProperty VATPositionTex = null;

    MaterialProperty VATNormalTex = null;

    MaterialProperty VATFrameFactor = null;

    MaterialProperty VATTime = null;

    MaterialProperty CustomVAT = null;

    MaterialProperty VATFrameC1 = null;

    MaterialProperty VATFrameC2 = null;

    MaterialProperty ParticleVAT = null;

    //  MaterialProperty VATRotate = null;

    //   MaterialProperty VATPivot = null;

    //�Զ������
    MaterialProperty IfCustomLight = null;

    MaterialProperty LightScale = null;

    MaterialProperty NormalTex = null;

    MaterialProperty NormalTexC = null;

    MaterialProperty NormalTexCV = null;

    MaterialProperty NormalTex_Rotat = null;

    MaterialProperty NormalScale = null;

    MaterialProperty NormalTex_Uspeed = null;

    MaterialProperty NormalTex_Vspeed = null;

    MaterialProperty DistortNormalTex = null;

    MaterialProperty IfStaticNormal = null;

    MaterialProperty StaticNormalScale = null;

    MaterialProperty StaticNormalOffset = null;

    MaterialProperty IfCubemap = null;

    MaterialProperty CubemapScale = null;

    MaterialProperty CubeMap = null;



    //�Զ�������ѡ����Ҫ��ʾ������

    MaterialProperty MainAlpha = null;

 //   MaterialProperty MainAlphaPower = null;

    MaterialProperty DepthFade = null;

    MaterialProperty Qubaohedu = null;

    MaterialProperty DepthColor = null;

    MaterialProperty DepthF = null;

    MaterialProperty ParaTex = null;

    MaterialProperty IfPara = null;

    MaterialProperty Parallax = null;

    MaterialProperty Reference = null;

    MaterialProperty Comparison = null;

    MaterialProperty Pass = null;

    MaterialProperty Fail = null;



    //���Զ������Ҫ��ʾ������ָ��shader�����Ӧ����
    public void FindProperties(MaterialProperty[] props)
    {


      

        //����ͼ����ָ��

        mainTex_Sampler = FindProperty("_MainTex", props);

        TinColor = FindProperty("_MainColor", props);
     
        MainUspeed = FindProperty("_MainTex_Uspeed", props);

        MainVspeed = FindProperty("_MainTex_Vspeed", props);

        MainTexrotat = ShaderGUI.FindProperty("_MainTex_rotat", props);

        MainTexC= ShaderGUI.FindProperty("_MaintexC", props);

        MainTexCV = ShaderGUI.FindProperty("_MaintexCV", props);

        MainTexAR = ShaderGUI.FindProperty("_MainTex_ar", props);

       BackFaceColor = ShaderGUI.FindProperty("_BackFaceColor", props);

        Face = ShaderGUI.FindProperty("_Face", props);


        MainTexUVS = ShaderGUI.FindProperty("_MainTexUVS", props);
        TexCenter = ShaderGUI.FindProperty("_TexCenter", props);
        CenterU = ShaderGUI.FindProperty("_CenterU", props);
        CenterV = ShaderGUI.FindProperty("_CenterV", props);
        ScreenAsMain = ShaderGUI.FindProperty("_ScreenAsMain", props);

        MainTexRefine= ShaderGUI.FindProperty("_MainTexRefine", props);

        MainOffsetUC1 = ShaderGUI.FindProperty("_MainOffsetUC1", props);

        MainOffsetUC2 = ShaderGUI.FindProperty("_MainOffsetUC2", props);

        MainOffsetVC1 = ShaderGUI.FindProperty("_MainOffsetVC1", props);

        MainOffsetVC2 = ShaderGUI.FindProperty("_MainOffsetVC2", props);

        MainTexAC= ShaderGUI.FindProperty("_MainTexAC", props);

        //������ͼ
        AddTex_Sampler = FindProperty("_AddTex", props);
        AddTexBlendMode = FindProperty("_AddTexBlendMode", props);
        AddTexBlend = FindProperty("_AddTexBlend", props);
        AddTexC = FindProperty("_AddTexC", props);
        AddTexCV = FindProperty("_AddTexCV", props);
     //   AddTexChanel= FindProperty("_AddTexChanel", props);
        AddTexColor = FindProperty("_AddTexColor", props);
        AddTexRo = FindProperty("_AddTexRo", props);
        AddTexUspeed = FindProperty("_AddTexUspeed", props);
        AddTexVspeed = FindProperty("_AddTexVspeed", props);
        AddTexRefine = ShaderGUI.FindProperty("_AddTexRefine", props);
        AddTexAR = ShaderGUI.FindProperty("_AddTexAR", props);
        IfAddTexAlpha = ShaderGUI.FindProperty("_IfAddTexAlpha", props);

        IfAddTexColor = ShaderGUI.FindProperty("_IfAddTexColor", props);
        AddTexUVS = ShaderGUI.FindProperty("_AddTexUVS", props);
        CAddTexUVT = ShaderGUI.FindProperty("_CAddTexUVT", props);
        
        AddTexAC= ShaderGUI.FindProperty("_AddTexAC", props);


        //Mask����ָ��
        Mask_Sampler = FindProperty("_MaskTex", props);

        MaskScale = FindProperty("_Mask_scale", props);

        Maskrotat = FindProperty("_Mask_rotat", props);

        MaskUspeed = FindProperty("_Mask_Uspeed", props);

        MaskVspeed = FindProperty("_Mask_Vspeed", props);

        MaskAR= FindProperty("_MaskAlphaRA", props);

        MaskC= FindProperty("_MaskC", props);

        MaskCV = FindProperty("_MaskCV", props);

        IfMaskColor = FindProperty("_IfMaskColor", props);


        MaskTexUVS = ShaderGUI.FindProperty("_MaskTexUVS", props);

        IfMaskPlusTex = FindProperty("_IfMaskPlusTex", props);

        MaskPlusTex = FindProperty("_MaskPlusTex", props);

        MaskPlusAR = FindProperty("_MaskPlusAR", props);

        MaskPlusR = FindProperty("_MaskPlusR", props);

        MaskPlusC = FindProperty("_MaskPlusC", props);

        MaskPlusCV = FindProperty("_MaskPlusCV", props);

        MaskPlusUspeed = FindProperty("_MaskPlusUspeed", props);

        MaskPlusVspeed = FindProperty("_MaskPlusVspeed", props);

        MaskOffsetUC1 = ShaderGUI.FindProperty("_MaskOffsetUC1", props);

        MaskOffsetUC2 = ShaderGUI.FindProperty("_MaskOffsetUC2", props);

        MaskOffsetVC1 = ShaderGUI.FindProperty("_MaskOffsetVC1", props);

        MaskOffsetVC2 = ShaderGUI.FindProperty("_MaskOffsetVC2", props);


        //�ܽ�����ָ��

        Dissolve_Tex = FindProperty("_DissloveTex", props);

        Dissolve_Color= FindProperty("_DIssloveColor", props);

        Dissolve_Factor= FindProperty("_DIssloveFactor", props);

        Dissolve_Soft= FindProperty("_DIssloveSoft", props);

        Dissolve_Uspeed= FindProperty("_DisTex_Uspeed", props);

        Dissolve_Vspeed = FindProperty("_DisTex_Vspeed", props);

        Dissolve_Rotation= FindProperty("_DIssolve_rotat", props);

        Dissolve_Wide = FindProperty("_DIssloveWide", props);

        DissolveAR = FindProperty("_DissolveAR", props);

        DissolveC = FindProperty("_DissolveC", props);

        DissolveCV = FindProperty("_DissolveCV", props);

        DissloveTexPlus= FindProperty("_DissloveTexPlus", props);

        IfDissolvePlus = FindProperty("_IfDissolvePlus", props);

        DissolvePlusAR= FindProperty("_DissolvePlusAR", props);

        DissolvePlusC = FindProperty("_DissolvePlusC", props);

        DissolvePlusCV = FindProperty("_DissolvePlusCV", props);

        DissolveTexDivide = FindProperty("_DissolveTexDivide", props);

        DissolvePlusR = FindProperty("_DissolvePlusR", props);



        DissolveTexUVS2 = ShaderGUI.FindProperty("_DissolveTexUVS2", props);

        DissolveTexExp= ShaderGUI.FindProperty("_DissolveTexExp", props);

        IfDissolveColor = ShaderGUI.FindProperty("_IfDissolveColor", props);

        DissolveFactorC1 = ShaderGUI.FindProperty("_DissolveFactorC1", props);

        DissolveFactorC2 = ShaderGUI.FindProperty("_DissolveFactorC2", props);

        DissolveOffsetUC1 = ShaderGUI.FindProperty("_DissolveOffsetUC1", props);

        DissolveOffsetUC2 = ShaderGUI.FindProperty("_DissolveOffsetUC2", props);

        DissolveOffsetVC1 = ShaderGUI.FindProperty("_DissolveOffsetVC1", props);

        DissolveOffsetVC2 = ShaderGUI.FindProperty("_DissolveOffsetVC2", props);

        IfDissolveOffsetC = ShaderGUI.FindProperty("_IfDissolveOffsetC", props);
        //UVŤ��

        Distort_Tex = FindProperty("_DistortTex", props);

        Distort_Factor = FindProperty("_DistortFactor", props);

        Distort_Uspeed= FindProperty("_DistortTex_Uspeed", props);

        Distort_Vspeed = FindProperty("_DistortTex_Vspeed", props);

        IfFlowmap = FindProperty("_IfFlowmap", props);

        IfNormalDistort = FindProperty("_IfNormalDistort", props);

        DistortTexAR = FindProperty("_DistortTexAR", props);

        DistortMaskTexAR = FindProperty("_DistortMaskTexAR", props);

        DistortMaskTex = FindProperty("_DistortMaskTex", props);

        DistortMaskTexC = FindProperty("_DistortMaskTexC", props);

        DistortMaskTexCV = FindProperty("_DistortMaskTexCV", props);

        DistortMaskTexR = FindProperty("_DistortMaskTexR", props);

        DistortRemap = FindProperty("_DistortRemap", props);

        DistortFactorC1 = FindProperty("_DistortFactorC1", props);

        DistortFactorC2 = FindProperty("_DistortFactorC2", props);
        //�����

        Fnl_Color = FindProperty("_fnl_color", props);

        Fnl_Scale = FindProperty("_fnl_sacle", props);

        Fnl_Power = FindProperty("_fnl_power", props);

        Fnl_Soft= FindProperty("_softFacotr", props);

        Dir = FindProperty("_Dir", props);

        IfFNLAlpha = FindProperty("_IfFNLAlpha", props);

        //VTO
        Vto_Tex = FindProperty("_VTOTex", props);

        Vto_Mask= FindProperty("_VTOMaskTex", props);

        Vto_Scale= FindProperty("_VTOFactor", props);

        Vto_Uspeed= FindProperty("_VTOTex_Uspeed", props);

        Vto_Vspeed = FindProperty("_VTOTex_Vspeed", props);

        VTOAR= FindProperty("_VTOAR", props);

        VTOC = FindProperty("_VTOC", props);

        VTOCV = FindProperty("_VTOCV", props);

        VTOR = FindProperty("_VTOR", props);

        VTOMaskAR = FindProperty("_VTOMaskAR", props);

        VTOMaskC = FindProperty("_VTOMaskC", props);

        VTOMaskCV = FindProperty("_VTOMaskCV", props);

        VTOMaskR = FindProperty("_VTOMaskR", props);


        VTORemap = FindProperty("_VTORemap", props);


        VTOTexExp = ShaderGUI.FindProperty("_VTOTexExp", props);

        VTOFactorC1 = FindProperty("_VTOFactorC1", props);

        VTOFactorC2 = FindProperty("_VTOFactorC2", props);

        IfVAT = FindProperty("_IfVAT", props);

        VATPositionTex = FindProperty("_VATPositionTex", props);

        VATNormalTex = FindProperty("_VATNormalTex", props);

        VATFrameFactor = FindProperty("_VATFrameFactor", props);

        VATTime = FindProperty("_VATTime", props);

        CustomVAT = FindProperty("_CustomVAT", props);

        VATFrameC1 = FindProperty("_VATFrameC1", props);

        VATFrameC2 = FindProperty("_VATFrameC2", props);

        ParticleVAT = FindProperty("_ParticleVAT", props);

        //   VATRotate = FindProperty("_VATRotate", props);

        //   VATPivot = FindProperty("_VATPivot", props);

        //�Զ������ָ��

        IfCustomLight = FindProperty("_IfCustomLight", props);

        LightScale = FindProperty("_LightScale", props);

        NormalTex = FindProperty("_NormalTex", props);

        NormalScale= FindProperty("_NormalScale", props);

        NormalTexC = FindProperty("_NormalTexC", props);

        NormalTexCV = FindProperty("_NormalTexCV", props);

        NormalTex_Rotat = FindProperty("_NormalTex_Rotat", props);

        NormalTex_Uspeed= FindProperty("_NormalTex_Uspeed", props);

        NormalTex_Vspeed = FindProperty("_NormalTex_Vspeed", props);

        DistortNormalTex= FindProperty("_DistortNormalTex", props);

        IfStaticNormal = FindProperty("_IfStaticNormal", props);

        IfCubemap = FindProperty("_IfCubemap", props);

        CubemapScale= FindProperty("_CubemapScale", props);

        CubeMap= FindProperty("_CubeMap", props);

        StaticNormalScale = FindProperty("_StaticNormalScale", props);

        StaticNormalOffset = FindProperty("_StaticNormalOffset", props);

        DistortNormalTex = FindProperty("_DistortNormalTex", props);
        //�ۺ�ѡ������ָ��

        MainAlpha = FindProperty("_MainAlpha", props);

        DepthFade= FindProperty("_DepthfadeFactor", props);

        Qubaohedu= FindProperty("_qubaohedu", props);

       DepthColor = FindProperty("_DepthColor", props);

        DepthF = FindProperty("_DepthF", props);

        IfPara = FindProperty("_IfPara", props);

        ParaTex= FindProperty("_ParaTex", props);

        Parallax = FindProperty("_Parallax", props);

        Reference= FindProperty("_Reference", props);
        Comparison = FindProperty("_Comparison", props);
        Pass = FindProperty("_Pass", props);
        Fail = FindProperty("_Fail", props);
    }

    //�����涨���������ʾ�������
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {

      


        FindProperties(props); 

        m_MaterialEditor = materialEditor;

        Material material = materialEditor.target as Material;

        //�������������˵�
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _Base_Foldout = Foldout(_Base_Foldout, "��������(BasicSettings)");

        if (_Base_Foldout)
        {
            EditorGUI.indentLevel++;

     

            GUI_Base(material);

          

            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();

      
            //����ͼ�����˵�

            if (mainTex_Sampler.textureValue != null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                _Maintextures_Foldout = Foldout(_Maintextures_Foldout, "����ͼ(MainTexture)");

                if (_Maintextures_Foldout)
                {
                    EditorGUI.indentLevel++;

                    //     EditorGUILayout.Space();


                    GUI_Maintextures(material);

                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                _Maintextures_Foldout = Foldout2(_Maintextures_Foldout, "����ͼ(MainTexture)");

                if (_Maintextures_Foldout)
                {
                    EditorGUI.indentLevel++;

                    //     EditorGUILayout.Space();

                    GUI_Maintextures(material);

                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
            }

            //AddTex�����˵�

            if (mainTex_Sampler.textureValue != null || material.GetFloat("_ScreenAsMain") == 1)
            { 

                if (AddTex_Sampler.textureValue != null)
            {

                if (material.GetFloat("_IfAddTex") == 1)
                {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        _AddTex = Foldout(_AddTex, "�๦��ͼ(AddTexture)");

                    if (_AddTex)
                    {
                        EditorGUI.indentLevel++;

                        //   EditorGUILayout.Space();

                        GUI_Add(material);

                        EditorGUI.indentLevel--;
                    }
                        EditorGUILayout.EndVertical();
                    }

            }
            else
            {
               
                    if (material.GetFloat("_IfAddTex") == 1)
                {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        _AddTex = Foldout2(_AddTex, "�๦��ͼ(AddTexture)");

                    if (_AddTex)
                    {
                        EditorGUI.indentLevel++;

                        //   EditorGUILayout.Space();

                        GUI_Add(material);

                        EditorGUI.indentLevel--;
                    }
                        EditorGUILayout.EndVertical();
                    }


            }
            }




        //mask�����˵�


        if (Mask_Sampler.textureValue != null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                _Mask_Foldout = Foldout(_Mask_Foldout, "������ͼ(MaskTexture)");

                if (_Mask_Foldout)
                {
                    EditorGUI.indentLevel++;

                    //   EditorGUILayout.Space();

                    GUI_Mask(material);

                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
            }
            else {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                _Mask_Foldout = Foldout2(_Mask_Foldout, "������ͼ(MaskTexture)");

                if (_Mask_Foldout)
                {
                    EditorGUI.indentLevel++;

                    //   EditorGUILayout.Space();

                    GUI_Mask(material);

                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
            }

            //�ܽ������˵�

            if (Dissolve_Tex.textureValue != null) {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                _Dissolve_Foldout = Foldout(_Dissolve_Foldout, "�ܽ�(Dissolve)");

                if (_Dissolve_Foldout)
                {
                    EditorGUI.indentLevel++;

                    //   EditorGUILayout.Space();

                    GUI_Dissolve(material);

                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
            }
            else {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                _Dissolve_Foldout = Foldout2(_Dissolve_Foldout, "�ܽ�(Dissolve)");

                if (_Dissolve_Foldout)
                {
                    EditorGUI.indentLevel++;

                    //   EditorGUILayout.Space();

                    GUI_Dissolve(material);

                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
            }

            //UVŤ�������˵�


            if (Distort_Tex.textureValue != null) {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                _Distort_Foldout = Foldout(_Distort_Foldout, "UVŤ��(UVDistort)");

                if (_Distort_Foldout)
                {
                    EditorGUI.indentLevel++;

                    //   EditorGUILayout.Space();

                    GUI_Distort(material);

                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
            }
            else {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                _Distort_Foldout = Foldout2(_Distort_Foldout, "UVŤ��(UVDistort)");

                if (_Distort_Foldout)
                {
                    EditorGUI.indentLevel++;

                    //   EditorGUILayout.Space();

                    GUI_Distort(material);

                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
            }
        



       
        //FNL�����˵�

        if (material.GetFloat("_fnl_sacle") != 0 )
            {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            _FNL_Foldout = Foldout(_FNL_Foldout, "�����Ч��(Fresnel)");

            if (_FNL_Foldout)
            {
                EditorGUI.indentLevel++;

                //    EditorGUILayout.Space();

                GUI_FNL(material);

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        else if (material.GetFloat("_FNLfanxiangkaiguan") != 0)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            _FNL_Foldout = Foldout(_FNL_Foldout, "�����Ч��(Fresnel)");

            if (_FNL_Foldout)
            {
                EditorGUI.indentLevel++;

                GUI_FNL(material);

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }


        else 
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            _FNL_Foldout = Foldout2(_FNL_Foldout, "�����Ч��(Fresnel)");

            if (_FNL_Foldout)
            {
                EditorGUI.indentLevel++;

                //    EditorGUILayout.Space();

                GUI_FNL(material);

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }














        //VTO�����˵�

        if (Vto_Tex.textureValue != null || material.GetFloat("_IfVAT") == 1)
        {

         

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                _VTO_Foldout = Foldout(_VTO_Foldout, "���㶯��(VertextOffset)");

                if (_VTO_Foldout)
                {
                    EditorGUI.indentLevel++;

                //    EditorGUILayout.Space();

                    GUI_VTO(material);

                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
            
        }
        else {
   

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                _VTO_Foldout = Foldout2(_VTO_Foldout, "���㶯��(VertextOffset)");

                if (_VTO_Foldout)
                {
                    EditorGUI.indentLevel++;

              //      EditorGUILayout.Space();

                    GUI_VTO(material);

                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
            


        }


        //Normal����
        if (material.GetFloat("_IfCustomLight") == 1)
        {
             if ( material.GetFloat("_LightScale") != 0)
              {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _NormalTexx = Foldout(_NormalTexx, "�Զ������(CustomLight)");

        if (_NormalTexx)
        {
            EditorGUI.indentLevel++;



            GUI_CustomLight(material);

            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();
              }
            else
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                _NormalTexx = Foldout2(_NormalTexx, "�Զ������(CustomLight)");

                if (_NormalTexx)
                {
                    EditorGUI.indentLevel++;



                    GUI_CustomLight(material);

                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
            }
        }

        if (material.GetFloat("_IfPara") == 1) { 

            if (ParaTex.textureValue != null)
        {



            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            _shichax = Foldout(_shichax, "�Ӳ�ӳ��(ParallaxMapping)");

            if (_shichax)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.Space();

                GUI_para(material);

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();

        }
        else
        {


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            _shichax = Foldout2(_shichax, "�Ӳ�ӳ��(ParallaxMapping)");

            if (_shichax)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.Space();

                GUI_para(material);

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();



        }

        }

        //mengban

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        if (material.GetFloat("_StencilStyle") == 0) { 

        _mengbanxx = Foldout2(_mengbanxx, "ģ�建��(StencilBuffer)");

        if (_mengbanxx)
        {
            EditorGUI.indentLevel++;



            GUI_mengban(material);



            EditorGUI.indentLevel--;
        }
        }
        else { 
        _mengbanxx = Foldout(_mengbanxx, "ģ�建��(StencilBuffer)");

        if (_mengbanxx)
        {
            EditorGUI.indentLevel++;



            GUI_mengban(material);



            EditorGUI.indentLevel--;
        }


        }







        EditorGUILayout.EndVertical();


        //�ۺ�ѡ�������˵�
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        _Main_Foldout = Foldout(_Main_Foldout, "�ۺ�����(ComprehensiveSettings)");

        if (_Main_Foldout)
        {
            EditorGUI.indentLevel++;

        //    EditorGUILayout.Space();

            GUI_Main(material);

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();


        //˵��

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        _Tip_Foldout = Foldout(_Tip_Foldout, "˵��(Illustrate)");

            if (_Tip_Foldout)
            {
               EditorGUI.indentLevel++;


                GUI_Tip(material);

                EditorGUI.indentLevel--;
            }

        EditorGUILayout.EndVertical();


    }



    //��������������ʾ

    void GUI_Base(Material material)

    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
       


            EditorGUILayout.BeginHorizontal();

       

        EditorGUILayout.PrefixLabel("���ģʽ");

     

            if (material.GetFloat("_Scr") == 1)
            {
                if (GUILayout.Button("Add", shortButtonStyle))
                {
                    material.SetFloat("_Scr", 5);
                    material.SetFloat("_Dst", 10);
                    material.SetFloat("_AlphaAdd", 0);
                }
            }
            else
            {
                if (GUILayout.Button("Alpha", shortButtonStyle))
                {
                    material.SetFloat("_Scr", 1);
                    material.SetFloat("_Dst", 1);
                    material.SetFloat("_AlphaAdd", 1);
                }
            }

            EditorGUILayout.EndHorizontal();


        if (_tips == true)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);


       
            style.fontSize = 10;
            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            style.wordWrap = true;

            GUILayout.Label("*����Alpha���Ӻ�Add���ֵ���ģʽ", style);



            EditorGUILayout.EndVertical();
        }


        GUILayout.Space(5);









        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PrefixLabel("�޳�ģʽ");


      

        if (material.GetFloat("_Cullmode") == 0)
        {
            if (GUILayout.Button("˫����ʾ", shortButtonStyle))
            {
                material.SetFloat("_Cullmode", 1);
            }
        }
        else 
        {
            if (material.GetFloat("_Cullmode") == 1)
            {
                if (GUILayout.Button("��ʾ����", shortButtonStyle))
                {
                    material.SetFloat("_Cullmode", 2);
                }
            }

            else
            {
                if (GUILayout.Button("��ʾ����", shortButtonStyle))
                {
                    material.SetFloat("_Cullmode", 0);
                }
            }
        }

        EditorGUILayout.EndHorizontal();

        if (_tips == true)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            style.fontSize = 10;
            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            GUILayout.Label("*������ʾ���棬��ʾ���棬˫����ʾ����ģʽ", style);
            EditorGUILayout.EndVertical();
        }



        GUILayout.Space(5);


        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PrefixLabel("��ʾ����ǰ��");


        if (material.GetFloat("_Ztest") == 4)
        {
            if (GUILayout.Button("��", shortButtonStyle))
            {
                material.SetFloat("_Ztest", 8);
              
            }
        }
        else
        {
            if (GUILayout.Button("��", shortButtonStyle))
            {
                material.SetFloat("_Ztest", 4);
            }
        }

        EditorGUILayout.EndHorizontal();


        if (_tips == true)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            style.fontSize = 10;
            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            GUILayout.Label("*��������Ȳ���ZTest����Ϊalways����������Ч��ʾ�ڽ�ɫ֮ǰ�����⴩ģ", style);
            EditorGUILayout.EndVertical();
        }

        GUILayout.Space(5);


        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PrefixLabel("д�����");


        if (material.GetFloat("_Zwrite") == 0)
        {
            if (GUILayout.Button("��", shortButtonStyle))
            {
                material.SetFloat("_Zwrite", 1);

            }
        }
        else
        {
            if (GUILayout.Button("��", shortButtonStyle))
            {
                material.SetFloat("_Zwrite", 0);
            }
        }

        EditorGUILayout.EndHorizontal();
        if (_tips == true)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            style.fontSize = 10;
            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            GUILayout.Label("*���������д��ZWrite��Ϊon�����������һ���̶ȷ�ֹģ������,����͸��������������Ч�㼶��ʾ���������", style);
            EditorGUILayout.EndVertical();
        }








        EditorGUILayout.EndVertical();
        GUILayout.Space(5);




  











    }
    //����ͼ������ʾ����

    public void GUI_Maintextures(Material material)
    {
//
       EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        m_MaterialEditor.ShaderProperty(ScreenAsMain, "��ĻͼΪ����ͼ");

       
       EditorGUILayout.EndVertical();

        if (_tips == true)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            style.fontSize = 10;
            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            GUILayout.Label("*������ʹ��GrabScreen��Ϊ����ͼ��UVΪGrabScreenPosition,ʹ��UVŤ����Ť������ͼ���ɴﵽ��Ļ��Ť��Ч��", style);
            EditorGUILayout.EndVertical();
        }

    
     

        GUILayout.Space(5);
      
        m_MaterialEditor.TexturePropertySingleLine(new GUIContent("����ͼ"), mainTex_Sampler,TinColor, BackFaceColor);
            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*����ͼ������ɫ�������ף�������ŵ�������ɫ�����ֱ�Ϊ������ɫ�ͱ������ɫ", style);
                EditorGUILayout.EndVertical();
            }

        if (mainTex_Sampler.textureValue != null) 
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            _Mainxx = Foldouts(_Mainxx, "��ͼ����(�����)");

                 if (_Mainxx)
                 {

                    EditorGUI.indentLevel++;
                
                  EditorGUILayout.BeginHorizontal();
                    m_MaterialEditor.ShaderProperty(MainTexC, "UVClamp");

                    m_MaterialEditor.ShaderProperty(MainTexCV, "");

                    EditorGUILayout.EndHorizontal();

                    if (_tips == true)
                    {

                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        style.fontSize = 10;
                        style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                        GUILayout.Label("*������ѡ��ֱ��ʾ��ͼU����Clamp��V����Clamp�����Թ�ѡ����һ����Ҳ�����������򶼹�ѡ", style);
                        EditorGUILayout.EndVertical();
                    }




                    m_MaterialEditor.ShaderProperty(MainTexAR, "ʹ��Rͨ��");

                    if (_tips == true)
                    {

                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        style.fontSize = 10;
                        style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                        GUILayout.Label("*��ѡ��Redͨ����Ϊ͸��ͨ��������ѡ��Alphaͨ��Ϊ͸��ͨ����û��͸��ͨ���ĺڰ�ͼ���Թ�ѡ��ѡ���͸��ͨ����Ҫ��ѡ", style);
                        EditorGUILayout.EndVertical();
                    }


                    m_MaterialEditor.ShaderProperty(MainTexrotat, "��ͼ��ת");
                    if (_tips == true)
                    {

                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        style.fontSize = 10;
                        style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                        GUILayout.Label("*��ͼ��ת��������ͼ����ʡȥ������ͼ�ı䳯��", style);
                        EditorGUILayout.EndVertical();
                    }


                    m_MaterialEditor.ShaderProperty(MainTexRefine, "Refine");
                    if (_tips == true)
                    {

                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        style.fontSize = 10;
                        style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                        GUILayout.Label("*��ͼУɫ������������ͼ�����Աȣ���������ɫ�ʱ仯�Ե�������ԭ�����Ǹ�ԭͼһ��powerֵ��ʹ���Ե�����������ں�ԭͼlerp��ʽ�����������ĸ�ѡ��ֱ��Ӧԭͼǿ�ȣ��޸�ͼǿ�ȣ��޸�ͼpowerֵ��ԭͼ�޸�ͼlerp��ϳ̶ȣ�0-1֮�伴�ɣ�", style);
                        EditorGUILayout.EndVertical();
                    }
                         

                  EditorGUI.indentLevel--;
    
                }
               EditorGUILayout.EndVertical();

               if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*չ������Զ���ͼ������һЩ���ã�����uvclamp��͸��ͨ������ת��Уɫ", style);
                    EditorGUILayout.EndVertical();
                }

                m_MaterialEditor.TextureScaleOffsetProperty(mainTex_Sampler);
                EditorGUILayout.EndVertical();

                //
            GUILayout.Space(5);
                //

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("�Զ���ƫ��");
                if (material.GetFloat("_CustomdataMainTexUV") == 0)
            {
                if (GUILayout.Button("�ѹر�", shortButtonStyle))
                {
                    material.SetFloat("_CustomdataMainTexUV", 1);

             

                }
            }
                else
            {
                if (GUILayout.Button("�ѿ���", shortButtonStyle))
                {
                    material.SetFloat("_CustomdataMainTexUV", 0);


                }
            }

                EditorGUILayout.EndHorizontal();

                if (material.GetFloat("_CustomdataMainTexUV") == 1)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

          
                m_MaterialEditor.ShaderProperty(MainOffsetUC1, "U����");
                m_MaterialEditor.ShaderProperty(MainOffsetUC2, "Uͨ��");
           
                GUILayout.Space(5);

                    m_MaterialEditor.ShaderProperty(MainOffsetVC1, "V����");
                    m_MaterialEditor.ShaderProperty(MainOffsetVC2, "Vͨ��");

                    EditorGUILayout.EndVertical();


                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.PrefixLabel("Ӱ��๦��ͼ");


                    if (material.GetFloat("_CAddTexUV") == 0)
                    {
                        if (GUILayout.Button("�ѹر�", shortButtonStyle))
                        {
                            material.SetFloat("_CAddTexUV", 1);



                        }
                    }
                    else
                    {
                        if (GUILayout.Button("�ѿ���", shortButtonStyle))
                        {
                            material.SetFloat("_CAddTexUV", 0);


                        }
                    }





                    EditorGUILayout.EndHorizontal();


                    m_MaterialEditor.ShaderProperty(CAddTexUVT, "ͬ��ƫ��");


                    EditorGUILayout.EndVertical();





                }

                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*�����Զ������ݿ�����ͼUVOffset��������ͼһ��������ʹ�ã������Զ�������ʱ����custom1.xyzw,������custom2.xyzw��Ȼ��Ĭ��custom1.x����U����Offset��custom1.y����V����Offset�����Զ�������", style);
                    EditorGUILayout.EndVertical();
                }
   
                EditorGUILayout.EndVertical();

               //


                GUILayout.Space(5);

                //
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                m_MaterialEditor.ShaderProperty(MainTexUVS, "UVѡ��");
                if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*����normalĬ��UV��polar������UV��cylinderԲͲUV,UV2����ģʽ��Ĭ��UV��ģ���Դ�UV�������꼴������������ɢUV��ԲͲUV������������һ��ԲͲ������UV������ͼ����ԲͲUV����������ɫ�����������������߽���Ч����UV2��ʹ�ڶ���UV��", style);
                EditorGUILayout.EndVertical();
            }
                if (material.GetFloat("_MainTexUVS") == 1)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                m_MaterialEditor.ShaderProperty(CenterU, "����U");
                m_MaterialEditor.ShaderProperty(CenterV, "����V");
                EditorGUILayout.EndVertical();

                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*������UV���ĵ�λ��", style);
                    EditorGUILayout.EndVertical();
                }

            }
                if (material.GetFloat("_MainTexUVS") == 2)
            {
                m_MaterialEditor.ShaderProperty(Face, "Բ������");

                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*��������ϵ��Բ��UV�ĳ���", style);
                    EditorGUILayout.EndVertical();
                }

                m_MaterialEditor.ShaderProperty(TexCenter, "Բ������");

                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*��������ϵ��Բ��UV��λ��", style);
                    EditorGUILayout.EndVertical();
                }
            }
                EditorGUILayout.EndVertical();

                //

                GUILayout.Space(5);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            m_MaterialEditor.ShaderProperty(MainTexAC, "����ƫ��");
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);


            //
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                m_MaterialEditor.ShaderProperty(MainUspeed, "U����");
                m_MaterialEditor.ShaderProperty(MainVspeed, "V����");
                EditorGUILayout.EndVertical();

                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*����ͼ��UV�����ٶ�ֵ", style);
                    EditorGUILayout.EndVertical();
                }

                //

                GUILayout.Space(5);

                //
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("�๦��ͼ");
                if (material.GetFloat("_IfAddTex") == 0)
                {
                    if (GUILayout.Button("�ѹر�", shortButtonStyle))
                    {
                        material.SetFloat("_IfAddTex", 1);


                    }
                }
                else
                {
                    if (GUILayout.Button("�ѿ���", shortButtonStyle))
                    {
                        material.SetFloat("_IfAddTex", 0);


                    }
                }
                EditorGUILayout.EndHorizontal();
                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*�๦��ͼ��������ͼ�Ļ�������������ϸ�ڣ�͸��ͨ��ϸ�ڵĶ�����ͼ�����Ե�����ɫ��Ҳ���Գ䵱���ֵȵ�", style);
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
                //


        }
        if (mainTex_Sampler.textureValue == null)
             {
                material.SetFloat("_IfAddTex", 0);
              }

             if (material.GetFloat("_IfAddTex") == 0) 
        
             {       
                material.SetFloat("_AddTexBlend", 0);
                material.SetFloat("_AddTexBlendMode", 0);
             }

       

    }




    //Mask��ͼ����
    void GUI_Mask(Material material)

    {
      

      
        m_MaterialEditor.TexturePropertySingleLine(new GUIContent("������ͼ"), Mask_Sampler);
        if (_tips == true)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            style.fontSize = 10;
            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            GUILayout.Label("*������ͼ��Ҫ�����͸��ͨ�����������޳�����Ҫ�Ĳ���", style);
            EditorGUILayout.EndVertical();
        }


        if (Mask_Sampler.textureValue != null)

        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            _Maskxx = Foldouts(_Maskxx, "��ͼ����(�����)");

            if (_Maskxx)
            {
                EditorGUI.indentLevel++;

                //   EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                m_MaterialEditor.ShaderProperty(MaskC, "UVClamp");
                m_MaterialEditor.ShaderProperty(MaskCV, "");
                EditorGUILayout.EndHorizontal();
                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*������ѡ��ֱ��ʾ��ͼU����Clamp��V����Clamp�����Թ�ѡ����һ����Ҳ�����������򶼹�ѡ", style);
                    EditorGUILayout.EndVertical();
                }



                m_MaterialEditor.ShaderProperty(MaskAR, "ʹ��Rͨ��");


                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*��ѡ��Redͨ����Ϊ͸��ͨ��������ѡ��Alphaͨ��Ϊ͸��ͨ����û��͸��ͨ���ĺڰ�ͼ���Թ�ѡ��ѡ���͸��ͨ����Ҫ��ѡ", style);
                    EditorGUILayout.EndVertical();
                }





                m_MaterialEditor.ShaderProperty(Maskrotat, "��ͼ��ת");

                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*��ͼ��ת��������ͼ����ʡȥ������ͼ�ı䳯��", style);
                    EditorGUILayout.EndVertical();
                }

                m_MaterialEditor.ShaderProperty(IfMaskColor, "��ͼ��ɫ");
                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*������Mask��RGBֵҲ�˵�����ͼ�ϣ�����������һЩ��ɫ����", style);
                    EditorGUILayout.EndVertical();
                }

                EditorGUI.indentLevel--;
            

        }
            EditorGUILayout.EndVertical();
            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*չ������Զ���ͼ������һЩ���ã�����uvclamp��͸��ͨ������ת������������ɫ", style);
                EditorGUILayout.EndVertical();
            }
            m_MaterialEditor.TextureScaleOffsetProperty(Mask_Sampler);
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);




            EditorGUILayout.BeginVertical(EditorStyles.helpBox);


            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("�Զ���ƫ��");


            if (material.GetFloat("_CustomdataMaskUV") == 0)
            {




                if (GUILayout.Button("�ѹر�", shortButtonStyle))
                {
                    material.SetFloat("_CustomdataMaskUV", 1);

                  
                }
            }
            else
            {
                if (GUILayout.Button("�ѿ���", shortButtonStyle))
                {
                    material.SetFloat("_CustomdataMaskUV", 0);


                }
            }

            EditorGUILayout.EndHorizontal();




            if (material.GetFloat("_CustomdataMaskUV") == 1) { 


                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            m_MaterialEditor.ShaderProperty(MaskOffsetUC1, "U����");
            m_MaterialEditor.ShaderProperty(MaskOffsetUC2, "Uͨ��");

            GUILayout.Space(5);

            m_MaterialEditor.ShaderProperty(MaskOffsetVC1, "V����");
            m_MaterialEditor.ShaderProperty(MaskOffsetVC2, "Vͨ��");

            EditorGUILayout.EndVertical();

            }
            EditorGUILayout.EndVertical();

            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*�����Զ������ݿ�����ͼUVOffset��������ͼһ��������ʹ�ã������Զ�������ʱ��������custom1.xyzw,������custom2.xyzw��Ĭ��custom1.z����U����Offset��custom1.w����V����Offset", style);
                EditorGUILayout.EndVertical();
            }






            //   GUILayout.Space(5);




            GUILayout.Space(5);




            m_MaterialEditor.FloatProperty(MaskScale, "����ǿ��");

            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*������͸��ͨ������˵ı���", style);
                EditorGUILayout.EndVertical();
            }
            GUILayout.Space(5);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            m_MaterialEditor.ShaderProperty(MaskTexUVS, "UVѡ��");
            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("**����normalĬ��UV��polar������UV��cylinderԲͲUV,UV2����ģʽ��Ĭ��UV��ģ���Դ�UV�������꼴������������ɢUV��ԲͲUV������������һ��ԲͲ������UV������ͼ����ԲͲUV����������ɫ�����������������߽���Ч����UV2��ʹ�ڶ���UV��", style);
                EditorGUILayout.EndVertical();
            }


            if (material.GetFloat("_MaskTexUVS") == 1)
            {

                m_MaterialEditor.ShaderProperty(CenterU, "����U");
                m_MaterialEditor.ShaderProperty(CenterV, "����V");


                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*������UV���ĵ�λ��", style);
                    EditorGUILayout.EndVertical();
                }
            }



            if (material.GetFloat("_MaskTexUVS") == 2)
            {
               
                m_MaterialEditor.ShaderProperty(Face, "Բ������");
                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*��������ϵ��Բ��UV�ĳ���", style);
                    EditorGUILayout.EndVertical();
                }
                m_MaterialEditor.ShaderProperty(TexCenter, "Բ������");
                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*��������ϵ��Բ��UV��λ��", style);
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(5);

      

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            m_MaterialEditor.ShaderProperty(MaskUspeed, "U����");

     

            m_MaterialEditor.ShaderProperty(MaskVspeed, "V����");

            EditorGUILayout.EndVertical();

            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*����ͼ��UV�����ٶ�ֵ", style);
                EditorGUILayout.EndVertical();
            }
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            m_MaterialEditor.ShaderProperty(IfMaskPlusTex, "������������ͼ");

            if(material.GetFloat("_IfMaskPlusTex") == 1)
            {
            GUILayout.Space(5);

            m_MaterialEditor.TexturePropertySingleLine(new GUIContent("��������ͼ"), MaskPlusTex);
                //

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                _MaskPlusxx = Foldouts(_MaskPlusxx, "��ͼ����(�����)");

                if (_MaskPlusxx)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.BeginHorizontal();
                    m_MaterialEditor.ShaderProperty(MaskPlusC, "UVClamp");
                    m_MaterialEditor.ShaderProperty(MaskPlusCV, "");
                    EditorGUILayout.EndHorizontal();

                    m_MaterialEditor.ShaderProperty(MaskPlusAR, "ʹ��Rͨ��");
                    m_MaterialEditor.ShaderProperty(MaskPlusR, "��ͼ��ת");

                    EditorGUILayout.EndVertical();
                    EditorGUI.indentLevel--;

                }
                    //
                    m_MaterialEditor.TextureScaleOffsetProperty(MaskPlusTex);
                EditorGUILayout.EndVertical();
                GUILayout.Space(5);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                m_MaterialEditor.ShaderProperty(MaskPlusUspeed, "U����");
                m_MaterialEditor.ShaderProperty(MaskPlusVspeed, "V����");
                EditorGUILayout.EndVertical();
                GUILayout.Space(5);

            }

            EditorGUILayout.EndVertical();


        





        }
        GUILayout.Space(5);

        if (Mask_Sampler.textureValue == null)
        {
            material.SetFloat("_IfMaskPlusTex", 0);


        }



    }



    //�ܽ�����
    void GUI_Dissolve(Material material)

    {
       
        m_MaterialEditor.TexturePropertySingleLine(new GUIContent("�ܽ���ͼ"), Dissolve_Tex, Dissolve_Color);

        if (_tips == true)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            style.fontSize = 10;
            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            GUILayout.Label("*�����ܽ��ο�Ч�����ܽ��Ƿǳ��������ض���ģʽ�������м�֡����˿����Ч��������ܣ��������ɫ����Ϊ�ܽ��Ե��ɫ��ֵ��ע����ǽ������ɫ͸���ȵ�Ϊ0���ܽ��Ե��ɫ��Ϊ��ͼ����ɫ���������ܽ�ʱ�ǳ����á�", style);
            EditorGUILayout.EndVertical();
        }


        if (Dissolve_Tex.textureValue != null)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            _Dissolvexx = Foldouts(_Dissolvexx, "��ͼ����(�����)");

            if (_Dissolvexx)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                m_MaterialEditor.ShaderProperty(DissolveC, "UVClamp");
                m_MaterialEditor.ShaderProperty(DissolveCV, "");
                EditorGUILayout.EndHorizontal();
                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*������ѡ��ֱ��ʾ��ͼU����Clamp��V����Clamp�����Թ�ѡ����һ����Ҳ�����������򶼹�ѡ", style);
                    EditorGUILayout.EndVertical();
                }
                m_MaterialEditor.ShaderProperty(DissolveAR, "ʹ��Rͨ��");

                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*��ѡ��Redͨ����Ϊ�ܽ�ͨ��������ѡ��Alphaͨ��Ϊ�ܽ�ͨ����û��͸��ͨ���ĺڰ�ͼ���Թ�ѡ��ѡ���͸��ͨ����Ҫ��ѡ", style);
                    EditorGUILayout.EndVertical();
                }

                m_MaterialEditor.ShaderProperty(Dissolve_Rotation, "��ͼ��ת");

                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*��ͼ��ת��������ͼ����ʡȥ������ͼ�ı䳯��", style);
                    EditorGUILayout.EndVertical();
                }
                m_MaterialEditor.ShaderProperty(DissolveTexExp, "PowerExp");
                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*��ͼ������power���㣬ʹ����Ǹ��ӷ������ܽ�ʱ��Ե����ӷ�����power��0-1ʱ�෴���������ܽ�������", style);
                    EditorGUILayout.EndVertical();
                }

                m_MaterialEditor.ShaderProperty(IfDissolveColor, "�ܽ���ɫ�ܶ���ɫӰ��");

                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*��Ե��ɫ�Ƿ�������ϵͳ����ɫӰ��", style);
                    EditorGUILayout.EndVertical();
                }

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();


            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*չ������Զ���ͼ������һЩ���ã�����uvclamp��ͨ��ѡ����ת����ͼ����power����", style);
                EditorGUILayout.EndVertical();
            }
            m_MaterialEditor.TextureScaleOffsetProperty(Dissolve_Tex);
            EditorGUILayout.EndVertical();

            GUILayout.Space(5);



            //




            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("�����ܽ�ͼ");


            if (material.GetFloat("_IfDissolvePlus") == 0)
            {
                if (GUILayout.Button("�ѹر�", shortButtonStyle))
                {
                    material.SetFloat("_IfDissolvePlus", 1);
                    material.SetFloat("_DissolveTexDivide", 1);


                }
            }
            else
            {
                if (GUILayout.Button("�ѿ���", shortButtonStyle))
                {
                    material.SetFloat("_IfDissolvePlus", 0);


                }
            }

            EditorGUILayout.EndHorizontal();
            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*��ʼ�����ܽ�ͼ�󣬿������������ܽ��ˡ���������ң����ϵ��£������ĵ����ܵȵ�Ч��", style);
                EditorGUILayout.EndVertical();
            }

         //   GUILayout.Space(5);


            if (material.GetFloat("_IfDissolvePlus") == 1)
            {
               
                m_MaterialEditor.TexturePropertySingleLine(new GUIContent("�����ܽ���ͼ"), DissloveTexPlus);
                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*�����ܽ�ͼ����ʹ��һ�Ž���ͼ����������ҵĺڰ׽��䣬�����ܽ�ʱ�ͻ��д����ҵ��ܽ����ƣ�ͬ��������ʹ�������������һ����ɢ״�ĺڰ׽����������ܽⷽ�����Ƹ�", style);
                    EditorGUILayout.EndVertical();
                }

                if (DissloveTexPlus.textureValue != null)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    _Dissolveplusxx = Foldouts(_Dissolveplusxx, "��ͼ����(�����)");

                    if (_Dissolveplusxx)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.BeginHorizontal();
                        m_MaterialEditor.ShaderProperty(DissolvePlusC, "UVClamp");
                        m_MaterialEditor.ShaderProperty(DissolvePlusCV, "");
                        EditorGUILayout.EndHorizontal();
                        if (_tips == true)
                        {

                            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                            style.fontSize = 10;
                            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                            GUILayout.Label("*������ѡ��ֱ��ʾ��ͼU����Clamp��V����Clamp�����Թ�ѡ����һ����Ҳ�����������򶼹�ѡ", style);
                            EditorGUILayout.EndVertical();
                        }
                        m_MaterialEditor.ShaderProperty(DissolvePlusAR, "ʹ��Rͨ��");
                        if (_tips == true)
                        {

                            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                            style.fontSize = 10;
                            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                            GUILayout.Label("*��ѡ��Redͨ����Ϊ�ܽ�ͨ��������ѡ��Alphaͨ��Ϊ�ܽ�ͨ����û��͸��ͨ���ĺڰ�ͼ���Թ�ѡ��ѡ���͸��ͨ����Ҫ��ѡ", style);
                            EditorGUILayout.EndVertical();
                        }


                        m_MaterialEditor.ShaderProperty(DissolvePlusR, "��ͼ��ת");


                        if (_tips == true)
                        {

                            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                            style.fontSize = 10;
                            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                            GUILayout.Label("*��ͼ��ת��������ͼ����ʡȥ������ͼ�ı䳯��", style);
                            EditorGUILayout.EndVertical();
                        }

                     


                        EditorGUI.indentLevel--;
                    }

                    EditorGUILayout.EndVertical();

                    if (_tips == true)
                    {

                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        style.fontSize = 10;
                        style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                        GUILayout.Label("*չ������Զ���ͼ������һЩ���ã�����uvclamp��ͨ��ѡ����ת", style);
                        EditorGUILayout.EndVertical();
                    }
                    m_MaterialEditor.TextureScaleOffsetProperty(DissloveTexPlus);


                 






                    EditorGUILayout.EndVertical();


                }
                else
                {
                    material.SetFloat("_DissolveTexDivide",1);
                }
                

                m_MaterialEditor.ShaderProperty(DissolveTexDivide, "�ܽ�����ͼ˥��");

                GUILayout.Space(5);
                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*�ܽ�ͼ˥���󣬸����ܽ�ͼ���ܽ����ƻ��ǿ���ܽ�ķ���л��ǿ", style);
                    EditorGUILayout.EndVertical();
                }


            

            }



            EditorGUILayout.EndVertical();




            GUILayout.Space(5);


            //

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("�Զ���ƫ��");


            if (material.GetFloat("_IfDissolveOffsetC") == 0)
            {
                if (GUILayout.Button("�ѹر�", shortButtonStyle))
                {
                    material.SetFloat("_IfDissolveOffsetC", 1);




                }
            }
            else
            {
                if (GUILayout.Button("�ѿ���", shortButtonStyle))
                {
                    material.SetFloat("_IfDissolveOffsetC", 0);



                }


            }



            EditorGUILayout.EndHorizontal();


            if (material.GetFloat("_IfDissolveOffsetC") == 1) {


                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                m_MaterialEditor.ShaderProperty(DissolveOffsetUC1, "U����");
                m_MaterialEditor.ShaderProperty(DissolveOffsetUC2, "Uͨ��");

                GUILayout.Space(5);

                m_MaterialEditor.ShaderProperty(DissolveOffsetVC1, "V����");
                m_MaterialEditor.ShaderProperty(DissolveOffsetVC2, "Vͨ��");

                EditorGUILayout.EndVertical();

            }



                if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*�����Զ������ݿ����ܽ�ƫ�ƣ������Զ�������ʱ��������custom1.xyzw,������custom2.xyzw��Ĭ��custom2.x��custom2.y�����ܽ�ƫ�Ƴ̶�", style);
                EditorGUILayout.EndVertical();
            }








            EditorGUILayout.EndVertical();
            GUILayout.Space(5);


            //

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        //   
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("�Զ����ܽ�ϵ��");


            if (material.GetFloat("_CustomdataDis") == 0)
            {
                if (GUILayout.Button("�ѹر�", shortButtonStyle))
                {
                    material.SetFloat("_CustomdataDis", 1);



   








                }
            }
            else
            {
                if (GUILayout.Button("�ѿ���", shortButtonStyle))
                {
                    material.SetFloat("_CustomdataDis", 0);




               


                }
       

            }



            EditorGUILayout.EndHorizontal();

            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*�����Զ������ݿ����ܽ�̶ȣ������Զ�������ʱ��������custom1.xyzw,������custom2.xyzw��Ĭ��custom2.z�����ܽ�̶ȣ�����ͨ�����������ѡ���Զ���ͨ���������Զ������ݺ��ܽ�ϵ������ʧЧ��������", style);
                EditorGUILayout.EndVertical();
            }

        //    GUILayout.Space(5);


            if (material.GetFloat("_CustomdataDis") == 1) {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                m_MaterialEditor.ShaderProperty(DissolveFactorC1, "����");


            m_MaterialEditor.ShaderProperty(DissolveFactorC2, "ͨ��");
                EditorGUILayout.EndVertical();
                GUILayout.Space(5);


                if (material.GetFloat("_CustomdataDis") == 1)
                {

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("��βģʽ");
                    if (material.GetFloat("_CustomdataDisT") == 0)
                    {
                        if (GUILayout.Button("�ѹر�", shortButtonStyle))
                        {
                            material.SetFloat("_CustomdataDisT", 1);
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("�ѿ���", shortButtonStyle))
                        {
                            material.SetFloat("_CustomdataDisT", 0);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    if (_tips == true)
                    {

                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        style.fontSize = 10;
                        style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                        GUILayout.Label("*�Զ������ݶ�������β������Ч���������ǿ�����β�ܽ�ģʽ�󣬽�����ʹ���Զ������ݣ����Ǹ�������ϵͳ��ColoroverLifetime��Alphaͨ���������ܽ�̶�", style);
                        EditorGUILayout.EndVertical();
                    }

                    GUILayout.Space(5);

                }
                //    
            }

            EditorGUILayout.EndVertical();


           


            if (material.GetFloat("_CustomdataDis") == 0) 
            
            {
                material.SetFloat("_CustomdataDisT",0);
            }









            GUILayout.Space(5);


            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("�ܽ�ģʽ");


            if (material.GetFloat("_soft_sting") == 0)
            {


                if (GUILayout.Button("�����ܽ�ģʽ", shortButtonStyle))
                {
                    material.SetFloat("_soft_sting", 1);

                    material.SetFloat("_sot_sting_A", 1);


                }
            }
            else
            {


                if (GUILayout.Button("Ӳ���ܽ�ģʽ", shortButtonStyle))
                {
                    material.SetFloat("_soft_sting", 0);

                    material.SetFloat("_sot_sting_A", 0);


                }
            }
           
            EditorGUILayout.EndHorizontal();

            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*�ܽ�ģʽ�������ܽ��Ӳ���ܽ�����ģʽ�����ܽ�ʱ��Ե��͸���Ƚ�����ɱȽ���ͣ�Ӳ���ܽ�����cutout����ʽ", style);
                EditorGUILayout.EndVertical();
            }
            GUILayout.Space(5);


            if (material.GetFloat("_CustomdataDis") == 0)
            {


                m_MaterialEditor.ShaderProperty(Dissolve_Factor, "�ܽ�ϵ��");
                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*�ܽ�̶ȣ�0Ϊ�պò��ܽ⣬1Ϊǡ����ȫ�ܽ⣬�ܽ�ϵ���ڿ����Զ������ݺ�ʧЧ�����أ������Զ���������ȫ�����ܽ�", style);
                    EditorGUILayout.EndVertical();
                }

                GUILayout.Space(5);
            }






            if (material.GetFloat("_soft_sting") == 0)
            { 

                m_MaterialEditor.ShaderProperty(Dissolve_Soft, "��Ӳ�̶�");

                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*���ܽ�ģʽ�ܽ��Ե����Ӳ�̶ȣ���ֵԽ���ܽ��ԵԽ������Խ�ӽ�Ӳ�ܽ�ģʽ���෴���ܽ��Խ�����", style);
                    EditorGUILayout.EndVertical();
                }






                GUILayout.Space(5);
            }
            if (material.GetFloat("_soft_sting") == 1)
            { 

                m_MaterialEditor.ShaderProperty(Dissolve_Wide, "�ܽ����");
                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*�ܽ��Ե�Ŀ��ȣ�����Ӳ�ܽⶼ��Ч", style);
                    EditorGUILayout.EndVertical();
                }


                GUILayout.Space(5);
            }


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            m_MaterialEditor.ShaderProperty(DissolveTexUVS2, "UVѡ��");
            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("**����normalĬ��UV��polar������UV��cylinderԲͲUV,UV2����ģʽ��Ĭ��UV��ģ���Դ�UV�������꼴������������ɢUV��ԲͲUV������������һ��ԲͲ������UV������ͼ����ԲͲUV����������ɫ�����������������߽���Ч����UV2��ʹ�ڶ���UV��", style);
                EditorGUILayout.EndVertical();
            }


            if (material.GetFloat("_DissolveTexUVS2") == 1)
            {

                m_MaterialEditor.ShaderProperty(CenterU, "����U");
                m_MaterialEditor.ShaderProperty(CenterV, "����V");

                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*������UV�����ĵ�λ��", style);
                    EditorGUILayout.EndVertical();
                }
            }


            if (material.GetFloat("_DissolveTexUVS2") == 2)
            {
               
                m_MaterialEditor.ShaderProperty(Face, "Բ������");
                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*����������Բ���ĳ���", style);
                    EditorGUILayout.EndVertical();
                }
                m_MaterialEditor.ShaderProperty(TexCenter, "Բ������");
                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*����������Բ����λ��", style);
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(5);








            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            m_MaterialEditor.ShaderProperty(Dissolve_Uspeed, "U����");

    

            m_MaterialEditor.ShaderProperty(Dissolve_Vspeed, "V����");

            EditorGUILayout.EndVertical();

            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*�ܽ�ͼ��UV�����ٶ�", style);
                EditorGUILayout.EndVertical();
            }

        

        }

        GUILayout.Space(5);



        if (Dissolve_Tex.textureValue == null)
        {

            material.SetFloat("_IfDissolvePlus", 0);

            material.SetFloat("_DissolveTexDivide", 1);
        }


    }


    //UVŤ������
    void GUI_Distort(Material material)

    {
     

        m_MaterialEditor.TexturePropertySingleLine(new GUIContent("UVŤ����ͼ"), Distort_Tex);

       
        if (_tips == true)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            style.fontSize = 10;
            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            GUILayout.Label("*Ť����ͼuv��Ϣ�����Ը�������ѡ����ҪŤ������ͼ��Ť��uv����ͼ����Һ��Ч�������ʺ�����ˮ���ƣ�дʵ������ӿ��Ч��", style);
            EditorGUILayout.EndVertical();
        }







        if (Distort_Tex.textureValue != null)
        {

            //
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            _DistortTexxx = Foldouts(_DistortTexxx, "��ͼ����(�����)");


            if (_DistortTexxx)
            {
                EditorGUI.indentLevel++;


               



                if (material.GetFloat("_IfFlowmap") == 0) {

                    m_MaterialEditor.ShaderProperty(DistortTexAR, "ʹ��Rͨ��");
                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*������Rͨ������ΪŤ��ͨ����������ʹ��Alphaͨ��ΪŤ��ͨ�����ڰ�ͼû��Alphaͨ���빴ѡ��ѡ���Alphaͨ������ͼ�벻Ҫ��ѡ", style);
                    EditorGUILayout.EndVertical();
                }

                if(material.GetFloat("_IfNormalDistort") == 0) {
                    m_MaterialEditor.ShaderProperty(DistortRemap, "Remap");
                    if (_tips == true)
                    {

                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        style.fontSize = 10;
                        style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                        GUILayout.Label("*��uvŤ�������ݴ�0��1ӳ�䵽-0.5��0.5��ʹŤ��������ӷḻ", style);
                        EditorGUILayout.EndVertical();
                    }


                    }


                    m_MaterialEditor.ShaderProperty(IfNormalDistort, "���߻�");
                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*�����󣬽���Ť����ͼ���з��߻��������ٶ�������ҪŤ�������������Ť����������Ť�������ƽ�������������ɢ�ƶ���������Ť���������������Ͻ��ƶ����ʺ���������ĻŤ����Ч��", style);
                    EditorGUILayout.EndVertical();
                }


                }


                m_MaterialEditor.ShaderProperty(IfFlowmap, "ʹ��FlowMapͼ");
                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*������ʹ��FLowMap�㷨����ΪŤ����ͼ���������������̳���ɢЧ��", style);
                    EditorGUILayout.EndVertical();
                }

                EditorGUI.indentLevel--;

            }

            EditorGUILayout.EndVertical();










            m_MaterialEditor.TextureScaleOffsetProperty(Distort_Tex);
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);





            //

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            m_MaterialEditor.TexturePropertySingleLine(new GUIContent("UVŤ��������ͼ"), DistortMaskTex);


            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*uvŤ������ͼ", style);
                EditorGUILayout.EndVertical();
            }







            if (DistortMaskTex.textureValue != null)
            {


                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                _DistortMaskTexxx = Foldouts(_DistortMaskTexxx, "��ͼ����(�����)");


                if (_DistortMaskTexxx)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.BeginHorizontal();
                    m_MaterialEditor.ShaderProperty(DistortMaskTexC, "UVClamp");

                    m_MaterialEditor.ShaderProperty(DistortMaskTexCV, "");

                    EditorGUILayout.EndHorizontal();

                    if (_tips == true)
                    {

                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        style.fontSize = 10;
                        style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                        GUILayout.Label("*������ѡ��ֱ��ʾ��ͼU����Clamp��V����Clamp�����Թ�ѡ����һ����Ҳ�����������򶼹�ѡ", style);
                        EditorGUILayout.EndVertical();
                    }


                    m_MaterialEditor.ShaderProperty(DistortMaskTexAR, "ʹ��Rͨ��");
                    if (_tips == true)
                    {

                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        style.fontSize = 10;
                        style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                        GUILayout.Label("*������Rͨ������ΪŤ������ͨ����������ʹ��Alphaͨ��ΪŤ������ͨ�����ڰ�ͼû��Alphaͨ���빴ѡ��ѡ���Alphaͨ������ͼ�벻Ҫ��ѡ", style);
                        EditorGUILayout.EndVertical();
                    }



                    m_MaterialEditor.ShaderProperty(DistortMaskTexR, "��ͼ��ת");
                    if (_tips == true)
                    {

                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        style.fontSize = 10;
                        style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                        GUILayout.Label("*��ͼ��ת��������ͼ����ʡȥ������ͼ�ı䳯��", style);
                        EditorGUILayout.EndVertical();
                    }




                  






                    EditorGUI.indentLevel--;

                }

                EditorGUILayout.EndVertical();










                m_MaterialEditor.TextureScaleOffsetProperty(DistortMaskTex);
                EditorGUILayout.EndVertical();
                GUILayout.Space(5);


            }
            EditorGUILayout.EndVertical();

            //



            GUILayout.Space(5);



            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("�Զ���ǿ��");


            if (material.GetFloat("_CustomDistort") == 0)
            {
                if (GUILayout.Button("�ѹر�", shortButtonStyle))
                {
                    material.SetFloat("_CustomDistort", 1);

    

                }
            }
            else
            {
                if (GUILayout.Button("�ѿ���", shortButtonStyle))
                {
                    material.SetFloat("_CustomDistort", 0);


                }
            }

          







                EditorGUILayout.EndHorizontal();



            if (material.GetFloat("_CustomDistort") == 1)
            {


                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                m_MaterialEditor.ShaderProperty(DistortFactorC1, "����");


                m_MaterialEditor.ShaderProperty(DistortFactorC2, "ͨ��");
                EditorGUILayout.EndVertical();


            }
            EditorGUILayout.EndVertical();
            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*�����Զ������ݿ�����ͼŤ���̶ȣ�����ܽ������������flowmap�ܽ�Ч���������Զ�������ʱ����custom1.xyzw,������custom2.xyzw��Ȼ��custom2.w����Ť���̶ȣ������Զ������ݺ�Ť���̶Ƚ���ʧЧ��������", style);
                EditorGUILayout.EndVertical();
            }

            GUILayout.Space(5);

            if (material.GetFloat("_CustomDistort") == 0)
            { 

                m_MaterialEditor.ShaderProperty(Distort_Factor, "UVŤ���̶�");

                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*Ť���̶ȣ������Զ������ݺ�Ť���̶Ƚ���ʧЧ��������", style);
                    EditorGUILayout.EndVertical();
                }

                GUILayout.Space(5);


            }
      

        

              EditorGUILayout.BeginVertical(EditorStyles.helpBox);
              GUILayout.Label("    UV������ͼһ��");
              EditorGUILayout.EndVertical();



      

          
      

            GUILayout.Space(5);





            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            m_MaterialEditor.ShaderProperty(Distort_Uspeed, "U����");



            m_MaterialEditor.ShaderProperty(Distort_Vspeed, "V����");


            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*Ť����ͼ��UV�����ٶ�", style);
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();

            GUILayout.Space(5);

           

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("Ť������ͼ");


            if (material.GetFloat("_DistortMainTex") == 0)
            {
                if (GUILayout.Button("�ѹر�", shortButtonStyle))
                {
                    material.SetFloat("_DistortMainTex", 1);


                }
            }
            else
            {
                if (GUILayout.Button("�ѿ���", shortButtonStyle))
                {
                    material.SetFloat("_DistortMainTex", 0);


                }
            }

            EditorGUILayout.EndHorizontal();







           if (material.GetFloat("_IfAddTex") == 1)
            {

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Ť������ͼ");


                if (material.GetFloat("_DistortAddTex") == 0)
                {
                    if (GUILayout.Button("�ѹر�", shortButtonStyle))
                    {
                        material.SetFloat("_DistortAddTex", 1);


                    }
                }
                else
                {
                    if (GUILayout.Button("�ѿ���", shortButtonStyle))
                    {
                        material.SetFloat("_DistortAddTex", 0);


                    }
                }

                EditorGUILayout.EndHorizontal();


            }






            if (Mask_Sampler.textureValue != null)

            { 
                EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("Ť������ͼ");


            if (material.GetFloat("_DistortMask") == 0)
            {
                if (GUILayout.Button("�ѹر�", shortButtonStyle))
                {
                    material.SetFloat("_DistortMask", 1);


                }
            }
            else
            {
                if (GUILayout.Button("�ѿ���", shortButtonStyle))
                {
                    material.SetFloat("_DistortMask", 0);


                }
            }

            EditorGUILayout.EndHorizontal();

         //   GUILayout.Space(5);

            }




            if (Dissolve_Tex.textureValue != null)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PrefixLabel("Ť���ܽ���ͼ");


                if (material.GetFloat("_DistortDisTex") == 0)
                {
                    if (GUILayout.Button("�ѹر�", shortButtonStyle))
                    {
                        material.SetFloat("_DistortDisTex", 1);


                    }
                }
                else
                {
                    if (GUILayout.Button("�ѿ���", shortButtonStyle))
                    {
                        material.SetFloat("_DistortDisTex", 0);


                    }
                }

                EditorGUILayout.EndHorizontal();

         

            }



            if (NormalTex.textureValue != null && material.GetFloat("_IfCustomLight") == 1)

            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.PrefixLabel("Ť������ͼ");


                if (material.GetFloat("_DistortNormalTex") == 0)
                {
                    if (GUILayout.Button("�ѹر�", shortButtonStyle))
                    {
                        material.SetFloat("_DistortNormalTex", 1);


                    }
                }
                else
                {
                    if (GUILayout.Button("�ѿ���", shortButtonStyle))
                    {
                        material.SetFloat("_DistortNormalTex", 0);


                    }
                }

                EditorGUILayout.EndHorizontal();

            //    GUILayout.Space(5);

            }


         
        }

        if (Distort_Tex.textureValue == null)
        {
            material.SetFloat("_DistortFactor", 0);

                DistortMaskTex.textureValue = null;


        }
        if (_tips == true)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            style.fontSize = 10;
            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            GUILayout.Label("*�ֱ�����ر�Ť������ͼ���UV��Ϣ", style);
            EditorGUILayout.EndVertical();
        }

        GUILayout.Space(5);
    }


    //FNL����
    void GUI_FNL(Material material)

    {

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);




        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PrefixLabel("��������");


                 if (material.GetFloat("_FNLfanxiangkaiguan") == 0)
                {
                  if (GUILayout.Button("�ѹر�", shortButtonStyle))
                   { 
                     material.SetFloat("_FNLfanxiangkaiguan", 1);


                   }

                 }
                 else
                 {
                       if (GUILayout.Button("�ѿ���", shortButtonStyle))
                       { 
                     material.SetFloat("_FNLfanxiangkaiguan", 0);

                       }

                  }

        EditorGUILayout.EndHorizontal();
        if (_tips == true)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            style.fontSize = 10;
            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            GUILayout.Label("*�����������������ģ�ͱ�Ե��������Ч��ģ�͸�", style);
            EditorGUILayout.EndVertical();
        }

        if (material.GetFloat("_FNLfanxiangkaiguan") == 1) 
        {

            GUILayout.Space(5);
            m_MaterialEditor.ShaderProperty(Fnl_Soft, "ģ�������̶�");
            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*�����̶�Խ�ߣ�ģ�ͱ�Ե�޳�Խ�࣬ģ�͸�Խ��", style);
                EditorGUILayout.EndVertical();
            }


            GUILayout.Space(5);


            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("�����޳�����");


            if (material.GetFloat("_softback") == 0)
            {
                if (GUILayout.Button("�ѹر�", shortButtonStyle))
                {
                    material.SetFloat("_softback", 1);


                }
            }
            else
            {
                if (GUILayout.Button("�ѿ���", shortButtonStyle))
                {
                    material.SetFloat("_softback", 0);


                }
            }

            EditorGUILayout.EndHorizontal();
            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*��������ֱ���޳������棬�ڿ����������ɫʱ��ģ�ͱ�Ե�������棬��ѡ�����һ���̶ȼ������棬һ�㲻�ÿ���", style);
                EditorGUILayout.EndVertical();
            }


            GUILayout.Space(5);











        }



        if (material.GetFloat("_FNLfanxiangkaiguan") == 0)
        {
            material.SetFloat("_softFacotr", 0);
        }



        //  }



        EditorGUILayout.EndVertical();




        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        m_MaterialEditor.ShaderProperty(IfFNLAlpha, "Ӱ��͸��ͨ��");
        if (_tips == true)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            style.fontSize = 10;
            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            GUILayout.Label("*��������������ֵ�����͸��ͨ����", style);
            EditorGUILayout.EndVertical();
        }
        GUILayout.Space(5);
        m_MaterialEditor.ShaderProperty(Fnl_Color, "�������ɫ");
        if (_tips == true)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            style.fontSize = 10;
            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            GUILayout.Label("*������ĵ�����ɫ", style);
            EditorGUILayout.EndVertical();
        }

        GUILayout.Space(5);


            m_MaterialEditor.ShaderProperty(Fnl_Scale, "�����ǿ��");
        if (_tips == true)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            style.fontSize = 10;
            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            GUILayout.Label("*�������ǿ�ȣ���ɫ��Ҳ���Ե�ǿ�ȣ������������������Ϊ�˷���k֡", style);
            EditorGUILayout.EndVertical();
        }

        GUILayout.Space(5);



     


            m_MaterialEditor.ShaderProperty(Fnl_Power, "�������");
        if (_tips == true)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            style.fontSize = 10;
            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            GUILayout.Label("*�������PowerExpֵ��ʹ�����Ч����������", style);
            EditorGUILayout.EndVertical();
        }
        GUILayout.Space(5);

            m_MaterialEditor.ShaderProperty(Dir, "�����ƫ��");
        if (_tips == true)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            style.fontSize = 10;
            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            GUILayout.Label("*��������ӽ�ƫ�ƣ�������ֵ���Ե������ƹ�Դ����ĸо�", style);
            EditorGUILayout.EndVertical();
        }


        
      



        EditorGUILayout.EndVertical();


        EditorGUILayout.EndVertical();


        GUILayout.Space(5);














    }

    //VTO����
    void GUI_VTO(Material material)

    {


        if (material.GetFloat("_IfVAT") == 0) { 




        m_MaterialEditor.TexturePropertySingleLine(new GUIContent("����ƫ����ͼ"), Vto_Tex);

        if (_tips == true)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            style.fontSize = 10;
            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            GUILayout.Label("*�����������㶯����������������ֻ�ܸ��ݷ��߷�����ж����ƶ��������Ҫ�ı��ƶ����򣬿���ȥ�޸�ģ�͵ķ��߳���", style);
            EditorGUILayout.EndVertical();
        }




        if (Vto_Tex.textureValue != null)

        {


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            _VTOxx = Foldouts(_VTOxx, "��ͼ����(�����)");

            if (_VTOxx)
        {
            EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                m_MaterialEditor.ShaderProperty(VTOC, "UVClamp");
    
                m_MaterialEditor.ShaderProperty(VTOCV, "");

                EditorGUILayout.EndHorizontal();

                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*������ѡ��ֱ��ʾ��ͼU����Clamp��V����Clamp�����Թ�ѡ����һ����Ҳ�����������򶼹�ѡ", style);
                    EditorGUILayout.EndVertical();
                }
                m_MaterialEditor.ShaderProperty(VTOAR, "ʹ��Rͨ��");

                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*��ѡ��Redͨ����Ϊƫ��ͨ��������ѡ��Alphaͨ��Ϊƫ��ͨ����û��͸��ͨ���ĺڰ�ͼ���Թ�ѡ��ѡ���͸��ͨ����Ҫ��ѡ", style);
                    EditorGUILayout.EndVertical();
                }
                m_MaterialEditor.ShaderProperty(VTOR, "��ͼ��ת");

                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*��ͼ��ת��������ͼ����ʡȥ������ͼ�ı䳯��", style);
                    EditorGUILayout.EndVertical();
                }
                m_MaterialEditor.ShaderProperty(VTOTexExp, "PowerExp");


                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*��ͼ������power�����������ý�����Ӷ��ͣ�����ƫ�Ƶı仯�̶�Ҳ����Ӿ��ң��¶ȸ��Ӷ���", style);
                    EditorGUILayout.EndVertical();
                }
            


                m_MaterialEditor.ShaderProperty(VTORemap, "Remap");


                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*����ͼ���ݴ�0to1ת����0.5to-0.5", style);
                    EditorGUILayout.EndVertical();
                }
                EditorGUI.indentLevel--;




            }
            EditorGUILayout.EndVertical();

            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*չ������Զ���ͼ������һЩ���ã�����uvclamp��ͨ��ѡ����ת����ͼ����power����", style);
                EditorGUILayout.EndVertical();
            }

            m_MaterialEditor.TextureScaleOffsetProperty(Vto_Tex);
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
        }









     
        m_MaterialEditor.TexturePropertySingleLine(new GUIContent("����������ͼ"), Vto_Mask);

        if (_tips == true)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            style.fontSize = 10;
            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            GUILayout.Label("*�����������㶯�������֣���ֹ���棬�������ε���������λ�÷ǳ��������棬��������ֻ��Ҫ����һ��������һ������С����������ͼ��������Բ����������λ�ò����ж���ƫ�ƣ��Ӷ���ֹ����", style);
            EditorGUILayout.EndVertical();
        }
        if (Vto_Mask.textureValue != null)

        {


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            _VTOMaskxx = Foldouts(_VTOMaskxx, "��ͼ����(�����)");

            if (_VTOMaskxx)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();

                m_MaterialEditor.ShaderProperty(VTOMaskC, "UVClamp");

                m_MaterialEditor.ShaderProperty(VTOMaskCV, "");

                EditorGUILayout.EndHorizontal();
                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*������ѡ��ֱ��ʾ��ͼU����Clamp��V����Clamp�����Թ�ѡ����һ����Ҳ�����������򶼹�ѡ", style);
                    EditorGUILayout.EndVertical();
                }


                m_MaterialEditor.ShaderProperty(VTOMaskAR, "ʹ��Rͨ��");

                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*��ѡ��Redͨ����Ϊƫ������ͨ��������ѡ��Alphaͨ��Ϊƫ������ͨ����û��͸��ͨ���ĺڰ�ͼ���Թ�ѡ��ѡ���͸��ͨ����Ҫ��ѡ", style);
                    EditorGUILayout.EndVertical();
                }

                m_MaterialEditor.ShaderProperty(VTOMaskR, "��ͼ��ת");


                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*��ͼ��ת��������ͼ����ʡȥ������ͼ�ı䳯��", style);
                    EditorGUILayout.EndVertical();
                }


                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();

            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*չ������Զ���ͼ������һЩ���ã�����uvclamp��ͨ��ѡ����ת", style);
                EditorGUILayout.EndVertical();
            }
            m_MaterialEditor.TextureScaleOffsetProperty(Vto_Mask);
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
        }






      // 

        if (Vto_Tex.textureValue != null)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("�Զ���ǿ��");


            if (material.GetFloat("_ToggleSwitch0") == 0)
            {
                if (GUILayout.Button("�ѹر�", shortButtonStyle))
                {
                    material.SetFloat("_ToggleSwitch0", 1);



                }
            }
            else
            {
                if (GUILayout.Button("�ѿ���", shortButtonStyle))
                {
                    material.SetFloat("_ToggleSwitch0", 0);



                }
            }


        




                EditorGUILayout.EndHorizontal();



            if (material.GetFloat("_ToggleSwitch0") == 1)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                m_MaterialEditor.ShaderProperty(VTOFactorC1, "����");


                m_MaterialEditor.ShaderProperty(VTOFactorC2, "ͨ��");
                EditorGUILayout.EndVertical();

            }



            EditorGUILayout.EndVertical();

            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*�����Զ������ݿ���ģ�����ͳ̶ȣ�����������ը֮���Ч���������Զ�������ʱ��������custom1.xyzw,������custom2.xyzw��Ĭ��custom2.w�������ͳ̶ȣ������Զ������ݺ����ͳ̶Ƚ���ʧЧ��������", style);
                EditorGUILayout.EndVertical();
            }

            GUILayout.Space(5);

            if (material.GetFloat("_ToggleSwitch0") == 0)
            {
                m_MaterialEditor.ShaderProperty(Vto_Scale, "����ƫ�Ƴ̶�");
                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*���ͳ̶ȣ������Զ������ݺ����ͳ̶Ƚ���ʧЧ��������", style);
                    EditorGUILayout.EndVertical();
                }

                GUILayout.Space(5);
            }






      

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("    UV������ͼһ��");
            EditorGUILayout.EndVertical();







            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            m_MaterialEditor.ShaderProperty(Vto_Uspeed, "U����");
     
            m_MaterialEditor.ShaderProperty(Vto_Vspeed, "V����");

            EditorGUILayout.EndVertical();

            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*����ƫ��ͼ��UV�����ٶ�", style);
                EditorGUILayout.EndVertical();
            }

        }

        if (Vto_Tex.textureValue == null)
        {
            material.SetFloat("_VTOFactor", 0);
        }

        GUILayout.Space(5);
        }

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        m_MaterialEditor.ShaderProperty(IfVAT, "����VAT");
        if (material.GetFloat("_IfVAT") == 1) { 
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        m_MaterialEditor.TexturePropertySingleLine(new GUIContent("VATλ��ͼ"), VATPositionTex);

        m_MaterialEditor.TexturePropertySingleLine(new GUIContent("VAT����ͼ"), VATNormalTex);


        EditorGUILayout.EndVertical();

            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("��������ϵͳģʽ");


            if (material.GetFloat("_ParticleVAT") == 0)
            {
                if (GUILayout.Button("�ѹر�", shortButtonStyle))
                {
                    material.SetFloat("_ParticleVAT", 1);



                }
            }
            else
            {
                if (GUILayout.Button("�ѿ���", shortButtonStyle))
                {
                    material.SetFloat("_ParticleVAT", 0);



                }
            }
            EditorGUILayout.EndHorizontal();






            GUILayout.Space(5);



            EditorGUILayout.BeginVertical(EditorStyles.helpBox);



            if (material.GetFloat("_ParticleVAT") == 1) { 

                EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("�Զ������");


            if (material.GetFloat("_CustomVAT") == 0)
            {
                if (GUILayout.Button("�ѹر�", shortButtonStyle))
                {
                    material.SetFloat("_CustomVAT", 1);



                }
            }
            else
            {
                if (GUILayout.Button("�ѿ���", shortButtonStyle))
                {
                    material.SetFloat("_CustomVAT", 0);



                }
            }
            EditorGUILayout.EndHorizontal();

            if (material.GetFloat("_CustomVAT") == 1)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                m_MaterialEditor.ShaderProperty(VATFrameC1, "����");


               m_MaterialEditor.ShaderProperty(VATFrameC2, "ͨ��");
                EditorGUILayout.EndVertical();

            }

            }

            if (material.GetFloat("_CustomVAT") == 0)
                {
                    m_MaterialEditor.ShaderProperty(VATFrameFactor, "��������");


                }

                EditorGUILayout.EndVertical();

            if (material.GetFloat("_ParticleVAT") == 0) {
                material.SetFloat("_CustomVAT",0);
            }




            //     EditorGUILayout.BeginVertical(EditorStyles.helpBox);


                //    m_MaterialEditor.ShaderProperty(VATRotate, "��ת�Ƕ�");
                //     m_MaterialEditor.ShaderProperty(VATPivot, "����λ��");

                //   EditorGUILayout.EndVertical();




        }
     


        EditorGUILayout.EndVertical();


    }



    //�ۺ�����������ʾ

    void GUI_Main(Material material)

    {

    
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            m_MaterialEditor.ShaderProperty(MainAlpha, "��͸����");
        if (_tips == true)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            style.fontSize = 10;
            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            GUILayout.Label("*�����������͸���ȱ��������ֵ���ᳬ��1", style);
            EditorGUILayout.EndVertical();
        }

        GUILayout.Space(5);


     //       m_MaterialEditor.ShaderProperty(MainAlphaPower, "��͸������");

        
    //        GUILayout.Space(5);

            m_MaterialEditor.ShaderProperty(Qubaohedu, "ȥ���Ͷ�");
        if (_tips == true)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            style.fontSize = 10;
            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            GUILayout.Label("*ȥ���ͳ̶�0Ϊ��ȥ����1Ϊ��ȫ�ڰ�", style);
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndVertical();
            GUILayout.Space(5);


        //

      






        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PrefixLabel("������");
       


            if (material.GetFloat("_Depthfadeon") == 0)
        {
            if (GUILayout.Button("�ѹر�", shortButtonStyle))
            {
                material.SetFloat("_Depthfadeon", 1);


            }
        }
        else
        {
            if (GUILayout.Button("�ѿ���", shortButtonStyle))
            {
                material.SetFloat("_Depthfadeon", 0);


            }
        }

        EditorGUILayout.EndHorizontal();
        if (_tips == true)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            style.fontSize = 10;
            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            GUILayout.Label("*��Ч��������ﴩ��ʱ�������������岿�֣�ʹ�ཻ��û�����Ժۼ����������뿪�����ͼ��ѡ�������Ч��������������������˹��ܽ�ʧЧ����Ϊ�������û�������ȣ�", style);
            EditorGUILayout.EndVertical();
        }






        if (material.GetFloat("_Depthfadeon") == 1)
        {

            GUILayout.Space(5);

            m_MaterialEditor.ShaderProperty(DepthFade, "�������𻯳̶�");
            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*�𻯳̶ȣ���ֵԽ��������ΧԽ����Ҫ����ģ�ͱ����ߵ���������������������Ӻ󣬴�ѡ���Ϊǿ�����紦�Ŀ���ֵ", style);
                EditorGUILayout.EndVertical();
            }


            GUILayout.Space(5);


            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("����������");



            if (material.GetFloat("_DepthF") == 0)
            {
                if (GUILayout.Button("�ѹر�", shortButtonStyle))
                {
                    material.SetFloat("_DepthF", 1);


                }
            }
            else
            {
                if (GUILayout.Button("�ѿ���", shortButtonStyle))
                {
                    material.SetFloat("_DepthF", 0);


                }
            }

            EditorGUILayout.EndHorizontal();

            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*��������Ч��ģ�ʹ��岿�ֽ����ǿ�����������������η����ֺͳ�������ʱ�ཻ���ֵ���ʾ����ʹ����������и�ǿ", style);
                EditorGUILayout.EndVertical();
            }






            if (material.GetFloat("_DepthF") == 1)
            {
                GUILayout.Space(5);

                m_MaterialEditor.ShaderProperty(DepthColor, "��Ե��ɫ");
                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*�����ߵĵ�����ɫ", style);
                    EditorGUILayout.EndVertical();
                }

            }








        }
        else {

            material.SetFloat("_DepthF", 0);
        }
        EditorGUILayout.EndVertical();





        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.BeginHorizontal();


        EditorGUILayout.PrefixLabel("�����Զ������");
        if (material.GetFloat("_IfCustomLight") == 0)
        {
            if (GUILayout.Button("�ѹر�", shortButtonStyle))
            {
                material.SetFloat("_IfCustomLight", 1);


            }
        }
        else
        {
            if (GUILayout.Button("�ѿ���", shortButtonStyle))
            {
                material.SetFloat("_IfCustomLight", 0);


            }
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        if (_tips == true)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            style.fontSize = 10;
            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            GUILayout.Label("*�����Զ�����պ󣬲��ʿ��Խ��ܳ�����ĵƹ⣬���Ҹ����˷������ݣ�������������дʵ������Ч�����������ۺ������Ϸ������Զ������ҳǩ", style);
            EditorGUILayout.EndVertical();
        }


        if (material.GetFloat("_IfCustomLight") == 0)
        {
            material.SetFloat("_LightScale", 0);
        }



        //

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.PrefixLabel("�Ӳ�ӳ��");



        if (material.GetFloat("_IfPara") == 0)
        {
            if (GUILayout.Button("�ѹر�", shortButtonStyle))
            {
                material.SetFloat("_IfPara", 1);


            }
        }
        else
        {
            if (GUILayout.Button("�ѿ���", shortButtonStyle))
            {
                material.SetFloat("_IfPara", 0);


            }
        }

        EditorGUILayout.EndHorizontal();


        if (_tips == true)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            style.fontSize = 10;
            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            GUILayout.Label("*�����Ӳ�ӳ��󣬲��ʱ�������°��ı��֣��ǳ��ʺ�������������Ч�����ر�ע�����ʹ�õ�������ϵͳ����Ҫʹ�ù����ģʽ��Ҫʹ��mesh����һ��planeƬ���ɣ�������תʱ����bug", style);
            EditorGUILayout.EndVertical();
        }





        EditorGUILayout.EndVertical();











        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.BeginHorizontal();


        EditorGUILayout.PrefixLabel("������ѧ��ģʽ");
        if (_tips == false)
        {
            if (GUILayout.Button("�ѹر�", shortButtonStyle))
            {
                _tips = true;

            }
        }
        else

        {
            if (GUILayout.Button("�ѿ���", shortButtonStyle))
            {
                _tips = false;

            }
        }

        EditorGUILayout.EndHorizontal();

        if (_tips == true)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            style.fontSize = 10;
            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            GUILayout.Label("*���������ʾÿһ����������ϸ������Ϣ���ʺ���ʹ�ò��ʵĳ�ѧ��", style);
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();









        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUI.BeginChangeCheck();
        {
            MaterialProperty[] props = { };
            base.OnGUI(m_MaterialEditor, props);
        }
        EditorGUILayout.EndVertical();
        GUILayout.Space(5);
    }

    void GUI_Add(Material material)

    {




        m_MaterialEditor.TexturePropertySingleLine(new GUIContent("�๦��ͼ"), AddTex_Sampler, AddTexColor);


        if (_tips == true)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            style.fontSize = 10;
            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            GUILayout.Label("*����ͼ������������ͼ����ϸ��", style);
            EditorGUILayout.EndVertical();
        }

        if (AddTex_Sampler.textureValue != null)

        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            _AddTexxx = Foldouts(_AddTexxx, "��ͼ����(�����)");

            if (_AddTexxx)
            {
                EditorGUI.indentLevel++;

                //   EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                m_MaterialEditor.ShaderProperty(AddTexC, "UVClamp");
                m_MaterialEditor.ShaderProperty(AddTexCV, "");
                EditorGUILayout.EndHorizontal();

                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*������ѡ��ֱ��ʾ��ͼU����Clamp��V����Clamp�����Թ�ѡ����һ����Ҳ�����������򶼹�ѡ", style);
                    EditorGUILayout.EndVertical();
                }


                m_MaterialEditor.ShaderProperty(AddTexAR, "ʹ��Rͨ��");
                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*����ѡ����ͼ��Aͨ��Ϊ����͸��ͨ������ѡ����ͼ��Rͨ��Ϊ����͸��ͨ��", style);
                    EditorGUILayout.EndVertical();
                }



                m_MaterialEditor.ShaderProperty(IfAddTexColor, "������ɫͨ������");

                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*����ͼ����ɫҲ��Ӱ���������ɫ", style);
                    EditorGUILayout.EndVertical();
                }





                m_MaterialEditor.ShaderProperty(IfAddTexAlpha, "����͸��ͨ������");

                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*����ͼ��͸��ͨ��Ҳ��Ӱ�������͸����", style);
                    EditorGUILayout.EndVertical();
                }




                m_MaterialEditor.ShaderProperty(AddTexRo, "��ͼ��ת");

                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*��ͼ��ת��������ͼ����ʡȥ������ͼ�ı䳯��", style);
                    EditorGUILayout.EndVertical();
                }

                m_MaterialEditor.ShaderProperty(AddTexRefine, "Refine");
                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*��ͼУɫ������������ͼ�����Աȣ���������ɫ�ʱ仯�Ե�������ԭ�����Ǹ�ԭͼһ��powerֵ��ʹ���Ե�����������ں�ԭͼlerp��ʽ�����������ĸ�ѡ��ֱ��Ӧԭͼǿ�ȣ��޸�ͼǿ�ȣ��޸�ͼpowerֵ��ԭͼ�޸�ͼlerp��ϳ̶ȣ�0-1֮�伴�ɣ�", style);
                    EditorGUILayout.EndVertical();
                }

                GUILayout.Space(5);





                EditorGUI.indentLevel--;


            }

            EditorGUILayout.EndVertical();
            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*չ������Զ���ͼ������һЩ���ã�����uvclamp��͸��ͨ������ת��Уɫ", style);
                EditorGUILayout.EndVertical();
            }
            m_MaterialEditor.TextureScaleOffsetProperty(AddTex_Sampler);
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);

            m_MaterialEditor.ShaderProperty(AddTexBlendMode, "����ģʽ");
            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*����ͼ������ͼ�ĵ��ӷ�ʽ����lerp����alpha����add���֣�����Ч������ѡ�����ģʽ", style);
                EditorGUILayout.EndVertical();
            }

            GUILayout.Space(5);

            if (material.GetFloat("_AddTexBlendMode") == 0)
            {
                m_MaterialEditor.ShaderProperty(AddTexBlend, "�ں�ϵ��");
                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*����ͼ�븽��ͼ���ںϱ�����0Ϊ��ȫ��ʾ����ͼ��1Ϊ��ȫ��ʾ����ͼ", style);
                    EditorGUILayout.EndVertical();
                }

                material.SetFloat("_AddTexAlpha", 0);

                GUILayout.Space(5);
            }






            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            m_MaterialEditor.ShaderProperty(AddTexUVS, "UVѡ��");

            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("**����normalĬ��UV��polar������UV��cylinderԲͲUV,UV2����ģʽ��Ĭ��UV��ģ���Դ�UV�������꼴������������ɢUV��ԲͲUV������������һ��ԲͲ������UV������ͼ����ԲͲUV����������ɫ�����������������߽���Ч����UV2��ʹ�ڶ���UV��", style);
                EditorGUILayout.EndVertical();
            }







            if (material.GetFloat("_AddTexUVS") == 1)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                m_MaterialEditor.ShaderProperty(CenterU, "����U");
                m_MaterialEditor.ShaderProperty(CenterV, "����V");
                EditorGUILayout.EndVertical();

                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*������UV���ĵ�λ��", style);
                    EditorGUILayout.EndVertical();
                }

            }









            if (material.GetFloat("_AddTexUVS") == 2)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                m_MaterialEditor.ShaderProperty(Face, "Բ������");

                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*��������ϵ��Բ��UV�ĳ���", style);
                    EditorGUILayout.EndVertical();
                }

                m_MaterialEditor.ShaderProperty(TexCenter, "Բ������");

                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*��������ϵ��Բ��UV��λ��", style);
                    EditorGUILayout.EndVertical();
                }


                EditorGUILayout.EndVertical();



            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(5);




            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            m_MaterialEditor.ShaderProperty(AddTexAC, "����ƫ��");
            EditorGUILayout.EndVertical();

            GUILayout.Space(5);


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            m_MaterialEditor.ShaderProperty(AddTexUspeed, "U����");



            m_MaterialEditor.ShaderProperty(AddTexVspeed, "V����");

            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*����ͼUV�����ٶ�", style);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(5);




        }


        else 
        {

            material.SetFloat("_AddTexBlendMode", 0);

            material.SetFloat("_IfAddTexAlpha", 0);
        }

    }


    //Normal
    void GUI_CustomLight(Material material)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        m_MaterialEditor.ShaderProperty(LightScale, "����ǿ��");
        EditorGUILayout.EndVertical();
        if (_tips == true)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            style.fontSize = 10;
            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            GUILayout.Label("*���ܵĹ���ǿ�ȵ�����˥��", style);
            EditorGUILayout.EndVertical();
        }
  

        m_MaterialEditor.TexturePropertySingleLine(new GUIContent("������ͼ"), NormalTex);

        if (_tips == true)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            style.fontSize = 10;
            style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            GUILayout.Label("*����ͼ�������ӱ���Ĺ���ϸ�ڣ���ǿ�����", style);
            EditorGUILayout.EndVertical();
        }

        if (NormalTex.textureValue != null)

        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            _NormalTexxx = Foldouts(_NormalTexxx, "��ͼ����(�����)");

            if (_NormalTexxx)
            {
                EditorGUI.indentLevel++;

                //   EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                m_MaterialEditor.ShaderProperty(NormalTexC, "UVClamp");
                m_MaterialEditor.ShaderProperty(NormalTexCV, "");
                EditorGUILayout.EndHorizontal();

                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*������ѡ��ֱ��ʾ��ͼU����Clamp��V����Clamp�����Թ�ѡ����һ����Ҳ�����������򶼹�ѡ", style);
                    EditorGUILayout.EndVertical();
                }






                m_MaterialEditor.ShaderProperty(NormalTex_Rotat, "��ͼ��ת");

                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*��ͼ��ת��������ͼ����ʡȥ������ͼ�ı䳯��", style);
                    EditorGUILayout.EndVertical();
                }



                GUILayout.Space(5);





                EditorGUI.indentLevel--;


            }

            EditorGUILayout.EndVertical();
            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*չ������Զ���ͼ������һЩ���ã�����uvclamp����ת", style);
                EditorGUILayout.EndVertical();
            }
            m_MaterialEditor.TextureScaleOffsetProperty(NormalTex);
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("    UV������ͼһ��");
            EditorGUILayout.EndVertical();


            GUILayout.Space(5);


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            m_MaterialEditor.ShaderProperty(NormalScale, "����ǿ��");
            EditorGUILayout.EndVertical();
            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*����ǿ��ֵ������ʱ���߷�ת", style);
                EditorGUILayout.EndVertical();
            }

       

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            m_MaterialEditor.ShaderProperty(NormalTex_Uspeed, "U����");



            m_MaterialEditor.ShaderProperty(NormalTex_Vspeed, "V����");

            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*����ͼUV�����ٶ�", style);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();




            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();


            EditorGUILayout.PrefixLabel("������̬����");
            if (material.GetFloat("_IfStaticNormal") == 0)
            {
                if (GUILayout.Button("�ѹر�", shortButtonStyle))
                {
                    material.SetFloat("_IfStaticNormal", 1);
                }
            }
            else

            {
                if (GUILayout.Button("�ѿ���", shortButtonStyle))
                {
                    material.SetFloat("_IfStaticNormal", 0);

                }
            }

            EditorGUILayout.EndHorizontal();
            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*��̬�����Ǹ����ܽ�ͼ�ĺڰ���Ϣ���ɵĶ�̬���ߣ����ܽ�����У��ܽ��Ե�������ȸУ���������ˮ��ѪҺ��Һ����ʵ���ɢʱ�������", style);
                EditorGUILayout.EndVertical();
            }


            if (material.GetFloat("_IfStaticNormal") == 1)
            {
                GUILayout.Space(5);
                m_MaterialEditor.ShaderProperty(StaticNormalScale, "��̬����ǿ��");
                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*��̬���ߵ�ǿ��ֵ��ǿ��Խǿ�����Խǿ", style);
                    EditorGUILayout.EndVertical();
                }
                GUILayout.Space(5);

                m_MaterialEditor.ShaderProperty(StaticNormalOffset, "��̬����ƫ��");
                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*��̬���ߵĿ��ȣ�ƫ��Խ�ߣ���Ե���ߵĿ���Ҳ��Խ��", style);
                    EditorGUILayout.EndVertical();
                }
                GUILayout.Space(5);
            }
            EditorGUILayout.EndVertical();







        }
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();

        
            EditorGUILayout.PrefixLabel("��������ͼ����");
            if (material.GetFloat("_IfCubemap") == 0)
            {
                if (GUILayout.Button("�ѹر�", shortButtonStyle))
                {
                    material.SetFloat("_IfCubemap", 1);
                }
            }
            else

            {
                if (GUILayout.Button("�ѿ���", shortButtonStyle))
                {
                    material.SetFloat("_IfCubemap", 0);

                }
            }

            EditorGUILayout.EndHorizontal();


            if (material.GetFloat("_IfCubemap") == 1)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                m_MaterialEditor.TexturePropertySingleLine(new GUIContent("CubeMap��ͼ"), CubeMap);
                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*CubeMap��������ģ�⻷�����䣬���Ӳ��ʵ��ʸУ�ʹ���ʸ��Ӿ߹⻬��", style);
                    EditorGUILayout.EndVertical();
                }
                m_MaterialEditor.TextureScaleOffsetProperty(CubeMap);
                EditorGUILayout.EndVertical();

                m_MaterialEditor.ShaderProperty(CubemapScale, "Cubemapͼǿ��");
                if (_tips == true)
                {

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    style.fontSize = 10;
                    style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    GUILayout.Label("*CubeMap������ǿ��ֵ", style);
                    EditorGUILayout.EndVertical();
                }
            }
            else {
                material.SetFloat("_CubemapScale", 0);
            }






            EditorGUILayout.EndVertical();




           



            GUILayout.Space(5);















    }

   // 
        
    void GUI_para(Material material)

    {


     //   

         
            m_MaterialEditor.TexturePropertySingleLine(new GUIContent("�����ͼ"), ParaTex);
  
            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*�Ҷ�ͼ���ɣ���ɫ���ֻ��°�����ɫ���ֲ���", style);
                EditorGUILayout.EndVertical();
            }



            GUILayout.Space(5);

            m_MaterialEditor.ShaderProperty(Parallax, "���ֵ");

            if (_tips == true)
            {

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                style.fontSize = 10;
                style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                GUILayout.Label("*�°���ǿ��ֵ", style);
                EditorGUILayout.EndVertical();
            }



            GUILayout.Space(5);

        if (ParaTex.textureValue == null) {

            material.SetFloat("_Parallax",0);
        }




    }




    void GUI_mengban(Material material)

    {

        EditorGUILayout.BeginHorizontal();


        EditorGUILayout.PrefixLabel("ģ����Է���");
        if (material.GetFloat("_StencilStyle") == 0)
        {
            if (GUILayout.Button("�ѹر�", shortButtonStyle))
            {
                material.SetFloat("_StencilStyle", 1);
                material.SetFloat("_Reference", 0);
                material.SetFloat("_Comparison", 8);
                material.SetFloat("_Pass", 0);
                material.SetFloat("_Fail", 0);
            }
        }
       
        else {
            if (GUILayout.Button("����ģʽ", shortButtonStyle))
            {
                material.SetFloat("_StencilStyle", 0);
                material.SetFloat("_Reference", 0);
                material.SetFloat("_Comparison", 8);
                material.SetFloat("_Pass", 0);
                material.SetFloat("_Fail", 0);
            }
        }



        EditorGUILayout.EndHorizontal();


        GUILayout.Space(5);

        if (material.GetFloat("_StencilStyle") == 1) 
        { 
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            m_MaterialEditor.ShaderProperty(Reference, "ģ��ο�ֵ");

            m_MaterialEditor.ShaderProperty(Comparison, "�ȽϷ�ʽ");

            m_MaterialEditor.ShaderProperty(Pass, "ͨ������");

            m_MaterialEditor.ShaderProperty(Fail, "ʧ������");
            EditorGUILayout.EndVertical();
        }
    }

    void GUI_Tip(Material material)

    {


        style.fontSize = 12;
        style.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
        style.wordWrap = true;


        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label(" 1.�����Զ�������ʱ��������custom1.xyzw,������custom2.xyzw", style);
       
        GUILayout.Space(5); GUILayout.Label(" 2.�����ܽ���βģʽ��͸���ȸ�Ϊ�ܽ����", style);

        GUILayout.Space(5);
        EditorGUILayout.EndVertical();



        EditorGUILayout.BeginVertical(EditorStyles.helpBox);


        GUILayout.Label("��shader���������˻���è�������ر��л�����ޣ�Nor_Zed��sion��123ľͷ�ˣ�lolming��Allen��������AmantJy���ٶ�����ͷ,����,Cokey,J��,AimerLily,�ȾȺ���,XRX�İ�����֧��", style);
        EditorGUILayout.EndVertical();

    }







  

}