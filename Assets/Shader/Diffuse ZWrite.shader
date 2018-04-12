Shader "Transparent/Diffuse ZWrite" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader {
    Tags {
	"Queue"="Transparent" 
	"IgnoreProjector"="True" 
	"RenderType"="Transparent"
	}
    LOD 200

    // extra pass that renders to depth buffer only
    Pass {
        //ZWrite On
        //ColorMask 0
		//Cull Back
    }

    // paste in forward rendering passes from Transparent/Diffuse
    UsePass "Transparent/Diffuse/FORWARD"
}
Fallback "Transparent/VertexLit"
}
