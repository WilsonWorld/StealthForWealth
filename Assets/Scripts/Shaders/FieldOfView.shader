Shader "FieldOfView" {
	Properties{  
		// Properties of the material
		_MainTex("Base (RGB)", 2D) = "white" {}
		_FOVColor("Field Of View Color", Color) = (1, 1, 1)
		_MainColor("MainColor", Color) = (1, 1, 1)
		_Position1("Position1",  Vector) = (0,0,0)
		_Direction1("Direction1",  Vector) = (0,0,0)
		_Position2("Position2",  Vector) = (0,0,0)
		_Direction2("Direction2",  Vector) = (0,0,0)
		_Position3("Position3",  Vector) = (0,0,0)
		_Direction3("Direction3",  Vector) = (0,0,0)
	}
		SubShader{
		Tags{ "RenderType" = "Diffuse" }
		// https://docs.unity3d.com/Manual/SL-SurfaceShaders.html
		CGPROGRAM
#pragma surface surf Lambert

	sampler2D _MainTex;
	//https://docs.unity3d.com/Manual/SL-DataTypesAndPrecision.html
	fixed3 _FOVColor; //Precision
	fixed3 _MainColor;
	float3 _Position1;
	float3 _Direction1;
	float3 _Position2;
	float3 _Direction2;
	float3 _Position3;
	float3 _Direction3;

	// Values that interpolated from vertex data.
	struct Input {
		float2 uv_MainTex;
		float3 worldPos;
	};

	// Barycentric coordinates
	// http://mathworld.wolfram.com/BarycentricCoordinates.html
	bool isPointInTriangle(float2 p1, float2 p2, float2 p3, float2 pointInQuestion)
	{
		float denominator = ((p2.y - p3.y) * (p1.x - p3.x) + (p3.x - p2.x) * (p1.y - p3.y));
		float a = ((p2.y -p3.y) * (pointInQuestion.x - p3.x) + (p3.x - p2.x) * (pointInQuestion.y - p3.y)) / denominator;
		float b  = ((p3.y - p1.y) * (pointInQuestion.x - p3.x) + (p1.x - p3.x) * (pointInQuestion.y - p3.y)) / denominator;
		float c  = 1 - a - b;

		return 0 <= a && a <= 1 && 0 <= b && b <= 1 && 0 <= c && c <= 1;
	}

	bool isPointInTheCircle(float2 circleCenterPoint, float2 thisPoint, float radius)
	{
		return distance(circleCenterPoint, thisPoint) <= radius;
	}
	
	void surf(Input IN, inout SurfaceOutput o) {
		half4 c = tex2D(_MainTex, IN.uv_MainTex);

		c.rgb *= _MainColor;

		// Shared Variables
		float dist = 25.0;
		float offsetAngle = 15.0;
		float midoffsetAngle = 30.0;
		float faroffsetAngle = 45.0;

		float offsetAngleRadians = offsetAngle * (3.14 / 180.0);
		float midoffsetAngleRadians = midoffsetAngle * (3.14 / 180.0);
		float faroffsetAngleRadians = faroffsetAngle * (3.14 / 180.0);

		float adjDist = dist / cos(offsetAngleRadians);

		float3 pointInQuestion = IN.worldPos;

		// First Enemy Shader
		float3 camDir1 = _Direction1;
		float viewAngle1 = atan2(camDir1.z, camDir1.x);

		float3 basePoint1 = _Position1.xyz;
		float3 centerPoint1 = (float3(cos(viewAngle1), 0, sin(viewAngle1)) * adjDist) + basePoint1;
		float3 leftPoint1 = (float3(cos(viewAngle1 + offsetAngleRadians), 0, sin(viewAngle1 + offsetAngleRadians)) * adjDist) + basePoint1;
		float3 midleftPoint1 = (float3(cos(viewAngle1 + midoffsetAngleRadians), 0, sin(viewAngle1 + midoffsetAngleRadians)) * adjDist) + basePoint1;
		float3 farleftPoint1 = (float3(cos(viewAngle1 + faroffsetAngleRadians), 0, sin(viewAngle1 + faroffsetAngleRadians)) * adjDist) + basePoint1;
		float3 rightPoint1 = (float3(cos(viewAngle1 - offsetAngleRadians), 0, sin(viewAngle1 - offsetAngleRadians)) * adjDist) + basePoint1;
		float3 midrightPoint1 = (float3(cos(viewAngle1 - midoffsetAngleRadians), 0, sin(viewAngle1 - midoffsetAngleRadians)) * adjDist) + basePoint1;
		float3 farrightPoint1 = (float3(cos(viewAngle1 - faroffsetAngleRadians), 0, sin(viewAngle1 - faroffsetAngleRadians)) * adjDist) + basePoint1;

		// Second Enemy Shader
		float3 camDir2 = _Direction2;
		float viewAngle2 = atan2(camDir2.z, camDir2.x);

		float3 basePoint2 = _Position2.xyz;
		float3 centerPoint2 = (float3(cos(viewAngle2), 0, sin(viewAngle2)) * adjDist) + basePoint2;
		float3 leftPoint2 = (float3(cos(viewAngle2 + offsetAngleRadians), 0, sin(viewAngle2 + offsetAngleRadians)) * adjDist) + basePoint2;
		float3 midleftPoint2 = (float3(cos(viewAngle2 + midoffsetAngleRadians), 0, sin(viewAngle2 + midoffsetAngleRadians)) * adjDist) + basePoint2;
		float3 farleftPoint2 = (float3(cos(viewAngle2 + faroffsetAngleRadians), 0, sin(viewAngle2 + faroffsetAngleRadians)) * adjDist) + basePoint2;
		float3 rightPoint2 = (float3(cos(viewAngle2 - offsetAngleRadians), 0, sin(viewAngle2 - offsetAngleRadians)) * adjDist) + basePoint2;
		float3 midrightPoint2 = (float3(cos(viewAngle2 - midoffsetAngleRadians), 0, sin(viewAngle2 - midoffsetAngleRadians)) * adjDist) + basePoint2;
		float3 farrightPoint2 = (float3(cos(viewAngle2 - faroffsetAngleRadians), 0, sin(viewAngle2 - faroffsetAngleRadians)) * adjDist) + basePoint2;

		// Third Enemy Shader
		float3 camDir3 = _Direction3;
		float viewAngle3 = atan2(camDir3.z, camDir3.x);

		float3 basePoint3 = _Position3.xyz;
		float3 centerPoint3 = (float3(cos(viewAngle3), 0, sin(viewAngle3)) * adjDist) + basePoint3;
		float3 leftPoint3 = (float3(cos(viewAngle3 + offsetAngleRadians), 0, sin(viewAngle3 + offsetAngleRadians)) * adjDist) + basePoint3;
		float3 midleftPoint3 = (float3(cos(viewAngle3 + midoffsetAngleRadians), 0, sin(viewAngle3 + midoffsetAngleRadians)) * adjDist) + basePoint3;
		float3 farleftPoint3 = (float3(cos(viewAngle3 + faroffsetAngleRadians), 0, sin(viewAngle3 + faroffsetAngleRadians)) * adjDist) + basePoint3;
		float3 rightPoint3 = (float3(cos(viewAngle3 - offsetAngleRadians), 0, sin(viewAngle3 - offsetAngleRadians)) * adjDist) + basePoint3;
		float3 midrightPoint3 = (float3(cos(viewAngle3 - midoffsetAngleRadians), 0, sin(viewAngle3 - midoffsetAngleRadians)) * adjDist) + basePoint3;
		float3 farrightPoint3 = (float3(cos(viewAngle3 - faroffsetAngleRadians), 0, sin(viewAngle3 - faroffsetAngleRadians)) * adjDist) + basePoint3;

		if (isPointInTriangle(farrightPoint1.xz, midrightPoint1.xz, basePoint1.xz, pointInQuestion.xz) ||		// First Enemy Vision Cone
			isPointInTriangle(midrightPoint1.xz, rightPoint1.xz, basePoint1.xz, pointInQuestion.xz) ||
			isPointInTriangle(rightPoint1.xz, centerPoint1.xz, basePoint1.xz, pointInQuestion.xz) ||
			isPointInTriangle(centerPoint1.xz, leftPoint1.xz, basePoint1.xz, pointInQuestion.xz) ||
			isPointInTriangle(leftPoint1.xz, midleftPoint1.xz, basePoint1.xz, pointInQuestion.xz) ||
			isPointInTriangle(midleftPoint1.xz, farleftPoint1.xz, basePoint1.xz, pointInQuestion.xz) ||
			isPointInTriangle(farrightPoint2.xz, midrightPoint2.xz, basePoint2.xz, pointInQuestion.xz) ||			// Second Enemy Vision Cone
			isPointInTriangle(midrightPoint2.xz, rightPoint2.xz, basePoint2.xz, pointInQuestion.xz) ||
			isPointInTriangle(rightPoint2.xz, centerPoint2.xz, basePoint2.xz, pointInQuestion.xz) ||
			isPointInTriangle(centerPoint2.xz, leftPoint2.xz, basePoint2.xz, pointInQuestion.xz) ||
			isPointInTriangle(leftPoint2.xz, midleftPoint2.xz, basePoint2.xz, pointInQuestion.xz) ||
			isPointInTriangle(midleftPoint2.xz, farleftPoint2.xz, basePoint2.xz, pointInQuestion.xz) ||
			isPointInTriangle(farrightPoint3.xz, midrightPoint3.xz, basePoint3.xz, pointInQuestion.xz) ||			// Third Enemy Vision Cone
			isPointInTriangle(midrightPoint3.xz, rightPoint3.xz, basePoint3.xz, pointInQuestion.xz) ||
			isPointInTriangle(rightPoint3.xz, centerPoint3.xz, basePoint3.xz, pointInQuestion.xz) ||
			isPointInTriangle(centerPoint3.xz, leftPoint3.xz, basePoint3.xz, pointInQuestion.xz) ||
			isPointInTriangle(leftPoint3.xz, midleftPoint3.xz, basePoint3.xz, pointInQuestion.xz) ||
			isPointInTriangle(midleftPoint3.xz, farleftPoint3.xz, basePoint3.xz, pointInQuestion.xz))
		{
			o.Albedo = c.rgb * _FOVColor;
		}
		else
		{
			o.Albedo = c.rgb;
		}
	
		o.Alpha = c.a;
	}
	ENDCG
	}
		FallBack "Diffuse" //If we cannot use the subshader on specific hardware we will fallback to Diffuse shader
}
