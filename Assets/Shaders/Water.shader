Shader "Custom/Water"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 0.5)
		_EdgeColor("Edge Color", Color) = (1, 1, 1, 0.5)
		 _DepthFactor ("_DepthFactor ", float) = 1
		
		_WaveSpeed("Wave Speed", float) = 5
		_WaveAmp("Wave Amp", float) = 0.2
		_DepthRampTex("Depth Ramp", 2D) = "white" {}
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_MainTex("Main Texture", 2D) = "white" {}
		_DistortStrength("Distort Strength", float) = 0.5
		_ExtraHeight("Extra Height", float) = -0.32
		_ScrollY("Scroll Y", Range(-5,5)) = 2.1

		_Transparencey(" Transparencey", float) = 0.5

	}

	SubShader
	{
        Tags{ "RenderType"="Transparent" "Queue"="Transparent"}
		

		// Grab the screen behind the object into _BackgroundTexture
        GrabPass
        {
            "_BackgroundTexture"
        }

        // Background distortion
        Pass
        {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Properties
            sampler2D _BackgroundTexture;
            sampler2D _NoiseTex;
            float     _DistortStrength;
			float  _WaveSpeed;
			float  _WaveAmp;

            struct vertexInput
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float3 texCoord : TEXCOORD0;
            };

            struct vertexOutput
            {
                float4 pos : SV_POSITION;
                float4 grabPos : TEXCOORD0;
            };

            vertexOutput vert(vertexInput input)
            {
                vertexOutput output;
				_WaveSpeed *= _Time;
                // convert input to world space
                output.pos = UnityObjectToClipPos(input.vertex);
                float4 normal4 = float4(input.normal, 0.0);
				float3 normal = normalize(mul(normal4, unity_WorldToObject).xyz);

                // use ComputeGrabScreenPos function from UnityCG.cginc
                // to get the correct texture coordinate
                output.grabPos = ComputeGrabScreenPos(output.pos);

                // distort based on bump map
				float noiseSample = tex2Dlod(_NoiseTex, float4(input.texCoord.xy, 0, 0));
				output.grabPos.y += sin(_Time*_WaveSpeed*noiseSample)*_WaveAmp * _DistortStrength;
                output.grabPos.x += cos(_Time*_WaveSpeed*noiseSample)*_WaveAmp * _DistortStrength;

                return output;
            }

            float4 frag(vertexOutput input) : COLOR
            {
                return tex2Dproj(_BackgroundTexture, input.grabPos);
            }
            ENDCG
        }

		Pass
		{
           
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
 

			CGPROGRAM
           #define SMOOTHSTEP_AA 0.01

			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fog
			 #include "UnityCG.cginc"
			
			// Properties
			float4 _Color;
			float4 _EdgeColor;
			float  _DepthFactor;
			float  _WaveSpeed;
			float  _WaveAmp;
			float _ExtraHeight;
			sampler2D _CameraDepthTexture;
			sampler2D _DepthRampTex;
			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _ScrollY;
			half _Transparencey;

			struct vertexInput
			{
				float4 vertex : POSITION;
				float2 texCoord : TEXCOORD0;
				float4 uv: TEXCOORD1;

			};

			struct vertexOutput
			{
				float2 texCoord : TEXCOORD0;
				//UNITY_FOG_COORDS(1)
				float4 pos : SV_POSITION;
				float4 screenPos : TEXCOORD1;
				float2 displUV : TEXCOORD2;

			};

			vertexOutput vert(vertexInput input)
			{
				vertexOutput output;
				// convert to world space
				output.pos = UnityObjectToClipPos(input.vertex);

				// apply wave animation
				float noiseSample = tex2Dlod(_NoiseTex, float4(input.texCoord.xy, 0, 0));
				output.pos.y += sin(_Time*_WaveSpeed*noiseSample)*_WaveAmp + _ExtraHeight;
				output.pos.x += cos(_Time*_WaveSpeed*noiseSample)*_WaveAmp;

				// compute depth
				output.screenPos = ComputeScreenPos(output.pos);
				
				// texture coordinates 
				//output.texCoord = input.texCoord;
				output.texCoord  = TRANSFORM_TEX (input.texCoord, _MainTex);

				//UNITY_TRANSFER_FOG(output,output.pos);
				
				return output; 
			}

			float4 frag(vertexOutput input) : SV_TARGET
			{
				_ScrollY *= _Time;
				// apply depth texture
				float4 depthSample = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, input.screenPos);
				float depth = LinearEyeDepth(depthSample).r;

				// create foamline
				// apply the DepthFactor to be able to tune at what depth values
				// the foam line actually starts
				float foamLine = 1 - saturate(_DepthFactor  * (depth - input.screenPos.w));
				// sample the ramp texture
				foamLine*= _EdgeColor;
				//foam ramp
				float4 foamRamp = float4(tex2D(_DepthRampTex, float2(foamLine, 0.2)).rgb, 1.0);

				// sample main texture- Albedo comes from a texture tinted by color
				float4 col = tex2D(_MainTex, input.texCoord.xy + float2(0,_ScrollY)) * _Color;// + foamLine * _EdgeColor;

				
				col.rgb = lerp(col.rgb, _EdgeColor.rgb, foamLine);
				col.a =_Transparencey;

				
				//UNITY_APPLY_FOG(input.fogCoord, col);

                return col  * foamRamp + foamLine;

			}

			ENDCG
		}

	}
	 FallBack "VertexLit"
}