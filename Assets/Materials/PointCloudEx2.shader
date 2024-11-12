Shader "Custom/PointCloudEx2"
{
    Properties
    {
        _PointSize("Point Size", Range(0.001, 1)) = 0.005
    }

        SubShader
    {
        Pass
    {
        Tags{ "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM

#pragma vertex VERT
#pragma fragment FRAG
#pragma geometry GEO

#include "UnityCG.cginc"

    struct VERT_INPUT
    {
        float4 pos : POSITION;
        float4 color : COLOR;
    };

    struct GEO_INPUT
    {
        float4    pos    : POSITION;
        fixed4 color : COLOR;
    };

    struct FRAG_INPUT
    {
        float4    pos    : POSITION;
        fixed4 color : COLOR;
    };

    float _PointSize;

    GEO_INPUT VERT(VERT_INPUT v)
    {
        GEO_INPUT o = (GEO_INPUT)0;
        o.pos = v.pos;
        o.color = v.color;
        return o;
    }

    [maxvertexcount(4)]
    void GEO(point GEO_INPUT p[1], inout TriangleStream<FRAG_INPUT> triStream)
    {
        float3 cameraUp = UNITY_MATRIX_IT_MV[1].xyz;
        float3 cameraForward = normalize(UNITY_MATRIX_IT_MV[2].xyz);
        float3 right = cross(cameraUp, cameraForward);

        float4 v[4];
/*TROUBLE LINE*/
        float size = _PointSize;
        v[0] = float4(p[0].pos + size * right - size * cameraUp, 1.0f);
        v[1] = float4(p[0].pos + size * right + size * cameraUp, 1.0f);
        v[2] = float4(p[0].pos - size * right - size * cameraUp, 1.0f);
        v[3] = float4(p[0].pos - size * right + size * cameraUp, 1.0f);


        float4x4 vp;

        vp[0] = UnityObjectToClipPos(unity_WorldToObject[0]);
        vp[1] = UnityObjectToClipPos(unity_WorldToObject[1]);
        vp[2] = UnityObjectToClipPos(unity_WorldToObject[2]);
        vp[3] = UnityObjectToClipPos(unity_WorldToObject[3]);


        FRAG_INPUT newVert;

        newVert.pos = mul(vp, v[0]);
        newVert.color = p[0].color;
        triStream.Append(newVert);

        newVert.pos = mul(vp, v[1]);
        newVert.color = p[0].color;
        triStream.Append(newVert);

        newVert.pos = mul(vp, v[2]);
        newVert.color = p[0].color;
        triStream.Append(newVert);

        newVert.pos = mul(vp, v[3]);
        newVert.color = p[0].color;
        triStream.Append(newVert);
    }

    fixed4 FRAG(FRAG_INPUT input) : COLOR
    {
        return input.color;
    }

        ENDCG
    }
    }
}