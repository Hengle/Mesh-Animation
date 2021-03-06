Shader "Mobile/Diffuse (Mesh Animation)" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _AnimTex ("Animation (RGB)", 2D) = "white" {}
        _AnimMul ("Animation Bounds Size", Vector) = (1, 1, 1, 0)
        _AnimAdd ("Animation Bounds Offset", Vector) = (0, 0, 0, 0)
        [PerRendererData] _AnimTime ("Animation Time", Vector) = (0, 1, 1, 0) /* (x: start, y: length, z: speed, w: startTime) */
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 150
    
        CGPROGRAM
        #pragma surface surf Lambert noforwardadd vertex:vert
        #pragma multi_compile_instancing 
        #pragma target 3.0
        #pragma require samplelod
        
        sampler2D _MainTex;
        sampler2D _AnimTex;
        float4 _AnimTex_TexelSize;
        
        float4 _AnimMul;
        float4 _AnimAdd;
        
        UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float4, _AnimTime)
        UNITY_INSTANCING_BUFFER_END(Props)
        
        struct appdata
        {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float2 texcoord : TEXCOORD0;
            float4 color : COLOR0;
            uint vertexId : SV_VertexID;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };
        
        struct Input {
            float2 uv_MainTex;
        };
        
        void vert (inout appdata v) {            
            UNITY_SETUP_INSTANCE_ID(v);
            
            float4 t = UNITY_ACCESS_INSTANCED_PROP(Props, _AnimTime);
                        
            float vertCoord = (0.5 + v.vertexId) * _AnimTex_TexelSize.x;
            float animCoord = (0.5 + t.x + frac((_Time.y - t.w) * t.z) * t.y) * _AnimTex_TexelSize.y;            
            float4 position = tex2Dlod(_AnimTex, float4(vertCoord, animCoord, 0, 0)) * _AnimMul + _AnimAdd;
            
            v.vertex = position;
        }
        
        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    
    Fallback "Mobile/Diffuse"
}
